using UnityEngine;

public class Dog : MonoBehaviour, ITakeDamage
{
    void Start()
    {
        GetComponentInChildren<ShootAnimationWrapper>().OnShoot += Shoot;
    }
    void Shoot() => Debug.Log("Shooting");

    public void TakeDamage()
    {
        gameObject.SetActive(false);
    }
}
