using UnityEngine;

public class ChangeLogPopup : CharacterPopup
{
	[SerializeField]
	private UILabel topTitle;

	[SerializeField]
	private UILabel mainLabel;

	[SerializeField]
	private UILabel buttonText;

	public override void Show()
	{
		base.Show();
		SetCharacter(Characters.CharacterType.fresh);
		topTitle.text = Strings.Get(StringID.CHANGELOG_POPUP_TITLE);
		mainLabel.text = Strings.Get(StringID.CHANGELOG_POPUP_TEXT);
		buttonText.text = Strings.Get(StringID.CHANGELOG_POPUP_BUTTON);
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
