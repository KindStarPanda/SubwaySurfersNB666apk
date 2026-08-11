using UnityEngine;

public class VungleClipsProvider : VideoAdProvider
{
	private bool _initialized;

	private int _defaultReward;

	private bool rewardFlag;

	private static VungleClipsProvider _instance;

	public static VungleClipsProvider instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new VungleClipsProvider();
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
			return _initialized;
		}
	}

	private VungleClipsProvider()
	{
	}

	public override void Init()
	{
		if (!_initialized)
		{
			VungleManager.vungleMoviePlayedEvent += vungleMoviePlayedEventAndroid;
			VungleManager.vungleViewDidDisappearEvent += vungleViewDidDisappearEventAndroid;
			VungleManager.vungleViewWillAppearEvent += vungleViewWillAppearEventAndroid;
			VungleBridge.Instance.init("507686ae771615941001aca5");
			_initialized = true;
		}
	}

	private void HandleVungleManagervungleMoviePlayedEvent(string obj)
	{
	}

	public override bool PlayVideoIfAvailable(int defaultReward)
	{
		if (_initialized)
		{
			if (VungleBridge.Instance.isVideoAvailable())
			{
				_defaultReward = defaultReward;
				VungleBridge.Instance.displayIncentivizedAdvert(false);
				return true;
			}
			Debug.Log("VungleBridge: video is not available!");
		}
		else
		{
			Debug.LogError("FlurryClipsProvider PlayVideoIfAvailable() called before initialized");
		}
		return false;
	}

	private void vungleMoviePlayedEventAndroid(string percentPlayed)
	{
		Debug.Log("CallbackEvent: vungleMoviePlayedEventAndroid: " + percentPlayed);
		InvokeHandlerIfNotNull(_onVideoWatched, this, _defaultReward);
	}

	private void vungleViewDidDisappearEventAndroid()
	{
		Debug.Log("CallbackEvent: vungleViewDidDisappearEventAndroid");
		InvokeHandlerIfNotNull(_onTakeoverEnded, this);
	}

	private void vungleViewWillAppearEventAndroid()
	{
		Debug.Log("CallbackEvent: vungleViewWillAppearEventAndroid");
		InvokeHandlerIfNotNull(_onTakeoverBegan, this);
	}
}
