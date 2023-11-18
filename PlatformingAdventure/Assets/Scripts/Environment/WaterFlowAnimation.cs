using UnityEngine;

public class WaterFlowAnimation : MonoBehaviour
{
    [SerializeField] float _scrollSpeed = 1f;
    SpriteRenderer _spriteRenderer;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        float x = Mathf.Repeat(Time.time * _scrollSpeed, 1);
        Vector2 offset = new Vector2(x, 0);
        _spriteRenderer.material.SetTextureOffset("_MainTex", offset);
    }
}