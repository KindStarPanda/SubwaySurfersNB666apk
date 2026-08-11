using System;
using UnityEngine;

public class DoubleCoinUpgradeHelper : DiscountButton
{
	private string doubleCoinVersion;

	private bool inited;

	public bool OnSale
	{
		get
		{
			if (DiscountButton.DiscountDoubleCoins)
			{
				priceModifier = OnlineSettings.instance.GetValue("double_coin_discount", 0);
				if (priceModifier < 0)
				{
					return true;
				}
			}
			return false;
		}
	}

	public override void Awake()
	{
		base.Awake();
	}

	private void ForceEndDiscount()
	{
		Debug.Log("Force End Discount", this);
		InAppManager instance = InAppManager.Instance;
		instance.onProductRequestSuccess = (Action)Delegate.Remove(instance.onProductRequestSuccess, new Action(UpdatePrice));
		_Setup(true);
	}

	private void _DefaultSetup()
	{
		_Setup();
	}

	private void _Setup(bool forceEndDiscount = false)
	{
		if (PlayerInfo.Instance.hasDoubleCoins)
		{
			doubleCoinVersion = "com.kiloo.subways.doublecoins";
			InAppManager instance = InAppManager.Instance;
			instance.onPurchaseSuccess = (Action)Delegate.Remove(instance.onPurchaseSuccess, new Action(_DefaultSetup));
			ShowNoDiscount(doubleCoinVersion, string.Empty);
			fillColorSprite.spriteName = "button_check_grey";
			fillColorSprite.pivot = UIWidget.Pivot.Center;
			if (DeviceInfo.isHighres)
			{
				fillColorSprite.cachedTransform.localScale = new Vector3(fillColorSprite.sprite.inner.width / 2f, fillColorSprite.sprite.inner.height / 2f, 1f);
			}
			else
			{
				fillColorSprite.cachedTransform.localScale = new Vector3(fillColorSprite.sprite.inner.width, fillColorSprite.sprite.inner.height, 1f);
			}
			fillColorSprite.cachedTransform.localPosition = new Vector3(97f, 2f, 0f);
			price.enabled = false;
			GetComponent<BoxCollider>().enabled = false;
		}
		else if (OnSale && !forceEndDiscount)
		{
			doubleCoinVersion = "com.kiloo.subways.doublecoinsdiscount";
			ShowOnSale("com.kiloo.subways.doublecoinsdiscount", string.Empty);
			if (base.transform.parent.name != "000")
			{
				foreach (Transform item in base.transform.parent)
				{
					if (item.name == "000")
					{
						base.transform.parent = item;
						base.transform.localPosition = new Vector3(0f, -75f, 0f);
					}
				}
			}
		}
		else
		{
			doubleCoinVersion = "com.kiloo.subways.doublecoins";
			ShowNoDiscount("com.kiloo.subways.doublecoins", string.Empty);
			if (base.transform.parent.name == "000")
			{
				base.transform.parent = base.transform.parent.parent;
			}
		}
		InAppManager instance2 = InAppManager.Instance;
		instance2.onProductRequestSuccess = (Action)Delegate.Combine(instance2.onProductRequestSuccess, new Action(UpdatePrice));
	}

	public void Init()
	{
		InAppManager instance = InAppManager.Instance;
		instance.onPurchaseSuccess = (Action)Delegate.Combine(instance.onPurchaseSuccess, new Action(_DefaultSetup));
		inited = true;
		_Setup();
	}

	private void OnEnable()
	{
		if (inited)
		{
			_Setup();
		}
	}

	private void UpdatePrice()
	{
		price.text = InAppData.inAppData[doubleCoinVersion].price;
	}

	private void OnClick()
	{
		if (!PlayerInfo.Instance.hasDoubleCoins)
		{
			InAppManager.Instance.BuyInApp(doubleCoinVersion);
		}
	}
}
