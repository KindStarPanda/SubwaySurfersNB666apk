using UnityEngine;

public class FrontscreenMenuButton : MonoBehaviour
{
	private enum MenuType
	{
		_notset = 0,
		FriendMenu = 1,
		MeMenu = 2,
		ShopMenu = 3
	}

	[SerializeField]
	private MenuType relevantmenu;

	private void OnClick()
	{
		string text = string.Empty;
		if (relevantmenu == MenuType.FriendMenu)
		{
			text = UIScreens.friendsMenu_lastScreen;
		}
		else if (relevantmenu == MenuType.MeMenu)
		{
			text = UIScreens.meMenu_lastScreen;
		}
		else if (relevantmenu == MenuType.ShopMenu)
		{
			text = UIScreens.shopMenu_lastScreen;
		}
		if (!string.IsNullOrEmpty(text))
		{
			Debug.Log("Pushing screen: " + text);
			UIScreenController.Instance.PushScreen(text);
		}
	}
}
