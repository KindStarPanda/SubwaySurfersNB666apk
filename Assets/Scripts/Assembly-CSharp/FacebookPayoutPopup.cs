public class FacebookPayoutPopup : UIScreen
{
	public UILabel titleLabel;

	public UILabel descLabel;

	public override void Init()
	{
		base.Init();
		titleLabel.text = Strings.Get(StringID.POPUP_PAYOUT_FACEBOOK_TITLE);
		descLabel.text = string.Format(Strings.Get(StringID.POPUP_PAYOUT_FACEBOOK_DESC), 5000);
		titleLabel.panel.Refresh();
	}
}
