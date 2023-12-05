using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BeeEncounter : MonoBehaviour, ITakeDamage
{
    [SerializeField] List<Transform> _lightnings;
    [SerializeField] float _delayBeforeDamage = 1.5f;
    [SerializeField] float _lightningAnimationTime = 2f;
    [SerializeField] float _delayBetweenLightning = 1f;
    [SerializeField] float _delayBetweenStrikes = 0.25f;
    [SerializeField] float _lightningRadius = 1f;
    [SerializeField] float _minIdleTime = 1f;
    [SerializeField] float _maxIdleTime = 2f;
    [SerializeField] int _numberOfLightnings = 1;
    [SerializeField] int _maxHealth = 50;
    [SerializeField] GameObject _bee;
    [SerializeField] GameObject _beeLaser;
    [SerializeField] Water _water;
    [SerializeField] Animator _animator;
    [SerializeField] Rigidbody2D _beeRigidBody;
    [SerializeField] Collider2D _floodGroundCollider;
    [SerializeField] LayerMask _playerLayer;
    [SerializeField] Transform[] _beeDestinations;
    [SerializeField] Image _beeHealth;

    Collider2D[] _playerHitResults = new Collider2D[10];
    List<Transform> _activeLightnings;

    bool _shotStarted;
    bool _shotFinished;
    float _currentHealth;

    void OnValidate()
    {
        if (_lightningAnimationTime <= _delayBeforeDamage)
            _delayBeforeDamage = _lightningAnimationTime;
    }

    void OnEnable()
    {
        _currentHealth = _maxHealth;
        StartCoroutine(StartLightning());
        StartCoroutine(StartMovement());
        var wrapper = GetComponentInChildren<ShootAnimationWrapper>();
        wrapper.OnShoot += () => _shotStarted = true;
        wrapper.OnReload += () => _shotFinished = true;
        StartCoroutine(UpdateHealthUI());
    }

    IEnumerator UpdateHealthUI()
    {
        while (true)
        {
            float healthPercentage = _currentHealth / (float)_maxHealth;

            _beeHealth.fillAmount = healthPercentage;

            yield return new WaitForSeconds(0.1f);
        }
    }


    IEnumerator StartMovement()
    {
        _beeLaser.SetActive(false);
        GrabBag<Transform> grabBag = new GrabBag<Transform>(_beeDestinations);

        while (true)
        {
            var destinations = grabBag.Grab();
            if (destinations == null) yield break;

            _animator.SetBool("Move", true);

            while (Vector2.Distance(_bee.transform.position, destinations.position) > 0.1f)
            {
                _bee.transform.position = Vector2.MoveTowards(_bee.transform.position, destinations.position, Time.deltaTime);
                yield return null;
            }

            _animator.SetBool("Move", false);

            yield return new WaitForSeconds(UnityEngine.Random.Range(_minIdleTime, _maxIdleTime));
            _animator.SetTrigger("Fire");

            yield return new WaitUntil(() => _shotStarted);
            _shotStarted = false;
            _beeLaser.SetActive(true);

            yield return new WaitUntil(() => _shotFinished);
            _shotFinished = false;
            _beeLaser.SetActive(false);
        }
    }

    IEnumerator StartLightning()
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

    public void TakeDamage()
    {
        _currentHealth--;

        if(_currentHealth == _maxHealth / 2)
        {
            StartCoroutine(ToggleFlood(true));
        }

        if (_currentHealth <= 0)
        {
            StopAllCoroutines();
            StopCoroutine(ToggleFlood(false));
            _animator.SetBool("Dead", true);
            _beeRigidBody.bodyType = RigidbodyType2D.Dynamic;
            foreach (var collider in _bee.GetComponentsInChildren<Collider2D>())
            {
                collider.gameObject.layer = LayerMask.NameToLayer("Dead");
            }
        }
        else
        {
            _animator.SetTrigger("Hit");
        }
    }

    IEnumerator ToggleFlood(bool enableFlood)
    {
        float initialWaterY = _water.transform.position.y;
        var targetWaterY = enableFlood ? initialWaterY + 1 : initialWaterY - 1;
        float duration = 1f;
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;
            float y = Mathf.Lerp(initialWaterY, targetWaterY, progress);
            var destination = new Vector3(_water.transform.position.x, y, _water.transform.position.z);
            _water.transform.position = destination;
            yield return null;
        }
        _floodGroundCollider.enabled = !enableFlood;
        _water.SetSpeed(enableFlood ? 5f : 0f);
    }
}