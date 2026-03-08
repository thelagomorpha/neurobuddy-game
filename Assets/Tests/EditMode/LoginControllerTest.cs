using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// TDD tests for LoginController.
/// Covers field validation, auth service invocation, token storage, navigation, and error handling.
/// </summary>
public class LoginControllerTest
{
    private class MockSceneLoader : ISceneLoader
    {
        public string LastLoadedScene { get; private set; }
        public int LoadCallCount { get; private set; }

        public void LoadScene(string sceneName)
        {
            LastLoadedScene = sceneName;
            LoadCallCount++;
        }
    }

    private class MockAuthService : IAuthService
    {
        public string LastUsername { get; private set; }
        public string LastPassword { get; private set; }
        public int CallCount { get; private set; }

        private bool _callbackEnabled = true;
        private bool _succeed;
        private LoginResult _result;
        private string _errorMessage;

        public MockAuthService ConfigureSuccess(LoginResult result)
        {
            _succeed = true;
            _result = result;
            return this;
        }

        public MockAuthService ConfigureError(string message)
        {
            _succeed = false;
            _errorMessage = message;
            return this;
        }

        public MockAuthService ConfigureDeferred()
        {
            _callbackEnabled = false;
            return this;
        }

        public void Login(string username, string password, MonoBehaviour coroutineHost,
                          System.Action<LoginResult> onSuccess, System.Action<string> onError)
        {
            LastUsername = username;
            LastPassword = password;
            CallCount++;

            if (!_callbackEnabled) return;

            if (_succeed)
                onSuccess(_result);
            else
                onError(_errorMessage);
        }
    }

