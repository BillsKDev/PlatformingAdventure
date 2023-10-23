using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] Vector2 _position1;
    [SerializeField] Vector2 _position2;
    [SerializeField] float _speed = 1f;

    [Range(0f, 1f)]
    [SerializeField] float _percentAcross;

    [ContextMenu(nameof(SetPosition1))] public void SetPosition1() => _position1 = transform.position;
    [ContextMenu(nameof(SetPosition2))] public void SetPosition2() => _position2 = transform.position;

    void Update()
    {
        _percentAcross = Mathf.PingPong(Time.time * _speed, 1);
        transform.position = Vector3.Lerp(_position1, _position2, _percentAcross);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        var collider = GetComponent<BoxCollider2D>();
        Gizmos.DrawWireCube(_position1, collider.bounds.size);
        Gizmos.DrawWireCube(_position2, collider.bounds.size);

        Gizmos.color = Color.yellow;
        var currentPosition = Vector3.Lerp(_position1, _position2, _percentAcross);
        Gizmos.DrawWireCube(currentPosition, collider.bounds.size);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        var player = collision.gameObject.GetComponent<Player>();

        if (player != null)
        {
            player.transform.SetParent(transform);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        var player = collision.gameObject.GetComponent<Player>();

        if (player != null)
        {
            player.transform.SetParent(null);
        }
    }
}
