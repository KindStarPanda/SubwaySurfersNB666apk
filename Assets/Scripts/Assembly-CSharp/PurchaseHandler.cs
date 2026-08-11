using System;
using UnityEngine;

public class PurchaseHandler
{
	private Action _onUpgradePurchase;

	private static PurchaseHandler _instance;

	public static PurchaseHandler Instance
	{
		get
		{
			return _instance ?? (_instance = new PurchaseHandler());
		}
	}

	public void AddOnUpgradePurchase(Action handler)
	{
		_onUpgradePurchase = (Action)Delegate.Combine(_onUpgradePurchase, handler);
	}

	public void RemoveOnUpgradePurchase(Action handler)
	{
		if (_onUpgradePurchase != null)
		{
			_onUpgradePurchase = (Action)Delegate.Remove(_onUpgradePurchase, handler);
		}
	}

	public void PurchaseCharacter(Characters.CharacterType characterType, UICharacterBuyButton sender)
	{
		Characters.Model model = Characters.characterData[characterType];
		if (model.unlockType != Characters.UnlockType.coins)
		{
			Debug.LogWarning("Cannot buy character with unlocktype: " + model.unlockType);
			sender.PurchaseFailure();
			return;
		}
		int price = model.Price;
		if (PlayerInfo.Instance.amountOfCoins >= price)
		{
			Missions.Instance.PlayerDidThis(Missions.MissionTarget.SpendCoin, price);
			PlayerInfo.Instance.CollectToken(characterType, price);
			PlayerInfo.Instance.amountOfCoins -= price;
			sender.PurchaseSuccessful();
			if (Characters.characterData[characterType].missionTargetKey != 0)
			{
				Missions.Instance.PlayerDidThis(Characters.characterData[characterType].missionTargetKey);
			}
		}
		else
		{
			InAppManager.Instance.SetupNativePopup(price, model.modelName);
			sender.PurchaseFailure();
		}
	}

	public void PurchaseHoverboard(BuyHoverboardButton sender)
	{
		int price = Upgrades.upgrades[PowerupType.hoverboard].getPrice(0);
		if (PlayerInfo.Instance.amountOfCoins >= price)
		{
			PlayerInfo.Instance.IncreaseUpgradeAmount(PowerupType.hoverboard);
			Missions.Instance.PlayerDidThis(Missions.MissionTarget.SpendCoin, price);
			PlayerInfo.Instance.amountOfCoins -= price;
			sender.PurchaseSuccessful();
			Action onUpgradePurchase = _onUpgradePurchase;
			if (onUpgradePurchase != null)
			{
				onUpgradePurchase();
			}
			PlayerInfo.Instance.SaveIfDirty();
		}
		else
		{
			InAppManager.Instance.SetupNativePopup(price, "Hoverboard");
			sender.PurchaseFailure();
		}
	}

	public void PurchaseHoverboardType(Hoverboards.BoardType boardType, HoverboardBuyButton sender)
	{
		Hoverboards.Board board = Hoverboards.boardData[boardType];
		if (board.unlockType == Hoverboards.UnlockType.free)
		{
			PlayerInfo.Instance.UnlockHoverboard(boardType);
			UIScreenController.Instance.QueueSlideIn(UIScreenController.SlideInType.Unlock, board.name);
			sender.PurchaseSuccessful();
		}
		else if (board.unlockType == Hoverboards.UnlockType.coins)
		{
			int price = board.price;
			if (PlayerInfo.Instance.amountOfCoins >= price)
			{
				Missions.Instance.PlayerDidThis(Missions.MissionTarget.SpendCoin, price);
				PlayerInfo.Instance.UnlockHoverboard(boardType);
				PlayerInfo.Instance.amountOfCoins -= price;
				UIScreenController.Instance.QueueSlideIn(UIScreenController.SlideInType.Unlock, board.name);
				sender.PurchaseSuccessful();
			}
			else
			{
				InAppManager.Instance.SetupNativePopup(price, board.name);
				sender.PurchaseFailure();
			}
		}
	}

	public void PurchaseUpgrade(PowerupType type, BuyButtonIngame sender)
	{
		int num = ((Upgrades.upgrades[type].numberOfTiers != 0) ? Upgrades.upgrades[type].getPrice(PlayerInfo.Instance.GetCurrentTier(type) + 1) : Upgrades.upgrades[type].getPrice(0));
		if (PlayerInfo.Instance.amountOfCoins >= num)
		{
			switch (type)
			{
			case PowerupType.hoverboard:
			case PowerupType.headstart500:
			case PowerupType.headstart2000:
				PlayerInfo.Instance.IncreaseUpgradeAmount(type);
				Missions.Instance.PlayerDidThis(Missions.MissionTarget.SpendCoin, num);
				if (type == PowerupType.headstart2000)
				{
					Missions.Instance.PlayerDidThis(Missions.MissionTarget.HaveHeadStartLarge);
				}
				break;
			case PowerupType.jetpack:
			case PowerupType.supersneakers:
			case PowerupType.coinmagnet:
			case PowerupType.letters:
			case PowerupType.doubleMultiplier:
				PlayerInfo.Instance.IncreasePowerupTier(type);
				Missions.Instance.PlayerDidThis(Missions.MissionTarget.SpendCoin, num);
				Missions.Instance.PlayerDidThis(Missions.MissionTarget.HaveUpgrades);
				break;
			case PowerupType.mysterybox:
				Missions.Instance.PlayerDidThis(Missions.MissionTarget.SpendCoin, num);
				Missions.Instance.PlayerDidThis(Missions.MissionTarget.BuyMysterybox);
				break;
			case PowerupType.skipmission1:
				Missions.Instance.PlayerDidThis(Missions.MissionTarget.SpendCoin, num);
				break;
			case PowerupType.skipmission2:
				Missions.Instance.PlayerDidThis(Missions.MissionTarget.SpendCoin, num);
				break;
			case PowerupType.skipmission3:
				Missions.Instance.PlayerDidThis(Missions.MissionTarget.SpendCoin, num);
				break;
			}
			PlayerInfo.Instance.amountOfCoins -= num;
			sender.PurchaseSuccessful();
			Action onUpgradePurchase = _onUpgradePurchase;
			if (onUpgradePurchase != null)
			{
				onUpgradePurchase();
			}
			PlayerInfo.Instance.SaveIfDirty();
		}
		else
		{
			InAppManager.Instance.SetupNativePopup(num, Upgrades.upgrades[type].name);
			sender.PurchaseFailure();
		}
	}
}
