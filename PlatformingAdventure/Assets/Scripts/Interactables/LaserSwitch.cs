using System;
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
    LaserSwitchData _data;

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
        if (_data.IsOn)
        {
            _data.IsOn = false;
            UpdateSwitchState();
        }
    }

    void TurnOff()
    {
        if (_data.IsOn == false)
        {
            _data.IsOn = true;
            UpdateSwitchState();
        }
    }

    

    public void Bind(LaserSwitchData data)
    {
        _data = data;
        UpdateSwitchState();
    }

    void UpdateSwitchState()
    {
        if (_data.IsOn)
        {
            _spriteRenderer.sprite = _left;
            _on.Invoke();
        }
        else
        {
            _spriteRenderer.sprite = _left;
            _off.Invoke();
        }
    }
}
