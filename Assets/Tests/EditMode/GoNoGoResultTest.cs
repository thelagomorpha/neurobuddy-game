using NUnit.Framework;

/// <summary>
/// EditMode tests for GoNoGoResult data class.
/// </summary>
public class GoNoGoResultTest
{
    [Test]
    public void Hit_GoTrialTapped_ShouldBeCorrect()
    {
        var result = new GoNoGoResult(isGoTrial: true, playerTapped: true, reactionTime: 0.5f, spawnTime: 1f);

        Assert.IsTrue(result.IsHit);
        Assert.IsFalse(result.IsMiss);
        Assert.IsFalse(result.IsFalseAlarm);
        Assert.IsFalse(result.IsCorrectRejection);
        Assert.IsTrue(result.IsCorrect);
    }

    [Test]
    public void Miss_GoTrialNotTapped_ShouldBeIncorrect()
    {
        var result = new GoNoGoResult(isGoTrial: true, playerTapped: false, reactionTime: -1f, spawnTime: 1f);

        Assert.IsFalse(result.IsHit);
        Assert.IsTrue(result.IsMiss);
        Assert.IsFalse(result.IsFalseAlarm);
        Assert.IsFalse(result.IsCorrectRejection);
        Assert.IsFalse(result.IsCorrect);
    }

    [Test]
    public void FalseAlarm_NoGoTrialTapped_ShouldBeIncorrect()
    {
        var result = new GoNoGoResult(isGoTrial: false, playerTapped: true, reactionTime: 0.3f, spawnTime: 1f);

        Assert.IsFalse(result.IsHit);
        Assert.IsFalse(result.IsMiss);
        Assert.IsTrue(result.IsFalseAlarm);
        Assert.IsFalse(result.IsCorrectRejection);
        Assert.IsFalse(result.IsCorrect);
    }

    [Test]
    public void CorrectRejection_NoGoTrialNotTapped_ShouldBeCorrect()
    {
        var result = new GoNoGoResult(isGoTrial: false, playerTapped: false, reactionTime: -1f, spawnTime: 1f);

        Assert.IsFalse(result.IsHit);
        Assert.IsFalse(result.IsMiss);
        Assert.IsFalse(result.IsFalseAlarm);
        Assert.IsTrue(result.IsCorrectRejection);
        Assert.IsTrue(result.IsCorrect);
    }

    [Test]
    public void ReactionTime_ShouldStoreProvidedValue()
    {
        var result = new GoNoGoResult(isGoTrial: true, playerTapped: true, reactionTime: 0.42f, spawnTime: 2f);

        Assert.AreEqual(0.42f, result.ReactionTime, 0.001f);
    }

    [Test]
    public void SpawnTime_ShouldStoreProvidedValue()
    {
        var result = new GoNoGoResult(isGoTrial: true, playerTapped: true, reactionTime: 0.5f, spawnTime: 3.5f);

        Assert.AreEqual(3.5f, result.SpawnTime, 0.001f);
    }

    [Test]
    public void NotTapped_ReactionTimeShouldBeNegativeOne()
    {
        var result = new GoNoGoResult(isGoTrial: true, playerTapped: false, reactionTime: -1f, spawnTime: 1f);

        Assert.AreEqual(-1f, result.ReactionTime, 0.001f);
    }
}
