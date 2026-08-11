using UnityEngine;

public class PrivacyPolicyButton : MonoBehaviour
{
	private const string POPUP_PATH = "Prefabs/Popups/";

	[SerializeField]
	private UIScreen myScreen;

	private void OnClick()
	{
		GameObject gameObject = NGUITools.AddChild(myScreen.gameObject, Resources.Load("Prefabs/Popups/PrivacyPolicyPopup") as GameObject);
		gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y, gameObject.transform.localPosition.z - 100f);
	}
}
