using System;
using System.Collections;
using UnityEngine;

public class GameOverScreen : UIScreen
{
	[SerializeField]
	private UILabel scoreLabel;

	[SerializeField]
	private UILabel collectedCoinLabel;

	[SerializeField]
	private UILabel coinboxLabel;

	[SerializeField]
	private UISprite doubleCoinSprite;

	[SerializeField]
	private UILabel gettingReadyLabel;

	[SerializeField]
	private CoinBoxSizer coinBoxSizer;

	private Friend[] _friends;

	private int scoreFrom;

	private int scoreTo;

	private int coinboxFrom;

	private int coinboxTo;

	private int collectedCoinsFrom;

	private int collectedCoinsTo;

	private bool countingUpCoins;

	private ScoreCounterSoundPlayer scoreCounterSoundPlayer;

	[SerializeField]
	private FriendHandlerBrag friendHandler;

	[SerializeField]
	private GameObject newUpgradesIcon;

	[SerializeField]
	private UILabel newUpgradesText;

	[SerializeField]
	private GameObject discountSticker;

	private Color darkBlue = new Color(0.043137256f, 14f / 85f, 0.3254902f);

	private Color yellow = new Color(1f, 0.8745098f, 4f / 85f);

	private bool hasBeenSetupAfterAGame;

	private bool cleanUpDone;

	public override void Init()
	{
		base.Init();
		scoreCounterSoundPlayer = GetComponent<ScoreCounterSoundPlayer>();
	}

	public override void Show()
	{
		base.Show();
		PlayerInfo instance = PlayerInfo.Instance;
		instance.onCoinsChanged = (Action)Delegate.Combine(instance.onCoinsChanged, new Action(OnCoinsChanged));
		Missions instance2 = Missions.Instance;
		instance2.onMissionComplete = (Missions.MissionCompleteHandler)Delegate.Combine(instance2.onMissionComplete, new Missions.MissionCompleteHandler(OnMissionComplete));
		SocialManager.instance.AddFriendsConsolidatedHandler(ReloadFriends);
		InAppManager instance3 = InAppManager.Instance;
		instance3.onPurchaseSuccess = (Action)Delegate.Combine(instance3.onPurchaseSuccess, new Action(UpdateUpgradeSticker));
		InAppManager instance4 = InAppManager.Instance;
		instance4.onPurchaseSuccess = (Action)Delegate.Combine(instance4.onPurchaseSuccess, new Action(UpdateDoubleCoinLabels));
		UIModelController.Instance.ActivateGameOverModel();
		UpdateUpgradeSticker();
	}

	public override void Hide()
	{
		base.Hide();
		PlayerInfo instance = PlayerInfo.Instance;
		instance.onCoinsChanged = (Action)Delegate.Remove(instance.onCoinsChanged, new Action(OnCoinsChanged));
		Missions instance2 = Missions.Instance;
		instance2.onMissionComplete = (Missions.MissionCompleteHandler)Delegate.Remove(instance2.onMissionComplete, new Missions.MissionCompleteHandler(OnMissionComplete));
		SocialManager.instance.RemoveFriendsConsolidatedHandler(ReloadFriends);
		InAppManager instance3 = InAppManager.Instance;
		instance3.onPurchaseSuccess = (Action)Delegate.Remove(instance3.onPurchaseSuccess, new Action(UpdateUpgradeSticker));
		InAppManager instance4 = InAppManager.Instance;
		instance4.onPurchaseSuccess = (Action)Delegate.Remove(instance4.onPurchaseSuccess, new Action(UpdateDoubleCoinLabels));
		UIModelController.Instance.ClearModels();
	}

	private void OnApplicationPause(bool pause)
	{
		if (!pause && !countingUpCoins)
		{
			ReloadFriends();
		}
	}

	public void SetupBeforeMysteryBox()
	{
		hasBeenSetupAfterAGame = true;
		scoreLabel.text = string.Empty + GameStats.Instance.score;
		scoreFrom = GameStats.Instance.score;
		if (false || Social.localUser.authenticated || SocialManager.instance.facebookIsLoggedIn)
		{
			friendHandler.ShowGettingReadyLabel();
		}
		else
		{
			friendHandler.GoOffline(false);
		}
	}

	public void SetupAfterMysteryBox()
	{
		if (hasBeenSetupAfterAGame)
		{
			coinBoxSizer.updateAutomatically = false;
			collectedCoinsFrom = GameStats.Instance.coins;
			collectedCoinsTo = 0;
			scoreTo = scoreFrom + GameStats.CoinToScoreConversion(collectedCoinsFrom);
			coinboxFrom = PlayerInfo.Instance.amountOfCoins;
			coinboxTo = coinboxFrom + GameStats.Instance.coins;
			PlayerInfo.Instance.amountOfCoins = coinboxTo;
			bool isNewHighscore = scoreTo > PlayerInfo.Instance.highestScore;
			if (PlayerInfo.Instance.highestScore != PlayerInfo.Instance.oldHighestScore)
			{
				PlayerInfo.Instance.SetOldestHighestScore();
			}
			PlayerInfo.Instance.highestScore = scoreTo;
			if (SocialManager.instance != null)
			{
				SocialManager.instance.ReportScore(PlayerInfo.Instance.highestScore, isNewHighscore, Mathf.Max(1, Mathf.RoundToInt(GameStats.Instance.meters)));
			}
			StartCoroutine("CountUpCoins");
			hasBeenSetupAfterAGame = false;
		}
	}

