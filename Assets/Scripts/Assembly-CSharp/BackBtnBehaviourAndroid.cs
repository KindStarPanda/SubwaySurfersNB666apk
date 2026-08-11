using UnityEngine;

public class BackBtnBehaviourAndroid : MonoBehaviour
{
	public enum ScreenChangeType
	{
		PushScreen = 0,
		SwitchScreen = 1,
		BackToPrevious = 2,
		QueuePopup = 3,
		ClosePopup = 4,
		ExitGame = 5
	}

	private GameObject target;

	private string functionName = string.Empty;

	public ScreenChangeType screenChangeType;

	public GameObject popupLayerAnchor;

	public string ScreenNameToOpen = string.Empty;

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Send();
		}
	}

	private void CheckForFunctionToExecute()
	{
		if (screenChangeType == ScreenChangeType.PushScreen)
		{
			functionName = "PushScreen";
		}
		else if (screenChangeType == ScreenChangeType.SwitchScreen)
		{
			functionName = "SwitchScreen";
		}
		else if (screenChangeType == ScreenChangeType.BackToPrevious)
		{
			functionName = "BackToPrevious";
		}
		else if (screenChangeType == ScreenChangeType.QueuePopup)
		{
			functionName = "QueuePopup";
		}
		else if (screenChangeType == ScreenChangeType.ClosePopup)
		{
			functionName = "ClosePopup";
		}
		else if (screenChangeType == ScreenChangeType.ExitGame)
		{
			functionName = "ExitGame";
		}
	}

	protected void Send()
	{
		if (UIScreenController.Instance.IsInAppPurchaseOverlayVisible() || UIScreenController.Instance.GetCurrentPopupName() == "MysteryBoxPopup")
		{
			return;
		}
		CheckForFunctionToExecute();
		if (!base.enabled || !base.gameObject.active)
		{
			return;
		}
		if (string.IsNullOrEmpty(ScreenNameToOpen) && (screenChangeType == ScreenChangeType.PushScreen || screenChangeType == ScreenChangeType.SwitchScreen || screenChangeType == ScreenChangeType.QueuePopup))
		{
			Debug.LogError(base.name + " tried to send an empty Change Screen message");
		}
		if (functionName.Equals("ExitGame"))
		{
			if (UIScreenController.Instance.IsPopupQueueEmpty())
			{
				EtceteraAndroidManager.alertButtonClickedEvent += alertButtonClickedEvent;
				EtceteraAndroid.showAlert("Alert", "Are you sure you want to quit the game?", "Quit", "Return");
			}
		}
		else
		{
			if (target == null)
			{
				target = MessageCenter.Instance.gameObject;
			}
			Transform[] componentsInChildren = target.gameObject.GetComponentsInChildren<Transform>();
			Transform[] array = componentsInChildren;
			foreach (Transform transform in array)
			{
				transform.SendMessage(functionName, base.gameObject, SendMessageOptions.DontRequireReceiver);
			}
		}
		LogFlurry();
	}

	private void LogFlurry()
	{
		string empty = string.Empty;
		string currentPopupName = UIScreenController.Instance.GetCurrentPopupName();
		empty = (string.IsNullOrEmpty(currentPopupName) ? UIScreenController.Instance.GetTopScreenName() : currentPopupName);
		if (string.IsNullOrEmpty(empty))
		{
			Flurry.LogEventWithAParameter("Back button pressed", "Screen Name", "Null");
		}
		else
		{
			Flurry.LogEventWithAParameter("Back button pressed", "Screen Name", empty);
		}
	}

	private void alertButtonClickedEvent(string buttonString)
	{
		EtceteraAndroidManager.alertButtonClickedEvent -= alertButtonClickedEvent;
		Debug.Log("alertButtonClickedEvent: " + buttonString);
		if (buttonString.Equals("Quit"))
		{
			PlayerInfo.Instance.SaveIfDirty();
			RRInappBillingPluginKit.StopUnityActivity();
		}
	}
}
