using UnityEngine;

public class TutorialFaceBookPopup : CharacterPopup
{
	private const float Y_LINE1_NO_REWARD = 0.5f;

	private const float Y_LINE1_WITH_REWARD = 9.5f;

	[SerializeField]
	private UILabel topTitle;

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
	private UILabel topLabel;

	[SerializeField]
	private UILabel line2Label1;

	[SerializeField]
	private UILabel line2Label2;

	[SerializeField]
	private UISprite line2CoinSprite;

	public override void Show()
	{
		base.Show();
		SetCharacter(Characters.CharacterType.tricky);
		topTitle.text = "Play with Friends!";
		line1Big.text = "DISCOVER who else is playing";
		line1Small.text = "Join the Subway Surfers party!";
		line2Big.text = "Have FUN with your buddies";
		line2Small.text = "Surfing friends join your FRIEND list";
		line3Big.text = "COMPETE with friends";
		line3Small.text = "Who gets the HIGHEST SCORE?";
		if (PlayerInfo.Instance.hasPayedOutFacebook)
		{
			topLabel.cachedTransform.localPosition = new Vector3(topLabel.cachedTransform.localPosition.x, 0.5f, topLabel.cachedTransform.localPosition.z);
			line2Label1.enabled = false;
			line2Label2.enabled = false;
			line2CoinSprite.enabled = false;
		}
		else
		{
			topLabel.cachedTransform.localPosition = new Vector3(topLabel.cachedTransform.localPosition.x, 9.5f, topLabel.cachedTransform.localPosition.z);
			line2Label1.enabled = true;
			line2Label2.enabled = true;
			line2CoinSprite.enabled = true;
		}
	}

	private void OkClicked()
	{
		SocialManager.instance.FacebookLogin(UIScreenController.Instance.FacebookLogIn);
		Flurry.LogOkPressedOnPopup(UIScreens.GetFlurryPopUpWithOkAndCloseName(base.gameObject.name));
		UIScreenController.Instance.ClosePopup();
	}

	private void CloseClicked()
	{
		Flurry.LogClosePressedOnPopup(UIScreens.GetFlurryPopUpWithOkAndCloseName(base.gameObject.name));
		UIScreenController.Instance.ClosePopup();
	}
}
