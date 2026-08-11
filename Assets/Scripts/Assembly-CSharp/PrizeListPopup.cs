using UnityEngine;

public class PrizeListPopup : UIScreen
{
	private void ClosePrizeList()
	{
		if (UIScreenController.Instance.GetCurrentPopupName() == "MysteryBoxPopup")
		{
			base.transform.parent.SendMessage("ClosePrizeList", SendMessageOptions.DontRequireReceiver);
		}
		else if (UIScreenController.Instance.GetTopScreenName() == "UpgradesUI_shop")
		{
			UIScreenController.Instance.ClosePopup(base.gameObject);
		}
		else if (UIScreenController.Instance.GetTopScreenName() == "GameoverUI")
		{
			UIScreenController.Instance.ClosePopup(base.gameObject);
			UIScreenController.Instance.QueuePopup("UpgradesUI_quick");
		}
		else
		{
			Debug.LogWarning("unhandled case, " + UIScreenController.Instance.GetCurrentPopupName() + " " + UIScreenController.Instance.GetTopScreenName());
			UIScreenController.Instance.ClosePopup(base.gameObject);
		}
	}
}
