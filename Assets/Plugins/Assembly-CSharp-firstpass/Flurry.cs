using System.Collections.Generic;
using UnityEngine;

public static class Flurry
{
	public const string EVENT_UISCREEN_CHANGED_PREFIX = "UI Screen ";

	public const string EVENT_POPUPSCREEN_CHANGED_PREFIX = "POPUP Screen ";

	public const string EVENT_10_SOCIAL_ACTIONS_TAKEN = "10 social actions taken";

	public const string EVENT_FIRST_GAMECENTER_LOGIN = "First GameCenter Login";

	public const string EVENT_FIRST_FACEBOOK_LOGIN = "First Facebook Login";

	public const string EVENT_SOCIAL_POKE = "Social friend poked";

	public const string EVENT_SOCIAL_BRAG = "Social bragged";

	public const string EVENT_SOCIAL_BRAGFACEBOOK = "Social bragged Facebook";

	public const string EVENT_SOCIAL_FACEBOOK_FRIENDS_RETRIEVED = "Social Facebook Friends Retrieved";

	public const string EVENT_SOCIAL_GC_FRIENDS_RETRIEVED = "Social GameCenter Friends Retrieved";

	public const string EVENT_SOCIAL_FRIENDS_CONSOLIDATED = "Social Friends Consolidated";

	public const string EVENT_MYSTERY_BOX_OPENED = "Mystery Box opened";

	public const string EVENT_INAPPPURCHASE_COMPLETED = "InApp purchase completed";

	public const string EVENT_INAPPPURCHASE_COINPACK1 = "InApp Coin Pack 1 purchased";

	public const string EVENT_INAPPPURCHASE_COINPACK2 = "InApp Coin Pack 2 purchased";

	public const string EVENT_INAPPPURCHASE_COINPACK3 = "InApp Coin Pack 3 purchased";

	public const string EVENT_INAPPPURCHASE_COINPACK4 = "InApp Coin Pack 4 purchased";

	public const string EVENT_INAPPPURCHASE_COINPACK5 = "InApp Coin Pack 5 purchased";

	public const string EVENT_INAPPPURCHASE_DOUBLECOINS = "Double Coin purchased";

	public const string EVENT_INAPPPURCHASE_DOUBLECOINS_POPUP = "Double Coin purchased GameOver";

	public const string EVENT_CHARACTER_UNLOCKED = "Character unlocked";

	public const string EVENT_AUTOMESSAGE_TURNED_OFF = "AutoBrag turned off";

	public const string EVENT_SEASON_TURNED_OFF = "Season turned off";

	public const string EVENT_MISSIONSET_COMPLETED = "Mission Set completed";

	public const string EVENT_DAILY_CHALLENGE_COMPLETED = "Daily Challenge completed";

	public const string EVENT_UPDATE_APP_POPUP_RESULT = "New Version Popup Result";

	public const string EVENT_FILEUTIL_LOAD_CORRUPTED = "FileUtil load corrupted";

	public const string EVENT_VIDEOADS_REQUEST = "VideoAds request";

	public const string EVENT_VIDEOADS_PROVIDER_REQUEST = "VideoAds {0} request";

	public const string EVENT_FRIENDREWARD_COLLECTED = "Friend reward collect";

	public const string EVENT_EARNCOINS_ITEM_CLICKED = "Earn Coins item clicked";

	public const string EVENT_HOVERBOARD_BOUGHT = "Hoverboard bought";

	public const string EVENT_FACEBOOK_SHARE = "Share link on Facebook";

	public const string EVENT_BREADCRUMBS_BEFORE_FREE_HOVERBOARD = "Bread crumbs before purchasing the free hoverboard";

	public const string EVENT_MORE_COINS_CLICKED = "More coins button clicked";

	public const string EVENT_BACK_BTN_PRESSED = "Back button pressed";

	public const string EVENT_HOME_BUTTON_PRESSED = "Home button pressed";

	public const string EVENT_BOOST_HEADSTART500_PURCHASED = "Boost Headstart500 purchased";

	public const string EVENT_BOOST_HEADSTART2000_PURCHASED = "Boost Headstart2000 purchased";

	public const string EVENT_BOOST_HOVERBOARD_PURCHASED = "Boost Hoverboard purchased";

	public const string EVENT_BOOST_COINMAGNET_PURCHASED = "Boost Coinmagnet purchased";

	public const string EVENT_BOOST_DOUBLEMULTIPLIER_PURCHASED = "Boost 2x multiplier purchased";

	public const string EVENT_BOOST_JETPACK_PURCHASED = "Boost jetpack purchased";

	public const string EVENT_BOOST_LETTERS_PURCHASED = "Boost letters purchased";

	public const string EVENT_BOOST_SUPERSNEAKERS_PURCHASED = "Boost supersneakers purchased";

	public const string EVENT_BOOST_MYSTERYBOX_PURCHASED = "Boost MysteryBox purchased";

	public const string EVENT_BOOST_MISSION_SKIP_PURCHASED = "Boost Mission Skip purchased";

	public const string EVENT_BOOST_MYSTERYBOX_VIEW_PRIZES = "Mysterybox view prices";

	public const string EVENT_ARGKEY_ID = "Id";

	public const string EVENT_ARGKEY_TIER = "Tier";

