public class FlurryClipsProvider : VideoAdProvider
{
	private bool _initialized;

	private int _defaultReward;

	private static FlurryClipsProvider _instance;

	public static FlurryClipsProvider instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new FlurryClipsProvider();
			}
			return _instance;
		}
	}

	public static bool isVideoProviderSupported
	{
		get
		{
			return false;
		}
	}

	public override bool isInitialized
	{
		get
		{
			return _initialized;
		}
	}

	private FlurryClipsProvider()
	{
	}

	public override void Init()
	{
		if (_initialized)
		{
		}
	}

	public override bool PlayVideoIfAvailable(int defaultReward)
	{
		return false;
	}

	private void OnTakeoverWillDisplay(string hook)
	{
		InvokeHandlerIfNotNull(_onTakeoverBegan, this);
	}

	private void OnTakeoverWillClose()
	{
		InvokeHandlerIfNotNull(_onTakeoverEnded, this);
	}

	private void OnVideoDidFinish(string hook)
	{
		InvokeHandlerIfNotNull(_onVideoWatched, this, _defaultReward);
	}

	private void OnVideoDidNotFinish(string hook)
	{
	}

	private void OnVideoAvailable()
	{
		InvokeHandlerIfNotNull(_onVideoAvailable, this);
	}

	private void OnVideoUnavailable()
	{
		InvokeHandlerIfNotNull(_onVideoUnavailable, this);
	}
}
