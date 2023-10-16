using UnityEngine;

public class Frog : MonoBehaviour
{
    [SerializeField] float _jumpDelay = 3f;
    [SerializeField] int _jumps = 2;
    [SerializeField] Vector2 _jumpForce;
    [SerializeField] Sprite _jumpSprite;

    Rigidbody2D _rb;
    SpriteRenderer _spriteRenderer;
    Sprite _defaultSprite;
    AudioSource _audioSource;

    int _jumpsRemaining;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rb = GetComponent<Rigidbody2D>();
        _audioSource = GetComponent<AudioSource>();
        _defaultSprite = _spriteRenderer.sprite;
        _jumpsRemaining = _jumps;

        InvokeRepeating("Jump", _jumpDelay, _jumpDelay);
    }

    void Jump()
    {
        if (_jumpsRemaining <= 0)
        {
            _jumpForce *= new Vector2(-1, 1);
            _jumpsRemaining = _jumps;
        }
        _jumpsRemaining--;

        _rb.AddForce(_jumpForce);
        _spriteRenderer.flipX = _jumpForce.x > 0;
        _spriteRenderer.sprite = _jumpSprite;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        _spriteRenderer.sprite = _defaultSprite;
        _audioSource.Play();
    }
}
