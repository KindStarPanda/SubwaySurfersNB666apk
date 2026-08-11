using UnityEngine;

public class ChangelogThemePopup : CharacterPopup
{
	[SerializeField]
	private UILabel topTitle;

	[SerializeField]
	private UILabel centerTitle;

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
		SetCharacter(Characters.CharacterType.elftricky);
		topTitle.text = Strings.Get(StringID.CHANGELOG_THEME_POPUP_TOPTITLE);
		centerTitle.text = Strings.Get(StringID.CHANGELOG_THEME_POPUP_CENTTITLE);
		bottomTitle.text = Strings.Get(StringID.CHANGELOG_THEME_POPUP_BOTTITLE);
		line1Big.text = Strings.Get(StringID.CHANGELOG_THEME_POPUP_LINE1BIG);
		line1Small.text = Strings.Get(StringID.CHANGELOG_THEME_POPUP_LINE1SMALL);
		line2Big.text = Strings.Get(StringID.CHANGELOG_THEME_POPUP_LINE2BIG);
		line2Small.text = Strings.Get(StringID.CHANGELOG_THEME_POPUP_LINE2SMALL);
		line3Big.text = Strings.Get(StringID.CHANGELOG_THEME_POPUP_LINE3BIG);
		line3Small.text = Strings.Get(StringID.CHANGELOG_THEME_POPUP_LINE3SMALL);
		buttonText.text = Strings.Get(StringID.CHANGELOG_THEME_POPUP_BUTTON);
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
