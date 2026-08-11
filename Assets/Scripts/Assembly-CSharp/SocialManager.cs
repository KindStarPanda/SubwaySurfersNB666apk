#define PRINT_DEBUG_ERROR_LOGS
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using Debug = UnityEngine.Debug;

public class SocialManager : MonoBehaviour
{
	private enum FacebookCurrentRequest
	{
		None = 0,
		Error = 1,
		LoggingIn = 2
	}

	private enum WWWRequestResult
	{
		Success = 0,
		Error = 1
	}

	private delegate void WWWComplete(WWWRequestResult result, string output, object cookie);

	private const byte VERSION = 1;

	private const string REGISTER_DEVICE_URL = "/register2.php?android";

	private const string REPORT_SCORE_URL = "/report2.php?android";

	private const string CONSOLIDATE_FRIENDS_URL = "/friends2.php?android";

	private const string UPDATE_FRIEND_SCORES_URL = "/scores2.php?android";

	private const string POKE_URL = "/poke.php?android";

	private const string BRAG_URL = "/brag.php?android";

	private const float FACEBOOK_LOGIN_TIMEOUT = 600f;

	private const string FACEBOOK_APPID = "254616967963463";

	public const string BASE_URL = "http://hoodrunner.kiloo.com";

	private const string SECRET = "resxrctrv7tgv7gb8h9h9u0909kllfmolkjnhghgjjkhjghg";

	private const string ONLINESETTINGS_CONSOLIDATE_MIN_SECONDS_KEY = "social_consolidate_min_seconds";

	private const int CONSOLIDATE_MIN_SECONDS_DEFAULT = 3600;

	private const string ONLINESETTINGS_REPORT_GAMES_COUNT_KEY = "social_report_games_count";

	private const int REPORT_GAMES_COUNT_DEFAULT = 5;

	private const string ONLINESETTINGS_REPORT_MIN_SECONDS_KEY = "social_report_min_seconds";

	private const int REPOST_MIN_SECONDS_DEFAULT = 1200;

	private const string LAST_REGISTER_TIMESTAMPTICKS_PLAYERPREFSKEY = "socmanlastregtime";

	private const string LAST_REGISTER_DATA_PLAYERPREFSKEY = "socmanlastregdata";

	private const string ONLINESETTINGS_REGISTER_MIN_SECONDS_KEY = "social_register_min_seconds";

	private const int REGISTER_MIN_SECONDS_DEFAULT = 3600;

	private const string ONLINESETTINGS_FRIENDSCORES_MIN_SECONDS_KEY = "social_friendscores_min_seconds";

	private const int FRIENDSCORES_MIN_SECONDS_DEFAULT = 120;

	private float _lastConsolidateFriendCompleteTime;

	private List<string> _latestPreConsolidationFacebookIds;

	private List<string> _latestSuccesfullyConsolidatedFacebookIds;

	private string _tempRegisterUserData;

	private static SocialManager _instance;

	private int _userid;

	private bool _isRunningFacebookLoginCoroutine;

	private Action<FacebookProfile> _facebookPictureDownloadedHandler;

	private Action _friendsConsolidatedHandler;

	private FacebookProfile _fbProfile;

	private List<Friend> _friends;

	private Dictionary<string, Hashtable> _fbFriends;

	private bool _fbReady;

	private DateTime _lastFriendScoreUpdateTimestamp = new DateTime(0L);

	private FacebookCurrentRequest _fbCurrentRequest;

	private bool _consolidatedFriendsCompleted;

	private Dictionary<string, Friend.Status> _friendStatus;

	private bool _dirty;

	private IAchievement[] achievement = new IAchievement[41];

	private Dictionary<string, FacebookProfile> _fbProfiles;

	public bool isRunningFacebookLogin
	{
		get
		{
			return _isRunningFacebookLoginCoroutine;
		}
	}

	public FacebookProfile facebookProfile
	{
		get
		{
			return _fbProfile;
		}
	}

	public Texture2D localUserImage
	{
		get
		{
			if (facebookProfile != null)
			{
				return facebookProfile.image;
			}
			if (Social.localUser != null && Social.localUser.authenticated)
			{
				return Social.localUser.image;
			}
			return null;
		}
	}

	public string localUserName
	{
		get
		{
			if (facebookProfile != null)
			{
				return facebookProfile.name;
			}
			if (Social.localUser != null && Social.localUser.authenticated)
			{
				return Social.localUser.userName;
			}
			return "Me";
		}
	}

	public static SocialManager instance
	{
		get
		{
			Init();
			return _instance;
		}
	}

	public bool facebookIsLoggedIn
	{
		get
		{
			return FacebookAndroid.isSessionValid();
		}
	}

	public bool consolidatedFriendsCompleted
	{
		get
		{
			return _consolidatedFriendsCompleted;
		}
	}

	public bool dirty
	{
		get
		{
			return _dirty;
		}
	}

	public void AddFacebookPictureDownloadedHandler(Action<FacebookProfile> handler)
	{
		_facebookPictureDownloadedHandler = (Action<FacebookProfile>)Delegate.Combine(_facebookPictureDownloadedHandler, handler);
	}

	public void RemoveFacebookPictureDownloadedHandler(Action<FacebookProfile> handler)
	{
		_facebookPictureDownloadedHandler = (Action<FacebookProfile>)Delegate.Remove(_facebookPictureDownloadedHandler, handler);
	}

	public void AddFriendsConsolidatedHandler(Action handler)
	{
		_friendsConsolidatedHandler = (Action)Delegate.Combine(_friendsConsolidatedHandler, handler);
	}

	public void RemoveFriendsConsolidatedHandler(Action handler)
	{
		_friendsConsolidatedHandler = (Action)Delegate.Remove(_friendsConsolidatedHandler, handler);
	}

	public Friend[] FriendsSortedByScore()
	{
		if (_friends != null)
		{
			Friend[] array = _friends.ToArray();
			Array.Sort(array, (Friend x, Friend y) => y.score - x.score);
			return array;
		}
		return new Friend[0];
	}

