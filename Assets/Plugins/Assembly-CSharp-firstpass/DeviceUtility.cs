using UnityEngine;

public class DeviceUtility
{
	public enum Location
	{
		none = 0,
		China = 1,
		AnywhereElse = 2
	}

	private static string _callbackGameObjectName;

	private static string _callbackDidCloseFunctionName;

	private static string _button1String;

	public static string GetBundleVersion()
	{
		return GetBundleVersionAndroid();
	}

	public static string GetIosSystemVersion()
	{
		return string.Empty;
	}

	public static string GetIosNotificationTypes()
	{
		return string.Empty;
	}

	public static bool IsOtherAudioPlaying()
	{
		return false;
	}

	public static Location GetLocation()
	{
		return Location.none;
	}

	public static void showNativePopup(string title, string message, string cancelButtonTitle)
	{
		showNativePopupAndroid(title, message, cancelButtonTitle);
	}

	public static void showNativePopupWithCallback(string callbackGameObjectName, string callbackDidCloseFunctionName, string title, string message, string cancelButtonTitle, string optionalButton2, string optionalButton3)
	{
		showNativePopupWithCallbackAndroid(callbackGameObjectName, callbackDidCloseFunctionName, title, message, cancelButtonTitle, optionalButton2, optionalButton3);
	}

	public static int GetVersionCode()
	{
		return GetVersionCodeAndroid();
	}

	private static string GetBundleVersionAndroid()
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		using (AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity"))
		{
			using (AndroidJavaObject androidJavaObject2 = new AndroidJavaObject("com.kiloo.subwaysurf.MainGCM", androidJavaObject))
			{
				string text = androidJavaObject2.CallStatic<string>("getVersionName", new object[0]);
				Debug.Log(string.Format("Version Name: {0}", text));
				return text;
			}
		}
	}

	private static int GetVersionCodeAndroid()
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		using (AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity"))
		{
			using (AndroidJavaObject androidJavaObject2 = new AndroidJavaObject("com.kiloo.subwaysurf.MainGCM", androidJavaObject))
			{
				int num = androidJavaObject2.CallStatic<int>("getVersionCode", new object[0]);
				Debug.Log(string.Format("Version Code: {0}", num));
				return num;
			}
		}
	}

	private static void showNativePopupAndroid(string title, string message, string cancelButtonTitle)
	{
		EtceteraAndroid.showAlert(title, message, cancelButtonTitle);
	}

	public static void showNativePopupWithCallbackAndroid(string callbackGameObjectName, string callbackDidCloseFunctionName, string title, string message, string cancelButtonTitle, string optionalButton2, string optionalButton3)
	{
		_callbackGameObjectName = callbackGameObjectName;
		_callbackDidCloseFunctionName = callbackDidCloseFunctionName;
		_button1String = cancelButtonTitle;
		EtceteraAndroidManager.alertButtonClickedEvent += alertButtonClickedEvent;
		EtceteraAndroid.showAlert(title, message, cancelButtonTitle, optionalButton2);
	}

	private static void alertButtonClickedEvent(string button)
	{
		EtceteraAndroidManager.alertButtonClickedEvent -= alertButtonClickedEvent;
		GameObject gameObject = GameObject.Find(_callbackGameObjectName);
		string value = ((!button.Equals(_button1String)) ? "1" : "0");
		gameObject.SendMessage(_callbackDidCloseFunctionName, value);
	}
}
