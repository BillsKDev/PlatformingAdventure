using UnityEngine;

public class CatBomb : MonoBehaviour
{
    [SerializeField] float _forceAmount = 300f;

    Rigidbody2D _rb;
    Animator _animator;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.simulated = false;
        _animator = GetComponentInChildren<Animator>();
        _animator.enabled = false;
    }

    public void Launch(Vector2 direction)
    {
        transform.SetParent(null);
        _rb.simulated = true;
        _rb.AddForce(direction * _forceAmount);
        _animator.enabled = true;
        Destroy(gameObject, 3f);
    }
}
