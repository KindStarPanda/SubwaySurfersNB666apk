using System.Runtime.InteropServices;
using UnityEngine;

public class InAppPurchaseHandler
{
	private static bool initializedForPurchase;

	private static bool initializedForProductRequest;

	private static bool initializedForRestore;

	private static string editorPurchaseGameObjectName;

	private static string editorProductRequestGameObjectName;

	private static string editorOnPurchaseSuccessMethodName;

	private static string editorRestoreGameObjectName;

	private static string editorRestoreRequestGameObjectName;

	private static string editorOnRestoreSuccessMethodName;

	private static string editorOnProductRequestSuccessMethodName;

	public static bool isInitializedForPurchase()
	{
		return initializedForPurchase;
	}

	public static bool isInitializedForProductRequest()
	{
		return initializedForProductRequest;
	}

	public static bool isInitializedForRestore()
	{
		return initializedForRestore;
	}

	public static string parseProductIdFromCallbackString(string transactionAndProductId)
	{
		return transactionAndProductId.Split(',')[1];
	}

	[DllImport("__Internal")]
	private static extern bool purchaseHandlerCanMakePayments();

	public static bool canMakePayments()
	{
		return true;
	}

	[DllImport("__Internal")]
	private static extern void purchaseHandlerInitPurchase(string gameobjectName, string onSuccessMethodName, string onFailureMethodName);

	public static void initPurchase(string gameobjectName, string onSuccessMethodName, string onFailureMethodName)
	{
		if (initializedForPurchase)
		{
			Debug.LogError("PurchaseHandler already initialized for purchase");
			return;
		}
		editorPurchaseGameObjectName = gameobjectName;
		editorOnPurchaseSuccessMethodName = onSuccessMethodName;
		Debug.Log("Init for purchase");
		initializedForPurchase = true;
	}

	[DllImport("__Internal")]
	private static extern void purchaseHandlerResetForPurchase();

	public static void resetForPurchase()
	{
		editorPurchaseGameObjectName = null;
		editorOnPurchaseSuccessMethodName = null;
		initializedForPurchase = false;
	}

	[DllImport("__Internal")]
	private static extern void purchaseHandlerInitProductRequest(string gameobjectName, string onSuccessMethodName, string onFailureMethodName);

	public static void initProductRequest(string gameobjectName, string onSuccessMethodName, string onFailureMethodName)
	{
		if (initializedForProductRequest)
		{
			Debug.LogError("PurchaseHandler already initialized for purchase");
			return;
		}
		editorProductRequestGameObjectName = gameobjectName;
		editorOnProductRequestSuccessMethodName = onSuccessMethodName;
		initializedForProductRequest = true;
	}

	[DllImport("__Internal")]
	private static extern void purchaseHandlerResetForProductRequest();

	public static void resetForProductRequest()
	{
		editorProductRequestGameObjectName = null;
		editorOnProductRequestSuccessMethodName = null;
		initializedForProductRequest = false;
	}

	[DllImport("__Internal")]
	private static extern void purchaseHandlerInitRestore(string gameobjectName, string onSuccessMethodName, string onFailureMethodName);

	public static void initRestore(string gameobjectName, string onSuccessMethodName, string onFailureMethodName)
	{
		if (initializedForRestore)
		{
			Debug.LogError("PurchaseHandler already initialized for restore");
			return;
		}
		editorRestoreGameObjectName = gameobjectName;
		editorOnRestoreSuccessMethodName = onSuccessMethodName;
		initializedForRestore = true;
	}

	[DllImport("__Internal")]
	private static extern void purchaseHandlerResetForRestore();

	public static void resetForRestore()
	{
		editorRestoreGameObjectName = null;
		editorOnRestoreSuccessMethodName = null;
		initializedForRestore = false;
	}

	[DllImport("__Internal")]
	private static extern void purchaseHandlerStartPurchase(string productIdentifier);

	public static void startPurchase(string productIdentifier)
	{
		Debug.Log("PurchaseHandler.startPurchase(" + productIdentifier + ")");
		if (!initializedForPurchase)
		{
			Debug.LogError("PurchaseHandler not initialized for purchase");
		}
		else
		{
			GameObject.Find(editorPurchaseGameObjectName).SendMessage(editorOnPurchaseSuccessMethodName, "," + productIdentifier);
		}
	}

	[DllImport("__Internal")]
	private static extern void purchaseHandlerCallbackHasBeenHandled(string transactionAndProductId);

	public static void callbackHasBeenHandled(string transactionAndProductIdentifier)
	{
		if (!initializedForPurchase)
		{
			Debug.LogError("PurchaseHandler not initialized for purchase");
		}
	}

	[DllImport("__Internal")]
	private static extern void purchaseHandlerRestoreProducts();

	public static void startRestore()
	{
		if (!initializedForRestore)
		{
			Debug.LogError("PurchaseHandler not initialized for restore");
			return;
		}
		string text = "com.kiloo.subwaysurfers.doublecoins";
		GameObject.Find(editorRestoreGameObjectName).SendMessage("RestoreSuccess", "debug," + text);
		GameObject.Find(editorRestoreGameObjectName).SendMessage("RestoreFinished", string.Empty);
	}

	[DllImport("__Internal")]
	private static extern void purchaseHandlerQueryProducts(string productIds);

	public static void queryProducts(string productIds)
	{
		if (!initializedForProductRequest)
		{
			Debug.LogError("PurchaseHandler not initialized for product request");
			return;
		}
		string[] array = productIds.Split(',');
		string text = string.Empty;
		for (int i = 0; i < array.Length; i++)
		{
			if (i > 0)
			{
				text += ";";
			}
			text = text + array[i] + ";0,99GBP";
		}
		GameObject.Find(editorProductRequestGameObjectName).SendMessage(editorOnProductRequestSuccessMethodName, text);
	}
}
