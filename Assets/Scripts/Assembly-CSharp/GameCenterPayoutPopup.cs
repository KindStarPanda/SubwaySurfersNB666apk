public class GameCenterPayoutPopup : UIScreen
{
	public UILabel titleLabel;

	public UILabel descLabel;

	public override void Init()
	{
		base.Init();
		titleLabel.text = Strings.Get(StringID.POPUP_PAYOUT_GAMECENTER_TITLE);
		descLabel.text = string.Format(Strings.Get(StringID.POPUP_PAYOUT_GAMECENTER_DESC), 250);
		titleLabel.panel.Refresh();
	}
}
