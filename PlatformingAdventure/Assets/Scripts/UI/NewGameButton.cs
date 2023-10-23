using UnityEngine;
using UnityEngine.UI;

public class NewGameButton : MonoBehaviour
{
    void Start() => GetComponent<Button>().onClick.AddListener(CreateNewGame);

    public void CreateNewGame() => GameManager.Instance.NewGame();

}
