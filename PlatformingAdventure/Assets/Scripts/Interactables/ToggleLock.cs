using UnityEngine;

public class ToggleLock : MonoBehaviour
{
    SpriteRenderer _spriteRenderer;
    bool _unlocked;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _unlocked = false;
        _spriteRenderer.color = Color.gray;
    }

    public void Toggle()
    {
        _unlocked = !_unlocked;
        _spriteRenderer.color = _unlocked ? Color.white : Color.gray;
    }
}
