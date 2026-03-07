using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// TDD tests for SessionCodeEntry.
/// Covers validation logic and navigation behaviour.
/// </summary>
public class SessionCodeEntryTest
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

    private (SessionCodeEntry entry, InputField inputField, MockSceneLoader mock, GameObject go, GameObject canvasGo)
        CreateEntry()
    {
        var canvasGo = new GameObject("Canvas");
        canvasGo.AddComponent<Canvas>();

        var inputGo = new GameObject("InputKode");
        inputGo.transform.SetParent(canvasGo.transform);
        var inputField = inputGo.AddComponent<InputField>();

        var textGo = new GameObject("Text");
        textGo.transform.SetParent(inputGo.transform);
        var textComp = textGo.AddComponent<Text>();
        inputField.textComponent = textComp;

        var go = new GameObject("TestEntry");
        var entry = go.AddComponent<SessionCodeEntry>();

        typeof(SessionCodeEntry)
            .GetField("inputKode", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(entry, inputField);

        var mock = new MockSceneLoader();
        entry.SetSceneLoader(mock);

        return (entry, inputField, mock, go, canvasGo);
    }

    // -----------------------------------------------------------------------
    // IsValidCode — pure static logic tests
    // -----------------------------------------------------------------------

    [Test]
    public void IsValidCode_Exactly6AlphanumericUppercase_ReturnsTrue()
    {
        Assert.IsTrue(SessionCodeEntry.IsValidCode("ABC123"));
    }

    [Test]
    public void IsValidCode_Exactly6AlphanumericLowercase_ReturnsTrue()
    {
        Assert.IsTrue(SessionCodeEntry.IsValidCode("abc123"));
    }

    [Test]
    public void IsValidCode_AllDigits_ReturnsTrue()
    {
        Assert.IsTrue(SessionCodeEntry.IsValidCode("123456"));
    }

    [Test]
    public void IsValidCode_AllLetters_ReturnsTrue()
    {
        Assert.IsTrue(SessionCodeEntry.IsValidCode("ABCDEF"));
    }

    [Test]
    public void IsValidCode_FewerThan6Chars_ReturnsFalse()
    {
        Assert.IsFalse(SessionCodeEntry.IsValidCode("AB12"));
    }

    [Test]
    public void IsValidCode_MoreThan6Chars_ReturnsFalse()
    {
        Assert.IsFalse(SessionCodeEntry.IsValidCode("ABCD1234"));
    }

    [Test]
    public void IsValidCode_ContainsSpace_ReturnsFalse()
    {
        Assert.IsFalse(SessionCodeEntry.IsValidCode("AB 123"));
    }

    [Test]
    public void IsValidCode_ContainsSymbol_ReturnsFalse()
    {
        Assert.IsFalse(SessionCodeEntry.IsValidCode("AB@123"));
    }

    [Test]
    public void IsValidCode_EmptyString_ReturnsFalse()
    {
        Assert.IsFalse(SessionCodeEntry.IsValidCode(string.Empty));
    }

    [Test]
    public void IsValidCode_NullString_ReturnsFalse()
    {
        Assert.IsFalse(SessionCodeEntry.IsValidCode(null));
    }

    // -----------------------------------------------------------------------
    // OnJoinButtonPressed — navigation behaviour tests
    // -----------------------------------------------------------------------

    [Test]
    public void OnJoinButtonPressed_ValidCode_NavigatesToLobby()
    {
        var (entry, inputField, mock, go, canvasGo) = CreateEntry();
        inputField.text = "ABC123";

        entry.OnJoinButtonPressed();

        Assert.AreEqual("Lobby", mock.LastLoadedScene);
        Object.DestroyImmediate(go);
        Object.DestroyImmediate(canvasGo);
    }

    [Test]
    public void OnJoinButtonPressed_ValidLowercaseCode_NavigatesToLobby()
    {
        var (entry, inputField, mock, go, canvasGo) = CreateEntry();
        inputField.text = "abc123";

        entry.OnJoinButtonPressed();

        Assert.AreEqual("Lobby", mock.LastLoadedScene);
        Object.DestroyImmediate(go);
        Object.DestroyImmediate(canvasGo);
    }

    [Test]
    public void OnJoinButtonPressed_TooShortCode_DoesNotNavigate()
    {
        var (entry, inputField, mock, go, canvasGo) = CreateEntry();
        inputField.text = "AB12";

        entry.OnJoinButtonPressed();

        Assert.AreEqual(0, mock.LoadCallCount);
        Object.DestroyImmediate(go);
        Object.DestroyImmediate(canvasGo);
    }

    [Test]
    public void OnJoinButtonPressed_TooLongCode_DoesNotNavigate()
    {
        var (entry, inputField, mock, go, canvasGo) = CreateEntry();
        inputField.text = "ABCD1234";

        entry.OnJoinButtonPressed();

        Assert.AreEqual(0, mock.LoadCallCount);
        Object.DestroyImmediate(go);
        Object.DestroyImmediate(canvasGo);
    }

    [Test]
    public void OnJoinButtonPressed_CodeWithSpace_DoesNotNavigate()
    {
        var (entry, inputField, mock, go, canvasGo) = CreateEntry();
        inputField.text = "AB 123";

        entry.OnJoinButtonPressed();

        Assert.AreEqual(0, mock.LoadCallCount);
        Object.DestroyImmediate(go);
        Object.DestroyImmediate(canvasGo);
    }

    [Test]
    public void OnJoinButtonPressed_CodeWithSymbol_DoesNotNavigate()
    {
        var (entry, inputField, mock, go, canvasGo) = CreateEntry();
        inputField.text = "AB@123";

        entry.OnJoinButtonPressed();

        Assert.AreEqual(0, mock.LoadCallCount);
        Object.DestroyImmediate(go);
        Object.DestroyImmediate(canvasGo);
    }

    [Test]
    public void OnJoinButtonPressed_ValidCode_CallsLoadSceneExactlyOnce()
    {
        var (entry, inputField, mock, go, canvasGo) = CreateEntry();
        inputField.text = "XYZ999";

        entry.OnJoinButtonPressed();

        Assert.AreEqual(1, mock.LoadCallCount);
        Object.DestroyImmediate(go);
        Object.DestroyImmediate(canvasGo);
    }

    [Test]
    public void OnJoinButtonPressed_CalledTwiceWithValidCode_NavigatesOnlyOnce()
    {
        var (entry, inputField, mock, go, canvasGo) = CreateEntry();
        inputField.text = "XYZ999";

        entry.OnJoinButtonPressed();
        entry.OnJoinButtonPressed();

        Assert.AreEqual(1, mock.LoadCallCount, "Navigation should only fire once even if pressed again.");
        Object.DestroyImmediate(go);
        Object.DestroyImmediate(canvasGo);
    }
}
