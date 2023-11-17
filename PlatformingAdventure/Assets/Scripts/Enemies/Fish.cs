using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

public class Fish : MonoBehaviour
{
    [SerializeField] SplineAnimate _splineAnimate;
    [SerializeField] Animator _animator;
    [SerializeField] SplineAttackPoints _splineAttackPoints;

    float _nextAttackPoint;
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
