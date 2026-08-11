using System;
using UnityEngine;

public class CoinButtonHelper : DiscountButton
{
	private string key;

	private int index;

	private bool inited;

	private UITable mainTable;

	private Transform onDisplayParent;

	private CoinScreen coinScreen;

	private bool scheduleSetup = true;

	private bool OnSale
	{
		get
		{
			if (DiscountButton.DiscountTier(index))
			{
				string text = "in_app_tier_" + (index + 1);
				priceModifier = OnlineSettings.instance.GetValue(text, 0);
				if (priceModifier < 0)
				{
					return true;
				}
			}
			return false;
		}
	}

	private bool ExtraStuff
	{
		get
		{
			if (DiscountButton.DiscountTier(index))
			{
				string text = "in_app_tier_" + (index + 1);
				priceModifier = OnlineSettings.instance.GetValue(text, 0);
				if (priceModifier > 0)
				{
					return true;
				}
			}
			return false;
		}
	}

	private void OnClick()
	{
		InAppManager.Instance.BuyInApp(key);
	}

	public void Init(int sendIndex)
	{
		inited = true;
		index = sendIndex;
		foreach (Transform item in base.transform.parent)
		{
			if (item.name == "000")
			{
				onDisplayParent = item;
			}
		}
		mainTable = onDisplayParent.parent.GetComponent<UITable>();
		coinScreen = NGUITools.FindInParents<CoinScreen>(base.transform.parent.gameObject);
		_Setup();
		icon.MakePixelPerfect();
		InAppManager instance = InAppManager.Instance;
		instance.onProductRequestSuccess = (Action)Delegate.Combine(instance.onProductRequestSuccess, new Action(UpdatePrice));
	}

	private void ForceEndDiscount()
	{
		InAppManager instance = InAppManager.Instance;
		instance.onProductRequestSuccess = (Action)Delegate.Remove(instance.onProductRequestSuccess, new Action(UpdatePrice));
		_Setup(true);
		coinScreen.counterForElementsOnDisplay = 0;
	}

	private void _Setup(bool forceEndDiscount = false)
	{
		if (OnSale && !forceEndDiscount)
		{
			key = InAppData.inAppTiersAndInAppTiersDiscount[index + InAppData.inAppTiersAndInAppTiersDiscount.Length / 2];
			ShowOnSale(key, string.Format("{0:n0}", InAppData.inAppData[key].amountOfCoins) + " Coins");
			if (base.transform.parent != onDisplayParent)
			{
				coinScreen.counterForElementsOnDisplay++;
				base.transform.parent = onDisplayParent;
				base.transform.localPosition = new Vector3(0f, -75 * coinScreen.counterForElementsOnDisplay + 15, 0f);
			}
		}
		else if (ExtraStuff && !forceEndDiscount)
		{
			key = InAppData.inAppTiersAndInAppTiersDiscount[index + InAppData.inAppTiersAndInAppTiersDiscount.Length / 2];
			ShowExtraStuff(key, string.Format("{0:n0}", InAppData.inAppData[key].amountOfCoins) + " Coins", string.Format("{0:n0}", InAppData.inAppData[key].amountOfCoins + priceModifier) + " Coins!");
			if (base.transform.parent != onDisplayParent)
			{
				coinScreen.counterForElementsOnDisplay++;
				base.transform.parent = onDisplayParent;
				base.transform.localPosition = new Vector3(0f, -75 * coinScreen.counterForElementsOnDisplay + 15, 0f);
			}
		}
		else
		{
			key = InAppData.inAppTiersAndInAppTiersDiscount[index];
			ShowNoDiscount(key, string.Format("{0:n0}", InAppData.inAppData[key].amountOfCoins) + " Coins");
			if (base.transform.parent == onDisplayParent)
			{
				base.transform.parent = onDisplayParent.parent;
			}
		}
		mainTable.repositionNow = true;
		mainTable.Reposition();
	}

	private void OnEnable()
	{
		if (inited)
		{
			scheduleSetup = true;
		}
	}

	private void Update()
	{
		if (scheduleSetup)
		{
			_Setup();
			scheduleSetup = false;
		}
	}

	private void UpdatePrice()
	{
		price.text = InAppData.inAppData[key].price;
	}
}
