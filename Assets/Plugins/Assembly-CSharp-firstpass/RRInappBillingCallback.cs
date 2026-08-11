using System.Runtime.CompilerServices;
using UnityEngine;

public class RRInappBillingCallback : MonoBehaviour
{
	public delegate void ProductPurchasedEventHandler(string productIdentifier);

	public delegate void StoreKitErrorEventHandler(string error);

	public delegate void PurchaseStateChangeEventHandler(string itemId);

	public delegate void ConfirmationFailedEventHandler(string productIdentifier);

	public delegate void RestoreEventHandler(string info);

	[method: MethodImpl(32)]
	public static event PurchaseStateChangeEventHandler OnPurchaseStatePurchased;

	[method: MethodImpl(32)]
	public static event PurchaseStateChangeEventHandler OnPurchaseStateCanceled;

	[method: MethodImpl(32)]
	public static event PurchaseStateChangeEventHandler OnPurchaseStateRefunded;

	[method: MethodImpl(32)]
	public static event ProductPurchasedEventHandler OnRequestSuccessful;

	[method: MethodImpl(32)]
	public static event StoreKitErrorEventHandler OnRequestFailed;

	[method: MethodImpl(32)]
	public static event StoreKitErrorEventHandler OnRequestCancelled;

	[method: MethodImpl(32)]
	public static event ConfirmationFailedEventHandler OnConfirmationFail;

	[method: MethodImpl(32)]
	public static event RestoreEventHandler OnFinishedRestore;

	[method: MethodImpl(32)]
	public static event RestoreEventHandler OnFailureRestore;

	private void Awake()
	{
		base.gameObject.name = GetType().ToString();
		Object.DontDestroyOnLoad(base.gameObject);
	}

	public void PurchaseStatePurchased(string itemId)
	{
		if (RRInappBillingCallback.OnPurchaseStatePurchased != null)
		{
			RRInappBillingCallback.OnPurchaseStatePurchased(itemId);
		}
	}

	public void PurchaseStateCanceled(string itemId)
	{
		if (RRInappBillingCallback.OnPurchaseStateCanceled != null)
		{
			RRInappBillingCallback.OnPurchaseStateCanceled(itemId);
		}
	}

	public void PurchaseStateRefunded(string itemId)
	{
		if (RRInappBillingCallback.OnPurchaseStateRefunded != null)
		{
			RRInappBillingCallback.OnPurchaseStateRefunded(itemId);
		}
	}

	public void RequestProductPurchased(string productIdentifier)
	{
		if (RRInappBillingCallback.OnRequestSuccessful != null)
		{
			RRInappBillingCallback.OnRequestSuccessful(productIdentifier);
		}
	}

	public void RequestProductCancelled(string error)
	{
		if (RRInappBillingCallback.OnRequestCancelled != null)
		{
			RRInappBillingCallback.OnRequestCancelled(error);
		}
	}

	public void RequestProductFailed(string error)
	{
		if (RRInappBillingCallback.OnRequestFailed != null)
		{
			RRInappBillingCallback.OnRequestFailed(error);
		}
	}

	public void ConfirmationFailed(string productIdentifier)
	{
		if (RRInappBillingCallback.OnConfirmationFail != null)
		{
			RRInappBillingCallback.OnConfirmationFail(productIdentifier);
		}
	}

	public void RestoreFinished(string info)
	{
		if (RRInappBillingCallback.OnFinishedRestore != null)
		{
			RRInappBillingCallback.OnFinishedRestore(info);
		}
	}

	public void RestoreFailure(string info)
	{
		if (RRInappBillingCallback.OnFailureRestore != null)
		{
			RRInappBillingCallback.OnFailureRestore(info);
		}
	}
}
