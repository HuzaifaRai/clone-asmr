using UnityEngine;
using System.Collections;

public class GameManager
{

	private static GameManager instance;

	public static GameManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new GameManager ();
			}
			return instance;
		}
	}

	public bool Initialized = false;
	public bool canShowFirstOpenAd = false;
	public bool canShowAds = true;
    public int selectedMode = 1;
    public int RewardedCoins;
    public int selectedlevel;
    public string modeName;
    public int TotalMode;


}