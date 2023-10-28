using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] float _jumpVelocity = 5f;
    [SerializeField] float _jumpDuration = 0.5f;
    [SerializeField] float _maxHorizontalSpeed = 5f;
    [SerializeField] float _footOffset = 0.35f;
    [SerializeField] float _acceleration = 10f;
    [SerializeField] float _snowAcceleration = 1f;
    [SerializeField] float _knockBackVelocity = 400f;
    [SerializeField] Sprite _jumpSprite;
    [SerializeField] LayerMask _layerMask;
    [SerializeField] AudioClip _coinSFX;
    [SerializeField] AudioClip _hurtSFX;

    public int Coins { get => _playerData.Coins; private set => _playerData.Coins = value; }
    public int Health { get => _playerData.Health; }
    public Vector2 Direction { get; private set; } = Vector2.right;

    public event Action CoinsChanged;
    public event Action HealthChanged;

    Rigidbody2D _rb;
    SpriteRenderer _spriteRenderer;
    Animator _animator;
    AudioSource _audioSource;
    PlayerInput _playerInput;
    PlayerData _playerData = new PlayerData();

    float _jumpEndTime;
    float _horizontal;
    int _jumpsRemaining;
    bool IsGrounded;
    bool IsOnSnow;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        Vector2 origin = new Vector2(transform.position.x, transform.position.y - spriteRenderer.bounds.extents.y);
        Gizmos.DrawLine(origin, origin + Vector2.down * 0.1f);

        //Draw left foot
        origin = new Vector2(transform.position.x - _footOffset, transform.position.y - spriteRenderer.bounds.extents.y);
        Gizmos.DrawLine(origin, origin + Vector2.down * 0.1f);

        //Draw right foot
        origin = new Vector2(transform.position.x + _footOffset, transform.position.y - spriteRenderer.bounds.extents.y);
        Gizmos.DrawLine(origin, origin + Vector2.down * 0.1f);
    }

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponentInChildren<Animator>();
        _audioSource = GetComponent<AudioSource>();
        _playerInput = GetComponent<PlayerInput>();

        FindObjectOfType<PlayerCanvas>().Bind(this);
    }

    void Update()
    {
        UpdateGrounding();

        var horizontalInput = _playerInput.actions["Move"].ReadValue<Vector2>().x;
        var vertical = _rb.velocity.y;

        if (_playerInput.actions["Jump"].WasPerformedThisFrame() && _jumpsRemaining > 0)
        {
            _jumpEndTime = Time.time + _jumpDuration;
            _jumpsRemaining--;
            _audioSource.pitch = _jumpsRemaining > 0 ? 1 : 1.2f;
            _audioSource.Play();
        }

        if (_playerInput.actions["Jump"].ReadValue<float>() > 0 && _jumpEndTime > Time.time)
            vertical = _jumpVelocity;

        var desiredHorizontal = horizontalInput * _maxHorizontalSpeed;
        var groundAcceleration = IsOnSnow ? _snowAcceleration : _acceleration;

        //_horizontal = Mathf.Lerp(_horizontal, desiredHorizontal, Time.deltaTime * groundAcceleration);

        if (desiredHorizontal > _horizontal)
        {
            _horizontal += _acceleration * Time.deltaTime;
            if (_horizontal > desiredHorizontal)
                _horizontal = desiredHorizontal;
        }
        else if (desiredHorizontal < _horizontal)
        {
            _horizontal -= _acceleration * Time.deltaTime;
            if (_horizontal < desiredHorizontal)
                _horizontal = desiredHorizontal;
        }
        _rb.velocity = new Vector2(_horizontal, vertical);

        UpdateAnimation();
        UpdateDirection();
    }

    void UpdateGrounding()
    {
        IsGrounded = false;
        IsOnSnow = false;

        //Check center
        Vector2 origin = new Vector2(transform.position.x, transform.position.y - _spriteRenderer.bounds.extents.y);
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, 0.1f, _layerMask);
        if (hit.collider)
        {
            IsGrounded = true;
            IsOnSnow = hit.collider.CompareTag("Snow");
        }

        //Check left
        origin = new Vector2(transform.position.x - _footOffset, transform.position.y - _spriteRenderer.bounds.extents.y);
        hit = Physics2D.Raycast(origin, Vector2.down, 0.1f, _layerMask);
        if (hit.collider)
        {
            IsGrounded = true;
            IsOnSnow = hit.collider.CompareTag("Snow");
        }

        //Check right
        origin = new Vector2(transform.position.x + _footOffset, transform.position.y - _spriteRenderer.bounds.extents.y);
        hit = Physics2D.Raycast(origin, Vector2.down, 0.1f, _layerMask);
        if (hit.collider)
        {
            IsGrounded = true;
            IsOnSnow = hit.collider.CompareTag("Snow");
        }

        if (IsGrounded && _rb.velocity.y <= 0)
            _jumpsRemaining = 2;

    }

    void UpdateAnimation()
    {
        _animator.SetBool("Jump", !IsGrounded);
        _animator.SetBool("Move", _horizontal != 0);
    }

    private void UpdateDirection()
    {
        if (_horizontal > 0)
        {
            _animator.transform.rotation = Quaternion.identity;
            Direction = Vector2.right;
        }
        else if (_horizontal < 0)
        {
            _animator.transform.rotation = Quaternion.Euler(0, 180, 0);
            Direction = Vector2.left;
        }
    }

    public void AddPoint()
    {
        Coins++;
        CoinsChanged?.Invoke();
        _audioSource.PlayOneShot(_coinSFX);
    }

    public void Bind(PlayerData playerData)
    {
        _playerData = playerData;
    }

    public void TakeDamage(Vector2 hitNormal)
    {
        _playerData.Health--;

        if (_playerData.Health <= 0)
        {
            SceneManager.LoadScene(0);
            return;
        }
        HealthChanged?.Invoke();
        _rb.AddForce(-hitNormal * _knockBackVelocity);
        _audioSource.PlayOneShot(_hurtSFX);
    }

    public void StopJump()
    {
        _jumpEndTime = Time.time;
    }

    public void Bounce(Vector2 normal, float bounciness)
    {
        _rb.AddForce(-normal * bounciness);
    }
}
