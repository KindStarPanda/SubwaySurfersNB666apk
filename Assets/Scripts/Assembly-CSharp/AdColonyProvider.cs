using System;
using UnityEngine;

public class AdColonyProvider : VideoAdProvider
{
	private const string ANDROID_AD_COLONY_GAMEOBJECT_NAME = "AdColony";

	private static AdColonyProvider _instance;

	private bool adColonySoundOnBeforeMute;

	public static AdColonyProvider instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new AdColonyProvider();
			}
			return _instance;
		}
	}

	public static bool isVideoProviderSupported
	{
		get
		{
			return true;
		}
	}

	public override bool isInitialized
	{
		get
		{
			return AdColonyAndroid.isInitialized;
		}
	}

	private AdColonyProvider()
	{
	}

	public override void Init()
	{
		InitAndroid();
	}

	public override bool PlayVideoIfAvailable(int defaultReward)
	{
		return PlayVideoIfAvailableAndroid(defaultReward);
	}

	private void InitAndroid()
	{
		if (!AdColonyAndroid.isInitialized)
		{
			CreateAndroidIAdColonyCallbackObject();
			AdColonyAndroid.Configure("1.5", "appc0d018638a3a47a3b725ab", "vzc54d2d8389a24681852d05");
			AdColonyAndroid.OnVideoStarted = (AdColonyAndroid.VideoStartedDelegate)Delegate.Combine(AdColonyAndroid.OnVideoStarted, new AdColonyAndroid.VideoStartedDelegate(OnVideoStarted));
			AdColonyAndroid.OnVideoFinished = (AdColonyAndroid.VideoFinishedDelegate)Delegate.Combine(AdColonyAndroid.OnVideoFinished, new AdColonyAndroid.VideoFinishedDelegate(OnVideoFinished));
			AdColonyAndroid.OnV4VCResult = (AdColonyAndroid.V4VCResultDelegate)Delegate.Combine(AdColonyAndroid.OnV4VCResult, new AdColonyAndroid.V4VCResultDelegate(OnV4VCResult));
		}
	}

	private void CreateAndroidIAdColonyCallbackObject()
	{
		GameObject gameObject = new GameObject("AdColony");
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
		gameObject.AddComponent<AdColonyAndroid>();
	}

	private bool PlayVideoIfAvailableAndroid(int defaultReward)
	{
		InitAndroid();
		if (AdColonyAndroid.isInitialized)
		{
			if (AdColonyAndroid.IsV4VCAvailable("vzc54d2d8389a24681852d05"))
			{
				AdColonyAndroid.ShowV4VC(true, "vzc54d2d8389a24681852d05");
				return true;
			}
			return false;
		}
		Debug.LogError("AdColonyProvider PlayVideoIfAvailable() called before initialized");
		return false;
	}

	private void OnVideoStarted()
	{
		Debug.Log("On Video Started");
		adColonySoundOnBeforeMute = Settings.optionSound;
		Settings.optionSound = false;
		InvokeHandlerIfNotNull(_onTakeoverBegan, this);
	}

	private void OnVideoFinished()
	{
		Debug.Log("On Video Finished");
		Settings.optionSound = adColonySoundOnBeforeMute;
		InvokeHandlerIfNotNull(_onTakeoverEnded, this);
	}

	private void OnV4VCResult(bool success, string name, int amount)
	{
		if (success)
		{
			Debug.Log("V4VC SUCCESS: name = " + name + ", amount = " + amount);
			Flurry.LogEventWithAParameter("VideoAds adcolony request", "video is received", "Ok");
			InvokeHandlerIfNotNull(_onVideoWatched, this, amount);
		}
		else
		{
			Flurry.LogEventWithAParameter("VideoAds adcolony request", "video is received", "No video");
			InvokeHandlerIfNotNull(_onVideoUnavailable, this);
		}
	}
}
