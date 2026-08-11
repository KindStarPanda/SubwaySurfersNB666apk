using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class FrontScreen : UIScreen
{
	private bool _hasTriggeredTweens;

	[SerializeField]
	private GameObject[] gameobjectsToTween;

	[SerializeField]
	private GameObject discountSticker;

	public bool buttonsHaveTweened;

	[method: MethodImpl(32)]
	public static event Action tweensHaveFinishedAnimating;

	private void Awake()
	{
		GameObject[] array = gameobjectsToTween;
		foreach (GameObject gameObject in array)
		{
			gameObject.SetActiveRecursively(false);
		}
	}

	public override void Init()
	{
		base.Init();
	}

	public override void Show()
	{
		base.Show();
		if (!_hasTriggeredTweens)
		{
			Invoke("triggerTween", 0.5f);
			_hasTriggeredTweens = true;
		}
		else
		{
			UpdateApp.ShowIfNeeded();
		}
		CheckForDiscountButton();
	}

	private void triggerTween()
	{
		int num = 0;
		GameObject[] array = gameobjectsToTween;
		foreach (GameObject gameObject in array)
		{
			gameObject.SetActiveRecursively(true);
			SpringPosition springPosition = SpringPosition.Begin(gameObject, Vector3.zero, 5f);
			if (num == 0)
			{
				springPosition.callWhenFinished = "TweensHaveFinishedAnimating";
				springPosition.eventReceiver = base.gameObject;
				num++;
			}
		}
		CheckForDiscountButton();
	}

	private void TweensHaveFinishedAnimating()
	{
		UpdateApp.ShowIfNeeded();
		buttonsHaveTweened = true;
		FrontScreen.tweensHaveFinishedAnimating();
	}

	private void CheckForDiscountButton()
	{
		if (DiscountButton.DiscountDoubleCoins || DiscountButton.DiscountInCoinShop)
		{
			discountSticker.SetActiveRecursively(true);
		}
		else
		{
			discountSticker.SetActiveRecursively(false);
		}
	}

	private void OnApplicationPause(bool paused)
	{
		if (!paused)
		{
			UpdateApp.ShowIfNeeded();
		}
	}
}
