using UnityEngine;
using UnityEngine.UI;

public class LoadGameButton : MonoBehaviour
{
    void Start() => GetComponent<Button>().onClick.AddListener(LoadGame);

    public void LoadGame() => GameManager.Instance.LoadGame();

}
