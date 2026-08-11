using System;
using System.Globalization;
using UnityEngine;

public class HouseKeeper : MonoBehaviour
{
	private const string EVENT_SEASON_RUNNING_KEY = "season";

	private const string END_SEASON_DATETIME = "end_season_datetime";

	private const string ANDROID_FLURRY_GAMEOBJECT_NAME = "FlurryAndroidGameObject";

	private const string ANDROID_IN_APP_BILLING_GAMEOBJECT_NAME = "InAppBillingAndroidGameObject";

	private const string ANDROID_CHARTBOOST_GAMEOBJECT_NAME = "ChartBoost";

	private const float MIN_REFRESH_INTERVAL = 3600f;

	private const string REFRESH_INTERVAL_KEY = "refreshinterval";

	private static float _onlineSettingsLastDownloadTime = float.NegativeInfinity;

	private void Start()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		FlurryClips.InitAndEnableVideoAds();
		Flurry.StartSession("YR898G65YFPWNMQ6X5H5");
		RefreshOnlineSettingsAndInappsIfNeeded();
		ChartBoostManager.instance.CacheNow();
		CreateAndroidFlurryGameObject();
		CreateAndroidEtceteraGameObject();
		CreateAndroidInAppBillingCallbackObject();
		AudioListener.volume = 0f;
		SocialManager.Init();
		SocialManager.instance.AddFriendsConsolidatedHandler(CheckForLoginBonus);
		InAppManager.Init();
		Layers instance = Layers.Instance;
		CheckForSeason();
		AddVungleCallbackObject();
	}

	private void CheckForSeason()
	{
		DateTime seasonExpirationDateTime = GetSeasonExpirationDateTime();
		ThemeManager.Instance.themeExpirationDate = seasonExpirationDateTime;
		PlayerInfo instance = PlayerInfo.Instance;
		string valueString = string.Empty;
		if (OnlineSettings.instance.TryGetValue("season", out valueString))
		{
			Theme nORMAL = Theme.NORMAL;
			if (valueString.Equals("xmas"))
			{
				instance.currentSeasonAvailable = PlayerInfo.Season.xmas;
				nORMAL = Theme.XMAS;
				Debug.Log("XMAS time");
			}
			else
			{
				nORMAL = Theme.NORMAL;
				instance.currentSeasonAvailable = PlayerInfo.Season.none;
				Debug.Log("normal time");
			}
			if (PlayerPrefs.GetInt("OPTION_SEASON_XMAS", 1) != 0)
			{
				instance.currentSeasonPicked = instance.currentSeasonAvailable;
				ThemeManager.Instance.Theme = nORMAL;
			}
			else
			{
				ThemeManager.Instance.Theme = Theme.NORMAL;
			}
		}
		else
		{
			instance.currentSeasonAvailable = Globals.UPDATE_DEFAULT_SEASON;
			if (PlayerPrefs.GetInt("OPTION_SEASON_XMAS", 1) != 0)
			{
				instance.currentSeasonPicked = instance.currentSeasonAvailable;
				ThemeManager.Instance.Theme = Globals.UPDATE_DEFAULT_THEME;
				Debug.Log("Default theme: not online settings");
			}
			else
			{
				ThemeManager.Instance.Theme = Theme.NORMAL;
			}
		}
		if (PlayerInfo.Instance.currentSeasonAvailable != 0)
		{
			if (ThemeManager.Instance.themeForSeason(PlayerInfo.Instance.currentSeasonAvailable).TimeToExpire.Ticks < 0)
			{
				Debug.Log("Theme expired!");
				ThemeManager.Instance.Theme = Theme.NORMAL;
				instance.currentSeasonAvailable = PlayerInfo.Season.none;
				instance.currentSeasonPicked = instance.currentSeasonAvailable;
			}
		}
		else
		{
			ThemeManager.Instance.Theme = Theme.NORMAL;
		}
	}

	public DateTime GetSeasonExpirationDateTime()
	{
		string text = "dd-MM-yyyy hh:mm:ss";
		PlayerInfo instance = PlayerInfo.Instance;
		DateTime result;
		result = DateTime.Parse("02-01-9999 00:00:00", new CultureInfo("da-DK"));
		instance.currentSeasonExpirationDate = result.ToString(text);
		// if (instance.currentSeasonExpirationDate != null)
		// {
		// 	string currentSeasonExpirationDate = instance.currentSeasonExpirationDate;
		// 	result = DateTime.Parse(currentSeasonExpirationDate, new CultureInfo("da-DK"));
		// }
		// else
		// {
		// 	result = DateTime.Parse("02-01-2013 00:00:00", new CultureInfo("da-DK"));
		// 	instance.currentSeasonExpirationDate = result.ToString(text);
		// }
		string valueString;
		if (OnlineSettings.instance.TryGetValue("end_season_datetime", out valueString))
		{
			try
			{
				DateTime dateTime = DateTime.Parse(valueString, new CultureInfo("da-DK"));
				result = dateTime;
				instance.currentSeasonExpirationDate = result.ToString(text);
			}
			catch (FormatException)
			{
				Debug.Log(string.Format("Unable to convert '{0}'.", valueString));
				Debug.Log("Reading from online failed ");
			}
		}
		return result;
	}

	private void AddVungleCallbackObject()
	{
		GameObject gameObject = new GameObject("VungleManager");
		gameObject.name = "VungleManager";
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
		gameObject.AddComponent<VungleManager>();
	}

	private void OnApplicationPause(bool pause)
	{
		if (!pause)
		{
			if (DailyWord.Instance != null)
			{
				DailyWord.Instance.ForceSync();
			}
			CheckForSeason();
		}
	}

	private void CreateAndroidFlurryGameObject()
	{
		GameObject gameObject = new GameObject();
		gameObject.name = "FlurryAndroidGameObject";
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
		gameObject.AddComponent<FlurryInit>();
	}

	private void CreateAndroidEtceteraGameObject()
	{
		GameObject original = Resources.Load("Prefabs/Plugins/EtceteraAndroidManager", typeof(GameObject)) as GameObject;
		GameObject gameObject = UnityEngine.Object.Instantiate(original) as GameObject;
	}

	private void CreateAndroidInAppBillingCallbackObject()
	{
		GameObject gameObject = new GameObject("InAppBillingAndroidGameObject");
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
		gameObject.AddComponent<RRInappBillingCallback>();
	}

	private void CreateAndroidIChartBoostCallbackObject()
	{
		GameObject gameObject = new GameObject("ChartBoost");
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
		gameObject.AddComponent<ChartBoostAndroidManager>();
	}

	public void CheckForLoginBonus()
	{
		if (SocialManager.instance.facebookIsLoggedIn && !PlayerInfo.Instance.hasPayedOutFacebook)
		{
			PlayerInfo.Instance.amountOfCoins += 5000;
			PlayerInfo.Instance.hasPayedOutFacebook = true;
			PlayerInfo.Instance.SaveIfDirty();
			UIScreenController.QueueFacebookPayoutPopup();
		}
		if (Social.localUser.authenticated && !PlayerInfo.Instance.hasPayedOutGameCenter)
		{
			PlayerInfo.Instance.amountOfCoins += 250;
			PlayerInfo.Instance.hasPayedOutGameCenter = true;
			PlayerInfo.Instance.SaveIfDirty();
			UIScreenController.QueueGameCenterPayoutPopup();
		}
	}

	public static void RefreshOnlineSettingsAndInappsIfNeeded()
	{
		float num = 3600f;
		string valueString;
		if (OnlineSettings.instance.TryGetValue("refreshinterval", out valueString))
		{
			float result;
			if (float.TryParse(valueString, out result))
			{
				if (result >= 10f)
				{
					num = result;
				}
				else
				{
					Debug.LogError("OnlineSettings refresh interval too small: " + num);
				}
			}
			else
			{
				Debug.LogError("Failed to parse Onlinesettings refresh interval from: " + valueString);
			}
		}
		if (Time.realtimeSinceStartup > _onlineSettingsLastDownloadTime + num)
		{
			_onlineSettingsLastDownloadTime = Time.realtimeSinceStartup;
			if (!OnlineSettings.instance.isDownloading)
			{
				OnlineSettings.instance.DownloadNow();
			}
			InAppManager.Instance.QueryInApps();
		}
	}
}
