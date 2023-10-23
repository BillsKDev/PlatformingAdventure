using TMPro;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] Vector2 _direction = Vector2.left;
    [SerializeField] float _distance = 10f;
    LineRenderer _lineRenderer;
    bool _isOn;

    void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        Toggle(false);
    }

    public void Toggle(bool state)
    {
        _isOn = state;
        _lineRenderer.enabled = state;
    }

    void Update()
    {
        if (_isOn == false) return;

        var endPoint = (Vector2)transform.position + (_direction * _distance);

        RaycastHit2D firstThing = Physics2D.Raycast(transform.position, _direction, _distance);
        if (firstThing.collider != null)
        {
            endPoint = firstThing.point;
            var brick = firstThing.collider.GetComponent<Brick>();
            if (brick != null)
            {
                brick.TakeLaserDamage();
            }
        }
        _lineRenderer.SetPosition(1, endPoint);
    }
}
