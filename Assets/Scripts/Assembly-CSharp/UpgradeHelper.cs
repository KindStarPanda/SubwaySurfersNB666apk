using System;
using System.Collections;
using UnityEngine;

public class UpgradeHelper : MonoBehaviour
{
	private enum AnimState
	{
		Closed = 0,
		Opening = 1,
		Open = 2,
		Closing = 3
	}

	[SerializeField]
	private UISprite iconBG;

	public UISprite powerupIcon;

	public UILabel titleLabel;

	public UILabel descLabel;

	public UILabel priceLabel;

	public UILabel amountLabel;

	public BuyButtonIngame buyButton;

	public BoxCollider descriptionButtonCollider;

	public UILabel buyButtonTitle;

	public UISlicedSprite buyButtonBackground;

	public UITierHelper tierHelper;

	public UISprite coin;

	private PowerupType _type;

	private UpgradeScreen _upgradeScreen;

	private UISlicedSprite _fullBG;

	private bool _hasInited;

	private bool _fixCollider;

	public ContentChange contentChanger;

	private AnimState _animState;

	private AnimState animState
	{
		get
		{
			return _animState;
		}
		set
		{
			_animState = value;
		}
	}

	private void OnEnable()
	{
		PlayerInfo instance = PlayerInfo.Instance;
		instance.onPowerupAmountChanged = (Action)Delegate.Combine(instance.onPowerupAmountChanged, new Action(RefreshUpgrade));
		PurchaseHandler.Instance.AddOnUpgradePurchase(RefreshUpgrade);
		RefreshUpgrade();
	}

	private void OnDisable()
	{
		PlayerInfo instance = PlayerInfo.Instance;
		instance.onPowerupAmountChanged = (Action)Delegate.Remove(instance.onPowerupAmountChanged, new Action(RefreshUpgrade));
		PurchaseHandler.Instance.RemoveOnUpgradePurchase(RefreshUpgrade);
	}

	public void InitSingle(PowerupType type)
	{
		_type = type;
		powerupIcon.spriteName = Upgrades.upgrades[type].iconName;
		powerupIcon.MakePixelPerfect();
		Vector2 vector = new Vector2(iconBG.cachedTransform.localPosition.x, iconBG.cachedTransform.localPosition.y);
		Vector2 vector2 = new Vector2(iconBG.cachedTransform.localScale.x, iconBG.cachedTransform.localScale.y);
		Vector3 localPosition = new Vector3(vector.x + vector2.x / 2f, vector.y - vector2.y / 2f, powerupIcon.cachedTransform.localPosition.z);
		powerupIcon.cachedTransform.localPosition = localPosition;
		titleLabel.text = Upgrades.upgrades[type].name.ToUpper();
		descLabel.text = Upgrades.upgrades[type].description;
		priceLabel.text = string.Empty + Upgrades.upgrades[type].getPrice(0);
		animState = AnimState.Closed;
		switch (type)
		{
		case PowerupType.skipmission1:
		{
			MissionInfo missionInfo = Missions.Instance.GetMissionInfo(0);
			string format = ((missionInfo.mission.goal != 1) ? missionInfo.template.ultraShortDescription : missionInfo.template.ultraShortDescriptionSingle);
			amountLabel.text = string.Format(format, missionInfo.mission.goal);
			break;
		}
		case PowerupType.skipmission2:
		{
			MissionInfo missionInfo = Missions.Instance.GetMissionInfo(1);
			string format = ((missionInfo.mission.goal != 1) ? missionInfo.template.ultraShortDescription : missionInfo.template.ultraShortDescriptionSingle);
			amountLabel.text = string.Format(format, missionInfo.mission.goal);
			break;
		}
		case PowerupType.skipmission3:
		{
			MissionInfo missionInfo = Missions.Instance.GetMissionInfo(2);
			string format = ((missionInfo.mission.goal != 1) ? missionInfo.template.ultraShortDescription : missionInfo.template.ultraShortDescriptionSingle);
			amountLabel.text = string.Format(format, missionInfo.mission.goal);
			break;
		}
		case PowerupType.mysterybox:
			amountLabel.text = "Opens immediately";
			break;
		default:
			amountLabel.text = "You have: " + PlayerInfo.Instance.GetUpgradeAmount(type);
			break;
		}
		buyButton.initBuyButton(type);
		if (type != PowerupType.mysterybox && contentChanger.descriptionButton != null)
		{
			UnityEngine.Object.Destroy(contentChanger.descriptionButton);
		}
		_hasInited = true;
		RefreshUpgrade();
		_fixCollider = true;
	}

