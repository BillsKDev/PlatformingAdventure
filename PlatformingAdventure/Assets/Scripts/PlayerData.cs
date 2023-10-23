using System;
using System.Collections.Generic;

[Serializable]
public class PlayerData
{
    public int Coins;
    public int Health = 6;
}

[Serializable]
public class GameData
{
    public List<PlayerData> PlayerDatas = new List<PlayerData>();
    public string GameName;
}