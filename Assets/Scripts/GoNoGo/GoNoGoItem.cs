using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attached to each spawned Go/No-Go item.
/// Handles click detection, reaction time tracking, and auto-destruction.
/// </summary>
public class GoNoGoItem : MonoBehaviour
{
    private bool _isGoItem;
    private float _spawnTime;
    private float _displayDuration;
    private bool _wasTapped;
    private GoNoGoManager _manager;

    /// <summary>
    /// Initialize the item after instantiation.
    /// </summary>
    public void Initialize(bool isGoItem, float displayDuration, GoNoGoManager manager)
    {
        _isGoItem = isGoItem;
        _displayDuration = displayDuration;
        _manager = manager;
        _spawnTime = Time.time;
        _wasTapped = false;

        // Wire up button click
        var button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnTapped);
        }
    }

    /// <summary>
    /// Called when the player taps this item.
    /// </summary>
    public void OnTapped()
    {
        if (_wasTapped) return;
        _wasTapped = true;

        float reactionTime = Time.time - _spawnTime;
        var result = new GoNoGoResult(_isGoItem, true, reactionTime, _spawnTime);

        if (_manager != null)
        {
            _manager.RecordResult(result);
        }

        DestroySelf();
    }

    private void Update()
    {
        // Auto-destroy after display duration if not tapped
        if (Time.time - _spawnTime >= _displayDuration && !_wasTapped)
        {
            var result = new GoNoGoResult(_isGoItem, false, -1f, _spawnTime);

            if (_manager != null)
            {
                _manager.RecordResult(result);
            }

            DestroySelf();
        }
    }

    private void DestroySelf()
    {
        if (Application.isPlaying)
        {
            Destroy(gameObject);
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    // --- Testability accessors ---

    public bool IsGoItem => _isGoItem;
    public float SpawnTime => _spawnTime;
    public bool WasTapped => _wasTapped;
}