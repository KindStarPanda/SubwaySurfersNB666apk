using UnityEngine;

public class UpgradeCoinboxButton : MonoBehaviour
{
	private void OnClick()
	{
		Flurry.LogEvent("More coins button clicked");
		UIScreenController.Instance.QueuePopup("CoinsUI_quick");
		UIScreenController.Instance.QueuePopup("UpgradesUI_quick");
		UIScreenController.Instance.ClosePopup();
	}
}
