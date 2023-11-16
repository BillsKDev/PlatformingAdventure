using UnityEngine;
using UnityEngine.Events;

public class EventTrigger : MonoBehaviour
{
    [SerializeField] UnityEvent OnEventTriggered;
    bool _triggered;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        if (_triggered == true) return;

        _triggered = true;
        OnEventTriggered?.Invoke();
    }
}