    private (LoginController controller, InputField usernameField, InputField passwordField,
             Text feedbackText, Button loginButton, MockAuthService mockAuth,
             MockSceneLoader mockScene, GameObject go, GameObject canvasGo)
        CreateController()
    {
        var canvasGo = new GameObject("Canvas");
        canvasGo.AddComponent<Canvas>();

        var usernameGo = new GameObject("UsernameField");
        usernameGo.transform.SetParent(canvasGo.transform);
        var usernameField = usernameGo.AddComponent<InputField>();
        var usernameTextGo = new GameObject("Text");
        usernameTextGo.transform.SetParent(usernameGo.transform);
        usernameField.textComponent = usernameTextGo.AddComponent<Text>();

        var passwordGo = new GameObject("PasswordField");
        passwordGo.transform.SetParent(canvasGo.transform);
        var passwordField = passwordGo.AddComponent<InputField>();
        var passwordTextGo = new GameObject("PasswordText");
        passwordTextGo.transform.SetParent(passwordGo.transform);
        passwordField.textComponent = passwordTextGo.AddComponent<Text>();

        var feedbackGo = new GameObject("FeedbackText");
        feedbackGo.transform.SetParent(canvasGo.transform);
        var feedbackText = feedbackGo.AddComponent<Text>();

        var buttonGo = new GameObject("LoginButton");
        buttonGo.transform.SetParent(canvasGo.transform);
        var loginButton = buttonGo.AddComponent<Button>();

        var go = new GameObject("LoginController");
        var controller = go.AddComponent<LoginController>();

        var type = typeof(LoginController);
        type.GetField("usernameField", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(controller, usernameField);
        type.GetField("passwordField", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(controller, passwordField);
        type.GetField("feedbackText", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(controller, feedbackText);
        type.GetField("loginButton", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(controller, loginButton);

        var mockAuth = new MockAuthService();
        var mockScene = new MockSceneLoader();
        controller.SetAuthService(mockAuth);
        controller.SetSceneLoader(mockScene);

        return (controller, usernameField, passwordField, feedbackText, loginButton, mockAuth, mockScene, go, canvasGo);
    }

    [TearDown]
    public void TearDown()
    {
        TokenStore.Clear();
    }

    // -----------------------------------------------------------------------
    // Field validation tests
    // -----------------------------------------------------------------------

    [Test]
    public void OnLoginButtonPressed_EmptyUsername_ShowsValidationFeedback()
    {
        var (controller, _, passwordField, feedbackText, _, mockAuth, _, go, canvasGo) = CreateController();
        passwordField.text = "secret";

        controller.OnLoginButtonPressed();

        Assert.AreEqual("Nama pengguna dan kata sandi wajib diisi.", feedbackText.text);
        Object.DestroyImmediate(go);
        Object.DestroyImmediate(canvasGo);
    }

    [Test]
    public void OnLoginButtonPressed_EmptyUsername_DoesNotCallAuthService()
    {
        var (controller, _, passwordField, _, _, mockAuth, _, go, canvasGo) = CreateController();
        passwordField.text = "secret";

        controller.OnLoginButtonPressed();

        Assert.AreEqual(0, mockAuth.CallCount);
        Object.DestroyImmediate(go);
        Object.DestroyImmediate(canvasGo);
    }

    [Test]
    public void OnLoginButtonPressed_EmptyPassword_ShowsValidationFeedback()
    {
        var (controller, usernameField, _, feedbackText, _, mockAuth, _, go, canvasGo) = CreateController();
        usernameField.text = "student1";

        controller.OnLoginButtonPressed();

        Assert.AreEqual("Nama pengguna dan kata sandi wajib diisi.", feedbackText.text);
        Object.DestroyImmediate(go);
        Object.DestroyImmediate(canvasGo);
    }

    [Test]
    public void OnLoginButtonPressed_EmptyPassword_DoesNotCallAuthService()
    {
        var (controller, usernameField, _, _, _, mockAuth, _, go, canvasGo) = CreateController();
        usernameField.text = "student1";

        controller.OnLoginButtonPressed();

        Assert.AreEqual(0, mockAuth.CallCount);
        Object.DestroyImmediate(go);
        Object.DestroyImmediate(canvasGo);
    }

    // -----------------------------------------------------------------------
    // Auth service invocation tests
    // -----------------------------------------------------------------------

    [Test]
    public void OnLoginButtonPressed_ValidCredentials_CallsAuthServiceWithCorrectArgs()
    {
        var (controller, usernameField, passwordField, _, _, mockAuth, _, go, canvasGo) = CreateController();
        mockAuth.ConfigureDeferred();
        usernameField.text = "student1";
        passwordField.text = "pass123";

        controller.OnLoginButtonPressed();

        Assert.AreEqual("student1", mockAuth.LastUsername);
        Assert.AreEqual("pass123", mockAuth.LastPassword);
        Assert.AreEqual(1, mockAuth.CallCount);
        Object.DestroyImmediate(go);
        Object.DestroyImmediate(canvasGo);
    }

    // -----------------------------------------------------------------------
    // Success path tests
    // -----------------------------------------------------------------------

    [Test]
    public void OnLoginSuccess_SavesAccessTokenToTokenStore()
    {
        var (controller, usernameField, passwordField, _, _, mockAuth, _, go, canvasGo) = CreateController();
        mockAuth.ConfigureSuccess(new LoginResult { access = "test-access", refresh = "test-refresh", role = "student" });
        usernameField.text = "student1";
        passwordField.text = "pass123";

        controller.OnLoginButtonPressed();

        Assert.AreEqual("test-access", TokenStore.GetAccess());
        Object.DestroyImmediate(go);
        Object.DestroyImmediate(canvasGo);
    }

    [Test]
    public void OnLoginSuccess_SavesRefreshTokenToTokenStore()
    {
        var (controller, usernameField, passwordField, _, _, mockAuth, _, go, canvasGo) = CreateController();
        mockAuth.ConfigureSuccess(new LoginResult { access = "test-access", refresh = "test-refresh", role = "student" });
        usernameField.text = "student1";
        passwordField.text = "pass123";

        controller.OnLoginButtonPressed();

        Assert.AreEqual("test-refresh", TokenStore.GetRefresh());
        Object.DestroyImmediate(go);
        Object.DestroyImmediate(canvasGo);
    }

    [Test]
    public void OnLoginSuccess_NavigatesToMenuScene()
    {
        var (controller, usernameField, passwordField, _, _, mockAuth, mockScene, go, canvasGo) = CreateController();
        mockAuth.ConfigureSuccess(new LoginResult { access = "test-access", refresh = "test-refresh", role = "student" });
        usernameField.text = "student1";
        passwordField.text = "pass123";

        controller.OnLoginButtonPressed();

        Assert.AreEqual("Menu", mockScene.LastLoadedScene);
        Object.DestroyImmediate(go);
        Object.DestroyImmediate(canvasGo);
    }

    [Test]
    public void OnLoginSuccess_NavigatesExactlyOnce()
    {
        var (controller, usernameField, passwordField, _, _, mockAuth, mockScene, go, canvasGo) = CreateController();
        mockAuth.ConfigureSuccess(new LoginResult { access = "test-access", refresh = "test-refresh", role = "student" });
        usernameField.text = "student1";
        passwordField.text = "pass123";

        controller.OnLoginButtonPressed();

        Assert.AreEqual(1, mockScene.LoadCallCount);
        Object.DestroyImmediate(go);
        Object.DestroyImmediate(canvasGo);
    }

    // -----------------------------------------------------------------------
    // Error path tests
    // -----------------------------------------------------------------------

    [Test]
    public void OnLoginError_ShowsIndonesianErrorMessage()
    {
        var (controller, usernameField, passwordField, feedbackText, _, mockAuth, _, go, canvasGo) = CreateController();
        mockAuth.ConfigureError("Unauthorized");
        usernameField.text = "student1";
        passwordField.text = "wrongpass";

        controller.OnLoginButtonPressed();

        Assert.AreEqual("Login gagal. Periksa nama pengguna dan kata sandi Anda.", feedbackText.text);
        Object.DestroyImmediate(go);
        Object.DestroyImmediate(canvasGo);
    }

    [Test]
    public void OnLoginError_DoesNotNavigate()
    {
        var (controller, usernameField, passwordField, _, _, mockAuth, mockScene, go, canvasGo) = CreateController();
        mockAuth.ConfigureError("Unauthorized");
        usernameField.text = "student1";
        passwordField.text = "wrongpass";

        controller.OnLoginButtonPressed();

        Assert.AreEqual(0, mockScene.LoadCallCount);
        Object.DestroyImmediate(go);
        Object.DestroyImmediate(canvasGo);
    }

    [Test]
    public void OnLoginError_DoesNotSaveTokens()
    {
        var (controller, usernameField, passwordField, _, _, mockAuth, _, go, canvasGo) = CreateController();
        mockAuth.ConfigureError("Unauthorized");
        usernameField.text = "student1";
        passwordField.text = "wrongpass";

        controller.OnLoginButtonPressed();

        Assert.IsFalse(TokenStore.HasToken());
        Object.DestroyImmediate(go);
        Object.DestroyImmediate(canvasGo);
    }

    // -----------------------------------------------------------------------
    // Double-submit guard test
    // -----------------------------------------------------------------------

    [Test]
    public void OnLoginButtonPressed_WhileLoading_DoesNotCallAuthServiceAgain()
    {
        var (controller, usernameField, passwordField, _, _, mockAuth, _, go, canvasGo) = CreateController();
        mockAuth.ConfigureDeferred(); // callbacks never fire, so _isLoading stays true
        usernameField.text = "student1";
        passwordField.text = "pass123";

        controller.OnLoginButtonPressed(); // sets _isLoading = true
        controller.OnLoginButtonPressed(); // should be blocked by guard

        Assert.AreEqual(1, mockAuth.CallCount);
        Object.DestroyImmediate(go);
        Object.DestroyImmediate(canvasGo);
    }

    // Password masking test
    // -----------------------------------------------------------------------

    [Test]
    public void Awake_SetsPasswordFieldInputTypeToPassword()
    {
        var (controller, _, passwordField, _, _, _, _, go, canvasGo) = CreateController();

        // In the real game, Unity sets serialized fields before Awake fires.
        // In Edit Mode tests, AddComponent triggers Awake before reflection injection,
        // so we re-invoke Awake after injection; ??= operators preserve the existing mocks.
        typeof(LoginController)
            .GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance)
            .Invoke(controller, null);

        Assert.AreEqual(InputField.InputType.Password, passwordField.inputType);

        Object.DestroyImmediate(go);
        Object.DestroyImmediate(canvasGo);
    }
}
