using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPanel : MonoBehaviour
{
    [SerializeField] TMP_Text _scoreText;
    [SerializeField] Image[] _hearts;
    [SerializeField] Image _flashImage;
    Player _player;

    public void Bind(Player player)
    {
        _player = player;
        _player.CoinsChanged += UpdateCoins;
        _player.HealthChanged += UpdateHealth;
        UpdateCoins();
        UpdateHealth();
    }

    void UpdateCoins()
    {
        _scoreText.SetText(_player.Coins.ToString());
    }

    void UpdateHealth()
    {
        for (int i = 0; i < _hearts.Length; i++)
        {
            Image heart = _hearts[i];
            heart.enabled = i < _player.Health;
        }
    }
}
