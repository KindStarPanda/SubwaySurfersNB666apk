using System;
using UnityEngine;

public class FacebookEventListener : MonoBehaviour
{
	private void OnEnable()
	{
		FacebookManager.loginSucceededEvent += facebookLogin;
		FacebookManager.loginFailedEvent += facebookLoginFailed;
		FacebookManager.loggedOutEvent += facebookDidLogoutEvent;
		FacebookManager.accessTokenExtendedEvent += facebookDidExtendTokenEvent;
		FacebookManager.sessionInvalidatedEvent += facebookSessionInvalidatedEvent;
		FacebookManager.dialogCompletedEvent += facebokDialogCompleted;
		FacebookManager.dialogCompletedWithUrlEvent += facebookDialogCompletedWithUrl;
		FacebookManager.dialogDidNotCompleteEvent += facebookDialogDidntComplete;
		FacebookManager.dialogFailedEvent += facebookDialogFailed;
		FacebookManager.customRequestReceivedEvent += facebookReceivedCustomRequest;
		FacebookManager.customRequestFailedEvent += facebookCustomRequestFailed;
		FacebookManager.facebookComposerCompletedEvent += facebookComposerCompletedEvent;
		FacebookManager.facebookReauthorizationWithReadPermissionsFailedEvent += facebookReauthorizationWithReadPermissionsFailedEvent;
		FacebookManager.facebookReauthorizationWithReadPermissionsSucceededEvent += facebookReauthorizationWithReadPermissionsSucceededEvent;
		FacebookManager.facebookReauthorizationWithPublishPermissionsFailedEvent += facebookReauthorizationWithPublishPermissionsFailedEvent;
		FacebookManager.facebookReauthorizationWithPublishPermissionsSucceededEvent += facebookReauthorizationWithPublishPermissionsSucceededEvent;
	}

	private void OnDisable()
	{
		FacebookManager.loginSucceededEvent -= facebookLogin;
		FacebookManager.loginFailedEvent -= facebookLoginFailed;
		FacebookManager.loggedOutEvent -= facebookDidLogoutEvent;
		FacebookManager.accessTokenExtendedEvent -= facebookDidExtendTokenEvent;
		FacebookManager.sessionInvalidatedEvent -= facebookSessionInvalidatedEvent;
		FacebookManager.dialogCompletedEvent -= facebokDialogCompleted;
		FacebookManager.dialogCompletedWithUrlEvent -= facebookDialogCompletedWithUrl;
		FacebookManager.dialogDidNotCompleteEvent -= facebookDialogDidntComplete;
		FacebookManager.dialogFailedEvent -= facebookDialogFailed;
		FacebookManager.customRequestReceivedEvent -= facebookReceivedCustomRequest;
		FacebookManager.customRequestFailedEvent -= facebookCustomRequestFailed;
		FacebookManager.facebookComposerCompletedEvent -= facebookComposerCompletedEvent;
		FacebookManager.facebookReauthorizationWithReadPermissionsFailedEvent -= facebookReauthorizationWithReadPermissionsFailedEvent;
		FacebookManager.facebookReauthorizationWithReadPermissionsSucceededEvent -= facebookReauthorizationWithReadPermissionsSucceededEvent;
		FacebookManager.facebookReauthorizationWithPublishPermissionsFailedEvent -= facebookReauthorizationWithPublishPermissionsFailedEvent;
		FacebookManager.facebookReauthorizationWithPublishPermissionsSucceededEvent -= facebookReauthorizationWithPublishPermissionsSucceededEvent;
	}

	private void facebookLogin()
	{
		Debug.Log("Successfully logged in to Facebook");
	}

	private void facebookLoginFailed(string error)
	{
		Debug.Log("Facebook login failed: " + error);
	}

	private void facebookDidLogoutEvent()
	{
		Debug.Log("facebookDidLogoutEvent");
	}

	private void facebookDidExtendTokenEvent(DateTime newExpiry)
	{
		Debug.Log("facebookDidExtendTokenEvent: " + newExpiry);
	}

	private void facebookSessionInvalidatedEvent()
	{
		Debug.Log("facebookSessionInvalidatedEvent");
	}

	private void facebokDialogCompleted()
	{
		Debug.Log("facebokDialogCompleted");
	}

	private void facebookDialogCompletedWithUrl(string url)
	{
		Debug.Log("facebookDialogCompletedWithUrl: " + url);
	}

	private void facebookDialogDidntComplete()
	{
		Debug.Log("facebookDialogDidntComplete");
	}

	private void facebookDialogFailed(string error)
	{
		Debug.Log("facebookDialogFailed: " + error);
	}

	private void facebookReceivedCustomRequest(object obj)
	{
		Debug.Log("facebookReceivedCustomRequest");
	}

	private void facebookCustomRequestFailed(string error)
	{
		Debug.Log("facebookCustomRequestFailed failed: " + error);
	}

	private void facebookComposerCompletedEvent(bool didSucceed)
	{
		Debug.Log("facebookComposerCompletedEvent did succeed: " + didSucceed);
	}

	private void facebookReauthorizationWithReadPermissionsFailedEvent(string error)
	{
		Debug.Log("facebookReauthorizationWithReadPermissionsFailedEvent: " + error);
	}

	private void facebookReauthorizationWithReadPermissionsSucceededEvent()
	{
		Debug.Log("facebookReauthorizationWithReadPermissionsSucceededEvent");
	}

	private void facebookReauthorizationWithPublishPermissionsFailedEvent(string error)
	{
		Debug.Log("facebookReauthorizationWithPublishPermissionsFailedEvent: " + error);
	}

	private void facebookReauthorizationWithPublishPermissionsSucceededEvent()
	{
		Debug.Log("facebookReauthorizationWithPublishPermissionsSucceededEvent");
	}
}
