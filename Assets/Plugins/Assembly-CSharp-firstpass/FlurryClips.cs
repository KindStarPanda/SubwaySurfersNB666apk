using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class FlurryClips : MonoBehaviour
{
	private const string BRIDGE_DELEGATE_GAMEOBJECT_NAME = "FlurryClipsBridge";

	private static FlurryClips bridgeDelegate;

	private static Action _onVideoAvailable;

	private static Action _onVideoUnavailable;

	private static Action<string> _onTakeoverWillDisplay;

	private static Action _onTakeoverWillClose;

	private static Action<string> _onVideoDidNotFinish;

	private static Action<string> _onVideoDidFinish;

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

	private void fcbridge_videoAvailable()
	{
		invokeHandlerIfNotNull(_onVideoAvailable);
	}

	private void fcbridge_videoUnavailable()
	{
		invokeHandlerIfNotNull(_onVideoUnavailable);
	}

	private void fcbridge_takeoverWillDisplay(string hook)
	{
		invokeHandlerIfNotNull(_onTakeoverWillDisplay, hook);
	}

	private void fcbridge_takeoverWillClose()
	{
		invokeHandlerIfNotNull(_onTakeoverWillClose);
	}

	private void fcbridge_videoDidNotFinish(string hook)
	{
		invokeHandlerIfNotNull(_onVideoDidNotFinish, hook);
	}

	private void fcbridge_videoDidFinish(string hook)
	{
		invokeHandlerIfNotNull(_onVideoDidFinish, hook);
	}

	public static void InitAndEnableVideoAds()
	{
	}

	public static bool VideoAdIsAvailable(string hook)
	{
		return false;
	}

	public static void OpenVideoTakeover(string hook, string orientation, string rewardMessage)
	{
	}

	public static void AddVideoAvailableHandler(Action handler)
	{
		_onVideoAvailable = (Action)Delegate.Combine(_onVideoAvailable, handler);
	}

	public static void RemoveVideoAvailableHandler(Action handler)
	{
		_onVideoAvailable = (Action)Delegate.Remove(_onVideoAvailable, handler);
	}

	public static void AddVideoUnavailableHandler(Action handler)
	{
		_onVideoUnavailable = (Action)Delegate.Combine(_onVideoUnavailable, handler);
	}

	public static void RemoveVideoUnavailableHandler(Action handler)
	{
		_onVideoUnavailable = (Action)Delegate.Remove(_onVideoUnavailable, handler);
	}

	public static void AddTakeoverWillDisplayHandler(Action<string> handler)
	{
		_onTakeoverWillDisplay = (Action<string>)Delegate.Combine(_onTakeoverWillDisplay, handler);
	}

	public static void RemoveTakeoverWillDisplayHandler(Action<string> handler)
	{
		_onTakeoverWillDisplay = (Action<string>)Delegate.Remove(_onTakeoverWillDisplay, handler);
	}

	public static void AddTakeoverWillCloseHandler(Action handler)
	{
		_onTakeoverWillClose = (Action)Delegate.Combine(_onTakeoverWillClose, handler);
	}

	public static void RemoveTakeoverWillCloseHandler(Action handler)
	{
		_onTakeoverWillClose = (Action)Delegate.Remove(_onTakeoverWillClose, handler);
	}

	public static void AddVideoDidNotFinishHandler(Action<string> handler)
	{
		_onVideoDidNotFinish = (Action<string>)Delegate.Combine(_onVideoDidNotFinish, handler);
	}

	public static void RemoveVideoDidNotFinishHandler(Action<string> handler)
	{
		_onVideoDidNotFinish = (Action<string>)Delegate.Remove(_onVideoDidNotFinish, handler);
	}

	public static void AddVideoDidFinishHandler(Action<string> handler)
	{
		_onVideoDidFinish = (Action<string>)Delegate.Combine(_onVideoDidFinish, handler);
	}

	public static void RemoveVideoDidFinishHandler(Action<string> handler)
	{
		_onVideoDidFinish = (Action<string>)Delegate.Remove(_onVideoDidFinish, handler);
	}

	[DllImport("__Internal")]
	private static extern void fcbridge_initAndEnableVideoAds();

	[DllImport("__Internal")]
	private static extern bool fcbridge_videoAdIsAvailable(string hook);

	[DllImport("__Internal")]
	private static extern void fcbridge_openVideoTakeover(string hook, string orientation, string rewardMessage);
}
