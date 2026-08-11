using System;
using UnityEngine;

public class UICharacterBuyButton : MonoBehaviour
{
	[SerializeField]
	private UILabel priceLabel;

	[SerializeField]
	private UILabel expirePriceLabel;

	[SerializeField]
	private UILabel expireTimeLabel;

	private BoxCollider col;

	private bool isEnabled = true;

	private bool _purchaseInProgress;

	public Action OnChangedCurrentlyShown;

	private bool _hasInited;

	private Characters.CharacterType _currentCharacter;

	private int _totalSeconds = -1;

	private void OnEnable()
	{
		if (_hasInited)
		{
			isEnabled = true;
			UIModelController.Instance.AddOnChangedCurrentlyShownHandler(OnChangedCurrentlyShownModel);
			OnChangedCurrentlyShownModel();
		}
	}

	private void OnDisable()
	{
		UIModelController.Instance.RemoveOnChangedCurrentlyShownHandler(OnChangedCurrentlyShownModel);
	}

	private void Start()
	{
		if (!_hasInited)
		{
			col = GetComponent<BoxCollider>();
			isEnabled = true;
			UIModelController.Instance.AddOnChangedCurrentlyShownHandler(OnChangedCurrentlyShownModel);
			OnChangedCurrentlyShownModel();
			_hasInited = true;
		}
	}

	private void OnClick()
	{
		if (!_purchaseInProgress)
		{
			PurchaseHandler.Instance.PurchaseCharacter(UIModelController.Instance.currentlyShownModel, this);
		}
	}

	private void Update()
	{
		if (expireTimeLabel.gameObject.active && expireTimeLabel.enabled)
		{
			TimeSpan timeSpan = ThemeManager.Instance.themeForSeason(Characters.characterData[_currentCharacter].characterSeason).TimeToExpire;
			if (timeSpan < TimeSpan.Zero)
			{
				timeSpan = TimeSpan.Zero;
			}
			if (_totalSeconds != (int)timeSpan.TotalSeconds)
			{
				expireTimeLabel.text = string.Format("{0:0} day{1} {2:00}:{3:00}:{4:00}", timeSpan.Days, (timeSpan.Days == 1) ? string.Empty : "s", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
				_totalSeconds = (int)timeSpan.TotalSeconds;
			}
		}
	}

	private void OnChangedCurrentlyShownModel()
	{
		Characters.CharacterType currentlyShownModel = UIModelController.Instance.currentlyShownModel;
		Characters.Model model = Characters.characterData[currentlyShownModel];
		if (model.unlockType != Characters.UnlockType.coins)
		{
			hideAndDisable();
			return;
		}
		if (PlayerInfo.Instance.IsCollectionComplete(currentlyShownModel) ? true : false)
		{
			hideAndDisable();
			return;
		}
		int price = model.Price;
		priceLabel.text = price.ToString();
		expirePriceLabel.text = price.ToString();
		if (model.characterSeason != 0)
		{
			TimeSpan timeSpan = ThemeManager.Instance.themeForSeason(Characters.characterData[_currentCharacter].characterSeason).TimeToExpire;
			if (timeSpan < TimeSpan.Zero)
			{
				timeSpan = TimeSpan.Zero;
			}
			_currentCharacter = currentlyShownModel;
			expireTimeLabel.text = string.Format("{0:0} day{1} {2:00}:{3:00}:{4:00}", timeSpan.Days, (timeSpan.Days == 1) ? string.Empty : "s", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
			expireTimeLabel.gameObject.active = true;
			expirePriceLabel.gameObject.active = true;
			expirePriceLabel.enabled = true;
			expireTimeLabel.enabled = true;
			expirePriceLabel.SendMessage("CreatePanel", SendMessageOptions.DontRequireReceiver);
			expireTimeLabel.SendMessage("CreatePanel", SendMessageOptions.DontRequireReceiver);
			priceLabel.enabled = false;
		}
		else
		{
			priceLabel.enabled = true;
			expirePriceLabel.enabled = false;
			expireTimeLabel.enabled = false;
			priceLabel.SendMessage("CreatePanel", SendMessageOptions.DontRequireReceiver);
		}
		showAndEnable();
	}

	private void hideAndDisable()
	{
		for (int i = 0; i < base.transform.childCount; i++)
		{
			Transform child = base.transform.GetChild(i);
			child.gameObject.active = false;
		}
		col.enabled = false;
		isEnabled = false;
		expireTimeLabel.gameObject.active = false;
		expirePriceLabel.gameObject.active = false;
	}

	private void showAndEnable()
	{
		if (!isEnabled)
		{
			for (int i = 0; i < base.transform.childCount; i++)
			{
				Transform child = base.transform.GetChild(i);
				child.gameObject.active = true;
			}
			if (priceLabel.enabled)
			{
				priceLabel.panel.Refresh();
			}
			else
			{
				expirePriceLabel.panel.Refresh();
			}
			col.enabled = true;
			isEnabled = true;
		}
	}

	public void PurchaseSuccessful()
	{
		_purchaseInProgress = false;
		UIModelController.Instance.SelectCurrentModel();
	}

	public void PurchaseFailure()
	{
		_purchaseInProgress = false;
	}
}
