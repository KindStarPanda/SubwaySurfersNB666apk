using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SocialManagerAndroid : MonoBehaviour
{
	private static SocialManagerAndroid _instance;

	private bool _gcmReady;

	private string _androidToken = string.Empty;

	public string AndroidToken
	{
		get
		{
			return _androidToken;
		}
	}

	public static SocialManagerAndroid instance
	{
		get
		{
			Init();
			return _instance;
		}
	}

	[method: MethodImpl(32)]
	public static event Action<string> onGCMRegistrationComplete;

	[method: MethodImpl(32)]
	public static event Action onGCMRegistrationError;

	private static void Init()
	{
		if (_instance == null)
		{
			GameObject gameObject = new GameObject();
			gameObject.name = "SocialManagerAndroid";
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			gameObject.AddComponent<SocialManagerAndroid>();
		}
	}

	private void Awake()
	{
		_instance = this;
	}

	public void InitPushNotifications()
	{
		_gcmReady = false;
		GCMPluginKit.getGCMToken(base.gameObject.name);
	}

	public bool isAuthenticated()
	{
		Debug.Log("isAuthenticated : " + _gcmReady);
		return _gcmReady;
	}

	public void TryToGetGCMToken()
	{
		if (string.IsNullOrEmpty(_androidToken) && _gcmReady)
		{
			_gcmReady = false;
			GCMPluginKit.getGCMToken(base.gameObject.name);
		}
	}

	private void OnGCMRegistrationComplete(string regId)
	{
		Debug.Log("OnGCMRegistrationComplete: " + regId);
		_androidToken = regId;
		_gcmReady = true;
		if (SocialManagerAndroid.onGCMRegistrationComplete != null)
		{
			SocialManagerAndroid.onGCMRegistrationComplete(regId);
		}
	}

	private void OnGCMRegistrationError()
	{
		Debug.LogError("OnGCMRegistrationError");
		_androidToken = string.Empty;
		_gcmReady = true;
		if (SocialManagerAndroid.onGCMRegistrationError != null)
		{
			SocialManagerAndroid.onGCMRegistrationError();
		}
	}
}
