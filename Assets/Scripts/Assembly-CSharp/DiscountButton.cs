using System;
using UnityEngine;

public class DiscountButton : MonoBehaviour
{
	protected const int SALE_BUTTON_GROWTH = 10;

	protected readonly Color descriptionColor = new Color(0.5803922f, 0.7490196f, 0.5254902f);

	protected Vector3 originalDescriptionPosition;

	protected Vector3 originalPricePosition;

	protected Vector3 originalFillGrayScale;

	protected Vector3 originalFillColorScale;

	protected Vector3 originalOutlineScale;

	protected Vector3 originalContentPosition;

	protected Vector3 originalIconBgPosition;

	protected Vector3 originalIconPosition;

	protected string originalFillColorSpirteName;

	protected string originalFillGraySpriteName;

	[SerializeField]
	protected UISprite iconBG;

	[SerializeField]
	protected UISprite icon;

	[SerializeField]
	protected UILabel title;

	[SerializeField]
	protected UILabel description;

	[SerializeField]
	protected UILabel description2;

	[SerializeField]
	protected UISprite lineThrough;

	[SerializeField]
	protected UILabel price;

	[SerializeField]
	protected UISprite fillGraySprite;

	[SerializeField]
	protected UISprite fillColorSprite;

	[SerializeField]
	protected UISprite outlineSprite;

	[SerializeField]
	protected Transform content;

	[SerializeField]
	protected UISprite discountSticker;

	[SerializeField]
	protected UILabel discountLabel;

	[SerializeField]
	protected UILabel limitedTitle;

	[SerializeField]
	protected UILabel limitedTime;

	protected static double endTimeDouble;

	protected static TimeSpan timeLeft;

	protected int priceModifier;

	private static bool Discount
	{
		get
		{
			endTimeDouble = OnlineSettings.instance.GetValue("discount_end_time", 0.0);
			DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(endTimeDouble);
			if ((dateTime - DateTime.UtcNow).Ticks > 0)
			{
				return true;
			}
			return false;
		}
	}

	public static bool DiscountDoubleCoins
	{
		get
		{
			if (!PlayerInfo.Instance.hasDoubleCoins && Discount && OnlineSettings.instance.GetValue("double_coin_discount", 0) < 0 && InAppData.inAppData["com.kiloo.subways.doublecoinsdiscount"].validInApp)
			{
				return true;
			}
			return false;
		}
	}

	public static bool DiscountInCoinShop
	{
		get
		{
			for (int i = 0; i < InAppData.inAppTiersAndInAppTiersDiscount.Length / 2; i++)
			{
				if (DiscountTier(i))
				{
					return true;
				}
			}
			return false;
		}
	}

	protected static bool DiscountTier(int i)
	{
		if (Discount && OnlineSettings.instance.HasValue("in_app_tier_" + (i + 1)) && InAppData.inAppData[InAppData.inAppTiersAndInAppTiersDiscount[i + InAppData.inAppTiersAndInAppTiersDiscount.Length / 2]].validInApp)
		{
			return true;
		}
		return false;
	}

	public virtual void Awake()
	{
		originalFillColorSpirteName = fillColorSprite.spriteName;
		originalFillGraySpriteName = fillGraySprite.spriteName;
		originalFillGrayScale = fillGraySprite.cachedTransform.localScale;
		originalFillColorScale = fillColorSprite.cachedTransform.localScale;
		originalOutlineScale = outlineSprite.cachedTransform.localScale;
		originalContentPosition = content.localPosition;
		originalDescriptionPosition = description.cachedTransform.localPosition;
		originalPricePosition = price.cachedTransform.localPosition;
		originalIconBgPosition = iconBG.cachedTransform.localPosition;
		originalIconPosition = icon.cachedTransform.localPosition;
	}

	protected void ShowExtraStuff(string productString, string sendDescription, string sendDescription2)
	{
		fillGraySprite.cachedTransform.localScale = originalFillGrayScale + new Vector3(0f, 10f, 0f);
		fillColorSprite.cachedTransform.localScale = originalFillColorScale + new Vector3(0f, 10f, 0f);
		outlineSprite.cachedTransform.localScale = originalOutlineScale + new Vector3(0f, 10f, 0f);
		content.localPosition = originalContentPosition + new Vector3(0f, 5f, 0f);
		fillColorSprite.spriteName = "button_fill_shopItem_highlight_sale";
		fillGraySprite.spriteName = "button_fill_shopItem_main_sale";
		limitedTitle.gameObject.active = true;
		limitedTime.gameObject.active = true;
		limitedTitle.enabled = true;
		limitedTime.enabled = true;
		discountSticker.enabled = true;
		discountLabel.enabled = true;
		discountSticker.gameObject.active = true;
		discountLabel.gameObject.active = true;
		discountLabel.text = "COOL\nDEAL";
		price.transform.localPosition = originalPricePosition + new Vector3(0f, 4f, 0f);
		iconBG.transform.localPosition = originalIconBgPosition - new Vector3(0f, 4f, 0f);
		icon.transform.localPosition = originalIconPosition - new Vector3(0f, 4f, 0f);
		title.text = OnlineSettings.instance.GetValue("discount_deal_name", "Holiday Deal");
		description.text = sendDescription;
		description.cachedTransform.localPosition = originalDescriptionPosition + new Vector3(0f, 3f, 0f);
		description2.enabled = true;
		description2.text = sendDescription2;
		description.color = descriptionColor;
		description2.color = Color.white;
		lineThrough.enabled = true;
		lineThrough.cachedTransform.localScale = new Vector3(6f + description.relativeSize.x * description.cachedTransform.localScale.x, lineThrough.cachedTransform.localScale.y, 1f);
		if (DeviceInfo.isHighres)
		{
			lineThrough.spriteName = "sale_strikeover_hi";
		}
		else
		{
			lineThrough.spriteName = "sale_strikeover_lo";
		}
		iconBG.spriteName = "icon_background_theme";
		Common(productString);
		_setUpTheIcon();
	}