	public void InitPermanent(PowerupType type)
	{
		_type = type;
		powerupIcon.spriteName = Upgrades.upgrades[type].iconName;
		powerupIcon.MakePixelPerfect();
		Vector2 vector = new Vector2(iconBG.cachedTransform.localPosition.x, iconBG.cachedTransform.localPosition.y);
		Vector2 vector2 = new Vector2(iconBG.cachedTransform.localScale.x, iconBG.cachedTransform.localScale.y);
		Vector3 localPosition = new Vector3(vector.x + vector2.x / 2f, vector.y - vector2.y / 2f, powerupIcon.cachedTransform.localPosition.z);
		powerupIcon.cachedTransform.localPosition = localPosition;
		titleLabel.text = Upgrades.upgrades[type].name.ToUpper();
		descLabel.text = Upgrades.upgrades[type].description;
		tierHelper.SetupTiers(type);
		animState = AnimState.Closed;
		if (PlayerInfo.Instance.GetCurrentTier(type) >= Upgrades.upgrades[type].numberOfTiers - 1)
		{
			if (_fullBG == null)
			{
				priceLabel.text = "Full";
				priceLabel.color = new Color(23f / 85f, 0.2627451f, 0.2627451f, 1f);
				priceLabel.effectColor = new Color(0.85490197f, 0.85490197f, 0.85490197f, 1f);
				priceLabel.effectStyle = UILabel.Effect.Shadow;
				priceLabel.pivot = UIWidget.Pivot.Top;
				priceLabel.transform.localPosition = new Vector3(101f, priceLabel.transform.localPosition.y, priceLabel.transform.localPosition.z);
				UISlicedSprite uISlicedSprite = NGUITools.AddSprite(priceLabel.transform.parent.gameObject, coin.atlas, "background_buy_full") as UISlicedSprite;
				uISlicedSprite.depth = 5;
				uISlicedSprite.transform.localPosition = new Vector3(101f, 28f, 0f);
				uISlicedSprite.transform.localScale = new Vector3(52f, 20f, 1f);
				uISlicedSprite.pivot = UIWidget.Pivot.Center;
				uISlicedSprite.color = new Color(52f / 85f, 52f / 85f, 52f / 85f, 1f);
				uISlicedSprite.name = "4FullBG";
				uISlicedSprite.fillCenter = true;
				uISlicedSprite.MakePixelPerfect();
				_fullBG = uISlicedSprite;
				NGUITools.Destroy(coin);
				if (buyButton != null)
				{
					NGUITools.Destroy(buyButton.gameObject);
				}
			}
		}
		else
		{
			priceLabel.text = string.Empty + Upgrades.upgrades[type].getPrice(PlayerInfo.Instance.GetCurrentTier(type) + 1);
		}
		if (buyButton != null)
		{
			buyButton.initBuyButton(type);
		}
		_hasInited = true;
		RefreshUpgrade();
		_fixCollider = true;
	}

