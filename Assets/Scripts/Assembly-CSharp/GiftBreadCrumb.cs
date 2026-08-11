using System.Collections;
using UnityEngine;

public class GiftBreadCrumb : MonoBehaviour
{
	[SerializeField]
	private UISprite sticker;

	[SerializeField]
	private UILabel topLabel;

	[SerializeField]
	private UILabel bottomLabel;

	[SerializeField]
	private bool isFrontpage;

	private bool _isInited;

	private void OnEnable()
	{
		if (_isInited)
		{
			SetupBreadCrumb();
			return;
		}
		StartCoroutine(DelayedSetup());
		_isInited = true;
	}

	public void SetupBreadCrumb()
	{
		if (PlayerInfo.Instance.ShouldShowBreadCrumb())
		{
			if (isFrontpage)
			{
				PlayerInfo.Instance.BreadCrumbShownOnFrontPage();
			}
			sticker.enabled = true;
			topLabel.enabled = true;
			bottomLabel.enabled = true;
		}
		else
		{
			sticker.enabled = false;
			topLabel.enabled = false;
			bottomLabel.enabled = false;
		}
	}

	private IEnumerator DelayedSetup()
	{
		yield return null;
		SetupBreadCrumb();
	}
}
