using UnityEngine;

public class GamesListPanel : MonoBehaviour
{
    [SerializeField] LoadGameButton _buttonPrefab;

    void Start()
    {
        foreach (var gameName in GameManager.Instance.AllGameNames)
        {
            var button = Instantiate(_buttonPrefab, transform);
            button.SetGameName(gameName);
        }
    }
}
