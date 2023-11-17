using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

public class Fish : MonoBehaviour
{
    [SerializeField] SplineAnimate _splineAnimate;
    [SerializeField] Animator _animator;
    [SerializeField] SplineAttackPoints _splineAttackPoints;
    [SerializeField] int _spread = 15;
    [SerializeField] int _origin = 0;
    [SerializeField] float _fireSpeed = 5f;

    float _nextAttackPoint;
    int _spikeCount = 5;
    Queue<float> _attackPoints;

    void Start()
    {
        GetComponentInChildren<ShootAnimationWrapper>().OnShoot += ShootSpikes;
        RefreshAttackPoints();
    }

    void RefreshAttackPoints()
    {
        _attackPoints = _splineAttackPoints.GetAsQueue();
        _nextAttackPoint = _attackPoints.Dequeue();
    }

    void ShootSpikes()
    {
        for (int i = 0; i < _spikeCount; i++)
        {
            var angle = i - (_spikeCount / 2);
            var offset = _spread * angle;
            var finalAngle = _origin + offset;
            var spike = PoolManager.Instance.GetSpike();
            spike.transform.SetPositionAndRotation(transform.position, Quaternion.Euler(0, 0, finalAngle));
            spike.GetComponent<Rigidbody2D>().velocity = spike.transform.right * _fireSpeed;
        }
    }

    void Update()
    {
        var elapsed = _splineAnimate.NormalizedTime % 1;
        if (elapsed >= _nextAttackPoint)
        {
            _animator.SetTrigger("Attack");
            if (_attackPoints.Any())
                _nextAttackPoint = _attackPoints.Dequeue();
            else
                RefreshAttackPoints();
        }
    }
}
