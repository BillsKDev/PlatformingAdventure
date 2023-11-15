using UnityEngine;
using UnityEngine.InputSystem;

public class Key : MonoBehaviour
{
    [SerializeField] float _useRange = 1f;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var playerInventory = collision.GetComponent<PlayerInventory>();
            if (playerInventory != null)
            {
                playerInventory.Pickup(this);
                
            }
        }
    }

    public void Use()
    {
        var hits = Physics2D.OverlapCircleAll(transform.position, _useRange);
        foreach (var hit in hits)
        {
            var toggleLock = hit.GetComponent<ToggleLock>();

            if (toggleLock)
            {
                toggleLock.Toggle();
                break;
            }
        }
    }
}