	public const string EVENT_ARGKEY_UI_SCREENNAME = "Screen Name";

	public const string EVENT_ARGKEY_MISSIONSET = "Mission Set";

	public const string EVENT_ARGKEY_MISSIONSET_AND_INDEX = "Mission Set and Index";

	public const string EVENT_ARGKEY_TOTAL = "Total";

	public const string EVENT_ARGKEY_POPUPRESULT = "Result";

	public const string EVENT_ARGKEY_FILENAME = "Filename";

	public const string EVENT_ARGKEY_VIDEOADRESULT = "Result";

	public const string EVENT_ARGKEY_ITEM_IAP = "Item tiggered iap";

	public const string EVENT_ARGKEYS_SOCIAL_FRIENDS_RETRIEVED = "Total friends;Installed friends";

	public const string EVENT_ARGKEYS_SOCIAL_FRIENDS_CONSOLIDATED = "Total;Facebook;GameCenter";

	public const string EVENT_ARGKEY_POPUPRESULT_OK = "Ok";

	public const string EVENT_ARGKEY_POPUPRESULT_CANCEL = "Cancel";

	public const string EVENT_ARGKEY_VIDEOADRESULT_OK = "Ok";

	public const string EVENT_ARGKEY_VIDEOADRESULT_NOVIDEO = "No video";

	private static bool inSession = false;

	private static readonly int[][] FRIEND_BRACKETS = new int[5][]
	{
		new int[3] { -1, 10, 1 },
		new int[3] { 11, 30, 5 },
		new int[3] { 31, 100, 10 },
		new int[3] { 101, 1000, 100 },
		new int[3] { 1001, 999999, 1000 }
	};

	public static void LogGenericSocialAction()
	{
		int @int = PlayerPrefs.GetInt("flurry_social_total", 0);
		int int2 = PlayerPrefs.GetInt("flurry_social_unlogged", 0);
		@int++;
		int2++;
		Debug.Log("LogGenericSocialAction: new unlogged total = " + int2);
		if (int2 == 10)
		{
			int2 = 0;
			LogEventWithAParameter("10 social actions taken", "Total", @int.ToString());
		}
		PlayerPrefs.SetInt("flurry_social_total", @int);
		PlayerPrefs.SetInt("flurry_social_unlogged", int2);
	}

	public static void LogGameCenterLogin()
	{
	}

	public static void LogFacebookLogin()
	{
		if (!PlayerPrefs.HasKey("flurry_has_logged_fb"))
		{
			LogEvent("First Facebook Login");
			PlayerPrefs.SetInt("flurry_has_logged_fb", 1);
		}
	}

	public static void LogOkPressedOnPopup(string eventName)
	{
		if (eventName != string.Empty)
		{
			LogEventWithAParameter(eventName, "Result", "Ok");
		}
	}

	public static void LogClosePressedOnPopup(string eventName)
	{
		if (eventName != string.Empty)
		{
			LogEventWithAParameter(eventName, "Result", "Cancel");
		}
	}

	public static string ConvertFriendCountToBracket(int count)
	{
		for (int i = 0; i < FRIEND_BRACKETS.Length; i++)
		{
			int num = FRIEND_BRACKETS[i][0];
			int num2 = FRIEND_BRACKETS[i][1];
			int num3 = FRIEND_BRACKETS[i][2];
			if (count < num || count > num2)
			{
				continue;
			}
			while (num <= num2)
			{
				int num4 = num + num3;
				if (count >= num && count < num4)
				{
					if (num3 == 1)
					{
						return count.ToString();
					}
					return num + "-" + (num4 - 1);
				}
				num = num4;
			}
		}
		Debug.LogWarning("ConvertFriendCountToBracket: Count is outside any brackets: " + count);
		return count.ToString();
	}

	public static void StartSession(string apiKey)
	{
	}

	public static void SetUserInfo(string userId)
	{
		SetUserInfo(userId, 0, 0);
	}

	public static void SetUserInfo(string userId, int age, int gender)
	{
	}

	public static void LogEvent(string eventName)
	{
		LogEventAndroid(eventName);
	}

	public static void LogEventWithAParameter(string eventName, string argKey, string argValue)
	{
		LogEventWithAParameterAndroid(eventName, argKey, argValue);
	}

	public static void LogEventWithSeveralParameters(string eventName, string argKeys, string argValues)
	{
		LogEventWithSeveralParametersAndroid(eventName, argKeys, argValues);
	}

	public static void LogError(string errorName, string message)
	{
		LogErrorAndroid(errorName, message);
	}

	private static void SetUserInfoAndroid(string userId, int age, int gender)
	{
		inSession = true;
		FlurryAndroid.setUserID(userId);
	}

	private static void LogEventAndroid(string eventName)
	{
		FlurryAndroid.logEvent(eventName);
	}

	private static void LogEventWithAParameterAndroid(string eventName, string argKey, string argValue)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add(argKey, argValue);
		FlurryAndroid.logEvent(eventName, dictionary);
	}

	private static void LogEventWithSeveralParametersAndroid(string eventName, string argKeys, string argValues)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add(argKeys, argValues);
		FlurryAndroid.logEvent(eventName, dictionary);
	}

	private static void LogErrorAndroid(string errorName, string message)
	{
		FlurryAndroid.onError(errorName, message, string.Empty);
	}
}
