using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InAppManager : MonoBehaviour
{
	private enum InAppPurchaseState
	{
		NotStarted = 0,
		Started = 1,
		Failed = 2,
		Complete = 3
	}

	private const string GAME_OBJECT_NAME = "InAppManager";

	private static InAppManager _instance;

	private static InAppData inAppData;

	private Action onRestoreFinishDelegate;

	[NonSerialized]
	public AudioClip purchaseSuccessSound;

	private InAppPurchaseState _inAppPurchaseState;

	[HideInInspector]
	public bool productRequestSucceeded;

	public Action onPurchaseSuccess;

	public Action onProductRequestSuccess;

	private string inAppPurchaseKey = string.Empty;

	private string itemRequested = string.Empty;

	public static InAppManager Instance
	{
		get
		{
			Init();
			return _instance;
		}
	}

	public static bool IsInstanced()
	{
		return _instance != null;
	}

	public static void Init()
	{
		if (_instance == null)
		{
			Debug.Log("InAppManager init()");
			GameObject gameObject = new GameObject();
			gameObject.name = "InAppManager";
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			gameObject.AddComponent<InAppManager>();
		}
	}

	private void Awake()
	{
		_instance = this;
	}

	private void Start()
	{
		productRequestSucceeded = RRInappBillingPluginKit.InitInAppBillingSupport();
		inAppData = new InAppData();
		StartCoroutine(RestoreManagedAppPurchases());
	}

	public void QueryInApps()
	{
	}

	public void BuyInApp(string purchaseId)
	{
		StartPurchase(purchaseId);
	}

	public void BuyFromPopup(string purchaseId)
	{
		StartPurchase(purchaseId);
	}

	private void StartPurchase(string inAppPurchaseId)
	{
		StartPurchaseAndroid(inAppPurchaseId);
	}

	private void LogFlurry(string productId)
	{
		Flurry.LogEventWithAParameter("InApp purchase completed", "Id", productId);
		switch (productId)
		{
		case "com.kiloo.subways.coinstier1":
		case "com.kiloo.subways.coinstier1discount":
			if (string.IsNullOrEmpty(itemRequested))
			{
				Flurry.LogEventWithAParameter("InApp Coin Pack 1 purchased", "Mission Set", PlayerInfo.Instance.currentMissionSet.ToString());
			}
			else
			{
				Flurry.LogEventWithSeveralParameters("InApp Coin Pack 1 purchased", "Mission Set;Item tiggered iap", PlayerInfo.Instance.currentMissionSet + ";" + itemRequested);
			}
			break;
		case "com.kiloo.subways.coinstier2":
		case "com.kiloo.subways.coinstier2_discount":
			if (string.IsNullOrEmpty(itemRequested))
			{
				Flurry.LogEventWithAParameter("InApp Coin Pack 2 purchased", "Mission Set", PlayerInfo.Instance.currentMissionSet.ToString());
			}
			else
			{
				Flurry.LogEventWithSeveralParameters("InApp Coin Pack 2 purchased", "Mission Set;Item tiggered iap", PlayerInfo.Instance.currentMissionSet + ";" + itemRequested);
			}
			break;
		case "com.kiloo.subways.coinstier3":
		case "com.kiloo.subways.coinstier3discount":
			if (string.IsNullOrEmpty(itemRequested))
			{
				Flurry.LogEventWithAParameter("InApp Coin Pack 3 purchased", "Mission Set", PlayerInfo.Instance.currentMissionSet.ToString());
			}
			else
			{
				Flurry.LogEventWithSeveralParameters("InApp Coin Pack 3 purchased", "Mission Set;Item tiggered iap", PlayerInfo.Instance.currentMissionSet + ";" + itemRequested);
			}
			break;
		case "com.kiloo.subways.coinstier4":
		case "com.kiloo.subways.coinstier4discount":
			if (string.IsNullOrEmpty(itemRequested))
			{
				Flurry.LogEventWithAParameter("InApp Coin Pack 4 purchased", "Mission Set", PlayerInfo.Instance.currentMissionSet.ToString());
			}
			else
			{
				Flurry.LogEventWithSeveralParameters("InApp Coin Pack 4 purchased", "Mission Set;Item tiggered iap", PlayerInfo.Instance.currentMissionSet + ";" + itemRequested);
			}
			break;
		case "com.kiloo.subways.coinstier5":
		case "com.kiloo.subways.coinstier5discount":
			if (string.IsNullOrEmpty(itemRequested))
			{
				Flurry.LogEventWithAParameter("InApp Coin Pack 5 purchased", "Mission Set", PlayerInfo.Instance.currentMissionSet.ToString());
			}
			else
			{
				Flurry.LogEventWithSeveralParameters("InApp Coin Pack 5 purchased", "Mission Set;Item tiggered iap", PlayerInfo.Instance.currentMissionSet + ";" + itemRequested);
			}
			break;
		case "com.kiloo.subways.doublecoins":
		case "com.kiloo.subways.doublecoinsdiscount":
			if (UIScreenController.isInstanced)
			{
				if (string.IsNullOrEmpty(UIScreenController.Instance.GetCurrentPopupName()))
				{
					Flurry.LogEventWithAParameter("Double Coin purchased", "Mission Set", PlayerInfo.Instance.currentMissionSet.ToString());
				}
				else if (UIScreenController.Instance.GetCurrentPopupName() == "TutorialDoubleCoinsPopup")
				{
					Flurry.LogEventWithAParameter("Double Coin purchased GameOver", "Mission Set", PlayerInfo.Instance.currentMissionSet.ToString());
				}
				else
				{
					Flurry.LogEventWithAParameter("Double Coin purchased", "Mission Set", PlayerInfo.Instance.currentMissionSet.ToString());
				}
			}
			else
			{
				Flurry.LogEventWithAParameter("Double Coin purchased", "Mission Set", PlayerInfo.Instance.currentMissionSet.ToString());
			}
			break;
		}
	}

	public void SetupNativePopup(int cost, string senderName)
	{
		itemRequested = senderName;
		int num = 0;
		num = cost - PlayerInfo.Instance.amountOfCoins;
		string text = string.Empty;
		if (Instance.productRequestSucceeded)
		{
			foreach (KeyValuePair<string, InAppProfile> inAppDatum in InAppData.inAppData)
			{
				if (!inAppDatum.Value.validInApp || inAppDatum.Value.amountOfCoins <= num)
				{
					continue;
				}
				if (!string.IsNullOrEmpty(text))
				{
					if (InAppData.inAppData[text].amountOfCoins > InAppData.inAppData[inAppDatum.Key].amountOfCoins)
					{
						text = inAppDatum.Key;
					}
				}
				else
				{
					text = inAppDatum.Key;
				}
			}
		}
		inAppPurchaseKey = text;
		string title = "Not enough coins!";
		if (!string.IsNullOrEmpty(inAppPurchaseKey))
		{
			string message = string.Format("You need {0} more Coins to complete your purchase. Buy {1} Coins?", num, InAppData.inAppData[text].amountOfCoins);
			DeviceUtility.showNativePopupWithCallback("InAppManager", "NativePurchaseInappPack", title, message, "Cancel", "Buy", null);
		}
		else
		{
			string message2 = string.Format("You need {0} more Coins to complete your purchase. Buy more in the store", num);
			DeviceUtility.showNativePopup(title, message2, "Ok");
		}
	}

	public void NativePurchaseInappPack(string message)
	{
		if (message == "0")
		{
			inAppPurchaseKey = string.Empty;
		}
		else if (Instance.productRequestSucceeded)
		{
			Instance.BuyFromPopup(inAppPurchaseKey);
		}
		else
		{
			inAppPurchaseKey = string.Empty;
		}
	}

	public void PurchaseStatePurchased(string transactionAndProductId)
	{
		string text = transactionAndProductId.Split(',')[0];
		string text2 = transactionAndProductId.Split(',')[1];
		Debug.Log(string.Format("InAppManager PurchaseStatePurchased: {0} orderId {1}", text2, text));
		if (text == "debug")
		{
			if (InAppData.LIST_OF_MANAGED_INAPP_IDS.Contains(text2))
			{
				if (text2 == "com.kiloo.subways.doublecoins" || text2 == "com.kiloo.subways.doublecoinsdiscount")
				{
					PlayerInfo.Instance.hasDoubleCoins = true;
				}
			}
			else
			{
				PlayerInfo.Instance.amountOfCoins += InAppData.inAppData[text2].amountOfCoins;
				PlayerInfo.Instance.SaveIfDirty();
			}
			return;
		}
		if (InAppData.LIST_OF_MANAGED_INAPP_IDS.Contains(text2))
		{
			if (text2 == "com.kiloo.subways.doublecoins" || text2 == "com.kiloo.subways.doublecoinsdiscount")
			{
				PlayerInfo.Instance.hasDoubleCoins = true;
				if (UIScreenController.isInstanced)
				{
					UIScreenController.Instance.QueueSlideIn(UIScreenController.SlideInType.Unlock, "Double Coins");
				}
			}
			if (PlayerInfo.Instance.AddTransactionToHistory(text, text2))
			{
				PlayerInfo.Instance.inAppPurchaseCount++;
				Action action = onPurchaseSuccess;
				if (action != null)
				{
					action();
				}
				LogFlurry(text2);
			}
		}
		else if (PlayerInfo.Instance.AddTransactionToHistory(text, text2))
		{
			PlayerInfo.Instance.inAppPurchaseCount++;
			for (int i = 0; i < InAppData.inAppTiersAndInAppTiersDiscount.Length; i++)
			{
				if (InAppData.inAppTiersAndInAppTiersDiscount[i] == text2)
				{
					if (i < InAppData.inAppTiersAndInAppTiersDiscount.Length / 2)
					{
						PlayerInfo.Instance.amountOfCoins += InAppData.inAppData[text2].amountOfCoins;
						continue;
					}
					string key = "in_app_tier_" + (i - InAppData.inAppTiersAndInAppTiersDiscount.Length / 2 + 1);
					PlayerInfo.Instance.amountOfCoins += InAppData.inAppData[text2].amountOfCoins + Mathf.Clamp(OnlineSettings.instance.GetValue(key, 0), 0, int.MaxValue);
				}
			}
			Action action2 = onPurchaseSuccess;
			if (action2 != null)
			{
				action2();
			}
			LogFlurry(text2);
			string[] inAppTiersAndInAppTiersDiscount = InAppData.inAppTiersAndInAppTiersDiscount;
			foreach (string text3 in inAppTiersAndInAppTiersDiscount)
			{
				if (text3 == text2)
				{
					NGUITools.PlaySound(purchaseSuccessSound);
				}
			}
		}
		PlayerInfo.Instance.SaveIfDirty();
		if (UIScreenController.isInstanced)
		{
			UIScreenController.Instance.HideInAppPurchaseOverlay();
		}
		itemRequested = string.Empty;
		Screen.sleepTimeout = -2;
	}

	public void PurchaseStateCanceled(string transactionAndProductId)
	{
		if (UIScreenController.isInstanced)
		{
			UIScreenController.Instance.HideInAppPurchaseOverlay();
		}
		Screen.sleepTimeout = -2;
		Debug.Log(string.Format("InAppManager PurchaseStateCanceled: {0}", transactionAndProductId));
	}

	public void PurchaseStateRefunded(string transactionAndProductId)
	{
		string text = transactionAndProductId.Split(',')[0];
		string text2 = transactionAndProductId.Split(',')[1];
		Debug.Log(string.Format("InAppManager PurchaseStateRefunded: {0} orderId {1}", text2, text));
		if (PlayerInfo.Instance.RemoveTransactionFromHistory(text))
		{
			PlayerInfo.Instance.inAppPurchaseCount--;
			Debug.Log("InAppManager Transaction with orderId: " + text + " is deleted from history!");
		}
		if (InAppData.LIST_OF_MANAGED_INAPP_IDS.Contains(text2) && (text2 == "com.kiloo.subways.doublecoins" || text2 == "com.kiloo.subways.doublecoinsdiscount") && !PlayerInfo.Instance.HasTransactionHistoryItemId("com.kiloo.subways.doublecoins") && !PlayerInfo.Instance.HasTransactionHistoryItemId("com.kiloo.subways.doublecoinsdiscount"))
		{
			Debug.Log("InAppManager No history of double coins. Set double coins to false");
			PlayerInfo.Instance.hasDoubleCoins = false;
		}
		if (UIScreenController.isInstanced)
		{
			UIScreenController.Instance.HideInAppPurchaseOverlay();
		}
		Screen.sleepTimeout = -2;
	}

	public void RequestSuccessful(string productId)
	{
		Debug.Log(string.Format("InAppManager RequestSuccessful for: {0}", productId));
		if (UIScreenController.isInstanced)
		{
			UIScreenController.Instance.HideInAppPurchaseOverlay();
		}
		Screen.sleepTimeout = -2;
	}

	public void RequestFailed(string error)
	{
		Debug.Log("InAppManager RequestFailed: " + error);
		if (UIScreenController.isInstanced)
		{
			UIScreenController.Instance.HideInAppPurchaseOverlay();
		}
		Screen.sleepTimeout = -2;
	}

	public void RequestCancelled(string error)
	{
		Debug.Log("InAppManager RequestCancelled: " + error);
		if (UIScreenController.isInstanced)
		{
			UIScreenController.Instance.HideInAppPurchaseOverlay();
		}
		Screen.sleepTimeout = -2;
	}

	public void ConfirmationFail(string productId)
	{
		Debug.Log("InAppManager ConfirmationFail: " + productId);
	}

	private void StartPurchaseAndroid(string inAppPurchaseId)
	{
		if (RRInappBillingPluginKit.InitInAppBillingSupport())
		{
			Screen.sleepTimeout = -1;
			UIScreenController.Instance.ShowInAppPurchaseOverlay();
			if (!RRInappBillingPluginKit.BuyProduct(inAppPurchaseId))
			{
				UIScreenController.Instance.HideInAppPurchaseOverlay();
				EtceteraAndroid.showAlert("Alert", "There was an error while trying to connect to the server. Please try again later!", "Ok");
				Screen.sleepTimeout = -2;
			}
		}
	}

	private IEnumerator RestoreManagedAppPurchases()
	{
		if (!RestoreTransactions())
		{
			Debug.Log("InAppManager Restore Waiting for 5 seconds.");
			yield return new WaitForSeconds(5f);
			StartCoroutine(RestoreManagedAppPurchases());
		}
	}

	private static bool RestoreTransactions()
	{
		if (!PlayerPrefs.HasKey("inapp_restore_transactions"))
		{
			if (RRInappBillingPluginKit.RestoreTransactions())
			{
				Debug.Log("InAppManager Restore transactions succeded...");
				PlayerPrefs.SetInt("inapp_restore_transactions", 1);
				return true;
			}
			return false;
		}
		return true;
	}

	public void StartRestoreAndroid(Action onFinishDelegate)
	{
		onRestoreFinishDelegate = onFinishDelegate;
		if (RRInappBillingPluginKit.InitInAppBillingSupport())
		{
			Screen.sleepTimeout = -1;
			UIScreenController.Instance.ShowInAppPurchaseOverlay();
			if (!RRInappBillingPluginKit.RestoreTransactions())
			{
				UIScreenController.Instance.HideInAppPurchaseOverlay();
				EtceteraAndroid.showAlert("Alert", "There was an error while trying to connect to the server. Please try again later!", "Ok");
				Screen.sleepTimeout = -2;
			}
		}
	}

	public void RestoreFailure(string responseCode)
	{
		Debug.Log("InAppManager Restore Failure");
		Screen.sleepTimeout = -2;
		if (UIScreenController.isInstanced)
		{
			UIScreenController.Instance.HideInAppPurchaseOverlay();
			UIScreenController.Instance.QueueErrorMessageSlidein(Strings.Get(StringID.SLIDEIN_RESTORE_NETWORK_PROBLEM));
		}
		onRestoreFinishDelegate = null;
	}

	public void RestoreFinished(string responseCode)
	{
		Debug.Log("InAppManager Restore Finished");
		Screen.sleepTimeout = -2;
		if (UIScreenController.isInstanced)
		{
			UIScreenController.Instance.HideInAppPurchaseOverlay();
			if (!PlayerInfo.Instance.hasDoubleCoins)
			{
				UIScreenController.Instance.QueueErrorMessageSlidein(Strings.Get(StringID.SLIDEIN_RESTORE_NO_PREVIOUS_PURCHASES));
			}
			if (onRestoreFinishDelegate != null)
			{
				onRestoreFinishDelegate();
				onRestoreFinishDelegate = null;
			}
		}
	}

	private void OnEnable()
	{
		RRInappBillingCallback.OnRequestSuccessful += RequestSuccessful;
		RRInappBillingCallback.OnRequestFailed += RequestFailed;
		RRInappBillingCallback.OnRequestCancelled += RequestCancelled;
		RRInappBillingCallback.OnPurchaseStatePurchased += PurchaseStatePurchased;
		RRInappBillingCallback.OnPurchaseStateCanceled += PurchaseStateCanceled;
		RRInappBillingCallback.OnPurchaseStateRefunded += PurchaseStateRefunded;
		RRInappBillingCallback.OnConfirmationFail += ConfirmationFail;
		RRInappBillingCallback.OnFinishedRestore += RestoreFinished;
		RRInappBillingCallback.OnFailureRestore += RestoreFailure;
	}

	private void OnDisable()
	{
		RRInappBillingCallback.OnRequestSuccessful -= RequestSuccessful;
		RRInappBillingCallback.OnRequestFailed -= RequestFailed;
		RRInappBillingCallback.OnRequestCancelled -= RequestCancelled;
		RRInappBillingCallback.OnPurchaseStatePurchased -= PurchaseStatePurchased;
		RRInappBillingCallback.OnPurchaseStateCanceled -= PurchaseStateCanceled;
		RRInappBillingCallback.OnPurchaseStateRefunded -= PurchaseStateRefunded;
		RRInappBillingCallback.OnConfirmationFail -= ConfirmationFail;
		RRInappBillingCallback.OnFinishedRestore -= RestoreFinished;
		RRInappBillingCallback.OnFailureRestore -= RestoreFailure;
	}
}
