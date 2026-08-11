using UnityEngine;

public class AutoMessageButton : MonoBehaviour
{
	[SerializeField]
	private UISprite iconOff;

	[SerializeField]
	private UILabel Label;

	private string _labelOff = "Auto Message OFF";

	private string _labelOn = "Auto Message ON";

	private void Awake()
	{
		_SetupButton();
	}

	public void Click()
	{
		Settings.optionAutoMessage = !Settings.optionAutoMessage;
		_SetupButton();
	}

	private void _SetupButton()
	{
		if (Settings.optionAutoMessage)
		{
			Label.text = _labelOn;
			iconOff.enabled = false;
		}
		else
		{
			Label.text = _labelOff;
			iconOff.enabled = true;
		}
	}
}
