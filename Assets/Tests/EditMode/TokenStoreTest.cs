using NUnit.Framework;

/// <summary>
/// TDD tests for TokenStore.
/// Covers saving, retrieving, and clearing JWT tokens via PlayerPrefs.
/// </summary>
public class TokenStoreTest
{
    [TearDown]
    public void TearDown()
    {
        TokenStore.Clear();
    }

    [Test]
    public void HasToken_WhenNeverSaved_ReturnsFalse()
    {
        Assert.IsFalse(TokenStore.HasToken());
    }

    [Test]
    public void GetAccess_WhenNeverSaved_ReturnsEmpty()
    {
        Assert.AreEqual(string.Empty, TokenStore.GetAccess());
    }

    [Test]
    public void GetRefresh_WhenNeverSaved_ReturnsEmpty()
    {
        Assert.AreEqual(string.Empty, TokenStore.GetRefresh());
    }

    [Test]
    public void Save_ThenGetAccess_ReturnsAccessToken()
    {
        TokenStore.Save("access-token-abc", "refresh-token-xyz");

        Assert.AreEqual("access-token-abc", TokenStore.GetAccess());
    }

    [Test]
    public void Save_ThenGetRefresh_ReturnsRefreshToken()
    {
        TokenStore.Save("access-token-abc", "refresh-token-xyz");

        Assert.AreEqual("refresh-token-xyz", TokenStore.GetRefresh());
    }

    [Test]
    public void Save_MakesHasTokenTrue()
    {
        TokenStore.Save("access-token-abc", "refresh-token-xyz");

        Assert.IsTrue(TokenStore.HasToken());
    }

    [Test]
    public void Clear_AfterSave_MakesHasTokenFalse()
    {
        TokenStore.Save("access-token-abc", "refresh-token-xyz");

        TokenStore.Clear();

        Assert.IsFalse(TokenStore.HasToken());
    }

    [Test]
    public void Clear_AfterSave_GetAccessReturnsEmpty()
    {
        TokenStore.Save("access-token-abc", "refresh-token-xyz");

        TokenStore.Clear();

        Assert.AreEqual(string.Empty, TokenStore.GetAccess());
    }

    [Test]
    public void Clear_AfterSave_GetRefreshReturnsEmpty()
    {
        TokenStore.Save("access-token-abc", "refresh-token-xyz");

        TokenStore.Clear();

        Assert.AreEqual(string.Empty, TokenStore.GetRefresh());
    }

    [Test]
    public void Save_Twice_OverwritesWithLatestValues()
    {
        TokenStore.Save("first-access", "first-refresh");
        TokenStore.Save("second-access", "second-refresh");

        Assert.AreEqual("second-access", TokenStore.GetAccess());
        Assert.AreEqual("second-refresh", TokenStore.GetRefresh());
    }
}
