using System;
using UnityEngine;

public class Cat : MonoBehaviour, ITakeDamage
{
    [SerializeField] CatBomb _catBombPrefab;
    [SerializeField] Transform _firePoint;

    CatBomb _catBomb;

    void Start()
    {
        SpawnCatBomb();
        var shootAnimationWrapper = GetComponentInChildren<ShootAnimationWrapper>();
        shootAnimationWrapper.OnShoot += ShootCatBomb;
        shootAnimationWrapper.OnReload += SpawnCatBomb;
    }

    void ShootCatBomb()
    {
        _catBomb.Launch(Vector2.up + Vector2.left);
        _catBomb = null;
    }

    void SpawnCatBomb()
    {
        if (_catBomb == null)
            _catBomb = Instantiate(_catBombPrefab, _firePoint);
    }

    public void TakeDamage()
    {
        gameObject.SetActive(false);
    }
}