	private void UpdateUpgradeSticker()
	{
		int numberOfAffordableUpgrades = PlayerInfo.Instance.GetNumberOfAffordableUpgrades();
		newUpgradesIcon.active = false;
		newUpgradesText.gameObject.active = false;
		discountSticker.SetActiveRecursively(false);
		if (DiscountButton.DiscountDoubleCoins)
		{
			discountSticker.SetActiveRecursively(true);
		}
		else if (numberOfAffordableUpgrades > 1)
		{
			newUpgradesIcon.active = true;
			newUpgradesText.gameObject.active = true;
			newUpgradesText.text = numberOfAffordableUpgrades.ToString();
		}
	}

	private void OnCoinsChanged()
	{
		UpdateUpgradeSticker();
	}

	private void OnMissionComplete(string message)
	{
		UpdateUpgradeSticker();
	}

	private void ReloadFriends()
	{
		bool flag = true;
		StopCoroutine("CorouReloadFriends");
		StartCoroutine("CorouReloadFriends", flag);
	}

	private IEnumerator CorouReloadFriends(bool framedelay)
	{
		friendHandler.ClearEverything();
		if (false || Social.localUser.authenticated || SocialManager.instance.facebookIsLoggedIn)
		{
			if (framedelay)
			{
				float framestamp = Time.frameCount;
				while ((float)Time.frameCount < framestamp + 2f)
				{
					yield return null;
				}
			}
			friendHandler.ShowFriendList();
		}
		else
		{
			friendHandler.GoOffline(true);
		}
		yield return null;
	}

	private void CountUpCompleted()
	{
		PlayerInfo.Instance.SaveIfDirty();
		ReloadFriends();
	}

	public void FacebookLoggedIn()
	{
		ReloadFriends();
	}

	private void UpdateDoubleCoinLabels()
	{
		collectedCoinLabel.color = Color.white;
		doubleCoinSprite.color = darkBlue;
		if (PlayerInfo.Instance.hasDoubleCoins)
		{
			doubleCoinSprite.spriteName = "scoreboard_doublecoins_overlay";
			collectedCoinLabel.color = yellow;
			doubleCoinSprite.color = yellow;
			if (doubleCoinSprite.GetComponent<Collider>() != null)
			{
				UnityEngine.Object.Destroy(doubleCoinSprite.GetComponent<Collider>());
			}
		}
		else
		{
			doubleCoinSprite.spriteName = "scoreboard_doublecoins_overlay_i";
		}
	}

	private void OpenQuickUpgrade()
	{
		UIScreenController.Instance.QueuePopup("UpgradesUI_quick");
		StopCoroutine("CountUpCoins");
		scoreLabel.text = string.Empty + scoreTo;
		coinboxLabel.text = string.Empty + coinboxTo;
		collectedCoinLabel.text = string.Empty + collectedCoinsTo;
		coinBoxSizer.updateAutomatically = true;
		countingUpCoins = false;
		CountUpCompleted();
	}

	private IEnumerator CountUpCoins()
	{
		float countFactor2 = 0f;
		float countTime = Mathf.Lerp(0.3f, 2f, (float)collectedCoinsFrom / 100f);
		countingUpCoins = true;
		UpdateDoubleCoinLabels();
		if (PlayerInfo.Instance.hasDoubleCoins)
		{
			collectedCoinLabel.text = GameStats.Instance.coins + string.Empty;
		}
		else
		{
			collectedCoinLabel.text = string.Empty + GameStats.Instance.coins;
		}
		yield return new WaitForSeconds(1.2f);
		countFactor2 = 0f;
		while (countFactor2 < 1f)
		{
			scoreCounterSoundPlayer.PlayCoinSound(countFactor2);
			countFactor2 += Time.deltaTime / countTime;
			scoreLabel.text = string.Empty + Mathf.Round(Mathf.SmoothStep(scoreFrom, scoreTo, countFactor2));
			coinboxLabel.text = string.Empty + Mathf.RoundToInt(Mathf.SmoothStep(coinboxFrom, coinboxTo, countFactor2));
			collectedCoinLabel.text = string.Empty + Mathf.Round(Mathf.SmoothStep(collectedCoinsFrom, collectedCoinsTo, countFactor2));
			yield return null;
		}
		scoreCounterSoundPlayer.StopScoreSound();
		scoreLabel.text = string.Empty + scoreTo;
		coinboxLabel.text = string.Empty + coinboxTo;
		collectedCoinLabel.text = string.Empty + collectedCoinsTo;
		coinBoxSizer.updateAutomatically = true;
		countingUpCoins = false;
		CountUpCompleted();
	}

	private IEnumerator SplashLabelEffect(UILabel label, int expander)
	{
		label.gameObject.active = true;
		float countFactor = 0f;
		while (countFactor < 1f)
		{
			countFactor += Time.deltaTime;
			label.cachedTransform.localScale += new Vector3((float)expander * Time.deltaTime * 34f, (float)(34 * expander) * Time.deltaTime, 0f);
			label.alpha = 0.1f - countFactor * 0.1f;
			yield return null;
		}
	}
}
