using UnityEngine;

public class TutorialHoverboardPopup : CharacterPopup
{
	private const int NUMBER_OF_FREE_HOVERBOARDS = 3;

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

	private string hoverboardScreenType = string.Empty;

	public override void Show()
	{
		base.Show();
		SetCharacter(Characters.CharacterType.slick);
		if (Achievements.Instance.GetNumberOfHoverboardsUsed() == 0)
		{
			topTitle.text = "Need";
			bottomTitle.text = "Help?";
			subTitle.text = "Use the\nHOVERBOARDS!";
			line1Big.text = "SURF AWAY from trouble!";
			line1Small.text = "Boards save you from crashing";
			line2Big.text = "How to ACTIVATE?";
			line2Small.text = "DOUBLE TAP while running";
			line3Big.text = "Where can I get MORE?";
			line3Small.text = "Get Hoverboards in the Shop";
			buttonText.text = "Get 3 Free!";
			hoverboardScreenType = "GuidelineHoverboardNeverUsed";
		}
		else
		{
			topTitle.text = "Out of";
			bottomTitle.text = "Boards?";
			subTitle.text = "Here's 3 to\nget you going!";
			line1Big.text = "SURF AWAY from trouble!";
			line1Small.text = "Boards save you from crashing";
			line2Big.text = "Have another GO!";
			line2Small.text = "Get a flying start with 3 FREE Boards";
			line3Big.text = "Where can I get MORE?";
			line3Small.text = "Get Hoverboards in the Shop";
			buttonText.text = "Get 3 Free!";
			hoverboardScreenType = "GuidelineHoverboardNeverBought";
		}
	}

	private void OkClicked()
	{
		PlayerInfo.Instance.IncreaseUpgradeAmount(PowerupType.hoverboard, 3);
		UIScreenController.Instance.QueuePopup("HoverboardPopup");
		Flurry.LogOkPressedOnPopup("POPUP Screen " + hoverboardScreenType);
		UIScreenController.Instance.ClosePopup();
	}

	private void CloseClicked()
	{
		PlayerInfo.Instance.IncreaseUpgradeAmount(PowerupType.hoverboard, 3);
		UIScreenController.Instance.QueuePopup("HoverboardPopup");
		Flurry.LogClosePressedOnPopup("POPUP Screen " + hoverboardScreenType);
		UIScreenController.Instance.ClosePopup();
	}
}
