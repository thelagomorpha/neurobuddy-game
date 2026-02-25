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

    /// <summary>
    /// Loads the Lobby scene. Wire this to the Start Button's OnClick event.
    /// TODO: Implement scene loading (GREEN phase)
    /// </summary>
    public void GoToLobby()
    {
        // Not yet implemented — tests should FAIL (RED)
    }
}
