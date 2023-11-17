using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : MonoBehaviour
{
    [SerializeField] BlasterShot _blasterShotPrefab;
    [SerializeField] ReturnToPool _blasterImpactExplosionPrefab;

    public static PoolManager Instance { get; private set; }

    ObjectPool<BlasterShot> _blasterShotPool;
    ObjectPool<ReturnToPool> _blasterImpactExplosionPool;
    ObjectPool<ReturnToPool> _spikePool;

    void Awake()
    {
        Instance = this;
        _blasterShotPool = new ObjectPool<BlasterShot>(AddNewBlasterShotToPool,
            t => t.gameObject.SetActive(true),
            t => t.gameObject.SetActive(false));

        _blasterImpactExplosionPool = new ObjectPool<ReturnToPool>(
            () =>
            {
                var shot = Instantiate(_blasterImpactExplosionPrefab);
                shot.SetPool(_blasterImpactExplosionPool);
                return shot;
            },
            t => t.gameObject.SetActive(true),
            t => t.gameObject.SetActive(false));

    }

    BlasterShot AddNewBlasterShotToPool()
    {
        var shot = Instantiate(_blasterShotPrefab);
        shot.SetPool(_blasterShotPool);
        return shot;
    }

    public BlasterShot GetBlasterShot()
    {
        return _blasterShotPool.Get();
    }

    public ReturnToPool GetBlasterExplosion(Vector2 point)
    {
        var explosion = _blasterImpactExplosionPool.Get();
        explosion.transform.position = point;
        return explosion;
    }
}