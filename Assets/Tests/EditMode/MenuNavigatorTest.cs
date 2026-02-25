using NUnit.Framework;

/// <summary>
/// TDD Test for MenuNavigator — tests that GoToLobby() loads the "Lobby" scene.
/// Uses a mock ISceneLoader to avoid actually loading scenes in EditMode.
/// </summary>
public class MenuNavigatorTest
{
    /// <summary>
    /// Simple mock that records which scene was loaded.
    /// </summary>
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

    [Test]
    public void GoToLobby_ShouldLoadLobbyScene()
    {
        // Arrange
        var mockLoader = new MockSceneLoader();
        var navigator = new UnityEngine.GameObject().AddComponent<MenuNavigator>();
        navigator.SetSceneLoader(mockLoader);

        // Act
        navigator.GoToLobby();

        // Assert
        Assert.AreEqual("Lobby", mockLoader.LastLoadedScene);

        // Cleanup
        UnityEngine.Object.DestroyImmediate(navigator.gameObject);
    }

    [Test]
    public void GoToLobby_ShouldCallLoadSceneExactlyOnce()
    {
        // Arrange
        var mockLoader = new MockSceneLoader();
        var navigator = new UnityEngine.GameObject().AddComponent<MenuNavigator>();
        navigator.SetSceneLoader(mockLoader);

        // Act
        navigator.GoToLobby();

        // Assert
        Assert.AreEqual(1, mockLoader.LoadCallCount);

        // Cleanup
        UnityEngine.Object.DestroyImmediate(navigator.gameObject);
    }

    [Test]
    public void GoToLobby_CalledTwice_ShouldStillLoadLobby()
    {
        // Arrange
        var mockLoader = new MockSceneLoader();
        var navigator = new UnityEngine.GameObject().AddComponent<MenuNavigator>();
        navigator.SetSceneLoader(mockLoader);

        // Act
        navigator.GoToLobby();
        navigator.GoToLobby();

        // Assert
        Assert.AreEqual("Lobby", mockLoader.LastLoadedScene);
        Assert.AreEqual(2, mockLoader.LoadCallCount);

        // Cleanup
        UnityEngine.Object.DestroyImmediate(navigator.gameObject);
    }
}
