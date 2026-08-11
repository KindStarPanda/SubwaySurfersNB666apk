using UnityEngine;

public class ChangeLogStarter : MonoBehaviour
{
	private const string LAST_SEEN_BUNDLE_VERSION_KEY = "lastSeenBundleVersionKey";

	private const string IS_FIRST_LAUNCH = "isthisfirstlaunch";

	private void OnEnable()
	{
		FrontScreen.tweensHaveFinishedAnimating += ShowPopUpsIfNeeded;
	}

	private void OnDisable()
	{
		FrontScreen.tweensHaveFinishedAnimating -= ShowPopUpsIfNeeded;
	}

	private void Start()
	{
	}

	public static void ShowPopUpsIfNeeded()
	{
		if (!ShouldDisplayChangeLog())
		{
			return;
		}
		if (PlayerInfo.Instance.currentSeasonAvailable == PlayerInfo.Season.xmas)
		{
			UIScreenController.Instance.QueuePopup("ChangelogThemePopup");
			if (!PlayerPrefs.HasKey("isthisfirstlaunch"))
			{
				PlayerPrefs.SetInt("isthisfirstlaunch", 1);
			}
		}
		else if (!PlayerPrefs.HasKey("isthisfirstlaunch"))
		{
			PlayerPrefs.SetInt("isthisfirstlaunch", 1);
		}
		else
		{
			UIScreenController.Instance.QueuePopup("ChangeLogPopup");
		}
	}

	private static bool ShouldDisplayChangeLog()
	{
		string bundleVersion = DeviceUtility.GetBundleVersion();
		string @string = PlayerPrefs.GetString("lastSeenBundleVersionKey", string.Empty);
		if (@string.Equals(bundleVersion))
		{
			return false;
		}
		PlayerPrefs.SetString("lastSeenBundleVersionKey", DeviceUtility.GetBundleVersion());
		return true;
	}
}
