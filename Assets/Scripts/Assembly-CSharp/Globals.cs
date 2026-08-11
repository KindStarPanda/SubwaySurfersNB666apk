using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Globals
{
	private struct AddedAnimationEventInfo
	{
		public Animation animation;

		public string clipName;

		public float time;

		public string functionName;

		public AddedAnimationEventInfo(Animation animation, string clipName, float time, string functionName)
		{
			this.animation = animation;
			this.clipName = clipName;
			this.time = time;
			this.functionName = functionName;
		}
	}

	public const bool DEBUG_SOCIAL_MANAGER_SERVER = false;

	public const bool DEBUG = false;

	public const bool DEBUG_FREE_PURCHASES = false;

	public const bool DEBUG_ALL_CHARS = false;

	public const bool DEBUG_ALL_BOARDS = false;

	public const bool DEBUG_ALWAYS_ONLINE = false;

	public const bool DEBUG_ALWAYS_OFFLINE = false;

	public const bool DEBUG_FREE_INAPP_PURCHASE = false;

	public const bool DEBUG_USE_DEBUG_SEASON = false;

	public const bool DEBUG_USE_DEBUG_DAILYWORD = false;

	public const bool DEBUG_SKIP_TUTORIAL_IN_EDITOR = false;

	public const string DEBUG_DAILYWORD = "zoooooooooom";

	public const bool DEBUG_USE_DEBUG_LOGIN_STATE = false;

	public const int DEBUG_LOGIN_STATE = 0;

	public const int MAX_MULTIPLIER = 30;

	public const int MAX_RANK = 1;

	public const string DEFAULT_ENDSEASON_DATETIME = "02-01-2013 00:00:00";

	public const int BONUS_FACEBOOK = 5000;

	public const int BONUS_GAMECENTER = 250;

	public const int MIN_FRIEND_SCORE_REQUEST_INTERVAL = 15;

	public const int NUMBER_OF_BREADCRUMBS_TO_SHOW = 30;

	public const string FLURRY_API_KEY = "YR898G65YFPWNMQ6X5H5";

	public const string ADCOLONY_APPVERSION = "1.5";

	public const string ADCOLONY_APPID = "appc0d018638a3a47a3b725ab";

	public const string ADCOLONY_ZONEID = "vzc54d2d8389a24681852d05";

	public const string CHARTBOOST_APPID = "502e041317ba47dc7d000024";

	public const string CHARTBOOST_APPSIGNATURE = "e356ea8420eeb855ff880fba02ce0309364d0613";

	public const string VUNGLE_APPID = "507686ae771615941001aca5";

	public const string PrivacyPolicyURL = "http://www.kiloo.com/privacy/";

	public const int LAYER_2DGUI = 30;

	public const int LAYER_3DGUI = 31;

	public const int LAYER_2DOVERLAY = 28;

	public const int LAYER_3DCLIP = 29;

	public const float DRAG_THRESHOLD = 0.08f;

	public static PlayerInfo.Season DEBUG_SEASON = PlayerInfo.Season.xmas;

	public static PlayerInfo.Season UPDATE_DEFAULT_SEASON = PlayerInfo.Season.xmas;

	public static Theme UPDATE_DEFAULT_THEME = Theme.XMAS;

	private static List<AddedAnimationEventInfo> addedAnimEvents = new List<AddedAnimationEventInfo>();

	public static Friend[] GetDebugFriends(int numberOfFriends = 10)
	{
		return new Friend[numberOfFriends];
	}

	public static Dictionary<T, bool> convertStringToEnumBoolDictionary<T>(string sourceString)
	{
		string[] array = sourceString.Split(',');
		Dictionary<T, bool> dictionary = new Dictionary<T, bool>(array.Length / 2);
		Type typeFromHandle = typeof(T);
		for (int i = 0; i < array.Length - 1; i += 2)
		{
			string value = array[i];
			string value2 = array[i + 1];
			if (Enum.IsDefined(typeFromHandle, value))
			{
				T key = (T)Enum.Parse(typeFromHandle, value, true);
				bool result;
				if (bool.TryParse(value2, out result))
				{
					dictionary[key] = result;
				}
				else
				{
					Debug.LogError("Source string could not parse bool");
				}
			}
		}
		return dictionary;
	}

	public static string convertEnumBoolDictionaryToString<T>(Dictionary<T, bool> sourceDict)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (KeyValuePair<T, bool> item in sourceDict)
		{
			string name = Enum.GetName(typeof(T), item.Key);
			string arg = item.Value.ToString();
			stringBuilder.AppendFormat("{0},{1},", name, arg);
		}
		if (stringBuilder.Length > 0)
		{
			stringBuilder.Remove(stringBuilder.Length - 1, 1);
		}
		return stringBuilder.ToString();
	}

	public static Dictionary<string, string> convertStringToStringStringDictionary(string sourceString)
	{
		string[] array = sourceString.Split(',');
		Dictionary<string, string> dictionary = new Dictionary<string, string>(array.Length / 2);
		for (int i = 0; i < array.Length - 1; i += 2)
		{
			string key = array[i];
			string value = array[i + 1];
			dictionary[key] = value;
		}
		return dictionary;
	}

	public static string convertStringStringDictionaryToString(Dictionary<string, string> sourceDict)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (KeyValuePair<string, string> item in sourceDict)
		{
			string key = item.Key;
			string value = item.Value;
			stringBuilder.AppendFormat("{0},{1},", key, value);
		}
		if (stringBuilder.Length > 0)
		{
			stringBuilder.Remove(stringBuilder.Length - 1, 1);
		}
		return stringBuilder.ToString();
	}

	public static bool TryAddAnimationEvent(Animation animation, string clipName, AnimationEvent aniEvent)
	{
		foreach (AddedAnimationEventInfo addedAnimEvent in addedAnimEvents)
		{
			if (addedAnimEvent.animation == animation && addedAnimEvent.clipName == clipName && addedAnimEvent.time == aniEvent.time && addedAnimEvent.functionName == aniEvent.functionName)
			{
				return false;
			}
		}
		addedAnimEvents.Add(new AddedAnimationEventInfo(animation, clipName, aniEvent.time, aniEvent.functionName));
		animation[clipName].clip.AddEvent(aniEvent);
		return true;
	}
}
