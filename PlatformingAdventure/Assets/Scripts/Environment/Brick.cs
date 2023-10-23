using UnityEngine;

public class Brick : MonoBehaviour
{
    [SerializeField] ParticleSystem _brickParticles;
    [SerializeField] float _laserDestructionTime = 1f;
    SpriteRenderer _spriteRenderer;
    float _takenDamageTime;
    float _resetColorTime;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        var player = collision.gameObject.GetComponent<Player>();
        if (player == null) return;

        Vector2 normal = collision.contacts[0].normal;
        float dot = Vector2.Dot(normal, Vector2.up);
        Debug.Log(dot);

        if (dot > 0.5f)
        {
            player.StopJump();
            Explode();
        }
    }

    void Explode()
    {
        Instantiate(_brickParticles, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    public void TakeLaserDamage()
    {
        _spriteRenderer.color = Color.red;
        _resetColorTime = Time.time + 0.1f;
        _takenDamageTime += Time.deltaTime;
        if (_takenDamageTime > _laserDestructionTime) Explode();
    }

    void Update()
    {

        if (_resetColorTime > 0 && Time.time >= _resetColorTime)
        {
            _resetColorTime = 0;
            _spriteRenderer.color = Color.white;
        }
    }
}
