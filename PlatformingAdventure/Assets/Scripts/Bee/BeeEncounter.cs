using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BeeEncounter : MonoBehaviour
{
    [SerializeField] List<Transform> _lightnings;
    [SerializeField] float _delayBeforeDamage = 1.5f;
    [SerializeField] float _lightningAnimationTime = 2f;
    [SerializeField] float _delayBetweenLightning = 1f;
    [SerializeField] float _delayBetweenStrikes = 0.25f;
    [SerializeField] float _lightningRadius = 1f;
    [SerializeField] int _numberOfLightnings = 1;
    [SerializeField] LayerMask _playerLayer;

    Collider2D[] _playerHitResults = new Collider2D[10];
    List<Transform> _activeLightnings;

    void OnValidate()
    {
        if (_lightningAnimationTime <= _delayBeforeDamage)
            _delayBeforeDamage = _lightningAnimationTime;
    }

    void OnEnable() => StartCoroutine(StartEncounter());

    IEnumerator StartEncounter()
    {
        foreach (var lightnings in _lightnings)
            lightnings.gameObject.SetActive(false);

        _activeLightnings = new List<Transform>();
        while (true)
        {
            for (int i = 0; i < _numberOfLightnings; i++)
                yield return SpawnNewLightning();

            yield return new WaitUntil(() => _activeLightnings.All(t => !t.gameObject.activeSelf));
            _activeLightnings.Clear();
        }
    }

    IEnumerator SpawnNewLightning()
    {
        if (_activeLightnings.Count >= _lightnings.Count)
            yield break;

        int index = Random.Range(0, _lightnings.Count);
        var lightning = _lightnings[index];

        while (_activeLightnings.Contains(lightning))
        {
            index = Random.Range(0, _lightnings.Count);
            lightning = _lightnings[index];
        }

        StartCoroutine(ShowLightning(lightning));
        _activeLightnings.Add(lightning);

        yield return new WaitForSeconds(_delayBetweenStrikes);
    }

    IEnumerator ShowLightning(Transform lightning)
    {
        lightning.gameObject.SetActive(true);
        yield return new WaitForSeconds(_delayBeforeDamage);
        DamagePlayersInRange(lightning);
        yield return new WaitForSeconds(_lightningAnimationTime - _delayBeforeDamage);
        lightning.gameObject.SetActive(false);
        yield return new WaitForSeconds(_delayBetweenLightning);
    }

    void DamagePlayersInRange(Transform lightning)
    {
        var hits = Physics2D.OverlapCircleNonAlloc(lightning.position, _lightningRadius, _playerHitResults, _playerLayer);

        for (int i = 0; i < hits; i++)
        {
            _playerHitResults[i].GetComponent<Player>().TakeDamage(Vector3.zero);
        }
    }
}
