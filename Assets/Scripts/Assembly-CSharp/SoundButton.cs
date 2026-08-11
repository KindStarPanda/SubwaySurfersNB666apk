using UnityEngine;

public class SoundButton : MonoBehaviour
{
	[SerializeField]
	private UISprite iconOff;

	[SerializeField]
	private UILabel soundLabel;

	private string _labelOff = "Sound OFF";

	private string _labelOn = "Sound ON";

	private void Awake()
	{
		_SetupButton();
	}

	public void Click()
	{
		Settings.optionSound = !Settings.optionSound;
		_SetupButton();
	}

	private void _SetupButton()
	{
		if (Settings.optionSound)
		{
			soundLabel.text = _labelOn;
			iconOff.enabled = false;
		}
		else
		{
			soundLabel.text = _labelOff;
			iconOff.enabled = true;
		}
	}
}
