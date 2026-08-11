public class UIPreBragPopupHelper : UIScreen
{
	public UILabel description;

	private FriendHandlerBrag _bragHandler;

	public override void Init()
	{
		base.Init();
		_bragHandler = FriendHandlerBrag.instance;
	}

	public override void Show()
	{
		base.Show();
		description.text = _bragHandler.preBragPopupString;
	}

	private void BragClicked()
	{
		UIScreenController.Instance.QueuePopup("BragPopup");
		UIScreenController.Instance.ClosePopup();
	}

	private void CloseClicked()
	{
		UIScreenController.Instance.ClosePopup();
	}
}
