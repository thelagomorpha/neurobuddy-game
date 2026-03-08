using System;
using UnityEngine;

/// <summary>
/// Result payload returned by the backend on successful login.
/// Field names must match the JSON keys from POST /api/auth/login/.
/// </summary>
[Serializable]
public class LoginResult
{
    public string access;
    public string refresh;
    public string role;
}

/// <summary>
/// Abstraction for the authentication HTTP call.
/// Implement this interface to allow unit tests to mock network requests.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Attempt to log in with the given credentials.
    /// </summary>
    /// <param name="username">Username entered by the student.</param>
    /// <param name="password">Password entered by the student.</param>
    /// <param name="coroutineHost">MonoBehaviour used to start the HTTP coroutine (pass the calling component).</param>
    /// <param name="onSuccess">Invoked with the login result when authentication succeeds.</param>
    /// <param name="onError">Invoked with an error message string when authentication fails.</param>
    void Login(string username, string password, MonoBehaviour coroutineHost,
               Action<LoginResult> onSuccess, Action<string> onError);
}
