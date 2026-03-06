using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

/// <summary>
/// PlayMode test for UnitySceneLoader — verifies LoadScene actually loads a scene
/// via SceneManager. Cannot be tested in EditMode because SceneManager.LoadScene
/// requires PlayMode.
/// </summary>
public class UnitySceneLoaderTest
{
    [UnityTest]
    public IEnumerator LoadScene_ShouldLoadTargetScene()
    {
        // Arrange
        var loader = new UnitySceneLoader();
        var targetScene = "Level1";

        // Act
        loader.LoadScene(targetScene);
        yield return null; // wait one frame for scene to load

        // Assert
        Assert.AreEqual(targetScene, SceneManager.GetActiveScene().name);
    }
}
