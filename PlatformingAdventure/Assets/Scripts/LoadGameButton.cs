using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadGameButton : MonoBehaviour
{
    string _gameName;

    void Start() => GetComponent<Button>().onClick.AddListener(LoadGame);

    public void LoadGame() => GameManager.Instance.LoadGame(_gameName);

    public void DeleteGame()
    {
        GameManager.Instance.DeleteGame(_gameName);
        Destroy(gameObject);
    }

    public void SetGameName(string gameName)
    {
        _gameName = gameName;
        GetComponentInChildren<TMP_Text>().SetText(gameName);
    }
}
