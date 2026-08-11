using UnityEngine;

public class OfflineScreen : UIScreen
{
	public GameObject GameCenterBonus;

	public GameObject GameCenterNoBonus;

	public GameObject FacebookBonus;

	public GameObject FacebookNoBonus;

	public GameObject facebookButton;

	public GameObject gameCenterButton;

	public override void Show()
	{
		base.Show();
		InitOfflineScreen();
		Vector3 position = facebookButton.transform.position;
		position.x = 0f;
		facebookButton.transform.position = position;
		NGUITools.Destroy(gameCenterButton);
	}

	public override void AdjustToResolution()
	{
		base.AdjustToResolution();
	}

	private void InitOfflineScreen()
	{
		if (PlayerInfo.Instance.hasPayedOutFacebook)
		{
			NGUITools.SetActive(FacebookBonus, false);
			NGUITools.SetActive(FacebookNoBonus, true);
		}
		else
		{
			NGUITools.SetActive(FacebookBonus, true);
			NGUITools.SetActive(FacebookNoBonus, false);
		}
	}
}
