using UnityEngine;

public class ForceCoinScreenButton : MonoBehaviour
{
	private void OnClick()
	{
		Flurry.LogEvent("More coins button clicked");
		UIScreenController.Instance.ClosePopup();
		UIScreenController.Instance.QueuePopup("CoinsUI_quick");
		UIScreenController.Instance.QueuePopup("HoverboardPopup");
	}
}
