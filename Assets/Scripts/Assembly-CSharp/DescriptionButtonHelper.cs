public class DescriptionButtonHelper : UIBasicButton
{
	protected override void Send()
	{
		if (UIScreenController.Instance.GetCurrentPopupName() == "UpgradesUI_quick")
		{
			UIScreenController.Instance.ClosePopup();
			UIScreenController.Instance.QueuePopup("PrizeMBPopup");
			Flurry.LogEventWithAParameter("Mysterybox view prices", "Screen Name", "QuickBoost");
		}
		else
		{
			UIScreenController.Instance.QueuePopup("PrizeMBPopup");
			Flurry.LogEventWithAParameter("Mysterybox view prices", "Screen Name", "Boost");
		}
	}
}