	public Friend[] FriendsSortedByCash()
	{
		if (_friends != null)
		{
			Friend[] array = _friends.ToArray();
			Array.Sort(array, (Friend x, Friend y) => y.gamesToCashIn - x.gamesToCashIn);
			return array;
		}
		return new Friend[0];
	}

	public static void Init()
	{
		if (_instance == null)
		{
			GameObject gameObject = new GameObject();
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			gameObject.AddComponent<FacebookManager>();
			gameObject.AddComponent<SocialManager>();
		}
	}

	private void InitPushNotifications()
	{
		SocialManagerAndroid.instance.InitPushNotifications();
	}

	public void InitGameCenter()
	{
	}

	public bool FacebookLogin(Action<bool> onComplete)
	{
		if (!_isRunningFacebookLoginCoroutine)
		{
			_fbReady = false;
			_isRunningFacebookLoginCoroutine = true;
			StartCoroutine(FacebookLoginCoroutine(onComplete));
			return true;
		}
		LogWarning("SocialManager.FacebookLogin called, but we are already logging in", this);
		return false;
	}

	private void HandleLoggingInComplete(bool success, Action<bool> onComplete)
	{
		_isRunningFacebookLoginCoroutine = false;
		if (onComplete != null)
		{
			onComplete(success);
		}
	}

	public void FacebookLogout()
	{
		FacebookAndroid.logout();
	}

	public static bool IsOAuthException(string error, object obj)
	{
		if (string.IsNullOrEmpty(error) && obj != null)
		{
			Hashtable hashtable = obj as Hashtable;
			if (hashtable != null)
			{
				Hashtable hashtable2 = hashtable["error"] as Hashtable;
				if (hashtable2 != null && (string)hashtable2["type"] == "OAuthException")
				{
					return true;
				}
			}
		}
		if (error != null && error.Contains("java.io.FileNotFoundException"))
		{
			return true;
		}
		return false;
	}

	private IEnumerator FacebookLoginCoroutine(Action<bool> onComplete)
	{
		if (!facebookIsLoggedIn)
		{
			_fbCurrentRequest = FacebookCurrentRequest.LoggingIn;
			FacebookAndroid.loginWithRequestedPermissions(new string[3] { "publish_stream", "email", "user_birthday" });
			float timeOutLeft = 600f;
			float lastRealTime = Time.realtimeSinceStartup;
			while (_fbCurrentRequest != 0)
			{
				if (_fbCurrentRequest == FacebookCurrentRequest.Error)
				{
					HandleLoggingInComplete(false, onComplete);
					yield break;
				}
				float realTime = Time.realtimeSinceStartup;
				float deltaRealTime = realTime - lastRealTime;
				lastRealTime = realTime;
				if (deltaRealTime < 0.5f)
				{
					timeOutLeft -= deltaRealTime;
					if (timeOutLeft <= 0f)
					{
						_fbCurrentRequest = FacebookCurrentRequest.Error;
						HandleLoggingInComplete(false, onComplete);
						LogWarning("SocialManager WARNING: Facebook login timed out", this);
						yield break;
					}
				}
				yield return null;
			}
		}
		else
		{
			FacebookAndroid.extendAccessToken();
		}
	}

	public void Invalidate()
	{
		bool flag = false;
		if (!SocialManagerAndroid.instance.isAuthenticated() || (facebookIsLoggedIn && !_fbReady))
		{
			return;
		}
		if (ShouldRegisterUser())
		{
			RegisterUser(delegate
			{
			});
		}
		if (ShouldConsolidateFriends())
		{
			ConsolidateFriends(delegate
			{
				_consolidatedFriendsCompleted = true;
				_lastConsolidateFriendCompleteTime = Time.realtimeSinceStartup;
			});
		}
	}

	private bool ShouldRegisterUser()
	{
		if (PlayerPrefs.HasKey("socmanlastregtime"))
		{
			string @string = PlayerPrefs.GetString("socmanlastregtime");
			long result;
			if (long.TryParse(@string, out result))
			{
				DateTime dateTime = new DateTime(result);
				int value = OnlineSettings.instance.GetValue("social_register_min_seconds", 3600);
				if (DateTime.Now >= dateTime + new TimeSpan(0, 0, value))
				{
					return true;
				}
				string fbid;
				string gcid;
				string deviceToken;
				string bundleVersion;
				string highestScore;
				string registerUserData;
				FetchRegisterUserParams(out fbid, out gcid, out deviceToken, out bundleVersion, out highestScore, out registerUserData);
				if ((!string.IsNullOrEmpty(fbid) || !string.IsNullOrEmpty(gcid)) && _userid == 0)
				{
					return true;
				}
				string string2 = PlayerPrefs.GetString("socmanlastregdata", string.Empty);
				if (string2 != registerUserData)
				{
					return true;
				}
				return false;
			}
			LogError("SocialManager.ShouldRegisterUser() Invalid ticks in playerprefs: " + @string);
			return true;
		}
		return true;
	}

	private bool ShouldConsolidateFriends()
	{
		if (!consolidatedFriendsCompleted)
		{
			return true;
		}
		if (HasUnconsolidatedFacebookFriends())
		{
			return true;
		}
		float num = Time.realtimeSinceStartup - _lastConsolidateFriendCompleteTime;
		int value = OnlineSettings.instance.GetValue("social_consolidate_min_seconds", 3600);
		if (num >= (float)value)
		{
			return true;
		}
		return false;
	}

	private bool HasUnconsolidatedFacebookFriends()
	{
		if (_fbFriends != null)
		{
			if (_latestSuccesfullyConsolidatedFacebookIds == null)
			{
				return true;
			}
			foreach (string key in _fbFriends.Keys)
			{
				if (!_latestSuccesfullyConsolidatedFacebookIds.Contains(key))
				{
					return true;
				}
			}
		}
		return false;
	}

	public void CollectFriendReward(Friend friend)
	{
		friend.status.gamesCashedIn = friend.games;
		_dirty = true;
	}

