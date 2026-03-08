using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Real implementation of IAuthService that calls the backend login endpoint
/// using UnityWebRequest. The HTTP call is run as a coroutine on the provided host.
/// </summary>
public class AuthService : IAuthService
{
    [Serializable]
    private class LoginRequest
    {
        public string username;
        public string password;
    }

    /// <inheritdoc />
    public void Login(string username, string password, MonoBehaviour coroutineHost,
                      Action<LoginResult> onSuccess, Action<string> onError)
    {
        coroutineHost.StartCoroutine(LoginCoroutine(username, password, onSuccess, onError));
    }

    private IEnumerator LoginCoroutine(string username, string password,
                                       Action<LoginResult> onSuccess, Action<string> onError)
    {
        string url = $"{GameConfig.BaseUrl}/api/auth/login/";
        string body = JsonUtility.ToJson(new LoginRequest { username = username, password = password });

        var req = new UnityWebRequest(url, "POST");
        req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(body));
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            LoginResult result = JsonUtility.FromJson<LoginResult>(req.downloadHandler.text);
            onSuccess(result);
        }
        else
        {
            onError(req.error);
        }
    }
}
