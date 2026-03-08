using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles deep links of the form  neurobuddy://join?session=XXXXXX
/// Attach to a persistent GameObject in the very first scene (e.g. a "Bootstrap" scene
/// or any scene that loads before "Menu").  The component survives scene transitions via
/// DontDestroyOnLoad and loads the Menu scene automatically when a valid session code is
/// received so that SessionCodeEntry can auto-fill the field from PlayerPrefs.
/// </summary>
public class DeepLinkHandler : MonoBehaviour
{
    public const string PendingSessionKey = "pending_session_code";

    private static DeepLinkHandler _instance;

#if UNITY_EDITOR
    [SerializeField] private string _debugUrl = "neurobuddy://join?session=TESTCD";

    [ContextMenu("Simulate Deep Link")]
    private void SimulateDeepLink()
    {
        HandleUrl(_debugUrl);
    }
#endif

    private void Awake()
    {
        // Singleton – destroy duplicates if the scene is reloaded
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        Application.deepLinkActivated += OnDeepLinkActivated;

        // Cold-start: app was launched directly via the deep link
        if (!string.IsNullOrEmpty(Application.absoluteURL))
        {
            HandleUrl(Application.absoluteURL);
        }
    }

    private void OnDestroy()
    {
        Application.deepLinkActivated -= OnDeepLinkActivated;
    }

    private void OnDeepLinkActivated(string url)
    {
        HandleUrl(url);
    }

    /// <summary>
    /// Parses the session code from the URL and navigates to the Menu scene.
    /// Supports both  neurobuddy://join?session=CODE  and
    /// https://yourdomain.com/join?session=CODE  formats.
    /// </summary>
    internal void HandleUrl(string url)
    {
        if (string.IsNullOrEmpty(url)) return;

        string sessionCode = ParseSessionCode(url);

        if (string.IsNullOrEmpty(sessionCode) || !SessionCodeEntry.IsValidCode(sessionCode))
        {
            Debug.LogWarning($"[DeepLinkHandler] Invalid or missing session code in URL: {url}");
            return;
        }

        Debug.Log($"[DeepLinkHandler] Session code received via deep link: {sessionCode}");

        PlayerPrefs.SetString(PendingSessionKey, sessionCode.ToUpperInvariant());
        PlayerPrefs.Save();

        SceneManager.LoadScene("Menu");
    }

    /// <summary>
    /// Extracts the value of the "session" query parameter from a URL string.
    /// Returns null if the parameter is not found.
    /// </summary>
    internal static string ParseSessionCode(string url)
    {
        if (string.IsNullOrEmpty(url)) return null;

        int queryStart = url.IndexOf('?');
        if (queryStart < 0) return null;

        string query = url.Substring(queryStart + 1);
        string[] pairs = query.Split('&');

        foreach (string pair in pairs)
        {
            string[] kv = pair.Split(new[] { '=' }, 2);
            if (kv.Length == 2 && kv[0] == "session")
            {
                return Uri.UnescapeDataString(kv[1]);
            }
        }

        return null;
    }
}
