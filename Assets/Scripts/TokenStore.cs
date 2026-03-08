using UnityEngine;

/// <summary>
/// Persists and retrieves JWT tokens using Unity's PlayerPrefs.
/// Call Clear() on logout or when tokens become invalid.
/// </summary>
public static class TokenStore
{
    private const string AccessKey = "jwt_access";
    private const string RefreshKey = "jwt_refresh";

    /// <summary>
    /// Saves the access and refresh tokens to PlayerPrefs and flushes to disk.
    /// </summary>
    public static void Save(string access, string refresh)
    {
        PlayerPrefs.SetString(AccessKey, access);
        PlayerPrefs.SetString(RefreshKey, refresh);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Returns the stored JWT access token, or an empty string if none is saved.
    /// </summary>
    public static string GetAccess()
    {
        return PlayerPrefs.GetString(AccessKey, string.Empty);
    }

    /// <summary>
    /// Returns the stored JWT refresh token, or an empty string if none is saved.
    /// </summary>
    public static string GetRefresh()
    {
        return PlayerPrefs.GetString(RefreshKey, string.Empty);
    }

    /// <summary>
    /// Removes all stored tokens and flushes to disk.
    /// </summary>
    public static void Clear()
    {
        PlayerPrefs.DeleteKey(AccessKey);
        PlayerPrefs.DeleteKey(RefreshKey);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Returns true if a non-empty access token is currently stored.
    /// </summary>
    public static bool HasToken()
    {
        return !string.IsNullOrEmpty(PlayerPrefs.GetString(AccessKey, string.Empty));
    }
}
