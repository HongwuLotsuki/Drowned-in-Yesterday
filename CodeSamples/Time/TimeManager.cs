using UnityEngine;
using System.Collections.Generic;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }
    public WaterController waterController;

    private HashSet<TimeMemory> _memoryObjects = new HashSet<TimeMemory>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }


    // Debug output lives in Start so every object has registered by then
    private void Start()
    {
        // 1. Fallback: find every TimeMemory in the scene so an OnEnable race cannot miss one
        var allMemories = FindObjectsByType<TimeMemory>(FindObjectsSortMode.None);
        foreach (var mem in allMemories)
        {
            Register(mem); // HashSet ignores duplicates, so this is safe
        }

        // 2. Log the final ready state
        Debug.Log($"<color=green>[Ready]</color> Registered TimeMemory objects: {_memoryObjects.Count}");
        foreach (var obj in _memoryObjects)
        {
            Debug.Log($"Registered: {obj.gameObject.name}");
        }
    }

    public void Register(TimeMemory memory) => _memoryObjects.Add(memory);
    public void Unregister(TimeMemory memory) => _memoryObjects.Remove(memory);

    void Update()
    {
        // F records the current state
        if (Input.GetKeyDown(KeyCode.F))
        {
            RecordAll();
            Debug.Log("<color=yellow>[Record]</color> Saved the current position of every object.");
        }

        // R rewinds to the recorded state
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("<color=cyan>[Rewind]</color> Rewinding time...");
            RestoreAll();
        }
    }

    private void RecordAll()
    {
        foreach (var obj in _memoryObjects) obj.SaveState();
    }

    private void RestoreAll()
    {
        foreach (var obj in _memoryObjects)
        {
            Vector3 targetPos = obj.GetRecordedPosition();
            float targetTime = obj.GetRecordedTime();

            // Check whether the recorded position is underwater at the recorded time
            if (waterController != null && waterController.IsUnderwater(targetPos, targetTime))
            {
                Debug.Log($"<color=red>[Blocked]</color> {obj.gameObject.name}: target {targetPos} is underwater, rewind rejected.");
                continue;
            }

            obj.RestoreState();
            Debug.Log($"<color=white>[OK]</color> {obj.gameObject.name} rewound to {targetPos}");
        }
    }
}