using UnityEngine;

public class VungleBridge
{
	public enum Gender
	{
		Male = 0,
		Female = 1
	}

	public const string CLASS_PATH = "com.kiloo.vungleplugin.VungleMain";

	private static VungleBridge _instance;

	private AndroidJavaObject currentActivity;

	private AndroidJavaObject vungleObject;

	private bool isVungleInitialized;

	public static bool isInstanced
	{
		get
		{
			return _instance != null;
		}
	}

	public static VungleBridge Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new VungleBridge();
			}
			return _instance;
		}
	}

	private VungleBridge()
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		currentActivity = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
		vungleObject = new AndroidJavaObject("com.kiloo.vungleplugin.VungleMain", currentActivity);
	}

	public void init(string appkey)
	{
		vungleObject.CallStatic("initVungle", appkey);
		isVungleInitialized = true;
	}

	public void initWithOptions(string appkey, int age, Gender gender)
	{
		vungleObject.CallStatic("initVungleWithOptions", appkey, age, (int)gender);
		isVungleInitialized = true;
	}

	public bool isVideoAvailable()
	{
		if (!isVungleInitialized)
		{
			return false;
		}
		return vungleObject.CallStatic<bool>("isVideoAvailable", new object[0]);
	}

	public bool displayIncentivizedAdvert(bool showClose)
	{
		if (!isVungleInitialized)
		{
			return false;
		}
		return vungleObject.CallStatic<bool>("displayIncentivizedAdvert", new object[1] { showClose });
	}
}
