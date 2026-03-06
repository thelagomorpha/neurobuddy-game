using UnityEngine;

/// <summary>
/// Handles navigation from the Menu scene.
/// Attach to a GameObject in the Menu scene and wire up button OnClick to GoToLobby().
/// </summary>
public class MenuNavigator : MonoBehaviour
{
    private ISceneLoader _sceneLoader;

    /// <summary>
    /// Inject a custom scene loader (used for testing).
    /// </summary>
    public void SetSceneLoader(ISceneLoader loader)
    {
        _sceneLoader = loader;
    }

    private void Awake()
    {
        // Use the real Unity scene loader if none was injected
        _sceneLoader ??= new UnitySceneLoader();
    }

    /// <summary>
    /// Loads the Lobby scene. Wire this to the Start Button's OnClick event.
    /// </summary>
    public void GoToLobby()
    {
        _sceneLoader.LoadScene("Lobby");
    }

    public void GoToLevel1()
    {
        _sceneLoader.LoadScene("Level1");
    }

    public void GoToLevel2()
    {
        _sceneLoader.LoadScene("Level2");
    }

    public void GoToLevel3()
    {
        _sceneLoader.LoadScene("Level3");
    }
}
