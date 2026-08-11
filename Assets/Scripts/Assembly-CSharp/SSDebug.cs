using UnityEngine;

public class SSDebug : MonoBehaviour
{
	public Camera mainCamera;

	public UITexture backgroundTexture;

	private void Start()
	{
		if (DeviceInfo.formFactor == DeviceInfo.FormFactor.iPad)
		{
			backgroundTexture.GetComponent<UIStretch>().relativeSize = Vector2.one;
		}
		else
		{
			backgroundTexture.GetComponent<UIStretch>().relativeSize = new Vector2(1.0667f, 1.2f);
		}
	}

	private void DisableCamera()
	{
		backgroundTexture.enabled = true;
		mainCamera.enabled = false;
	}

	private void EnableCamera()
	{
		backgroundTexture.enabled = false;
		mainCamera.enabled = true;
	}
}
