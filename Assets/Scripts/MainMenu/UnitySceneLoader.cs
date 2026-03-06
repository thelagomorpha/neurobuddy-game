using UnityEngine.SceneManagement;

/// <summary>
/// Real implementation of ISceneLoader that uses Unity's SceneManager.
/// Used at runtime; tests use a mock instead.
/// </summary>
public class UnitySceneLoader : ISceneLoader
{
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