	public int CashIn(Friend friend, int max)
	{
		int num = friend.games - friend.status.gamesCashedIn;
		if (num > 0)
		{
			friend.status.gamesCashedIn = friend.games;
			_dirty = true;
			return Mathf.Max(num, max);
		}
		return 0;
	}

	public int CashInAll(int maxPerFriend)
	{
		if (_friends == null)
		{
			return 0;
		}
		int num = 0;
		foreach (Friend friend in _friends)
		{
			num += CashIn(friend, maxPerFriend);
		}
		return num;
	}

	public void WriteTo(Stream stream)
	{
		BinaryWriter binaryWriter = new BinaryWriter(stream);
		binaryWriter.Write((byte)1);
		if (_friendStatus != null)
		{
			binaryWriter.Write(_friendStatus.Count);
			{
				foreach (KeyValuePair<string, Friend.Status> item in _friendStatus)
				{
					binaryWriter.Write(item.Key);
					binaryWriter.Write(item.Value.gamesCashedIn);
					binaryWriter.Write(item.Value.lastPokeTime.ToBinary());
				}
				return;
			}
		}
		binaryWriter.Write(0);
	}

	public void ReadFrom(Stream stream)
	{
		BinaryReader binaryReader = new BinaryReader(stream);
		byte b = binaryReader.ReadByte();
		if (b == 1)
		{
			int num = binaryReader.ReadInt32();
			_friendStatus = new Dictionary<string, Friend.Status>(num);
			for (int i = 0; i < num; i++)
			{
				string text = binaryReader.ReadString();
				if (!string.IsNullOrEmpty(text))
				{
					Friend.Status status = new Friend.Status();
					status.gamesCashedIn = binaryReader.ReadInt32();
					status.lastPokeTime = DateTime.FromBinary(binaryReader.ReadInt64());
					_friendStatus[text] = status;
				}
			}
			return;
		}
		throw new IOException("Unsupported playerdata file version");
	}

	private static string GetSaveDataPath()
	{
		return Application.persistentDataPath + "/socialdata";
	}

	private static bool ArraysAreEqual<T>(T[] a, T[] b)
	{
		if (a == null && b == null)
		{
			return true;
		}
		if (a.Length != b.Length)
		{
			return false;
		}
		for (int i = 0; i < a.Length; i++)
		{
			if (!object.Equals(a[i], b[i]))
			{
				return false;
			}
		}
		return true;
	}

	public void Load()
	{
		try
		{
			string saveDataPath = GetSaveDataPath();
			byte[] buffer = FileUtil.Load(saveDataPath, "resxrctrv7tgv7gb8h9h9u0909kllfmolkjnhghgjjkhjghg");
			MemoryStream memoryStream = new MemoryStream(buffer);
			ReadFrom(memoryStream);
			memoryStream.Close();
			_dirty = false;
		}
		catch (FileNotFoundException)
		{
		}
		catch (Exception ex2)
		{
			LogError("Could not load data: " + ex2.Message, this);
		}
	}

	public bool Save()
	{
		try
		{
			MemoryStream memoryStream = new MemoryStream(8192);
			WriteTo(memoryStream);
			byte[] buffer = memoryStream.GetBuffer();
			FileUtil.Save(GetSaveDataPath(), "resxrctrv7tgv7gb8h9h9u0909kllfmolkjnhghgjjkhjghg", buffer, 0, (int)memoryStream.Length);
			memoryStream.Close();
			_dirty = false;
			return true;
		}
		catch (Exception ex)
		{
			LogError("Error saving social data: " + ex.GetType().Name + ": " + ex.Message + "\n" + ex.StackTrace, this);
		}
		return false;
	}

	private void OnGCMRegistrationComplete(string regId)
	{
		Invalidate();
	}

	private void OnGCMRegistrationError()
	{
		Invalidate();
	}

	private void Awake()
	{
		if (_instance != null)
		{
			LogError("SocialManager ERROR: Instance already set - has the script been added directly to a GameObject?", this);
			UnityEngine.Object.Destroy(this);
			return;
		}
		_instance = this;
		Load();
		string androidToken = SocialManagerAndroid.instance.AndroidToken;
		FacebookAndroid.init("254616967963463");
		InitPushNotifications();
		if (facebookIsLoggedIn)
		{
			FacebookLogin(null);
		}
	}

