using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class SocialManagerIOS : MonoBehaviour
{
	public enum GCState
	{
		LoggedOut = 0,
		Authenticating = 1,
		LoadingFriends = 2,
		LoggedIn = 3
	}

	private const string ONLINESETTINGS_FILTER_GC_FRIENDS_KEY = "social_filter_gc_friends";

	private const bool FILTER_GC_FRIENDS_USING_LEADERBOARD_DEFAULT = true;

	private const string GC_LEADERBOARD_ID = "com.kiloo.subwaysurfers.ScoreLeaderboard";

	private static SocialManagerIOS _instance;

	private GCState _gcState;

	private Dictionary<string, IUserProfile> _gcFriends;

	private List<string> _latestPreConsolidationGCIds;

	public GCState gameCenterState
	{
		get
		{
			return _gcState;
		}
		set
		{
			_gcState = value;
		}
	}

	public static SocialManagerIOS instance
	{
		get
		{
			Init();
			return _instance;
		}
	}

	public static bool isInstanced
	{
		get
		{
			return _instance != null;
		}
	}

	private static void Init()
	{
		if (_instance == null)
		{
			GameObject gameObject = new GameObject();
			gameObject.name = "SocialManagerIOS";
			Object.DontDestroyOnLoad(gameObject);
			gameObject.AddComponent<SocialManagerIOS>();
		}
	}

	private void Awake()
	{
		_instance = this;
	}
}
