using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float _jumpVelocity = 5f;
    [SerializeField] float _jumpDuration = 0.5f;
    
    Rigidbody2D _rb;
    float _jumpEndTime;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = _rb.velocity.y;

        if (Input.GetButtonDown("Jump"))
            _jumpEndTime = Time.time + _jumpDuration;

        if (Input.GetButtonDown("Jump") && _jumpEndTime > Time.time)
            vertical = _jumpVelocity;

        _rb.velocity = new Vector2(horizontal, vertical);
    }
}
