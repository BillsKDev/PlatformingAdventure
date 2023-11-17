using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameData _gameData;
    public static GameManager Instance { get; private set; }
    public static bool CinematicPlaying { get; private set; }
    public static bool IsLoading { get; private set; }

    public List<string> AllGameNames = new List<string>();

    PlayerInputManager _playerInputManager;

    public void ToggleCinematic(bool cinematicPlaying) => CinematicPlaying = cinematicPlaying;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _playerInputManager = GetComponent<PlayerInputManager>();
        _playerInputManager.onPlayerJoined += HandlePlayerJoined;

        SceneManager.sceneLoaded += HandleSceneLoaded;

        string commaSeparatedList = PlayerPrefs.GetString("AllGameNames");
        Debug.Log(commaSeparatedList);
        AllGameNames = commaSeparatedList.Split(",").ToList();
        AllGameNames.Remove("");
    }

    void HandleSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.name == "Menu")
            _playerInputManager.joinBehavior = PlayerJoinBehavior.JoinPlayersManually;
        else
        {
            _gameData.CurrentLevelName = arg0.name;
            _playerInputManager.joinBehavior = PlayerJoinBehavior.JoinPlayersWhenButtonIsPressed;

            var levelData = _gameData.LevelDatas.FirstOrDefault(t => t.LevelName == arg0.name);
            if (levelData == null)
            {
                levelData = new LevelData() { LevelName = arg0.name };
                _gameData.LevelDatas.Add(levelData);
            }

            BindCoins(levelData);
            BindLaserSwitch(levelData);

            var allPlayers = FindObjectsOfType<Player>();
            foreach (var player in allPlayers)
            {
                var playerInput = player.GetComponent<PlayerInput>();
                var data = GetPlayerData(playerInput.playerIndex);
                player.Bind(data);
                if(GameManager.IsLoading)
                {
                    player.RestorePositionAndVelocity();
                    IsLoading = false;
                }
            }
        }
    }

    void BindCoins(LevelData levelData)
    {
        var allCoins = FindObjectsOfType<Coin>();

        foreach (var coin in allCoins)
        {
            var data = levelData.CoinDatas.FirstOrDefault(t => t.Name == coin.name);
            if (data == null)
            {
                data = new CoinData() { IsCollected = false, Name = coin.name };
                levelData.CoinDatas.Add(data);
            }
            coin.Bind(data);
        }
    }

    void BindLaserSwitch(LevelData levelData)
    {
        var allLaserSwitch = FindObjectsOfType<LaserSwitch>();

        foreach (var laserSwitch in allLaserSwitch)
        {
            var data = levelData.LaserSwitchDatas.FirstOrDefault(t => t.Name == laserSwitch.name);
            if (data == null)
            {
                data = new LaserSwitchData() { IsOn = false, Name = laserSwitch.name };
                levelData.LaserSwitchDatas.Add(data);
            }
            laserSwitch.Bind(data);
        }
    }

    public void SaveGame()
    {
        if (string.IsNullOrWhiteSpace(_gameData.GameName))
            _gameData.GameName = "Game " + AllGameNames.Count;

        string text = JsonUtility.ToJson(_gameData);

        PlayerPrefs.SetString(_gameData.GameName, text);

        if (AllGameNames.Contains(_gameData.GameName) == false)
            AllGameNames.Add(_gameData.GameName);

        string commaSeperatedGameNames = string.Join(",", AllGameNames);
        PlayerPrefs.SetString("AllGameNames", commaSeperatedGameNames);
        PlayerPrefs.Save();
    }

    public void ReloadGame() => LoadGame(_gameData.GameName);

    public void LoadGame(string gameName)
    {
        IsLoading = true;
        string text = PlayerPrefs.GetString(gameName);
        _gameData = JsonUtility.FromJson<GameData>(text);
        if (String.IsNullOrWhiteSpace(_gameData.CurrentLevelName))
            _gameData.CurrentLevelName = "Level 1";
        SceneManager.LoadScene(_gameData.CurrentLevelName);
    }

    void HandlePlayerJoined(PlayerInput playerInput)
    {
        PlayerData playerData = GetPlayerData(playerInput.playerIndex);

        Player player = playerInput.GetComponent<Player>();
        player.Bind(playerData);
    }

    PlayerData GetPlayerData(int playerIndex)
    {
        if (_gameData.PlayerDatas.Count <= playerIndex)
        {
            var playerData = new PlayerData();
            _gameData.PlayerDatas.Add(playerData);
        }
        return _gameData.PlayerDatas[playerIndex];
    }

    public void NewGame()
    {
        Debug.Log("New Game Called");
        _gameData = new GameData();
        _gameData.GameName = DateTime.Now.ToString("G");
        SceneManager.LoadScene("Level 1");
    }

    public void DeleteGame(string gameName)
    {
        PlayerPrefs.DeleteKey(gameName);
        AllGameNames.Remove(gameName);

        string commaSeperatedGameNames = string.Join(",", AllGameNames);
        PlayerPrefs.SetString("AllGameNames", commaSeperatedGameNames);
        PlayerPrefs.Save();
    }
}