	protected void ShowOnSale(string productString, string sendDescription = "")
	{
		fillGraySprite.cachedTransform.localScale = originalFillGrayScale + new Vector3(0f, 10f, 0f);
		fillColorSprite.cachedTransform.localScale = originalFillColorScale + new Vector3(0f, 10f, 0f);
		outlineSprite.cachedTransform.localScale = originalOutlineScale + new Vector3(0f, 10f, 0f);
		content.localPosition = originalContentPosition + new Vector3(0f, 5f, 0f);
		fillColorSprite.spriteName = "button_fill_shopItem_highlight_sale";
		fillGraySprite.spriteName = "button_fill_shopItem_main_sale";
		limitedTitle.gameObject.active = true;
		limitedTime.gameObject.active = true;
		limitedTitle.enabled = true;
		limitedTime.enabled = true;
		discountSticker.enabled = true;
		discountLabel.enabled = true;
		discountSticker.gameObject.active = true;
		discountLabel.gameObject.active = true;
		discountLabel.text = Mathf.Abs(priceModifier) + " %\nOFF";
		price.transform.localPosition = originalPricePosition + new Vector3(0f, 4f, 0f);
		iconBG.transform.localPosition = originalIconBgPosition - new Vector3(0f, 4f, 0f);
		icon.transform.localPosition = originalIconPosition - new Vector3(0f, 4f, 0f);
		title.text = OnlineSettings.instance.GetValue("discount_deal_name", "Holiday Deal");
		description.text = InAppData.inAppData[productString].title;
		description.cachedTransform.localPosition = originalDescriptionPosition + new Vector3(0f, 3f, 0f);
		description2.enabled = true;
		if (string.IsNullOrEmpty(InAppData.inAppData[productString].description))
		{
			description2.text = sendDescription;
			description.color = descriptionColor;
			description2.color = Color.white;
		}
		else
		{
			description2.text = InAppData.inAppData[productString].description;
			description.color = Color.white;
			description2.color = descriptionColor;
		}
		if (lineThrough != null)
		{
			lineThrough.enabled = false;
		}
		iconBG.spriteName = "icon_background_theme";
		Common(productString);
		_setUpTheIcon();
	}

	protected void ShowNoDiscount(string productString, string backupDescription = "")
	{
		fillGraySprite.cachedTransform.localScale = originalFillGrayScale;
		fillColorSprite.cachedTransform.localScale = originalFillColorScale;
		outlineSprite.cachedTransform.localScale = originalOutlineScale;
		content.localPosition = originalContentPosition;
		fillColorSprite.spriteName = originalFillColorSpirteName;
		fillGraySprite.spriteName = originalFillGraySpriteName;
		iconBG.cachedTransform.localPosition = originalIconBgPosition;
		icon.cachedTransform.localPosition = originalIconPosition;
		limitedTitle.gameObject.active = false;
		limitedTime.gameObject.active = false;
		limitedTitle.enabled = false;
		limitedTime.enabled = false;
		discountSticker.enabled = false;
		discountLabel.enabled = false;
		discountSticker.gameObject.active = false;
		discountLabel.gameObject.active = false;
		price.transform.localPosition = originalPricePosition;
		title.text = InAppData.inAppData[productString].title;
		if (string.IsNullOrEmpty(InAppData.inAppData[productString].description))
		{
			description.text = backupDescription;
		}
		else
		{
			description.text = InAppData.inAppData[productString].description;
		}
		description.color = Color.white;
		description.cachedTransform.localPosition = originalDescriptionPosition;
		description2.enabled = false;
		if (lineThrough != null)
		{
			lineThrough.enabled = false;
		}
		iconBG.spriteName = "icon_background_normal";
		Common(productString);
		_setUpTheIcon();
	}

	private void Common(string productString)
	{
		icon.spriteName = InAppData.inAppData[productString].iconName;
		icon.MakePixelPerfect();
		Vector2 vector = new Vector2(iconBG.cachedTransform.localPosition.x, iconBG.cachedTransform.localPosition.y);
		Vector2 vector2 = new Vector2(iconBG.cachedTransform.localScale.x, iconBG.cachedTransform.localScale.y);
		Vector3 vector3 = new Vector3(vector.x + vector2.x / 2f, vector.y - vector2.y / 2f, icon.cachedTransform.localPosition.z);
		price.text = InAppData.inAppData[productString].price;
	}

	private void _setUpTheIcon()
	{
		icon.MakePixelPerfect();
		float num = (iconBG.cachedTransform.localScale.x - icon.cachedTransform.localScale.x) / 2f;
		if (num >= 0f)
		{
			Vector3 localPosition = new Vector3(iconBG.cachedTransform.localPosition.x + num, icon.cachedTransform.localPosition.y, iconBG.cachedTransform.localPosition.z);
			icon.cachedTransform.localPosition = localPosition;
		}
	}
}
