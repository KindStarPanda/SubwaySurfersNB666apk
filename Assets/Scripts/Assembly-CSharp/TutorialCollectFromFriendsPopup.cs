using UnityEngine;

public class TutorialCollectFromFriendsPopup : CharacterPopup
{
	[SerializeField]
	private UILabel topTitle;

	[SerializeField]
	private UILabel bottomTitle;

	[SerializeField]
	private UILabel subTitle;

	[SerializeField]
	private UILabel line1Big;

	[SerializeField]
	private UILabel line1Small;

	[SerializeField]
	private UILabel line2Big;

	[SerializeField]
	private UILabel line2Small;

	[SerializeField]
	private UILabel line3Big;

	[SerializeField]
	private UILabel line3Small;

	[SerializeField]
	private UILabel buttonText;

	public override void Show()
	{
		base.Show();
		SetCharacter(Characters.CharacterType.slick);
		topTitle.text = "Friends";
		bottomTitle.text = "Are Gold!";
		subTitle.text = "Get COINS when\nyour friends play !";
		line1Big.text = "HOW does it work?";
		line1Small.text = "Friend does 50 runs = COIN BAG";
		line2Big.text = "What's inside the BAG?";
		line2Small.text = "Up to 350 COINS in each!";
		line3Big.text = "Get COINS right now!";
		line3Small.text = "There's a BONUS waiting for you!";
		buttonText.text = "Collect Coins!";
	}

	private void OkClicked()
	{
		Flurry.LogOkPressedOnPopup(UIScreens.GetFlurryPopUpWithOkAndCloseName(base.gameObject.name));
		UIScreenController.Instance.ClosePopup();
	}

	private void CloseClicked()
	{
		Flurry.LogClosePressedOnPopup(UIScreens.GetFlurryPopUpWithOkAndCloseName(base.gameObject.name));
		UIScreenController.Instance.ClosePopup();
	}
}
