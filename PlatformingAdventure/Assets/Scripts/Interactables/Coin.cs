using UnityEngine;

public class Coin : MonoBehaviour
{
    CoinData _data;

    public void Bind(CoinData data)
    {
        _data = data;
        if (_data.IsCollected)
            gameObject.SetActive(false);
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.GetComponent<Player>();
        if (player)
        {
            _data.IsCollected = true;
            player.AddPoint();
            gameObject.SetActive(false);
        }
    }
}