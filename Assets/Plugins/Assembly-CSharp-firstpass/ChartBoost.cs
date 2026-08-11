using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class ChartBoost : MonoBehaviour
{
	private const string BRIDGE_DELEGATE_GAMEOBJECT_NAME = "ChartBoostBridge";

	private static ChartBoost bridgeDelegate;

	public static Action<string> didCacheInterstitial;

	public static Action<string> didFailToLoadInterstitial;

	public static Action<string> didDismissInterstitial;

	public static Action<string> didCloseInterstitial;

	public static Action<string> didClickInterstitial;

	public static Action didFailToLoadMoreApps;

	public static Action didDismissMoreApps;

	public static Action didCloseMoreApps;

	public static Action didClickMoreApps;

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

	private void invokeHandlerIfNotNull<T>(Action<T> handler, T val)
	{
		if (handler != null)
		{
			handler(val);
		}
	}

	private void bridge_didCacheInterstitial(string location)
	{
		invokeHandlerIfNotNull(didCacheInterstitial, location);
	}

	private void bridge_didFailToLoadInterstitial(string location)
	{
		invokeHandlerIfNotNull(didFailToLoadInterstitial, location);
	}

	private void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			Debug.Log("bridge_didDismissInterstitial");
			bridge_didDismissInterstitial(string.Empty);
			bridge_didCloseInterstitial(string.Empty);
		}
	}

	private void bridge_didDismissInterstitial(string location)
	{
		invokeHandlerIfNotNull(didDismissInterstitial, location);
	}

	private void bridge_didCloseInterstitial(string location)
	{
		invokeHandlerIfNotNull(didCloseInterstitial, location);
	}

	private void bridge_didClickInterstitial(string location)
	{
		invokeHandlerIfNotNull(didClickInterstitial, location);
	}

	private void bridge_didFailToLoadMoreApps()
	{
		invokeHandlerIfNotNull(didFailToLoadMoreApps);
	}

	private void bridge_didDismissMoreApps()
	{
		invokeHandlerIfNotNull(didDismissMoreApps);
	}

	private void bridge_didCloseMoreApps()
	{
		invokeHandlerIfNotNull(didCloseMoreApps);
	}

	private void bridge_didClickMoreApps()
	{
		invokeHandlerIfNotNull(didClickMoreApps);
	}

	public static void InitAndStartSession(string appId, string appSignature)
	{
	}

	public static void CacheInterstitial()
	{
	}

	public static void CacheInterstitial(string location)
	{
	}

	public static void ShowInterstitial()
	{
	}

	public static void ShowInterstitial(string location)
	{
	}

	public static bool HasCachedInterstitial()
	{
		return false;
	}

	public static bool HasCachedInterstitial(string location)
	{
		return false;
	}

	public static void CacheMoreApps()
	{
	}

	public static void ShowMoreApps()
	{
	}

	public static bool GetShouldRequestInterstitial()
	{
		return true;
	}

	public static void SetShouldRequestInterstitial(bool should)
	{
	}

	public static bool GetShouldDisplayInterstitial()
	{
		return true;
	}

	public static void SetShouldDisplayInterstitial(bool should)
	{
	}

	public static bool GetShouldDisplayLoadingViewForMoreApps()
	{
		return true;
	}

	public static void SetShouldDisplayLoadingViewForMoreApps(bool should)
	{
	}

	public static bool GetShouldDisplayMoreApps()
	{
		return true;
	}

	public static void SetShouldDisplayMoreApps(bool should)
	{
	}

	[DllImport("__Internal")]
	private static extern void bridge_initAndStartSession(string appId, string appSignature);

	[DllImport("__Internal")]
	private static extern void bridge_cacheInterstitial();

	[DllImport("__Internal")]
	private static extern void bridge_cacheInterstitialLocation(string location);

	[DllImport("__Internal")]
	private static extern void bridge_showInterstitial();

	[DllImport("__Internal")]
	private static extern void bridge_showInterstitialLocation(string location);

	[DllImport("__Internal")]
	private static extern bool bridge_hasCachedInterstitial();

	[DllImport("__Internal")]
	private static extern bool bridge_hasCachedInterstitialLocation(string location);

	[DllImport("__Internal")]
	private static extern void bridge_cacheMoreApps();

	[DllImport("__Internal")]
	private static extern void bridge_showMoreApps();

	[DllImport("__Internal")]
	private static extern bool bridge_getShouldRequestInterstitial();

	[DllImport("__Internal")]
	private static extern void bridge_setShouldRequestInterstitial(bool should);

	[DllImport("__Internal")]
	private static extern bool bridge_getShouldDisplayInterstitial();

	[DllImport("__Internal")]
	private static extern void bridge_setShouldDisplayInterstitial(bool should);

	[DllImport("__Internal")]
	private static extern bool bridge_getShouldDisplayLoadingViewForMoreApps();

	[DllImport("__Internal")]
	private static extern void bridge_setShouldDisplayLoadingViewForMoreApps(bool should);

	[DllImport("__Internal")]
	private static extern bool bridge_getShouldDisplayMoreApps();

	[DllImport("__Internal")]
	private static extern void bridge_setShouldDisplayMoreApps(bool should);
}
