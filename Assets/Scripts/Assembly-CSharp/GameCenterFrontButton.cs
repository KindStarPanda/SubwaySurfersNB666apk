using UnityEngine;

public class GameCenterFrontButton : MonoBehaviour
{
	private void OnClick()
	{
		Debug.Log("Game Center button clicked");
		if (!Social.localUser.authenticated)
		{
			DeviceUtility.showNativePopup("Game Center Disabled", "Sign in with the Game Center application to enable", "Ok");
		}
		else
		{
			Social.ShowLeaderboardUI();
		}
	}
}
