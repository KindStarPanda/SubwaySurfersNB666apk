using System;
using UnityEngine;

public class BuyHoverboardButton : MonoBehaviour
{
	private bool _purchaseInProgress;

	[SerializeField]
	private UILabel amountLabel;

	[SerializeField]
	private UILabel priceLabel;

	private void Start()
	{
		priceLabel.text = Upgrades.upgrades[PowerupType.hoverboard].getPrice(0).ToString();
	}

	private void OnEnable()
	{
		PlayerInfo instance = PlayerInfo.Instance;
		instance.onPowerupAmountChanged = (Action)Delegate.Combine(instance.onPowerupAmountChanged, new Action(UpdateLabels));
		UpdateLabels();
	}

	private void OnDisable()
	{
		PlayerInfo instance = PlayerInfo.Instance;
		instance.onPowerupAmountChanged = (Action)Delegate.Remove(instance.onPowerupAmountChanged, new Action(UpdateLabels));
	}

	private void OnClick()
	{
		if (!_purchaseInProgress)
		{
			PurchaseHandler.Instance.PurchaseHoverboard(this);
		}
	}

	private void UpdateLabels()
	{
		amountLabel.text = PlayerInfo.Instance.GetUpgradeAmount(PowerupType.hoverboard).ToString();
	}

	public void PurchaseSuccessful()
	{
		_purchaseInProgress = false;
	}

	public void PurchaseFailure()
	{
		_purchaseInProgress = false;
	}
}
