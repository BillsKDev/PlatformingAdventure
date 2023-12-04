using UnityEngine;

public class Ladybug : MonoBehaviour
{
    [SerializeField] float _speed = 1f;
    [SerializeField] float _raycastDistance = 0.2f;

    Vector2 _direction = Vector2.left;

    SpriteRenderer _spriteRenderer;
    Collider2D _collider;
    Rigidbody2D _rigidbody;
    

    void Awake()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    void OnDrawGizmos()
    {
        var collider = GetComponent<Collider2D>();

        Vector2 offset = _direction * collider.bounds.extents.x;
        Vector2 origin = (Vector2)transform.position + offset;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(origin, origin + (_direction * _raycastDistance));

        var bounds = collider.bounds;

        if (_direction == Vector2.left)
        {
            Vector2 bottomLeft = new Vector2(bounds.center.x - bounds.extents.x, bounds.center.y - bounds.extents.y);
            Gizmos.DrawLine(bottomLeft, bottomLeft + (Vector2.down * _raycastDistance));
        }
        else
        {
            Vector2 bottomRight = new Vector2(bounds.center.x + bounds.extents.x, bounds.center.y - bounds.extents.y);
            Gizmos.DrawLine(bottomRight, bottomRight + (Vector2.down * _raycastDistance));
        }
    }

    void Update()
    {
        Vector2 offset = _direction * _collider.bounds.extents.x;
        Vector2 origin = (Vector2)transform.position + offset;

        var hits = Physics2D.RaycastAll(origin, _direction, _raycastDistance);
        foreach(var hit in hits)
        {
            if (hit.collider != null && hit.collider.gameObject != gameObject)
            {
                _direction *= -1;
                _spriteRenderer.flipX = _direction == Vector2.right;
                break;
            }
        }

        _rigidbody.velocity = new Vector2(_direction.x * _speed, _rigidbody.velocity.y);
    }
}