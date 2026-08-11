using System;
using System.Text;
using UnityEngine;

public static class EarnCurrencyInfo
{
	public enum Id
	{
		FacebookKiloo = 0,
		FacebookSybo = 1,
		TwitterKiloo = 2,
		TwitterSybo = 3,
		YoutubeKiloo = 4,
		YoutubeSybo = 5,
		Review = 6,
		AdColony = 7,
		VideoAd = 8
	}

	public enum Type
	{
		OpenURL = 0,
		VideoAd = 1
	}

	public enum Repeatability
	{
		Once = 0,
		OncePerVersion = 1,
		Forever = 2
	}

	public class EarnCurrencyProfile
	{
		public Id id;

		public Type type;

		public Repeatability repeatability;

		public int defaultAmountOfCoins;

		public string title = string.Empty;

		public string desc = string.Empty;

		public string iconName = string.Empty;

		public string url;

		public string overrideAmountOfCoinsOnlineSettingKey;

		public int GetAmountOfCoins()
		{
			string valueString;
			if (!string.IsNullOrEmpty(overrideAmountOfCoinsOnlineSettingKey) && OnlineSettings.instance.TryGetValue(overrideAmountOfCoinsOnlineSettingKey, out valueString))
			{
				int result;
				if (int.TryParse(valueString, out result))
				{
					if (result > 0)
					{
						return result;
					}
				}
				else
				{
					Debug.LogError("EarnCurrencyProfile ERROR: Unable to parse int from onlinesetting " + overrideAmountOfCoinsOnlineSettingKey + ": " + valueString);
				}
			}
			return defaultAmountOfCoins;
		}
	}

	private const int VIDEO_AD_DEFAULT_REWARD = 100;

	private const string ONLINESETTINGS_VIDEOADS_DEFAULTREWARD_KEY = "videoads_defaultreward";

	private const string NOVIDEOS_NATIVE_POPUP_TITLE = "No videos available";

	private const string NOVIDEOS_NATIVE_POPUP_MESSAGE = "We are currently out of videos to show you. Please try again later.";

	private const string NOVIDEOS_NATIVE_POPUP_OKBUTTONTEXT = "OK";

	private const string DATA_PROFILE_MAIN_SPLIT = ";";

	private const string DATA_PROFILE_SUB_SPLIT = "=";

	public static readonly EarnCurrencyProfile[] profiles = new EarnCurrencyProfile[7]
	{
		new EarnCurrencyProfile
		{
			id = Id.VideoAd,
			type = Type.VideoAd,
			repeatability = Repeatability.Forever,
			defaultAmountOfCoins = 100,
			title = "Sponsored Video",
			desc = "Watch get {0} coins",
			iconName = "icon_coinPack_1",
			overrideAmountOfCoinsOnlineSettingKey = "videoads_defaultreward"
		},
		new EarnCurrencyProfile
		{
			defaultAmountOfCoins = 300,
			title = "Kiloo Facebook",
			desc = "'Like' get {0} coins",
			iconName = "icon_coinPack_1",
			url = "http://www.facebook.com/kiloogames"
		},
		new EarnCurrencyProfile
		{
			id = Id.FacebookSybo,
			defaultAmountOfCoins = 300,
			title = "Sybo Facebook",
			desc = "'Like' get {0} coins",
			iconName = "icon_coinPack_1",
			url = "http://www.facebook.com/sybogames"
		},
		new EarnCurrencyProfile
		{
			id = Id.TwitterKiloo,
			defaultAmountOfCoins = 300,
			title = "Kiloo Twitter",
			desc = "Follow get {0} coins",
			iconName = "icon_coinPack_1",
			url = "https://twitter.com/@kiloogames"
		},
		new EarnCurrencyProfile
		{
			id = Id.TwitterSybo,
			defaultAmountOfCoins = 300,
			title = "Sybo Twitter",
			desc = "Follow get {0} coins",
			iconName = "icon_coinPack_1",
			url = "https://twitter.com/@sybogames"
		},
		new EarnCurrencyProfile
		{
			id = Id.YoutubeKiloo,
			defaultAmountOfCoins = 300,
			title = "Kiloo YouTube",
			desc = "Subscribe get {0} coins",
			iconName = "icon_coinPack_1",
			url = "http://www.youtube.com/kiloomobile"
		},
		new EarnCurrencyProfile
		{
			id = Id.YoutubeSybo,
			defaultAmountOfCoins = 300,
			title = "Sybo YouTube",
			desc = "Subscribe get {0} coins",
			iconName = "icon_coinPack_1",
			url = "http://www.youtube.com/sybogames"
		}
	};

