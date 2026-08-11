using System;
using UnityEngine;

public class AdColony : MonoBehaviour
{
	public enum ZoneStatus
	{
		NoZone = 0,
		Off = 1,
		Loading = 2,
		Active = 3,
		Unknown = 4
	}

	private const string BRIDGE_DELEGATE_GAMEOBJECT_NAME = "AdColonyBridge";

	private static AdColony bridgeDelegate;

	public static Action noVideoFill;

	public static Action videoAdsReady;

	public static Action videoAdsNotReady;

	public static Action<string, int> virtualCurrencyAwarded;

	public static Action<string, int, string> virtualCurrencyNotAwarded;

	public static Action takeoverBegan;

	public static Action<bool> takeoverEndedWithVC;

	public static Action videoAdNotServed;

	public static bool isInitialized
	{
		get
		{
			return bridgeDelegate != null;
		}
	}

	private void OnDestroy()
	{
		if (bridgeDelegate == this)
		{
			bridgeDelegate = null;
		}
	}

	private void invokeHandlerIfNotNull(Action handler)
	{
		if (handler != null)
		{
			handler();
		}
	}

	private void bridge_noVideoFill()
	{
		invokeHandlerIfNotNull(noVideoFill);
	}

	private void bridge_videoAdsReady()
	{
		invokeHandlerIfNotNull(videoAdsReady);
	}

	private void bridge_videoAdsNotReady()
	{
		invokeHandlerIfNotNull(videoAdsNotReady);
	}

	private void bridge_virtualCurrencyAwarded(string message)
	{
		Action<string, int> action = virtualCurrencyAwarded;
		if (action == null)
		{
			return;
		}
		string[] array = message.Split(';');
		if (array.Length == 2)
		{
			int result = 0;
			if (int.TryParse(array[1], out result))
			{
				string arg = array[0];
				action(arg, result);
			}
			else
			{
				Debug.LogError("bridge_virtualCurrencyAwarded: Failed to parse amount: " + message);
			}
		}
		else
		{
			Debug.LogError("bridge_virtualCurrencyAwarded: Failed to parse message: " + message);
		}
	}

	private void bridge_virtualCurrencyNotAwarded(string message)
	{
		Action<string, int, string> action = virtualCurrencyNotAwarded;
		if (action == null)
		{
			return;
		}
		string[] array = message.Split(';');
		if (array.Length == 3)
		{
			int result = 0;
			if (int.TryParse(array[1], out result))
			{
				string arg = array[0];
				string arg2 = array[2];
				action(arg, result, arg2);
			}
			else
			{
				Debug.LogError("bridge_virtualCurrencyNotAwarded: Failed to parse amount: " + message);
			}
		}
		else
		{
			Debug.LogError("bridge_virtualCurrencyNotAwarded: Failed to parse message: " + message);
		}
	}

	private void bridge_takeoverBegan()
	{
		invokeHandlerIfNotNull(takeoverBegan);
	}

	private void bridge_takeoverEndedWithVC(string message)
	{
		Action<bool> action = takeoverEndedWithVC;
		if (action != null)
		{
			bool obj = message == "1";
			action(obj);
		}
	}

	private void bridge_videoAdNotServed()
	{
		invokeHandlerIfNotNull(videoAdNotServed);
	}

	public static void Init(string appId, string zoneId)
	{
	}

	public static bool VirtualCurrencyAwardAvailable()
	{
		return false;
	}

	public static void PlayVideoAdWithPrePopup(bool prePopup, bool postPopup)
	{
	}

	public static ZoneStatus GetZoneStatus()
	{
		return ZoneStatus.Unknown;
	}
}
