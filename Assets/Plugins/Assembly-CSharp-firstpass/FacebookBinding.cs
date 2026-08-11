using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class FacebookBinding
{
	static FacebookBinding()
	{
		FacebookManager.preLoginSucceededEvent += delegate
		{
			Facebook.instance.accessToken = getAccessToken();
		};
	}

	[DllImport("__Internal")]
	private static extern void _facebookInit(string applicationId);

	public static void init(string applicationId)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_facebookInit(applicationId);
		}
		Facebook.instance.accessToken = getAccessToken();
	}

	[DllImport("__Internal")]
	private static extern bool _facebookIsLoggedIn();

	public static bool isSessionValid()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			return _facebookIsLoggedIn();
		}
		return false;
	}

	[DllImport("__Internal")]
	private static extern string _facebookGetFacebookAccessToken();

	public static string getAccessToken()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			return _facebookGetFacebookAccessToken();
		}
		return string.Empty;
	}

	[DllImport("__Internal")]
	private static extern string _facebookGetSessionPermissions();

	public static ArrayList getSessionPermissions()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			string json = _facebookGetSessionPermissions();
			return JsonUtility.FromJson<ArrayList>(json);
		}
		return new ArrayList();
	}

	[DllImport("__Internal")]
	private static extern void _facebookLoginUsingDeprecatedAuthorizationFlowWithRequestedPermissions(string perms, string urlSchemeSuffix);

	[Obsolete("Note that this auth flow has been deprecated by Facebook and could be removed at any time at Facebook's discretion")]
	public static void loginUsingDeprecatedAuthorizationFlowWithRequestedPermissions(string[] permissions)
	{
		loginUsingDeprecatedAuthorizationFlowWithRequestedPermissions(permissions, null);
	}

	[Obsolete("Note that this auth flow has been deprecated by Facebook and could be removed at any time at Facebook's discretion")]
	public static void loginUsingDeprecatedAuthorizationFlowWithRequestedPermissions(string[] permissions, string urlSchemeSuffix)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			string perms = string.Join(",", permissions);
			_facebookLoginUsingDeprecatedAuthorizationFlowWithRequestedPermissions(perms, urlSchemeSuffix);
		}
	}

	public static void login()
	{
		loginWithRequestedReadPermissions(new string[0]);
	}

	public static void loginWithRequestedReadPermissions(string[] permissions)
	{
		loginWithRequestedReadPermissions(permissions, null);
	}

	[DllImport("__Internal")]
	private static extern void _facebookLoginWithRequestedPermissions(string perms, string urlSchemeSuffix);

	public static void loginWithRequestedReadPermissions(string[] permissions, string urlSchemeSuffix)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			string perms = string.Join(",", permissions);
			_facebookLoginWithRequestedPermissions(perms, urlSchemeSuffix);
		}
	}

	[DllImport("__Internal")]
	private static extern void _facebookReauthorizeWithReadPermissions(string perms);

	public static void reauthorizeWithReadPermissions(string[] permissions)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			string perms = string.Join(",", permissions);
			_facebookReauthorizeWithReadPermissions(perms);
		}
	}

	[DllImport("__Internal")]
	private static extern void _facebookReauthorizeWithPublishPermissions(string perms, int defaultAudience);

	public static void reauthorizeWithPublishPermissions(string[] permissions, FacebookSessionDefaultAudience defaultAudience)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			string perms = string.Join(",", permissions);
			_facebookReauthorizeWithPublishPermissions(perms, (int)defaultAudience);
		}
	}

	[DllImport("__Internal")]
	private static extern void _facebookLogout();

	public static void logout()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_facebookLogout();
		}
		Facebook.instance.accessToken = string.Empty;
	}

	[DllImport("__Internal")]
	private static extern void _facebookShowDialog(string dialogType, string json);

	[DllImport("__Internal")]
	private static extern void _facebookRestRequest(string restMethod, string httpMethod, string jsonDict);

	[DllImport("__Internal")]
	private static extern void _facebookGraphRequest(string graphPath, string httpMethod, string jsonDict);

	[DllImport("__Internal")]
	private static extern bool _facebookIsFacebookComposerSupported();

	public static bool isFacebookComposerSupported()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			return _facebookIsFacebookComposerSupported();
		}
		return false;
	}

	[DllImport("__Internal")]
	private static extern bool _facebookCanUserUseFacebookComposer();

	public static bool canUserUseFacebookComposer()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			return _facebookCanUserUseFacebookComposer();
		}
		return false;
	}

	[DllImport("__Internal")]
	private static extern void _facebookShowFacebookComposer(string message, string imagePath, string link);

	public static void showFacebookComposer(string message)
	{
		showFacebookComposer(message, null, null);
	}

	public static void showFacebookComposer(string message, string imagePath, string link)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_facebookShowFacebookComposer(message, imagePath, link);
		}
	}
}