	public void UpgradePurchased(PowerupType type)
	{
		switch (type)
		{
		case PowerupType.hoverboard:
		case PowerupType.headstart500:
		case PowerupType.headstart2000:
			if (amountLabel != null)
			{
				amountLabel.text = "You have: " + PlayerInfo.Instance.GetUpgradeAmount(type);
			}
			break;
		case PowerupType.jetpack:
		case PowerupType.supersneakers:
		case PowerupType.coinmagnet:
		case PowerupType.letters:
		case PowerupType.doubleMultiplier:
			if (tierHelper.ResetTiers())
			{
				if (_fullBG == null)
				{
					priceLabel.text = "Full";
					priceLabel.color = new Color(23f / 85f, 0.2627451f, 0.2627451f, 1f);
					priceLabel.effectColor = new Color(0.85490197f, 0.85490197f, 0.85490197f, 1f);
					priceLabel.effectStyle = UILabel.Effect.Shadow;
					priceLabel.pivot = UIWidget.Pivot.Top;
					priceLabel.transform.localPosition = new Vector3(101f, priceLabel.transform.localPosition.y, priceLabel.transform.localPosition.z);
					UISlicedSprite uISlicedSprite = NGUITools.AddSprite(priceLabel.transform.parent.gameObject, coin.atlas, "background_buy_full") as UISlicedSprite;
					uISlicedSprite.depth = 5;
					uISlicedSprite.transform.localPosition = new Vector3(101f, 28f, 0f);
					uISlicedSprite.transform.localScale = new Vector3(52f, 20f, 1f);
					uISlicedSprite.pivot = UIWidget.Pivot.Center;
					uISlicedSprite.color = new Color(52f / 85f, 52f / 85f, 52f / 85f, 1f);
					uISlicedSprite.name = "4FullBG";
					uISlicedSprite.fillCenter = true;
					uISlicedSprite.MakePixelPerfect();
					_fullBG = uISlicedSprite;
					NGUITools.Destroy(coin);
					if (buyButton != null)
					{
						NGUITools.Destroy(buyButton.gameObject);
					}
				}
			}
			else
			{
				priceLabel.text = string.Empty + Upgrades.upgrades[type].getPrice(PlayerInfo.Instance.GetCurrentTier(type) + 1);
			}
			break;
		case PowerupType.mysterybox:
			PlayerInfo.Instance.AddMysteryBoxToUnlock(MysteryBox.Type.Normal);
			UIScreenController.Instance.QueueMysteryBox();
			break;
		case PowerupType.skipmission1:
			Flurry.LogEventWithAParameter("Boost Mission Skip purchased", "Mission Set and Index", Missions.Instance.currentMissionSet + "-0");
			Missions.Instance.SkipMission(0);
			break;
		case PowerupType.skipmission2:
			Flurry.LogEventWithAParameter("Boost Mission Skip purchased", "Mission Set and Index", Missions.Instance.currentMissionSet + "-1");
			Missions.Instance.SkipMission(1);
			break;
		case PowerupType.skipmission3:
			Flurry.LogEventWithAParameter("Boost Mission Skip purchased", "Mission Set and Index", Missions.Instance.currentMissionSet + "-2");
			Missions.Instance.SkipMission(2);
			break;
		}
		switch (type)
		{
		case PowerupType.headstart500:
			Flurry.LogEvent("Boost Headstart500 purchased");
			break;
		case PowerupType.headstart2000:
			Flurry.LogEvent("Boost Headstart2000 purchased");
			break;
		case PowerupType.hoverboard:
			Flurry.LogEvent("Boost Hoverboard purchased");
			break;
		case PowerupType.coinmagnet:
			Flurry.LogEventWithAParameter("Boost Coinmagnet purchased", "Tier", PlayerInfo.Instance.GetCurrentTier(type).ToString());
			break;
		case PowerupType.doubleMultiplier:
			Flurry.LogEventWithAParameter("Boost 2x multiplier purchased", "Tier", PlayerInfo.Instance.GetCurrentTier(type).ToString());
			break;
		case PowerupType.jetpack:
			Flurry.LogEventWithAParameter("Boost jetpack purchased", "Tier", PlayerInfo.Instance.GetCurrentTier(type).ToString());
			break;
		case PowerupType.letters:
			Flurry.LogEventWithAParameter("Boost letters purchased", "Tier", PlayerInfo.Instance.GetCurrentTier(type).ToString());
			break;
		case PowerupType.supersneakers:
			Flurry.LogEventWithAParameter("Boost supersneakers purchased", "Tier", PlayerInfo.Instance.GetCurrentTier(type).ToString());
			break;
		case PowerupType.mysterybox:
			Flurry.LogEvent("Boost MysteryBox purchased");
			break;
		}
	}

