using UnityEngine;

public class Settings : MonoBehaviour
{
	private const string OPTION_SOUND_KEY = "OPTION_SOUND";

	private const int OPTION_SOUND_DEFAULT = 1;

	private const string OPTION_AUTOMESSAGE_KEY = "OPTION_AUTOMESSAGE";

	private const int OPTION_AUTOMESSAGE_DEFAULT = 0;

	public const string OPTION_SEASON_KEY = "OPTION_SEASON_XMAS";

	public const int OPTION_SEASON_DEFAULT = 1;

	private static AudioListener audioListener;

	private static bool _optionsLoaded;

	private static bool _optionSound;

	private static bool _optionAutoMessage;

	public static bool _optionSeason;

	public static bool optionAutoMessage
	{
		get
		{
			LoadOptionsIfNeeded();
			return _optionAutoMessage;
		}
		set
		{
			if (!value && !PlayerPrefs.HasKey("OPTION_AUTOMESSAGE"))
			{
				Flurry.LogEvent("AutoBrag turned off");
			}
			_optionAutoMessage = value;
			PlayerPrefs.SetInt("OPTION_AUTOMESSAGE", _optionAutoMessage ? 1 : 0);
		}
	}

	public static bool optionSound
	{
		get
		{
			LoadOptionsIfNeeded();
			return _optionSound;
		}
		set
		{
			_optionSound = value;
			PlayerPrefs.SetInt("OPTION_SOUND", _optionSound ? 1 : 0);
			AudioListener.volume = ((!_optionSound) ? 0f : 1f);
		}
	}

	public static bool optionSeason
	{
		get
		{
			LoadOptionsIfNeeded();
			return _optionSeason;
		}
		set
		{
			if (!value)
			{
				if (!PlayerPrefs.HasKey("OPTION_SEASON_XMAS"))
				{
					Flurry.LogEvent("Season turned off");
				}
				ThemeManager.Instance.Theme = Theme.NORMAL;
			}
			else
			{
				PlayerInfo instance = PlayerInfo.Instance;
				if (instance.currentSeasonPicked == PlayerInfo.Season.xmas)
				{
					ThemeManager.Instance.Theme = Theme.XMAS;
				}
			}
			_optionSeason = value;
			PlayerPrefs.SetInt("OPTION_SEASON_XMAS", _optionSeason ? 1 : 0);
		}
	}

	private void Awake()
	{
		if (audioListener == null)
		{
			audioListener = NGUITools.AddChild(Camera.main.gameObject).AddComponent<AudioListener>();
		}
		else
		{
			audioListener.transform.parent = Camera.main.transform;
			audioListener.transform.localPosition = Vector3.zero;
		}
		LoadOptionsIfNeeded();
	}

	private static void LoadOptionsIfNeeded()
	{
		if (!_optionsLoaded)
		{
			_optionSound = PlayerPrefs.GetInt("OPTION_SOUND", 1) != 0;
			AudioListener.volume = ((!_optionSound) ? 0f : 1f);
			_optionAutoMessage = PlayerPrefs.GetInt("OPTION_AUTOMESSAGE", 0) != 0;
			_optionSeason = PlayerPrefs.GetInt("OPTION_SEASON_XMAS", 1) != 0;
			PlayerInfo instance = PlayerInfo.Instance;
			if (_optionSeason)
			{
				instance.currentSeasonPicked = instance.currentSeasonAvailable;
			}
			else
			{
				instance.currentSeasonPicked = PlayerInfo.Season.none;
			}
			_optionsLoaded = true;
		}
	}
}