	private static readonly string[] DATA_PROFILE_ALL_SPLITS = new string[2] { "=", ";" };

	private static string[] profileData;

	public static bool ShouldShowInGUI(int profileIndex)
	{
		EarnCurrencyProfile earnCurrencyProfile = profiles[profileIndex];
		string text = null;
		if (earnCurrencyProfile.repeatability == Repeatability.Once || earnCurrencyProfile.repeatability == Repeatability.OncePerVersion)
		{
			string text2 = GetProfileData(profileIndex);
			if (string.IsNullOrEmpty(text2))
			{
				return true;
			}
			if (earnCurrencyProfile.repeatability == Repeatability.OncePerVersion)
			{
				if (text == null)
				{
					text = DeviceUtility.GetBundleVersion();
				}
				if (text2 != text)
				{
					return true;
				}
			}
			return false;
		}
		return true;
	}

	public static void Trigger(int profileIndex)
	{
		if (!ShouldShowInGUI(profileIndex))
		{
			Debug.LogError("An already trigerred coin earner button has been pressed!");
			return;
		}
		EarnCurrencyProfile earnCurrencyProfile = profiles[profileIndex];
		if (earnCurrencyProfile.type == Type.OpenURL)
		{
			int amountOfCoins = earnCurrencyProfile.GetAmountOfCoins();
			if (amountOfCoins > 0)
			{
				PlayerInfo.Instance.amountOfCoins += amountOfCoins;
			}
		}
		else if (earnCurrencyProfile.type != Type.VideoAd)
		{
			Debug.LogError("Unhandled earner type: " + earnCurrencyProfile.type);
		}
		if (earnCurrencyProfile.repeatability == Repeatability.Once || earnCurrencyProfile.repeatability == Repeatability.OncePerVersion)
		{
			string bundleVersion = DeviceUtility.GetBundleVersion();
			SetAndSaveProfileData(profileIndex, bundleVersion);
		}
		PlayerInfo.Instance.SaveIfDirty();
		if (earnCurrencyProfile.type == Type.OpenURL)
		{
			Application.OpenURL(earnCurrencyProfile.url);
		}
		else if (earnCurrencyProfile.type == Type.VideoAd)
		{
			VideoAdsManager instance = VideoAdsManager.instance;
			if (!instance.isInitialized)
			{
				instance.Init();
			}
			int amountOfCoins2 = earnCurrencyProfile.GetAmountOfCoins();
			if (!instance.PlayVideoIfAvailable(amountOfCoins2))
			{
				DeviceUtility.showNativePopup("No videos available", "We are currently out of videos to show you. Please try again later.", "OK");
			}
		}
		else
		{
			Debug.LogError("Unhandled earner type: " + earnCurrencyProfile.type);
		}
		Flurry.LogEventWithAParameter("Earn Coins item clicked", "Id", earnCurrencyProfile.id.ToString());
	}

	private static void InitProfileDataArrayIfNeeded()
	{
		if (profileData != null)
		{
			return;
		}
		profileData = new string[profiles.Length];
		string earnCurrenyData = PlayerInfo.Instance.earnCurrenyData;
		if (string.IsNullOrEmpty(earnCurrenyData))
		{
			return;
		}
		string[] array = earnCurrenyData.Split(DATA_PROFILE_ALL_SPLITS, StringSplitOptions.None);
		for (int i = 0; i < array.Length - 1; i++)
		{
			string value = array[i];
			string text = array[i + 1];
			if (!Enum.IsDefined(typeof(Id), value))
			{
				continue;
			}
			Id id = (Id)(int)Enum.Parse(typeof(Id), value, true);
			for (int j = 0; j < profiles.Length; j++)
			{
				if (profiles[j].id == id)
				{
					profileData[j] = text;
					break;
				}
			}
		}
	}

	private static string GetProfileData(int profileIndex)
	{
		InitProfileDataArrayIfNeeded();
		return profileData[profileIndex];
	}

	private static void SetAndSaveProfileData(int profileIndex, string data)
	{
		InitProfileDataArrayIfNeeded();
		profileData[profileIndex] = data;
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < profileData.Length; i++)
		{
			if (profileData[i] != null)
			{
				stringBuilder.Append(profiles[i].id.ToString());
				stringBuilder.Append("=");
				stringBuilder.Append(profileData[i]);
				stringBuilder.Append(";");
			}
		}
		Debug.Log("EarnCurrenyInfo: Saving profile data: " + stringBuilder.ToString());
		PlayerInfo.Instance.earnCurrenyData = stringBuilder.ToString();
		PlayerInfo.Instance.SaveIfDirty();
	}
}
