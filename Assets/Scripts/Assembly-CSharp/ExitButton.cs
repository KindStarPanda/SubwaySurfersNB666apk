using UnityEngine;

public class ExitButton : MonoBehaviour
{
	private void OnClick()
	{
		EtceteraAndroidManager.alertButtonClickedEvent += alertButtonClickedEvent;
		EtceteraAndroid.showAlert("Alert", "Are you sure you want to quit the game?", "Quit", "Return");
	}

	private void alertButtonClickedEvent(string positiveButton)
	{
		EtceteraAndroidManager.alertButtonClickedEvent -= alertButtonClickedEvent;
		Debug.Log("alertButtonClickedEvent: " + positiveButton);
		if (positiveButton.Equals("Quit"))
		{
			RRInappBillingPluginKit.StopUnityActivity();
		}
	}
}
