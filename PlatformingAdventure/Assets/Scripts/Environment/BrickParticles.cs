using UnityEngine;

public class BrickParticles : MonoBehaviour
{
    void Start()
    {
        var particleSystem = GetComponent<ParticleSystem>();
        Destroy(gameObject, particleSystem.main.duration);
    }
}
