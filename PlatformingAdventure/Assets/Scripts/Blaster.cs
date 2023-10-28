using UnityEngine;
using UnityEngine.InputSystem;

public class Blaster : MonoBehaviour
{
    [SerializeField] BlasterShot _blasterShotPrefab;
    [SerializeField] Transform _firePoint;

    PlayerInput _playerInput;
    Player _player;

    void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _player = GetComponent<Player>();
        _playerInput.actions["Fire"].performed += TryFire;
    }

    void TryFire(InputAction.CallbackContext context)
    {
        BlasterShot shot = Instantiate(_blasterShotPrefab, _firePoint.transform.position, _firePoint.transform.rotation);
        shot.Launch(_player.Direction);
    }
}
