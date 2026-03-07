/// <summary>
/// Data class that records the result of a single Go/No-Go trial.
/// </summary>
public class GoNoGoResult
{
    /// <summary>True if this was a Go (apple) trial, false if No-Go (fishbone).</summary>
    public bool IsGoTrial { get; private set; }

    /// <summary>True if the player tapped the item.</summary>
    public bool PlayerTapped { get; private set; }

    /// <summary>Reaction time in seconds from spawn to tap. -1 if not tapped.</summary>
    public float ReactionTime { get; private set; }

    /// <summary>Timestamp (Time.time) when the item spawned.</summary>
    public float SpawnTime { get; private set; }

    public GoNoGoResult(bool isGoTrial, bool playerTapped, float reactionTime, float spawnTime)
    {
        IsGoTrial = isGoTrial;
        PlayerTapped = playerTapped;
        ReactionTime = reactionTime;
        SpawnTime = spawnTime;
    }

    /// <summary>True if the player responded correctly (Hit or CorrectRejection).</summary>
    public bool IsCorrect => IsHit || IsCorrectRejection;

    /// <summary>Player tapped a Go item.</summary>
    public bool IsHit => IsGoTrial && PlayerTapped;

    /// <summary>Player failed to tap a Go item.</summary>
    public bool IsMiss => IsGoTrial && !PlayerTapped;

    /// <summary>Player tapped a No-Go item (error).</summary>
    public bool IsFalseAlarm => !IsGoTrial && PlayerTapped;

    /// <summary>Player correctly ignored a No-Go item.</summary>
    public bool IsCorrectRejection => !IsGoTrial && !PlayerTapped;
}