	private void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			Save();
			return;
		}
		if (facebookIsLoggedIn)
		{
			FacebookLogin(null);
		}
		SocialManagerAndroid.instance.TryToGetGCMToken();
	}

	private void OnEnable()
	{
		FacebookManager.loginSucceededEvent += facebookLoginSucceeded;
		FacebookManager.loginFailedEvent += facebookLoginFailed;
		FacebookManager.loggedOutEvent += facebookLoggedOut;
		FacebookManager.accessTokenExtendedEvent += facebookAccessTokenExtended;
		FacebookManager.failedToExtendTokenEvent += facebookFailedToExtendToken;
		FacebookManager.sessionInvalidatedEvent += facebookSessionInvalidatedEvent;
		FacebookManager.dialogCompletedEvent += facebokDialogCompleted;
		FacebookManager.dialogCompletedWithUrlEvent += facebookDialogCompletedWithUrl;
		FacebookManager.dialogDidNotCompleteEvent += facebookDialogDidNotComplete;
		FacebookManager.dialogFailedEvent += facebookDialogFailed;
		FacebookManager.customRequestReceivedEvent += facebookCustomRequestReceived;
		FacebookManager.customRequestFailedEvent += facebookCustomRequestFailed;
		SocialManagerAndroid.onGCMRegistrationComplete += OnGCMRegistrationComplete;
		SocialManagerAndroid.onGCMRegistrationError += OnGCMRegistrationError;
	}

	private void OnDisable()
	{
		FacebookManager.loginSucceededEvent -= facebookLoginSucceeded;
		FacebookManager.loginFailedEvent -= facebookLoginFailed;
		FacebookManager.loggedOutEvent -= facebookLoggedOut;
		FacebookManager.accessTokenExtendedEvent -= facebookAccessTokenExtended;
		FacebookManager.failedToExtendTokenEvent -= facebookFailedToExtendToken;
		FacebookManager.sessionInvalidatedEvent -= facebookSessionInvalidatedEvent;
		FacebookManager.dialogCompletedEvent -= facebokDialogCompleted;
		FacebookManager.dialogCompletedWithUrlEvent -= facebookDialogCompletedWithUrl;
		FacebookManager.dialogDidNotCompleteEvent -= facebookDialogDidNotComplete;
		FacebookManager.dialogFailedEvent -= facebookDialogFailed;
		FacebookManager.customRequestReceivedEvent -= facebookCustomRequestReceived;
		FacebookManager.customRequestFailedEvent -= facebookCustomRequestFailed;
		SocialManagerAndroid.onGCMRegistrationComplete -= OnGCMRegistrationComplete;
		SocialManagerAndroid.onGCMRegistrationError -= OnGCMRegistrationError;
	}

	public void ProgressThisAchievement(int achievementIndex, float percentCompleted)
	{
		if (!Social.localUser.authenticated)
		{
			return;
		}
		try
		{
			if (achievement[achievementIndex] == null)
			{
				achievement[achievementIndex] = Social.CreateAchievement();
			}
			if (achievement[achievementIndex].id == "unknown")
			{
				achievement[achievementIndex].id = Achievements.Instance.achievementIds[achievementIndex];
			}
			if (achievement[achievementIndex].percentCompleted == (double)percentCompleted)
			{
				return;
			}
			achievement[achievementIndex].percentCompleted = percentCompleted;
			achievement[achievementIndex].ReportProgress(delegate(bool result)
			{
				if (!result)
				{
				}
			});
		}
		catch (Exception ex)
		{
			LogWarning("Error while setting Achievement: " + ex.Message, this);
		}
	}

	public void CompleteThisAchievement(string achievementId, float percentCompleted = 100f)
	{
		if (!Social.localUser.authenticated)
		{
			return;
		}
		try
		{
			percentCompleted = Mathf.Clamp(percentCompleted, 0f, 100f);
			IAchievement achievement = Social.CreateAchievement();
			achievement.id = achievementId;
			achievement.percentCompleted = percentCompleted;
			achievement.ReportProgress(delegate(bool result)
			{
				if (!result)
				{
				}
			});
		}
		catch (Exception ex)
		{
			LogWarning("Error while setting Achievement: " + ex.Message, this);
		}
	}

	private void facebookLoginSucceeded()
	{
		if (_fbCurrentRequest == FacebookCurrentRequest.LoggingIn)
		{
			_fbCurrentRequest = FacebookCurrentRequest.None;
			Flurry.LogFacebookLogin();
		}
		else
		{
			LogWarning(string.Concat("Received facebook login message, but we are not in that state (is ", _fbCurrentRequest, ")"), this);
		}
	}

	private void facebookLoginFailed(string error)
	{
		Debug.Log("Facebook login failed: " + error, this);
		if (_fbCurrentRequest == FacebookCurrentRequest.LoggingIn)
		{
			_fbCurrentRequest = FacebookCurrentRequest.Error;
			string iosSystemVersion = DeviceUtility.GetIosSystemVersion();
			int num = iosSystemVersion.IndexOf(".");
			string s = ((num == -1) ? iosSystemVersion : iosSystemVersion.Substring(0, num));
			int result;
			if (int.TryParse(s, out result) && result >= 6)
			{
				DeviceUtility.showNativePopup("Facebook", "Please go to iOS Settings -> Facebook and allow Subway Surfers to use your account. ", "Ok");
			}
		}
		else
		{
			LogWarning("Received facebook login failed message, but we are not in that state", this);
		}
	}

	private void facebookLoggedOut()
	{
		if (_friends != null)
		{
			_friends.RemoveAll((Friend item) => item.gcProfile == null && item.fbProfile != null);
			_friends.ForEach(delegate(Friend item)
			{
				item.fbProfile = null;
			});
		}
		_latestSuccesfullyConsolidatedFacebookIds = null;
		_fbFriends = null;
	}

	private void facebookAccessTokenExtended(DateTime newExpiry)
	{
	}

	private void facebookFailedToExtendToken()
	{
	}

	private void facebookSessionInvalidatedEvent()
	{
	}

	private void facebookReceivedUsername(string username)
	{
	}

	private void facebookUsernameRequestFailed(string error)
	{
	}

	private void facebookPost()
	{
	}

	private void facebookPostFailed(string error)
	{
	}

	private void facebokDialogCompleted()
	{
	}

	private void facebookDialogCompletedWithUrl(string url)
	{
		if (url.Contains("post_id="))
		{
			Flurry.LogEvent("Share link on Facebook");
		}
	}

	private void facebookDialogDidNotComplete()
	{
	}

	private void facebookDialogFailed(string error)
	{
	}

	private void facebookCustomRequestReceived(object obj)
	{
		ResultLogger.logObject(obj);
	}

	private void facebookCustomRequestFailed(string error)
	{
	}

	private static string GetRandomIdentifier()
	{
		string text = ((!Application.isEditor) ? SystemInfo.deviceUniqueIdentifier : "0000000000000000000000000000000000000000");
		return text + UnityEngine.Random.Range(0, int.MaxValue);
	}

	public static string GetChecksum(string data)
	{
		return GetSHA1Hash(data + "resxrctrv7tgv7gb8h9h9u0909kllfmolkjnhghgjjkhjghg");
	}

	private static string GetChecksum(params string[] data)
	{
		return GetChecksum(string.Join(null, data));
	}

	private static string GetSHA1Hash(string unhashed)
	{
		SHA1 sHA = SHA1.Create();
		byte[] array = sHA.ComputeHash(Encoding.Default.GetBytes(unhashed));
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < array.Length; i++)
		{
			stringBuilder.Append(array[i].ToString("x2"));
		}
		return stringBuilder.ToString();
	}

	private static IEnumerator WWWRequestCoroutine(WWWComplete onWWWComplete, string relativeUrl, object cookie, params string[] postItems)
	{
		string url = "http://hoodrunner.kiloo.com" + relativeUrl;
		string identifier = GetRandomIdentifier();
		StringBuilder checksumSB = new StringBuilder();
		for (int j = 0; j < postItems.Length; j += 2)
		{
			if (postItems[j] == null)
			{
				LogError("WWWRequestCoroutine: Post item key " + j / 2 + " is null, excluding item");
				continue;
			}
			if (postItems[j + 1] == null)
			{
				LogError("WWWRequestCoroutine: Value for Post item " + postItems[j] + " is null, using empty string");
				postItems[j + 1] = string.Empty;
			}
			checksumSB.Append(postItems[j + 1]);
		}
		string checksum = GetChecksum(identifier + checksumSB.ToString());
		WWWForm postData = new WWWForm();
		postData.AddField("identifier", identifier);
		postData.AddField("checksum", checksum);
		StringBuilder sb = new StringBuilder();
		sb.Append("WWWRequest(").Append(url).Append(")\n");
		for (int i = 0; i < postItems.Length; i += 2)
		{
			sb.Append("Adding post data: \"").Append(postItems[i]).Append("\" = \"")
				.Append(postItems[i + 1])
				.Append("\"\n");
			if (postItems[i] != null)
			{
				postData.AddField(postItems[i], postItems[i + 1]);
			}
		}
		WWW www = new WWW(url, postData);
		yield return www;
		if (!string.IsNullOrEmpty(www.error))
		{
			yield break;
		}
		if (www.text != null)
		{
			sb.Append("Text: \"").Append(www.text).Append("\"\n");
		}
		if (www.error != null)
		{
			sb.Append("Error: \"").Append(www.error).Append("\"\n");
		}
		if (onWWWComplete == null)
		{
			yield break;
		}
		if (www.error != null)
		{
			onWWWComplete(WWWRequestResult.Error, null, cookie);
			yield break;
		}
		string result = null;
		int resultStart2 = www.text.IndexOf("<result>", StringComparison.OrdinalIgnoreCase);
		if (resultStart2 >= 0)
		{
			resultStart2 += 8;
			int resultEnd = www.text.IndexOf("</result>", resultStart2, StringComparison.OrdinalIgnoreCase);
			if (resultEnd > resultStart2)
			{
				result = www.text.Substring(resultStart2, resultEnd - resultStart2);
			}
			else if (resultEnd == resultStart2)
			{
				result = string.Empty;
			}
		}
		onWWWComplete((result == null) ? WWWRequestResult.Error : WWWRequestResult.Success, result, cookie);
	}

	private static string ByteArrayToHex(byte[] barray)
	{
		char[] array = new char[barray.Length * 2];
		for (int i = 0; i < barray.Length; i++)
		{
			byte b = (byte)(barray[i] >> 4);
			array[i * 2] = (char)((b <= 9) ? (b + 48) : (b + 55));
			b = (byte)(barray[i] & 0xFu);
			array[i * 2 + 1] = (char)((b <= 9) ? (b + 48) : (b + 55));
		}
		return new string(array);
	}

	private static string GetBundleVersion()
	{
		return DeviceUtility.GetBundleVersion();
	}

	private void RegisterUser(Action<bool> registerUserCompleted)
	{
		string fbid;
		string gcid;
		string deviceToken;
		string bundleVersion;
		string highestScore;
		FetchRegisterUserParams(out fbid, out gcid, out deviceToken, out bundleVersion, out highestScore, out _tempRegisterUserData);
		StartCoroutine(WWWRequestCoroutine(WWWRegisterUserCompleted, "/register2.php?android", registerUserCompleted, "version", bundleVersion, "fbid", fbid, "devicetoken", deviceToken, "score", highestScore, "meters", "0", "games", "0", "rank", PlayerInfo.Instance.GetCurrentRank().ToString()));
	}

	private void FetchRegisterUserParams(out string fbid, out string gcid, out string deviceToken, out string bundleVersion, out string highestScore, out string registerUserData)
	{
		fbid = ((_fbProfile != null) ? _fbProfile.id : string.Empty);
		bundleVersion = GetBundleVersion();
		highestScore = PlayerInfo.Instance.highestScore.ToString();
		deviceToken = SocialManagerAndroid.instance.AndroidToken;
		gcid = string.Empty;
		registerUserData = fbid + gcid + deviceToken + bundleVersion + highestScore;
	}

	private void WWWRegisterUserCompleted(WWWRequestResult result, string output, object cookie)
	{
		bool flag = false;
		if (result == WWWRequestResult.Success)
		{
			flag = true;
			Dictionary<string, string> dictionary = StringUtility.ParseProperties(output);
			if (dictionary.ContainsKey("userid"))
			{
				string text = dictionary["userid"];
				string text2 = dictionary["score"];
				string text3 = dictionary["meters"];
				string text4 = dictionary["games"];
				string text5 = dictionary["rank"];
				string strA = dictionary["checksum"];
				string checksum = GetChecksum(text, text2, text3, text4, text5);
				if (string.Compare(strA, checksum, true) == 0)
				{
					try
					{
						int userid = int.Parse(text);
						int highestScore = int.Parse(text2);
						int highestMeters = int.Parse(text3);
						_userid = userid;
						PlayerInfo.Instance.highestScore = highestScore;
						PlayerInfo.Instance.highestMeters = highestMeters;
					}
					catch (Exception)
					{
						LogError("Error parsing output data from register user");
						flag = false;
					}
				}
				else
				{
					LogError("Output data from register user corrupted or tampered with");
					flag = false;
				}
			}
		}
		if (cookie != null)
		{
			((Action<bool>)cookie)(flag);
		}
		if (flag && !string.IsNullOrEmpty(_tempRegisterUserData))
		{
			string value = DateTime.Now.Ticks.ToString();
			PlayerPrefs.SetString("socmanlastregtime", value);
			PlayerPrefs.SetString("socmanlastregdata", _tempRegisterUserData);
			_tempRegisterUserData = null;
		}
	}

	private string GetFBListAsStringAndSaveAsPreConsolidated()
	{
		if (_fbFriends != null)
		{
			if (_latestPreConsolidationFacebookIds == null)
			{
				_latestPreConsolidationFacebookIds = new List<string>();
			}
			else
			{
				_latestPreConsolidationFacebookIds.Clear();
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string key in _fbFriends.Keys)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(';');
				}
				stringBuilder.Append(key);
				_latestPreConsolidationFacebookIds.Add(key);
			}
			return stringBuilder.ToString();
		}
		return string.Empty;
	}

	private void ConsolidateFriends(Action<bool> consolidateFriendsCompleted)
	{
		string fBListAsStringAndSaveAsPreConsolidated = GetFBListAsStringAndSaveAsPreConsolidated();
		string empty = string.Empty;
		if (string.IsNullOrEmpty(fBListAsStringAndSaveAsPreConsolidated) && string.IsNullOrEmpty(empty))
		{
			if (consolidateFriendsCompleted != null)
			{
				consolidateFriendsCompleted(true);
			}
			Action friendsConsolidatedHandler = _friendsConsolidatedHandler;
			if (friendsConsolidatedHandler != null)
			{
				friendsConsolidatedHandler();
			}
		}
		else
		{
			StartCoroutine(WWWRequestCoroutine(WWWConsolidateFriendsCompleted, "/friends2.php?android", consolidateFriendsCompleted, "fblist", fBListAsStringAndSaveAsPreConsolidated, "gclist", empty));
		}
	}

	private static string[][] ParseSets(string setsString)
	{
		string[] separator = new string[1] { ");(" };
		string[] array = setsString.Split(separator, StringSplitOptions.RemoveEmptyEntries);
		if (array.Length > 0)
		{
			if (array[0][0] == '(')
			{
				array[0] = array[0].Substring(1);
			}
			int num = array.Length - 1;
			int num2 = array[num].Length - 1;
			if (array[num][num2] == ')')
			{
				array[num] = array[num].Remove(num2);
			}
			string[][] array2 = new string[array.Length][];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = array[i].Split(';');
			}
			return array2;
		}
		return new string[0][];
	}

	private void WWWConsolidateFriendsCompleted(WWWRequestResult result, string output, object cookie)
	{
		bool obj = false;
		if (result == WWWRequestResult.Success)
		{
			Dictionary<string, string> dictionary = StringUtility.ParseProperties(output);
			if (dictionary.ContainsKey("friendslist"))
			{
				string text = dictionary["friendslist"];
				string strA = dictionary["checksum"];
				string checksum = GetChecksum(text);
				if (string.Compare(strA, checksum, true) == 0)
				{
					if (string.IsNullOrEmpty(text))
					{
						_friends = null;
					}
					else
					{
						string[][] array = ParseSets(text);
						_friends = new List<Friend>(array.Length);
						string[][] array2 = array;
						foreach (string[] array3 in array2)
						{
							if (array3.Length >= 6 && (array3[1].Length > 0 || array3[2].Length > 0))
							{
								try
								{
									Friend friend = new Friend();
									friend.userid = int.Parse(array3[0]);
									string text2 = array3[1];
									if (text2.Length > 0)
									{
									}
									string text3 = array3[2];
									if (text3.Length > 0)
									{
										if (_fbProfiles == null)
										{
											_fbProfiles = new Dictionary<string, FacebookProfile>();
										}
										FacebookProfile facebookProfile;
										if (_fbProfiles.ContainsKey(text3))
										{
											facebookProfile = _fbProfiles[text3];
										}
										else
										{
											Hashtable hashtable = _fbFriends[text3];
											facebookProfile = new FacebookProfile();
											facebookProfile.id = text3;
											facebookProfile.name = (string)hashtable["first_name"];
											facebookProfile.fullName = (string)hashtable["name"];
											_fbProfiles[text3] = facebookProfile;
										}
										friend.fbProfile = facebookProfile;
									}
									friend.score = int.Parse(array3[3]);
									friend.meters = int.Parse(array3[4]);
									friend.games = int.Parse(array3[5]);
									friend.rank = ((array3.Length >= 7) ? int.Parse(array3[6]) : 0);
									Friend.Status status = null;
									if (_friendStatus == null)
									{
										_friendStatus = new Dictionary<string, Friend.Status>();
									}
									if (friend.fbProfile != null && _friendStatus.ContainsKey(friend.fbProfile.id))
									{
										status = _friendStatus[friend.fbProfile.id];
									}
									else if (friend.gcProfile != null && _friendStatus.ContainsKey(friend.gcProfile.id))
									{
										status = _friendStatus[friend.gcProfile.id];
									}
									else
									{
										status = new Friend.Status();
										status.gamesCashedIn = friend.games;
										string key = ((friend.fbProfile == null) ? friend.gcProfile.id : friend.fbProfile.id);
										_friendStatus[key] = status;
										_dirty = true;
									}
									friend.status = status;
									if (_friends != null)
									{
										_friends.Add(friend);
									}
								}
								catch (Exception ex)
								{
									LogError("Friend parse error " + ex.ToString());
								}
							}
							else
							{
								LogError("Malformed friend: (" + string.Join(";", array3) + ")");
							}
						}
						if (_fbProfiles != null)
						{
							StartCoroutine(DownloadFacebookPictures(_fbProfiles));
						}
					}
					if (ShouldLogDailyFlurryEvent("Social Friends Consolidated"))
					{
						int count = 0;
						int num = 0;
						int num2 = 0;
						if (_friends != null)
						{
							count = _friends.Count;
							foreach (Friend friend2 in _friends)
							{
								if (friend2.fbProfile != null)
								{
									num++;
								}
								if (friend2.gcProfile != null)
								{
									num2++;
								}
							}
						}
						string text4 = Flurry.ConvertFriendCountToBracket(count);
						string text5 = Flurry.ConvertFriendCountToBracket(num);
						string text6 = Flurry.ConvertFriendCountToBracket(num2);
						Flurry.LogEventWithSeveralParameters("Social Friends Consolidated", "Total;Facebook;GameCenter", text4 + ";" + text5 + ";" + text6);
					}
					if (_latestPreConsolidationFacebookIds != null)
					{
						_latestSuccesfullyConsolidatedFacebookIds = new List<string>(_latestPreConsolidationFacebookIds);
					}
					obj = true;
				}
				else
				{
					LogError("Consolidated friend data corrupted");
				}
			}
		}
		if (cookie != null)
		{
			((Action<bool>)cookie)(obj);
		}
		if (_friendsConsolidatedHandler != null)
		{
			_friendsConsolidatedHandler();
		}
	}

	public void ReportScore(int newScore, bool isNewHighscore, int newMeters)
	{
		if (_userid > 0)
		{
			int num = PlayerInfo.Instance.IncrementUnreportedGames();
			bool flag = false;
			if (isNewHighscore)
			{
				flag = true;
			}
			else if (num >= OnlineSettings.instance.GetValue("social_report_games_count", 5))
			{
				flag = true;
			}
			else if (num > 1 && DateTime.Now > PlayerInfo.Instance.GetFirstUnreportedGameTimestamp() + GetForceReportScoreTimeSpan())
			{
				flag = true;
			}
			if (flag)
			{
				StartCoroutine(WWWRequestCoroutine(WWWReportScoreCompleted, "/report2.php?android", null, "userid", _userid.ToString(), "score", newScore.ToString(), "games", num.ToString()));
			}
		}
	}

	private void WWWReportScoreCompleted(WWWRequestResult result, string output, object cookie)
	{
		if (result == WWWRequestResult.Success)
		{
			PlayerInfo.Instance.ClearUnreportedGames();
		}
	}

	private TimeSpan GetForceReportScoreTimeSpan()
	{
		int num = OnlineSettings.instance.GetValue("social_report_min_seconds", 1200);
		if (num < 0)
		{
			num = 1200;
		}
		return new TimeSpan(0, 0, num);
	}

	public void UpdateFriendScores(Action<bool> updateFriendsScoresCompleted)
	{
		DateTime now = DateTime.Now;
		double totalSeconds = (now - _lastFriendScoreUpdateTimestamp).TotalSeconds;
		if (totalSeconds < (double)OnlineSettings.instance.GetValue("social_friendscores_min_seconds", 120))
		{
			return;
		}
		_lastFriendScoreUpdateTimestamp = now;
		StringBuilder stringBuilder = new StringBuilder();
		if (_friends == null)
		{
			LogWarning("Friends is null. cannot update");
			return;
		}
		foreach (Friend friend in _friends)
		{
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Append(';');
			}
			stringBuilder.Append(friend.userid);
		}
		string text = stringBuilder.ToString();
		StartCoroutine(WWWRequestCoroutine(WWWUpdateFriendScoresCompleted, "/scores2.php?android", updateFriendsScoresCompleted, "idlist", text));
	}

	private void WWWUpdateFriendScoresCompleted(WWWRequestResult result, string output, object cookie)
	{
		bool obj = false;
		if (result == WWWRequestResult.Success)
		{
			Dictionary<string, string> dictionary = StringUtility.ParseProperties(output);
			if (dictionary.ContainsKey("scores"))
			{
				string text = dictionary["scores"];
				string strA = dictionary["checksum"];
				string checksum = GetChecksum(text);
				if (string.Compare(strA, checksum, true) == 0)
				{
					try
					{
						string[][] array = ParseSets(text);
						string[][] array2 = array;
						foreach (string[] array3 in array2)
						{
							if (array3.Length >= 4)
							{
								int userid = int.Parse(array3[0]);
								Friend friend = _friends.Find((Friend f) => f.userid == userid);
								if (friend != null)
								{
									friend.score = int.Parse(array3[1]);
									friend.meters = int.Parse(array3[2]);
									friend.games = int.Parse(array3[3]);
									friend.rank = ((array3.Length >= 5) ? int.Parse(array3[4]) : 0);
								}
								else
								{
									LogWarning("UpdateFriendScores: Unexpected friend user id");
								}
								continue;
							}
							LogError("UpdateFriendScores: Malformed score (" + string.Join(";", array3) + ")");
							throw new Exception();
						}
						obj = true;
					}
					catch (Exception)
					{
						LogError("UpdateFriendScores: Error parsing output data");
					}
				}
				else
				{
					LogError("UpdateFriendScores: Output data corrupt");
				}
			}
		}
		if (cookie != null)
		{
			((Action<bool>)cookie)(obj);
		}
	}

	public void Poke(Friend friend)
	{
		Missions.Instance.PlayerDidThis(Missions.MissionTarget.PokeFriend);
		string text = ((friend.fbProfile != null) ? _fbProfile.fullName : ((!Social.localUser.authenticated) ? string.Empty : Social.localUser.userName));
		StartCoroutine(WWWRequestCoroutine(null, "/poke.php?android", null, "friend", friend.userid.ToString(), "name", text));
		friend.status.lastPokeTime = DateTime.UtcNow;
		_dirty = true;
		Flurry.LogGenericSocialAction();
		Flurry.LogEvent("Social friend poked");
	}

	public void SetPokeFirstTime(Friend friend)
	{
		friend.status.lastPokeTime = DateTime.UtcNow;
		_dirty = true;
	}

	public void BragNotify(int oldScore, List<Friend> friends)
	{
		if (friends == null)
		{
			return;
		}
		int count = friends.Count;
		StringBuilder stringBuilder = new StringBuilder(count * 8);
		StringBuilder stringBuilder2 = new StringBuilder(count * 2);
		foreach (Friend friend in friends)
		{
			int relation = friend.relation;
			int userid = friend.userid;
			if (relation != 0 && userid != 0)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(';');
					stringBuilder2.Append(';');
				}
				stringBuilder.Append(userid);
				stringBuilder2.Append(relation);
			}
		}
		if (stringBuilder.Length > 0)
		{
			string text = ((_fbProfile == null) ? string.Empty : _fbProfile.name);
			string text2 = ((!Social.localUser.authenticated) ? string.Empty : Social.localUser.userName);
			StartCoroutine(WWWRequestCoroutine(null, "/brag.php?android", null, "oldscore", oldScore.ToString(), "newscore", PlayerInfo.Instance.highestScore.ToString(), "useridlist", stringBuilder.ToString(), "relationlist", stringBuilder2.ToString(), "fbname", text, "gcname", text2));
			Flurry.LogGenericSocialAction();
			Flurry.LogEvent("Social bragged");
		}
	}

	private static string GetDeviceTypeString()
	{
		return "iDevice";
	}

	public void RecommendAppFacebook()
	{
		if (facebookIsLoggedIn)
		{
			FacebookAndroid.showPostMessageDialogWithOptions("http://redirect.kiloo.com/subwayapp.php", "Subway Surfers", "http://hoodrunner.kiloo.com/fblogo.png", "Dodge the trains! Help Jake, Tricky and Fresh escape.");
		}
		else
		{
			LogError("Not logged in to facebook");
		}
	}

	public static void showPostMessageDialogWithOptions(string link, string linkName, string linkToImage, string caption)
	{
	}

	public void BragFacebook(List<Friend> friends)
	{
		if (facebookIsLoggedIn)
		{
			List<Friend> list = null;
			if (friends != null)
			{
				list = new List<Friend>(friends.Count);
				foreach (Friend friend in friends)
				{
					if (friend.fbProfile != null && friend.score < PlayerInfo.Instance.highestScore)
					{
						list.Add(friend);
					}
				}
				list.Sort((Friend x, Friend y) => y.score - x.score);
			}
			string value = ((list == null || list.Count == 0) ? ("I just scored " + PlayerInfo.Instance.highestScore + " points dodging trains in Subway Surfers on my " + GetDeviceTypeString() + ". Check it out!") : ((list.Count == 1) ? ("I just scored " + PlayerInfo.Instance.highestScore + " points in Subway Surfers on my " + GetDeviceTypeString() + " and beat " + list[0].fbProfile.fullName) : ((list.Count == 2) ? ("I just scored " + PlayerInfo.Instance.highestScore + " points in Subway Surfers on my " + GetDeviceTypeString() + " and beat " + list[0].fbProfile.fullName + " and " + list[1].fbProfile.fullName) : ((list.Count != 3) ? ("I just scored " + PlayerInfo.Instance.highestScore + " points in Subway Surfers on my " + GetDeviceTypeString() + " and beat " + list[0].fbProfile.fullName + ", " + list[1].fbProfile.fullName + " and " + (list.Count - 2) + " others") : ("I just scored " + PlayerInfo.Instance.highestScore + " points in Subway Surfers on my " + GetDeviceTypeString() + " and beat " + list[0].fbProfile.fullName + ", " + list[1].fbProfile.fullName + " and " + list[2].fbProfile.fullName)))));
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("link", "http://redirect.kiloo.com/subwayapp.php");
			dictionary.Add("name", "New Subway Surfers High Score");
			dictionary.Add("picture", "http://hoodrunner.kiloo.com/fblogo.png");
			dictionary.Add("caption", value);
			dictionary.Add("description", "Download Subway Surfers now");
			Dictionary<string, string> parameters = dictionary;
			FacebookAndroid.showDialog("stream.publish", parameters);
			Flurry.LogGenericSocialAction();
			Flurry.LogEvent("Social bragged Facebook");
		}
		else
		{
			LogError("Not logged in to facebook");
		}
	}

	private IEnumerator DownloadFacebookPicture(FacebookProfile profile)
	{
		if (profile == null)
		{
			LogError("facebook profile was null in DownloadFacebookPictures!");
			yield break;
		}
		string url = "http://graph.facebook.com/" + profile.id + "/picture?type=square";
		WWW www = new WWW(url);
		yield return www;
		if (www.error != null)
		{
		}
		Texture2D image = www.texture;
		if (!(image == null) && (image.width != 8 || image.height != 8))
		{
			profile.image = image;
			if (_facebookPictureDownloadedHandler != null)
			{
				_facebookPictureDownloadedHandler(profile);
			}
		}
	}

	private IEnumerator DownloadFacebookPictures(Dictionary<string, FacebookProfile> fbProfiles)
	{
		List<FacebookProfile> profiles = new List<FacebookProfile>(fbProfiles.Count);
		foreach (FacebookProfile profile2 in fbProfiles.Values)
		{
			if (profile2.image == null)
			{
				profiles.Add(profile2);
			}
		}
		foreach (FacebookProfile profile in profiles)
		{
			yield return StartCoroutine(DownloadFacebookPicture(profile));
		}
	}

	public static bool ShouldLogDailyFlurryEvent(string eventName)
	{
		int dayOfYear = DateTime.Now.DayOfYear;
		string key = "socflurdaily" + eventName;
		if (PlayerPrefs.HasKey(key))
		{
			int @int = PlayerPrefs.GetInt(key);
			if (dayOfYear == @int)
			{
				return false;
			}
		}
		PlayerPrefs.SetInt(key, dayOfYear);
		return true;
	}

	[Conditional("PRINT_DEBUG_ERROR_LOGS")]
	private static void LogError(string msg, UnityEngine.Object context = null)
	{
		Debug.LogError(msg, context);
	}

	[Conditional("PRINT_DEBUG_ERROR_LOGS")]
	private static void LogWarning(string msg, UnityEngine.Object context = null)
	{
		Debug.LogWarning(msg, context);
	}

	[Conditional("PRINT_DEBUG_LOGS")]
	private static void Log(string msg, UnityEngine.Object context = null)
	{
		Debug.Log(msg, context);
	}

	[Conditional("PRINT_DEBUG_LOGS")]
	private static void LogObject(object result)
	{
		if (result != null)
		{
			ResultLogger.logObject(result);
		}
		else
		{
			Debug.Log("null");
		}
	}
}
