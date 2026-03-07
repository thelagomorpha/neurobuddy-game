using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Main controller for the Go/No-Go game.
/// Manages countdown, spawning, timing, and result tracking.
/// </summary>
public class GoNoGoManager : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private float gameDuration = 30f;
    [SerializeField] private float spawnInterval = 1.5f;
    [SerializeField] private float itemDisplayDuration = 1.2f;
    [SerializeField, Range(0f, 1f)] private float goRatio = 0.7f;
    [SerializeField] private int countdownDuration = 3;

    [Header("Prefabs")]
    [SerializeField] private GameObject goItemPrefab;
[SerializeField] private GameObject noGoItemPrefab;
    [SerializeField] private Sprite goSprite;
    [SerializeField] private Sprite noGoSprite;

    [Header("UI References")]
    [SerializeField] private Text countdownText;
    [SerializeField] private Text timerText;
    [SerializeField] private GameObject resultsPanel;
    [SerializeField] private Text resultsSummaryText;
    [SerializeField] private RectTransform spawnArea;

    private readonly List<GoNoGoResult> _results = new List<GoNoGoResult>();
    private bool _isGameActive;
    private float _gameTimer;

    // --- Public accessors for testing ---

    public List<GoNoGoResult> Results => _results;
    public bool IsGameActive => _isGameActive;
    public float GameTimer => _gameTimer;
    public float GameDuration => gameDuration;
    public float SpawnInterval => spawnInterval;
    public float ItemDisplayDuration => itemDisplayDuration;
    public float GoRatio => goRatio;

    private void Start()
    {
        if (resultsPanel != null)
        {
            resultsPanel.SetActive(false);
        }

        StartCoroutine(GameSequence());
    }

    private IEnumerator GameSequence()
    {
        // === Countdown Phase ===
        for (int i = countdownDuration; i > 0; i--)
        {
            if (countdownText != null) countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        if (countdownText != null) countdownText.text = "GO!";
        yield return new WaitForSeconds(0.5f);
        if (countdownText != null) countdownText.gameObject.SetActive(false);

        // === Game Phase ===
        _isGameActive = true;
        _gameTimer = gameDuration;
        float spawnTimer = 0f;

        while (_gameTimer > 0f)
        {
            _gameTimer -= Time.deltaTime;
            spawnTimer -= Time.deltaTime;

            if (timerText != null)
            {
                timerText.text = Mathf.CeilToInt(_gameTimer).ToString();
            }

            if (spawnTimer <= 0f)
            {
                SpawnItem();
                spawnTimer = spawnInterval;
            }

            yield return null;
        }

        // === Results Phase ===
        _isGameActive = false;
        if (timerText != null) timerText.text = "0";

        ShowResults();
    }

    private void SpawnItem()
    {
        bool isGo = Random.value <= goRatio;
        GameObject prefab = isGo ? goItemPrefab : noGoItemPrefab;

        if (prefab == null) return;

        GameObject item = Instantiate(prefab, spawnArea != null ? spawnArea : transform);

        // Random position within spawn area
        if (spawnArea != null)
        {
            var rect = spawnArea.rect;
            float x = Random.Range(rect.xMin + 50f, rect.xMax - 50f);
            float y = Random.Range(rect.yMin + 50f, rect.yMax - 50f);
            item.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
        }

        var goNoGoItem = item.GetComponent<GoNoGoItem>();
        if (goNoGoItem == null)
        {
            goNoGoItem = item.AddComponent<GoNoGoItem>();
        }
        // Set the correct sprite on the Image component
        var img = item.GetComponent<Image>();
        if (img != null) img.sprite = isGo ? goSprite : noGoSprite;

        goNoGoItem.Initialize(isGo, itemDisplayDuration, this);
    }

    /// <summary>
    /// Record a trial result. Called by GoNoGoItem when it completes.
    /// </summary>
    public void RecordResult(GoNoGoResult result)
    {
        _results.Add(result);
    }

    private void ShowResults()
    {
        if (resultsPanel != null) resultsPanel.SetActive(true);

        int totalGo = _results.Count(r => r.IsGoTrial);
        int totalNoGo = _results.Count(r => !r.IsGoTrial);
        int hits = _results.Count(r => r.IsHit);
        int misses = _results.Count(r => r.IsMiss);
        int falseAlarms = _results.Count(r => r.IsFalseAlarm);
        int correctRejections = _results.Count(r => r.IsCorrectRejection);

        var hitReactionTimes = _results
            .Where(r => r.IsHit)
            .Select(r => r.ReactionTime)
            .ToList();

        float avgReactionTime = hitReactionTimes.Count > 0
            ? hitReactionTimes.Average()
            : 0f;

        string summary = $"=== RESULTS ===\n\n"
            + $"Total Trials: {_results.Count}\n"
            + $"Go Trials: {totalGo}  |  No-Go Trials: {totalNoGo}\n\n"
            + $"Hits: {hits}  |  Misses: {misses}\n"
            + $"False Alarms: {falseAlarms}  |  Correct Rejections: {correctRejections}\n\n"
            + $"Accuracy: {(_results.Count > 0 ? (float)_results.Count(r => r.IsCorrect) / _results.Count * 100f : 0f):F1}%\n"
            + $"Avg Reaction Time (Hits): {avgReactionTime:F3}s";

        Debug.Log(summary);

        if (resultsSummaryText != null)
        {
            resultsSummaryText.text = summary;
        }
    }

    // --- Summary methods for testing ---

    public int GetHitCount() => _results.Count(r => r.IsHit);
    public int GetMissCount() => _results.Count(r => r.IsMiss);
    public int GetFalseAlarmCount() => _results.Count(r => r.IsFalseAlarm);
    public int GetCorrectRejectionCount() => _results.Count(r => r.IsCorrectRejection);

    public float GetAverageReactionTime()
    {
        var hitTimes = _results.Where(r => r.IsHit).Select(r => r.ReactionTime).ToList();
        return hitTimes.Count > 0 ? hitTimes.Average() : 0f;
    }
}