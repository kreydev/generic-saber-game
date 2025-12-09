using UnityEngine;
using System.Collections.Concurrent;
using MultiMouse;

public class SaberManager : MonoBehaviour
{
    public static SaberManager Instance { get; private set; }

    private MultiMouseManager manager;

    // Thread-safe queue for Unity main thread
    private readonly ConcurrentQueue<MouseEvent> eventQueue = new();

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        manager = new MultiMouseManager();
        manager.OnMouseEvent += e => eventQueue.Enqueue(e);

        manager.Initialize();
    }

    void Update()
    {
        // Drain event queue on Unity’s main thread
        while (eventQueue.TryDequeue(out var evt))
        {
            HandleMouseEvent(evt);
        }
    }

    private void HandleMouseEvent(MouseEvent evt)
    {
        // Example: Debug print device movement
        Debug.Log($"Device {evt.DeviceId}: ΔX={evt.DeltaX}, ΔY={evt.DeltaY}");

        // OPTIONAL: forward to gameplay systems
        MultiMouseRouter.Dispatch(evt);
    }

    void OnDestroy()
    {
        manager?.Dispose();
    }
}
