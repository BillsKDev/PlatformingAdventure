using UnityEngine;
using UnityEngine.Events;

public class LaserSwitch : MonoBehaviour
{
    [SerializeField] Sprite _left;
    [SerializeField] Sprite _right;

    [SerializeField] UnityEvent _on;
    [SerializeField] UnityEvent _off;

    SpriteRenderer _spriteRenderer;
    Laser _laser;
    bool _isOn;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        var player = collision.GetComponent<Player>();

        if (player == null) return;

        var rigidBody = player.GetComponent<Rigidbody2D>();

        if (rigidBody.velocity.x > 0)
            TurnOn();
        else if (rigidBody.velocity.x < 0)
            TurnOff();
    }

    void TurnOn()
    {
        if (_isOn)
        {
            _isOn = false;
            _spriteRenderer.sprite = _right;
            _on.Invoke();
        }
    }

    void TurnOff()
    {
        if (_isOn == false)
        {
            _isOn = true;
            _spriteRenderer.sprite = _left;
            _off.Invoke();
        }
    }
}
