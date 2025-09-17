using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PlayerProps
{
    public string playerName;
    public int playerHealth;
    public int playerDamage;
    public int playerRange;
    public bool isLocked = true;
}

[System.Serializable]
public class ModeProps
{
    public List<bool> ModeLocked = new List<bool>(); 
}


[System.Serializable]
public class SaveData
{

    public static SaveData instance;
    public static SaveData Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new SaveData();
            }
            return instance;
        }
    }
    public bool RemoveAds = false;
    public int LevelsUnlocked = 1;
    public int EventsUnlocked = 0;
    public int PlayerSelectedAvatar = 0;
    public int opponentSelectedAvatar;
    public int Selectedvape;
    public string ProfileName;
    public bool ProfileCreated = false;
    public bool isSound = true, isMusic = true, isVibration = true, isRightControls = true;
    public int Coins = 2000;
    public List<PlayerProps> Players = new List<PlayerProps>();
    public ModeProps modeProps = new ModeProps();
    public string hashOfSaveData;

    //Constructor to save actual GameData
    public SaveData() { }

    //Constructor to check any tampering with the SaveData
    public SaveData(bool ads, int levelsUnlocked, int eventsUnlocked, int coins, bool soundOn, bool musicOn, bool vibrationOn, bool rightControls, List<PlayerProps> _players,
        ModeProps _modeProps)
    {
        RemoveAds = ads;
        LevelsUnlocked = levelsUnlocked;
        EventsUnlocked = eventsUnlocked;
        Coins = coins;
        isSound = soundOn;
        isMusic = musicOn;
        isVibration = vibrationOn;
        isRightControls = rightControls;
        Players = _players;
        modeProps = _modeProps;
    }
}