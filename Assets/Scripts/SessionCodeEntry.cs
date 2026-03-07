using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles session code entry on the Menu scene.
/// Student types a 6-character alphanumeric code given by the teacher.
/// Attach to a GameObject, wire InputKode (InputField) and JoinButton (Button) in the Inspector.
/// </summary>
public class SessionCodeEntry : MonoBehaviour
{
    [SerializeField] private InputField inputKode;
    [SerializeField] private Text feedbackText; // optional — leave unassigned to disable feedback

    private ISceneLoader _sceneLoader;
    private bool _hasNavigated;

    /// <summary>
    /// Inject a custom scene loader (used in tests).
    /// </summary>
    public void SetSceneLoader(ISceneLoader loader)
    {
        _sceneLoader = loader;
    }

    private void Awake()
    {
        _sceneLoader ??= new UnitySceneLoader();
    }

    /// <summary>
    /// Returns true if raw is exactly 6 characters of A-Z or 0-9 (case-insensitive).
    /// </summary>
    public static bool IsValidCode(string raw)
    {
        if (string.IsNullOrEmpty(raw)) return false;
        string upper = raw.ToUpperInvariant();
        return upper.Length == 6 && Regex.IsMatch(upper, @"^[A-Z0-9]{6}$");
    }

    /// <summary>
    /// Wire this to the Join Button's OnClick event in the Inspector.
    /// Validates the entered code and navigates to Lobby if valid.
    /// </summary>
    public void OnJoinButtonPressed()
    {
        if (_hasNavigated) return;

        string raw = inputKode != null ? inputKode.text : string.Empty;

        if (!IsValidCode(raw))
        {
            if (feedbackText != null) feedbackText.text = "Kode harus 6 karakter (A-Z, 0-9).";
            return;
        }

        if (feedbackText != null) feedbackText.text = string.Empty;
        _hasNavigated = true;
        _sceneLoader.LoadScene("Lobby");
    }
}
