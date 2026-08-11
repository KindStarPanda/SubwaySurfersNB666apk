using UnityEngine;

public class PrivacyPolicyPopUp : MonoBehaviour
{
	private void ClosePopup()
	{
		Flurry.LogClosePressedOnPopup(UIScreens.GetFlurryPopUpWithOkAndCloseName(base.gameObject.name));
		Object.Destroy(base.gameObject);
	}

	private void ReadPrivacyPolicy()
	{
		Flurry.LogOkPressedOnPopup(UIScreens.GetFlurryPopUpWithOkAndCloseName(base.gameObject.name));
		Object.Destroy(base.gameObject);
		Application.OpenURL("http://www.kiloo.com/privacy/");
	}
}
