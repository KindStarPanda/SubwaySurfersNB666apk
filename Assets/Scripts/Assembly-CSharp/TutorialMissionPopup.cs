using UnityEngine;

public class TutorialMissionPopup : CharacterPopup
{
	[SerializeField]
	private UILabel topTitle;

	[SerializeField]
	private UILabel middleTitle;

	[SerializeField]
	private UILabel bottomTitle;

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
		SetCharacter(Characters.CharacterType.king);
		topTitle.text = "Get";
		middleTitle.text = "Higher";
		bottomTitle.text = "Scores!";
		line1Big.text = "Complete a MISSION SET";
		line1Small.text = "Finish 3 MISSIONS to increase Multiplier";
		line2Big.text = "Get higher SCORE Multiplier!";
		line2Small.text = "All the way to SCORE x30!";
		line3Big.text = "Dominate the LEADERBOARD";
		line3Small.text = "Score x Multiplier = HIGHSCORE";
		buttonText.text = "Show Missions";
	}

	private void OkClicked()
	{
		UIScreenController.Instance.QueuePopup("Mission_popup");
		Flurry.LogOkPressedOnPopup(UIScreens.GetFlurryPopUpWithOkAndCloseName(base.gameObject.name));
		UIScreenController.Instance.ClosePopup();
	}

	private void CloseClicked()
	{
		Flurry.LogClosePressedOnPopup(UIScreens.GetFlurryPopUpWithOkAndCloseName(base.gameObject.name));
		UIScreenController.Instance.ClosePopup();
	}
}
