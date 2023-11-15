using UnityEngine;

public class Dog : MonoBehaviour, ITakeDamage
{
    public void Shoot()
    {

    }

    public void TakeDamage()
    {
        gameObject.SetActive(false);
    }
}
