using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class GoNoGoItemTest
{
    private GoNoGoManager CreateManager()
    {
        var go = new GameObject("Manager");
        return go.AddComponent<GoNoGoManager>();
    }

    private GoNoGoItem CreateItem(GoNoGoManager manager)
    {
        var go = new GameObject("Item");
        go.transform.SetParent(manager.transform);
        go.AddComponent<Button>();
        return go.AddComponent<GoNoGoItem>();
    }

    [Test]
    public void Initialize_ShouldSetProperties()
    {
        var manager = CreateManager();
        var item = CreateItem(manager);

        item.Initialize(true, 1.5f, manager);

        Assert.IsTrue(item.IsGoItem);
        Assert.IsFalse(item.WasTapped);

        Object.DestroyImmediate(manager.gameObject);
    }

    [Test]
    public void OnTapped_ShouldRecordResultAndDestroy()
    {
        var manager = CreateManager();
        var item = CreateItem(manager);
        item.Initialize(true, 1.5f, manager);

        item.OnTapped();

        Assert.AreEqual(1, manager.Results.Count);
        Assert.IsTrue(manager.Results[0].IsGoTrial);
        Assert.IsTrue(manager.Results[0].PlayerTapped);

        Object.DestroyImmediate(manager.gameObject);
    }

    [Test]
    public void OnTapped_WhenAlreadyTapped_ShouldDoNothing()
    {
        var manager = CreateManager();
        var item = CreateItem(manager);
        item.Initialize(true, 1.5f, manager);

        // Use reflection to simulate it already being tapped without causing destruction
        var field = typeof(GoNoGoItem).GetField("_wasTapped", BindingFlags.NonPublic | BindingFlags.Instance);
        field.SetValue(item, true);

        item.OnTapped(); // Should do nothing

        Assert.AreEqual(0, manager.Results.Count); // Should not record anything

        Object.DestroyImmediate(manager.gameObject);
    }

    [Test]
    public void Update_WhenExpired_ShouldRecordMissAndDestroy()
    {
        var manager = CreateManager();
        var item = CreateItem(manager);
        item.Initialize(true, 0.5f, manager);

        // Use reflection to set spawn time to the past
        var field = typeof(GoNoGoItem).GetField("_spawnTime", BindingFlags.NonPublic | BindingFlags.Instance);
        field.SetValue(item, Time.time - 1.0f); // 1 second ago, duration is 0.5

        // Call Update
        var updateMethod = typeof(GoNoGoItem).GetMethod("Update", BindingFlags.NonPublic | BindingFlags.Instance);
        updateMethod.Invoke(item, null);

        Assert.AreEqual(1, manager.Results.Count);
        Assert.IsFalse(manager.Results[0].PlayerTapped); // Because it expired without tap

        Object.DestroyImmediate(manager.gameObject);
    }

    [Test]
    public void Update_WhenNotExpired_ShouldDoNothing()
    {
        var manager = CreateManager();
        var item = CreateItem(manager);
        item.Initialize(true, 5.0f, manager);

        // It spawned at Time.time, duration is 5s. Calling update now should do nothing.
        var updateMethod = typeof(GoNoGoItem).GetMethod("Update", BindingFlags.NonPublic | BindingFlags.Instance);
        updateMethod.Invoke(item, null);

        Assert.AreEqual(0, manager.Results.Count);

        Object.DestroyImmediate(manager.gameObject);
    }
}
