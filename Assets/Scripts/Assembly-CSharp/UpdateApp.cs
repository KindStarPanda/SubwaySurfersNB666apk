using System;
using UnityEngine;

public class UpdateApp : CharacterPopup
{
	private const string ONLINESETTINGS_LATESTVERSION_KEY = "latestversion";

	private const string ONLINESETTINGS_LATESTVERSION_CHANGELIST_KEY = "latestversion_changelist";

	private const string APPSTORE_URL = "https://play.google.com/store/apps/details?id=com.kiloo.subwaysurf";

	private static bool isUpdateNowOnScreen;

	private static bool _hasShownThisSession;

	[SerializeField]
	private UILabel changeListLabel;

	public override void Show()
	{
		base.Show();
		SetCharacter(Characters.CharacterType.frizzy);
		string valueString;
		if (OnlineSettings.instance.TryGetValue("latestversion_changelist", out valueString))
		{
			changeListLabel.text = valueString + "\n";
			return;
		}
		Debug.LogError("Showing NewVersion popup, but no changeList found in OnlineSettings", this);
		changeListLabel.text = "New Content\n";
	}

	private void CloseClicked()
	{
		UIScreenController.Instance.ClosePopup();
		isUpdateNowOnScreen = false;
		Flurry.LogClosePressedOnPopup(UIScreens.GetFlurryPopUpWithOkAndCloseName(base.gameObject.name));
	}

	private void OkClicked()
	{
		Application.OpenURL("https://play.google.com/store/apps/details?id=com.kiloo.subwaysurf");
		UIScreenController.Instance.ClosePopup();
		isUpdateNowOnScreen = false;
		Flurry.LogOkPressedOnPopup(UIScreens.GetFlurryPopUpWithOkAndCloseName(base.gameObject.name));
	}

	public static void AllowAgainThisSession()
	{
		_hasShownThisSession = false;
	}

	public static void ShowIfNeeded()
	{
		string valueString;
		if (isUpdateNowOnScreen || !OnlineSettings.instance.TryGetValue("latestversion", out valueString))
		{
			return;
		}
		string bundleVersion = DeviceUtility.GetBundleVersion();
		bool flag = false;
		try
		{
			if (Utility.CompareVersions(bundleVersion, valueString) < 0)
			{
				flag = true;
			}
		}
		catch (FormatException ex)
		{
			Debug.LogError("Failed to parse versions for comparison: " + bundleVersion + " and " + valueString + "  : " + ex);
		}
		if (flag && !_hasShownThisSession)
		{
			UIScreenController.Instance.QueuePopup("UpdateAppPopup");
			isUpdateNowOnScreen = true;
			_hasShownThisSession = true;
		}
	}
}
