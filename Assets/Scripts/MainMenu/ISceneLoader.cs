/// <summary>
/// Interface for scene loading operations.
/// Allows mocking in tests without actually loading Unity scenes.
/// </summary>
public interface ISceneLoader
{
    void LoadScene(string sceneName);
}
