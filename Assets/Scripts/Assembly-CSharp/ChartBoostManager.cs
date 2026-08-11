using System;
using System.Collections.Generic;
using UnityEngine;

public class ChartBoostManager
{
	private const string ALLOWNEXT_TICKS_KEY = "cb_alwnxt_ticks";

	private const int FIRSTTIME_DELAY_SECONDS = 72000;

	private const int DEFAULT_DELAY_SECONDS = 30;

	private const string DELAY_SECONDS_ONLINESETTINGSKEY = "chartboost_delay_seconds";

	private static readonly List<string> ALLOWED_SHOW_SCREENS = new List<string> { "FrontUI" };

	private static readonly List<string> ALLOWED_CACHE_SCREENS = new List<string>
	{
		"CoinsUI_shop", "CharacterScreen", "GameoverUI", "TrophiesScreen", "UpgradesUI_shop", "BoardScreen", "CharacterScreen", "TrophiesScreen", "FriendsUI_online", "FriendsUI_offline",
		"LeaderboardUI_online", "LeaderboardUI_offline"
	};

	private static ChartBoostManager _instance;

	private bool _isCaching;

	public bool isInstanced
	{
		get
		{
			return _instance != null;
		}
	}

	public static ChartBoostManager instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new ChartBoostManager();
			}
			return _instance;
		}
	}

	private bool interstitialsEnabled
	{
		get
		{
			if (Application.systemLanguage == SystemLanguage.Chinese)
			{
				return true;
			}
			bool flag = Screen.height >= 2500;
			return PlayerInfo.Instance.inAppPurchaseCount <= 0 && !flag;
		}
	}

	private ChartBoostManager()
	{
		if (interstitialsEnabled)
		{
			ChartBoostAndroid.init("502e041317ba47dc7d000024", "e356ea8420eeb855ff880fba02ce0309364d0613");
			ChartBoostAndroidManager.didFinishInterstitialEvent += didFinishInterstitialEvent;
			ChartBoostAndroidManager.didFailToLoadInterstitialEvent += didFailToLoadInterstitialEvent;
		}
	}

	public void GameScreenChanged(string screenName)
	{
		if (string.IsNullOrEmpty(screenName))
		{
			Debug.LogError("ChartBoostManager.GameScreenChanged() invalid screenName: " + screenName);
		}
		else
		{
			ShowOrCacheForScreen(screenName);
		}
	}

	public void LastQueuedPopupsClosed(string currentScreenName)
	{
		if (string.IsNullOrEmpty(currentScreenName))
		{
			Debug.LogError("ChartBoostManager.LastQueuedPopupsClosed() invalid screenName: " + currentScreenName);
		}
		else
		{
			ShowOrCacheForScreen(currentScreenName);
		}
	}

	public void CacheNow()
	{
		if (interstitialsEnabled && !_isCaching)
		{
			CacheNowOnAndroid();
		}
	}

	private void ShowOrCacheForScreen(string screenName)
	{
		if (!interstitialsEnabled)
		{
			return;
		}
		if (ChartBoostHasCachedInterstitial())
		{
			if (_isCaching)
			{
				_isCaching = false;
			}
			if (!ALLOWED_SHOW_SCREENS.Contains(screenName) || UIScreenController.Instance.isShowingPopup)
			{
				return;
			}
			DateTime dateTime;
			if (PlayerPrefs.HasKey("cb_alwnxt_ticks"))
			{
				string @string = PlayerPrefs.GetString("cb_alwnxt_ticks");
				long result;
				if (!long.TryParse(@string, out result))
				{
					result = DateTime.Now.Ticks;
				}
				dateTime = new DateTime(result);
			}
			else
			{
				dateTime = DateTime.Now + TimeSpan.FromSeconds(72000.0);
				PlayerPrefs.SetString("cb_alwnxt_ticks", dateTime.Ticks.ToString());
			}
			DateTime now = DateTime.Now;
			if (!(now >= dateTime))
			{
				return;
			}
			ChartBoostShowInterstitial();
			int num = 30;
			string valueString;
			if (OnlineSettings.instance.TryGetValue("chartboost_delay_seconds", out valueString))
			{
				int result2;
				if (int.TryParse(valueString, out result2))
				{
					if (result2 >= 0)
					{
						num = result2;
					}
					else
					{
						Debug.LogError("ChartBoostManager: Delay seconds from OnlineSettings is not a positive number: " + result2);
					}
				}
				else
				{
					Debug.LogError("ChartBoostManager: Failed to parse delay seconds from OnlineSettings: " + valueString);
				}
			}
			PlayerPrefs.SetString("cb_alwnxt_ticks", (DateTime.Now + TimeSpan.FromSeconds(num)).Ticks.ToString());
		}
		else if (ALLOWED_CACHE_SCREENS.Contains(screenName))
		{
			CacheNow();
		}
	}

	private bool ChartBoostHasCachedInterstitial()
	{
		return ChartBoostAndroid.hasCachedInterstitial(null);
	}

	private void ChartBoostShowInterstitial()
	{
		ChartBoostAndroid.showInterstitial(null);
	}

	private void CacheNowOnAndroid()
	{
		if (!ChartBoostAndroid.hasCachedInterstitial(null))
		{
			_isCaching = true;
			ChartBoostAndroid.cacheInterstitial(null);
		}
	}

	private void OnDidCacheInterstitial(string location)
	{
		_isCaching = false;
	}

	private void OnDidFailToLoadInterstitial(string location)
	{
		_isCaching = false;
	}

	private void didFinishInterstitialEvent(string param)
	{
		OnDidCacheInterstitial(null);
	}

	private void didFailToLoadInterstitialEvent()
	{
		OnDidFailToLoadInterstitial(null);
	}
}
