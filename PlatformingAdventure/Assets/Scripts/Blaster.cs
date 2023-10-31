using UnityEngine;
using UnityEngine.InputSystem;

public class Blaster : MonoBehaviour
{
    [SerializeField] Transform _firePoint;

    PlayerInput _playerInput;
    Player _player;
    Animator _animator;

    void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _player = GetComponent<Player>();
        _animator = GetComponentInChildren<Animator>();
        _playerInput.actions["Fire"].performed += TryFire;
    }

    void TryFire(InputAction.CallbackContext context)
    {
        Fire();
    }

    void Fire()
    {
        BlasterShot shot = PoolManager.Instance.GetBlasterShot();
        shot.Launch(_player.Direction, _firePoint.position);
        _animator.SetTrigger("Fire");
    }

    void Update()
    {
        //if (_playerInput.actions["Fire"].ReadValue<float>() > 0)
          //  Fire();
    }
}
