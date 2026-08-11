using UnityEngine;

public class GameCenterButton : MonoBehaviour
{
	private void OnClick()
	{
		if (!Social.localUser.authenticated)
		{
			DeviceUtility.showNativePopup("Game Center Disabled", "Sign in with the Game Center application to enable", "Ok");
		}
	}
}
