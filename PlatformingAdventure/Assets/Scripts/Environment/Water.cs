using UnityEngine;

public class Water : MonoBehaviour
{

    void Start()
    {
        foreach (var waterFlowAnimation in GetComponentsInChildren<WaterFlowAnimation>())
            waterFlowAnimation.enabled = false;
    }
    void OnTriggerEnter2D(Collider2D collision) => GetComponent<AudioSource>().Play();

    public void SetSpeed(float speed)
    {
        GetComponent<BuoyancyEffector2D>().flowMagnitude = speed;
        foreach (var waterFlowAnimation in GetComponentsInChildren<WaterFlowAnimation>())
            waterFlowAnimation.enabled = speed != 0;
    }
}
