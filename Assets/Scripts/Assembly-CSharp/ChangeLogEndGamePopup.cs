using UnityEngine;

public class ChangeLogEndGamePopup : CharacterPopup
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
		SetCharacter(Characters.CharacterType.lucy);
		topTitle.text = "Yo Elite";
		bottomTitle.text = "Surfer!";
		subTitle.text = "More MISSIONS!\nNew CHARACTERS!";
		line1Big.text = "New Missions added!";
		line1Small.text = "Totally NEW CHALLENGES to master";
		line2Big.text = "5 awesome CHARACTERS";
		line2Small.text = "Meet TASHA, ZOE, BRODY + more";
		line3Big.text = "New SUPER MYSTERY BOX!";
		line3Small.text = "More ITEMS + bigger COIN WINS!";
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
