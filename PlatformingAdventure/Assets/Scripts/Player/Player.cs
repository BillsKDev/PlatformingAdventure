using Cinemachine;
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
    [SerializeField] float _wallDetectionDistance = 0.5f;
    [SerializeField] float _groundDetectionOffset = 1.01f;
    [SerializeField] float _buffer = 0.1f;
    [SerializeField] int _wallCheckPoints = 3;
    [SerializeField] LayerMask _layerMask;
    [SerializeField] LayerMask _waterLayerMask;
    [SerializeField] Collider2D _standingCollider;
    [SerializeField] Collider2D _duckCollider;

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
    RaycastHit2D[] _results = new RaycastHit2D[100];

    float _jumpEndTime;
    float _horizontal;
    int _jumpsRemaining;
    bool IsGrounded;
    bool IsOnSnow;
    bool IsInWater;
    bool IsDucking;
    bool IsTouchingRightWall;
    bool IsTouchingLeftWall;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        Vector2 origin = new Vector2(transform.position.x, transform.position.y - _groundDetectionOffset);
        Gizmos.DrawLine(origin, origin + Vector2.down * 0.1f);

        //Draw left foot
        origin = new Vector2(transform.position.x - _footOffset, transform.position.y - _groundDetectionOffset);
        Gizmos.DrawLine(origin, origin + Vector2.down * 0.1f);

        //Draw right foot
        origin = new Vector2(transform.position.x + _footOffset, transform.position.y - _groundDetectionOffset);
        Gizmos.DrawLine(origin, origin + Vector2.down * 0.1f);

        DrawGizmosForSide(Vector2.left);
        DrawGizmosForSide(Vector2.right);
    }

    void DrawGizmosForSide(Vector2 direction)
    {
        var activeCollider = IsDucking ? _duckCollider : _standingCollider;
        float colliderHeight = activeCollider.bounds.size.y - 2 * _buffer;
        float segmentSize = colliderHeight / (_wallCheckPoints - 1);

        for (int i = 0; i < _wallCheckPoints; i++)
        {
            var origin = transform.position - new Vector3(0, activeCollider.bounds.size.y / 2f, 0);
            origin += new Vector3(0, _buffer + segmentSize * i, 0);
            origin += (Vector3)direction * _wallDetectionDistance;
            Gizmos.DrawWireSphere(origin, 0.05f);
        }
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

    void OnEnable() => FindObjectOfType<CinemachineTargetGroup>()?.AddMember(transform, 1f, 1f);
    void OnDisable() => FindObjectOfType<CinemachineTargetGroup>()?.RemoveMember(transform);

    void Update()
    {
        if (GameManager.IsLoading) return;

        UpdateGrounding();
        UpdateWallTouching();

        if (GameManager.CinematicPlaying == false)
            UpdateMovement();

        UpdateAnimation();
        UpdateDirection();

        _playerData.Position = _rb.position;
        _playerData.Velocity = _rb.velocity;
    }

    bool CheckForWall(Vector2 direction)
    {
        var activeCollider = IsDucking ? _duckCollider : _standingCollider;
        float colliderHeight = activeCollider.bounds.size.y - 2 * _buffer;
        float segmentSize = colliderHeight / (_wallCheckPoints - 1);

        for (int i = 0; i < _wallCheckPoints; i++)
        {
            var origin = transform.position - new Vector3(0, activeCollider.bounds.size.y / 2f, 0);
            origin += new Vector3(0, _buffer + segmentSize * i, 0);
            origin += (Vector3)direction * _wallDetectionDistance;

            int hits = Physics2D.Raycast(origin, direction, new ContactFilter2D() { layerMask = _layerMask, useLayerMask = true }, _results, 0.1f);

            for (int hitIndex = 0; hitIndex < hits; hitIndex++)
            {
                var hit = _results[hitIndex];
                if (hit.collider && hit.collider.isTrigger == false) return true;
            }

        }
        return false;
    }

    void UpdateWallTouching()
    {
        IsTouchingRightWall = CheckForWall(Vector2.right);
        IsTouchingLeftWall = CheckForWall(Vector2.left);
    }

    void UpdateMovement()
    {
        var input = _playerInput.actions["Move"].ReadValue<Vector2>();
        var horizontalInput = input.x;
        var verticalInput = input.y;

        var vertical = _rb.velocity.y;

        if (_playerInput.actions["Jump"].WasPerformedThisFrame() && _jumpsRemaining > 0)
        {
            _jumpEndTime = Time.time + _jumpDuration;
            _jumpsRemaining--;
        }

        if (_playerInput.actions["Jump"].ReadValue<float>() > 0 && _jumpEndTime > Time.time)
            vertical = _jumpVelocity;

        var desiredHorizontal = horizontalInput * _maxHorizontalSpeed;
        var acceleration = IsOnSnow ? _snowAcceleration : _acceleration;

        _animator.SetBool("Duck", verticalInput < -0 && Mathf.Abs(verticalInput) > Mathf.Abs(horizontalInput));

        IsDucking = _animator.GetBool("Duck");
        if (IsDucking)
            desiredHorizontal = 0;

        _duckCollider.enabled = IsDucking;
        _standingCollider.enabled = !IsDucking;

        //_horizontal = Mathf.Lerp(_horizontal, desiredHorizontal, Time.deltaTime * groundAcceleration);

        if (desiredHorizontal > _horizontal)
        {
            _horizontal += acceleration * Time.deltaTime;
            if (_horizontal > desiredHorizontal)
                _horizontal = desiredHorizontal;
        }
        else if (desiredHorizontal < _horizontal)
        {
            _horizontal -= acceleration * Time.deltaTime;
            if (_horizontal < desiredHorizontal)
                _horizontal = desiredHorizontal;
        }

        if (desiredHorizontal > 0 && IsTouchingRightWall)
            _horizontal = 0;
        if (desiredHorizontal < 0 && IsTouchingLeftWall)
            _horizontal = 0;

        if (IsInWater)
            _rb.velocity = new Vector2(_rb.velocity.x, vertical);
        else
            _rb.velocity = new Vector2(_horizontal, vertical);
    }

    void UpdateGrounding()
    {
        IsGrounded = false;
        IsOnSnow = false;
        IsInWater = false;

        //Check center
        Vector2 origin = new Vector2(transform.position.x, transform.position.y - _groundDetectionOffset);
        CheckGrounding(origin);

        //Check left
        origin = new Vector2(transform.position.x - _footOffset, transform.position.y - _groundDetectionOffset);
        CheckGrounding(origin);

        //Check right
        origin = new Vector2(transform.position.x + _footOffset, transform.position.y - _groundDetectionOffset);
        CheckGrounding(origin);

        if ((IsGrounded || IsInWater) && _rb.velocity.y <= 0)
            _jumpsRemaining = 2;

    }

    void CheckGrounding(Vector2 origin)
    {
        int hits = Physics2D.Raycast(origin, Vector2.down, new ContactFilter2D() { layerMask = _layerMask, useLayerMask = true, useTriggers = true }, _results, 0.1f);

        for (int i = 0; i < hits; i++)
        {
            var hit = _results[i];
            if (!hit.collider) continue;

            IsGrounded = true;
            IsOnSnow |= hit.collider.CompareTag("Snow");
        }

        var water = Physics2D.OverlapPoint(origin, _waterLayerMask);
        if (water != null) IsInWater = true;
    }

    void UpdateAnimation()
    {
        _animator.SetBool("Jump", !IsGrounded);
        _animator.SetBool("Move", _horizontal != 0);
    }

    void UpdateDirection()
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
    }

    public void Bind(PlayerData playerData)
    {
        _playerData = playerData;
    }

    public void RestorePositionAndVelocity()
    {
        _rb.position = _playerData.Position;
        _rb.velocity = _playerData.Velocity;
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
