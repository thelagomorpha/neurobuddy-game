using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

/// <summary>
/// PlayMode tests for GoNoGoManager to cover the actual game loop and coroutines.
/// This ensures 100% lines coverage of GameSequence and SpawnItem.
/// </summary>
public class GoNoGoManagerPlayModeTest
{
    private GoNoGoManager manager;
    private Text countdownText;
    private Text timerText;
    private GameObject resultsPanel;
    private Text resultsSummaryText;
    private GameObject spawnArea;
    private GameObject applePrefab;
    private GameObject fishbonePrefab;

    [UnitySetUp]
    public IEnumerator Setup()
    {
        // Setup prefabs
        applePrefab = new GameObject("ApplePrefab");
        var appleRect = applePrefab.AddComponent<RectTransform>();
        applePrefab.AddComponent<Image>();
        applePrefab.AddComponent<Button>();

        fishbonePrefab = new GameObject("FishbonePrefab");
        var fishboneRect = fishbonePrefab.AddComponent<RectTransform>();
        fishbonePrefab.AddComponent<Image>();
        fishbonePrefab.AddComponent<Button>();

        // Setup UI
        var go = new GameObject("Manager");
        manager = go.AddComponent<GoNoGoManager>();

        countdownText = new GameObject("Countdown").AddComponent<Text>();
        timerText = new GameObject("Timer").AddComponent<Text>();
        resultsPanel = new GameObject("ResultsPanel");
        resultsSummaryText = new GameObject("ResultsSummary").AddComponent<Text>();
        spawnArea = new GameObject("SpawnArea");
        spawnArea.AddComponent<RectTransform>();

        // Set properties using reflection
        var type = typeof(GoNoGoManager);
        type.GetField("gameDuration", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(manager, 0.5f); // Very short game
        type.GetField("spawnInterval", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(manager, 0.2f);
        type.GetField("itemDisplayDuration", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(manager, 0.1f);
        type.GetField("countdownDuration", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(manager, 1);
        type.GetField("goItemPrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(manager, applePrefab);
        type.GetField("noGoItemPrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(manager, fishbonePrefab);

        var goSprite = Sprite.Create(new Texture2D(2, 2), new Rect(0, 0, 2, 2), Vector2.zero);
        var noGoSprite = Sprite.Create(new Texture2D(2, 2), new Rect(0, 0, 2, 2), Vector2.zero);
        type.GetField("goSprite", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(manager, goSprite);
        type.GetField("noGoSprite", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(manager, noGoSprite);

        type.GetField("countdownText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(manager, countdownText);
        type.GetField("timerText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(manager, timerText);
        type.GetField("resultsPanel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(manager, resultsPanel);
        type.GetField("resultsSummaryText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(manager, resultsSummaryText);
        type.GetField("spawnArea", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(manager, spawnArea.GetComponent<RectTransform>());

        // Start uses StartCoroutine, which requires the object to be active.
        // It starts naturally. Wait a frame.
        yield return null;
    }

    [UnityTearDown]
    public IEnumerator Teardown()
    {
        Object.Destroy(manager.gameObject);
        Object.Destroy(countdownText.gameObject);
        Object.Destroy(timerText.gameObject);
        Object.Destroy(resultsPanel.gameObject);
        Object.Destroy(resultsSummaryText.gameObject);
        Object.Destroy(spawnArea.gameObject);
        Object.Destroy(applePrefab);
        Object.Destroy(fishbonePrefab);
        yield return null;
    }

    [UnityTest]
    public IEnumerator FullGameLoop_ShouldPlayThroughAndShowResults()
    {
        // 1. Check countdown phase
        Assert.AreEqual("1", countdownText.text); // Since countdown is 1s
        Assert.IsFalse(manager.IsGameActive);
        
        // Wait for countdown to finish (1s for '1', 1s for 'GO!', 0.5s pause)
        yield return new WaitForSeconds(1.1f);
        Assert.AreEqual("GO!", countdownText.text);
        
        yield return new WaitForSeconds(0.6f);
        Assert.IsFalse(countdownText.gameObject.activeInHierarchy);
        Assert.IsTrue(manager.IsGameActive);

        // 2. Game is active. Game duration is 0.5s, spawn interval is 0.2s.
        // After 0.2s a spawn should occur. By 0.5s the game ends.
        yield return new WaitForSeconds(0.6f);

        // 3. Results Phase
        Assert.IsFalse(manager.IsGameActive);
        Assert.AreEqual("0", timerText.text);
        Assert.IsTrue(resultsPanel.activeSelf);
        Assert.IsNotEmpty(resultsSummaryText.text);

        // We expect at least 1 or 2 spawns during the 0.5s active period depending on timing variance.
        Assert.GreaterOrEqual(manager.Results.Count, 1, "At least one item should have spawned and reported its result due to timeout.");
    }
}
