using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var gameManager = (GameManager)target;
        
        if (GUILayout.Button("Save Game"))
            gameManager.SaveGame();

        if (GUILayout.Button("Reload Game"))
            gameManager.ReloadGame();
    }
}
