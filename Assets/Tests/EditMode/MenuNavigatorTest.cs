using NUnit.Framework;
using System.Reflection;

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

    [Test]
    public void GoToLevel1_ShouldLoadLevel1Scene()
    {
        var mockLoader = new MockSceneLoader();
        var navigator = new UnityEngine.GameObject().AddComponent<MenuNavigator>();
        navigator.SetSceneLoader(mockLoader);

        navigator.GoToLevel1();

        Assert.AreEqual("Level1", mockLoader.LastLoadedScene);
        Assert.AreEqual(1, mockLoader.LoadCallCount);

        UnityEngine.Object.DestroyImmediate(navigator.gameObject);
    }

    [Test]
    public void GoToLevel2_ShouldLoadLevel2Scene()
    {
        var mockLoader = new MockSceneLoader();
        var navigator = new UnityEngine.GameObject().AddComponent<MenuNavigator>();
        navigator.SetSceneLoader(mockLoader);

        navigator.GoToLevel2();

        Assert.AreEqual("Level2", mockLoader.LastLoadedScene);
        Assert.AreEqual(1, mockLoader.LoadCallCount);

        UnityEngine.Object.DestroyImmediate(navigator.gameObject);
    }

    [Test]
    public void GoToLevel3_ShouldLoadLevel3Scene()
    {
        var mockLoader = new MockSceneLoader();
        var navigator = new UnityEngine.GameObject().AddComponent<MenuNavigator>();
        navigator.SetSceneLoader(mockLoader);

        navigator.GoToLevel3();

        Assert.AreEqual("Level3", mockLoader.LastLoadedScene);
        Assert.AreEqual(1, mockLoader.LoadCallCount);

        UnityEngine.Object.DestroyImmediate(navigator.gameObject);
    }

    [Test]
    public void Awake_ShouldInitializeDefaultSceneLoader_WhenNoneInjected()
    {
        // Arrange — create navigator without calling SetSceneLoader
        var navigator = new UnityEngine.GameObject().AddComponent<MenuNavigator>();

        // Manually invoke Awake (EditMode may not auto-call it)
        navigator.SendMessage("Awake");

        // Assert — use reflection to verify private _sceneLoader field
        var field = typeof(MenuNavigator).GetField("_sceneLoader",
            BindingFlags.NonPublic | BindingFlags.Instance);
        var loader = field.GetValue(navigator);

        Assert.IsNotNull(loader);
        Assert.IsInstanceOf<UnitySceneLoader>(loader);

        UnityEngine.Object.DestroyImmediate(navigator.gameObject);
    }

    [Test]
    public void Awake_ShouldNotOverrideInjectedSceneLoader()
    {
        // Arrange — inject mock BEFORE Awake
        var mockLoader = new MockSceneLoader();
        var navigator = new UnityEngine.GameObject().AddComponent<MenuNavigator>();
        navigator.SetSceneLoader(mockLoader);

        // Act — call Awake again (should NOT overwrite the mock thanks to ??=)
        navigator.SendMessage("Awake");

        // Assert — _sceneLoader should still be the mock
        var field = typeof(MenuNavigator).GetField("_sceneLoader",
            BindingFlags.NonPublic | BindingFlags.Instance);
        var loader = field.GetValue(navigator);

        Assert.AreSame(mockLoader, loader);

        UnityEngine.Object.DestroyImmediate(navigator.gameObject);
    }
}