	private void RefreshUpgrade()
	{
		if (!_hasInited)
		{
			return;
		}
		switch (_type)
		{
		case PowerupType.hoverboard:
		case PowerupType.headstart500:
		case PowerupType.headstart2000:
			if (amountLabel != null)
			{
				amountLabel.text = "You have: " + PlayerInfo.Instance.GetUpgradeAmount(_type);
			}
			break;
		case PowerupType.jetpack:
		case PowerupType.supersneakers:
		case PowerupType.coinmagnet:
		case PowerupType.letters:
		case PowerupType.doubleMultiplier:
			if (tierHelper.ResetTiers())
			{
				if (_fullBG == null)
				{
					priceLabel.text = "Full";
					priceLabel.color = new Color(23f / 85f, 0.2627451f, 0.2627451f, 1f);
					priceLabel.effectColor = new Color(0.85490197f, 0.85490197f, 0.85490197f, 1f);
					priceLabel.effectStyle = UILabel.Effect.Shadow;
					priceLabel.pivot = UIWidget.Pivot.Top;
					priceLabel.transform.localPosition = new Vector3(101f, priceLabel.transform.localPosition.y, priceLabel.transform.localPosition.z);
					UISlicedSprite uISlicedSprite = NGUITools.AddSprite(priceLabel.transform.parent.gameObject, coin.atlas, "background_buy_full") as UISlicedSprite;
					uISlicedSprite.depth = 5;
					uISlicedSprite.transform.localPosition = new Vector3(101f, 28f, 0f);
					uISlicedSprite.transform.localScale = new Vector3(52f, 20f, 1f);
					uISlicedSprite.pivot = UIWidget.Pivot.Center;
					uISlicedSprite.color = new Color(52f / 85f, 52f / 85f, 52f / 85f, 1f);
					uISlicedSprite.name = "4FullBG";
					uISlicedSprite.fillCenter = true;
					uISlicedSprite.MakePixelPerfect();
					_fullBG = uISlicedSprite;
					NGUITools.Destroy(coin);
					if (buyButton != null)
					{
						NGUITools.Destroy(buyButton.gameObject);
					}
				}
			}
			else
			{
				priceLabel.text = string.Empty + Upgrades.upgrades[_type].getPrice(PlayerInfo.Instance.GetCurrentTier(_type) + 1);
			}
			break;
		}
		if (animState == AnimState.Closed)
		{
			if (buyButton != null && buyButton.GetComponent<Collider>() != null)
			{
				BoxCollider boxCollider = buyButton.GetComponent<Collider>() as BoxCollider;
				boxCollider.center = new Vector3(boxCollider.center.x, boxCollider.center.y, 15f);
			}
			if (descriptionButtonCollider != null)
			{
				descriptionButtonCollider.center = new Vector3(descriptionButtonCollider.center.x, descriptionButtonCollider.center.y, 15f);
			}
		}
	}

	private void OnClick()
	{
		AnimationStarted();
	}

	public void AnimationStarted()
	{
		powerupIcon.panel.widgetsAreStatic = false;
		if (animState == AnimState.Closed || animState == AnimState.Closing)
		{
			animState = AnimState.Opening;
			if (_upgradeScreen == null)
			{
				_upgradeScreen = NGUITools.FindInParents<UpgradeScreen>(base.gameObject);
			}
			foreach (UpgradeHelper cachedUpgradeHelper in _upgradeScreen.cachedUpgradeHelpers)
			{
				if (cachedUpgradeHelper != this && cachedUpgradeHelper.animState == AnimState.Open)
				{
					cachedUpgradeHelper.SendMessage("OnClick");
				}
			}
		}
		else
		{
			animState = AnimState.Closing;
		}
		contentChanger.FoldClicked();
		Collider collider = base.gameObject.GetComponent<Collider>();
		if (collider != null)
		{
			collider.enabled = false;
			UnityEngine.Object.Destroy(collider);
		}
	}

	public void AnimationEnded()
	{
		if (animState == AnimState.Closing || animState == AnimState.Opening)
		{
			NGUITools.AddWidgetCollider(base.gameObject);
			animState = ((animState != AnimState.Closing) ? AnimState.Open : AnimState.Closed);
			contentChanger.TriggerContent();
			powerupIcon.panel.SendMessage("LateUpdate");
			StartCoroutine(SetStatic());
		}
		else
		{
			Debug.LogError("Unexpected anim state: " + animState, this);
		}
	}

	private IEnumerator SetStatic()
	{
		yield return null;
		powerupIcon.panel.widgetsAreStatic = true;
	}

	private void ColliderFixer()
	{
		NGUITools.AddWidgetCollider(base.gameObject);
	}

	private void Update()
	{
		if (_fixCollider)
		{
			ColliderFixer();
			_fixCollider = false;
		}
	}
}
