using System.Collections;
using UnityEngine;

public class ResumeButtonHelper : MonoBehaviour
{
	private const float MIN_WAIT_TIME = 1f;

	private const float MAX_WAIT_TIME = 5f;

	private const string TEXT_WAIT = "WAIT";

	private const string TEXT_RESUME = "RESUME";

	[SerializeField]
	private UILabel label;

	private Color initColor;

	[SerializeField]
	private UISprite icon;

	private bool buttonEnabled = true;

	public bool isButtonEnabled
	{
		get
		{
			return buttonEnabled;
		}
	}

	private void OnApplicationPause(bool pause)
	{
		DisableButton();
		if (!pause)
		{
			StartCoroutine(EnableButtonWhenReady());
		}
	}

	private IEnumerator EnableButtonWhenReady()
	{
		float startTime = Time.realtimeSinceStartup;
		float timeWaited = 0f;
		while (timeWaited < 1f)
		{
			timeWaited = Time.realtimeSinceStartup - startTime;
			yield return new WaitForEndOfFrame();
		}
		EnableButton();
	}

	public void EnableButton()
	{
		if (!buttonEnabled)
		{
			NGUITools.AddWidgetCollider(base.gameObject);
			icon.color = initColor;
			buttonEnabled = true;
			label.text = "RESUME";
		}
	}

	public void DisableButton()
	{
		if (buttonEnabled)
		{
			if (base.gameObject.GetComponent<Collider>() != null)
			{
				Object.Destroy(base.gameObject.GetComponent<Collider>());
			}
			initColor = icon.color;
			icon.color = Color.gray;
			buttonEnabled = false;
			label.text = "WAIT";
		}
	}
}
