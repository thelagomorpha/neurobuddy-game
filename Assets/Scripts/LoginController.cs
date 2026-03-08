using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles student login on the Login scene.
/// Attach to a GameObject, wire all serialized fields in the Inspector.
/// </summary>
public class LoginController : MonoBehaviour
{
    [SerializeField] private InputField usernameField;
    [SerializeField] private InputField passwordField;
    [SerializeField] private Text feedbackText;
    [SerializeField] private Button loginButton;

    private IAuthService _authService;
    private ISceneLoader _sceneLoader;
    private bool _isLoading;

    /// <summary>
    /// Inject a custom auth service (used in tests).
    /// </summary>
    public void SetAuthService(IAuthService service)
    {
        _authService = service;
    }

    /// <summary>
    /// Inject a custom scene loader (used in tests).
    /// </summary>
    public void SetSceneLoader(ISceneLoader loader)
    {
        _sceneLoader = loader;
    }

    private void Awake()
    {
        _authService ??= new AuthService();
        _sceneLoader ??= new UnitySceneLoader();
        if (passwordField != null)
            passwordField.inputType = InputField.InputType.Password;
    }

    /// <summary>
    /// Wire this to the Login Button's OnClick event in the Inspector.
    /// Validates fields, calls the auth service, and navigates to Menu on success.
    /// </summary>
    public void OnLoginButtonPressed()
    {
        if (_isLoading) return;

        string username = usernameField != null ? usernameField.text : string.Empty;
        string password = passwordField != null ? passwordField.text : string.Empty;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            if (feedbackText != null) feedbackText.text = "Nama pengguna dan kata sandi wajib diisi.";
            return;
        }

        _isLoading = true;
        if (loginButton != null) loginButton.interactable = false;
        if (feedbackText != null) feedbackText.text = "Memuat...";

        _authService.Login(username, password, this, OnLoginSuccess, OnLoginError);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        TokenStore.Save(result.access, result.refresh);
        _isLoading = false;
        if (loginButton != null) loginButton.interactable = true;
        _sceneLoader.LoadScene("Menu");
    }

    private void OnLoginError(string error)
    {
        _isLoading = false;
        if (loginButton != null) loginButton.interactable = true;
        if (feedbackText != null) feedbackText.text = "Login gagal. Periksa nama pengguna dan kata sandi Anda.";
    }
}
