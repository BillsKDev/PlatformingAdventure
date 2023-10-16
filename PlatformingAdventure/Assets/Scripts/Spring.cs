using UnityEngine;

public class Spring : MonoBehaviour
{
    [SerializeField] Sprite _sprung;
    
    SpriteRenderer _spriteRenderer;
    Sprite _defaultSprite;
    AudioSource _audioSource;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _defaultSprite = _spriteRenderer.sprite;
        _audioSource = GetComponent<AudioSource>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            _spriteRenderer.sprite = _sprung;
            _audioSource.Play();
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
            _spriteRenderer.sprite = _defaultSprite;
    }
}
