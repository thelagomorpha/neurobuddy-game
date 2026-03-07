using NUnit.Framework;
using System.Reflection;

/// <summary>
/// EditMode tests for GoNoGoManager — tests result recording, summary calculations, and config defaults.
/// </summary>
public class GoNoGoManagerTest
{
    private GoNoGoManager CreateManager()
    {
        var go = new UnityEngine.GameObject();
        return go.AddComponent<GoNoGoManager>();
    }

    private void DestroyManager(GoNoGoManager manager)
    {
        UnityEngine.Object.DestroyImmediate(manager.gameObject);
    }

    [Test]
    public void RecordResult_ShouldAddToResultsList()
    {
        var manager = CreateManager();
        var result = new GoNoGoResult(true, true, 0.5f, 1f);

        manager.RecordResult(result);

        Assert.AreEqual(1, manager.Results.Count);
        Assert.AreSame(result, manager.Results[0]);

        DestroyManager(manager);
    }

    [Test]
    public void RecordResult_MultipleTimes_ShouldTrackAll()
    {
        var manager = CreateManager();

        manager.RecordResult(new GoNoGoResult(true, true, 0.3f, 1f));  // Hit
        manager.RecordResult(new GoNoGoResult(true, false, -1f, 2f));  // Miss
        manager.RecordResult(new GoNoGoResult(false, true, 0.4f, 3f)); // False Alarm
        manager.RecordResult(new GoNoGoResult(false, false, -1f, 4f)); // Correct Rejection

        Assert.AreEqual(4, manager.Results.Count);

        DestroyManager(manager);
    }

    [Test]
    public void GetHitCount_ShouldReturnCorrectCount()
    {
        var manager = CreateManager();

        manager.RecordResult(new GoNoGoResult(true, true, 0.3f, 1f));  // Hit
        manager.RecordResult(new GoNoGoResult(true, true, 0.4f, 2f));  // Hit
        manager.RecordResult(new GoNoGoResult(true, false, -1f, 3f));  // Miss
        manager.RecordResult(new GoNoGoResult(false, true, 0.2f, 4f)); // False Alarm

        Assert.AreEqual(2, manager.GetHitCount());

        DestroyManager(manager);
    }

    [Test]
    public void GetMissCount_ShouldReturnCorrectCount()
    {
        var manager = CreateManager();

        manager.RecordResult(new GoNoGoResult(true, false, -1f, 1f));  // Miss
        manager.RecordResult(new GoNoGoResult(true, true, 0.3f, 2f));  // Hit
        manager.RecordResult(new GoNoGoResult(false, false, -1f, 3f)); // Correct Rejection

        Assert.AreEqual(1, manager.GetMissCount());

        DestroyManager(manager);
    }

    [Test]
    public void GetFalseAlarmCount_ShouldReturnCorrectCount()
    {
        var manager = CreateManager();

        manager.RecordResult(new GoNoGoResult(false, true, 0.3f, 1f)); // False Alarm
        manager.RecordResult(new GoNoGoResult(false, true, 0.2f, 2f)); // False Alarm
        manager.RecordResult(new GoNoGoResult(true, true, 0.5f, 3f));  // Hit

        Assert.AreEqual(2, manager.GetFalseAlarmCount());

        DestroyManager(manager);
    }

    [Test]
    public void GetCorrectRejectionCount_ShouldReturnCorrectCount()
    {
        var manager = CreateManager();

        manager.RecordResult(new GoNoGoResult(false, false, -1f, 1f)); // Correct Rejection
        manager.RecordResult(new GoNoGoResult(true, true, 0.5f, 2f));  // Hit

        Assert.AreEqual(1, manager.GetCorrectRejectionCount());

        DestroyManager(manager);
    }

    [Test]
    public void GetAverageReactionTime_ShouldCalculateFromHitsOnly()
    {
        var manager = CreateManager();

        manager.RecordResult(new GoNoGoResult(true, true, 0.3f, 1f));  // Hit
        manager.RecordResult(new GoNoGoResult(true, true, 0.5f, 2f));  // Hit
        manager.RecordResult(new GoNoGoResult(false, true, 0.1f, 3f)); // False Alarm (excluded)
        manager.RecordResult(new GoNoGoResult(true, false, -1f, 4f));  // Miss (excluded)

        Assert.AreEqual(0.4f, manager.GetAverageReactionTime(), 0.001f);

        DestroyManager(manager);
    }

    [Test]
    public void GetAverageReactionTime_NoHits_ShouldReturnZero()
    {
        var manager = CreateManager();

        manager.RecordResult(new GoNoGoResult(true, false, -1f, 1f));  // Miss
        manager.RecordResult(new GoNoGoResult(false, false, -1f, 2f)); // Correct Rejection

        Assert.AreEqual(0f, manager.GetAverageReactionTime(), 0.001f);

        DestroyManager(manager);
    }

    [Test]
    public void DefaultConfig_ShouldHaveExpectedValues()
    {
        var manager = CreateManager();

        Assert.AreEqual(30f, manager.GameDuration, 0.001f);
        Assert.AreEqual(1.5f, manager.SpawnInterval, 0.001f);
        Assert.AreEqual(1.2f, manager.ItemDisplayDuration, 0.001f);
        Assert.AreEqual(0.7f, manager.GoRatio, 0.001f);

        DestroyManager(manager);
    }

    [Test]
    public void InitialState_ShouldNotBeActive()
    {
        var manager = CreateManager();

        Assert.IsFalse(manager.IsGameActive);
        Assert.AreEqual(0, manager.Results.Count);

        DestroyManager(manager);
    }
}
