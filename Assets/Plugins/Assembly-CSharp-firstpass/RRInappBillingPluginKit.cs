using System;
using UnityEngine;

public class RRInappBillingPluginKit : IDisposable
{
	void IDisposable.Dispose()
	{
	}

	public static bool InitInAppBillingSupport()
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass(BillingCONST.PLUGIN_PKG + ".InAppBillingManager");
		return androidJavaClass.CallStatic<bool>("IsBillingSupported", new object[0]);
	}

	public static bool BuyProduct(string productId)
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass(BillingCONST.PLUGIN_PKG + ".InAppBillingManager");
		bool flag = androidJavaClass.CallStatic<bool>("BuyItem", new object[1] { productId });
		Debug.Log(string.Format("In app billing response: {0}", flag));
		return flag;
	}

	public static bool RestoreTransactions()
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass(BillingCONST.PLUGIN_PKG + ".InAppBillingManager");
		bool flag = androidJavaClass.CallStatic<bool>("RestoreTransactions", new object[0]);
		Debug.Log(string.Format("Restore transactions response: {0}", flag));
		return flag;
	}

	public static void StopInAppBillingSupport()
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass(BillingCONST.PLUGIN_PKG + ".InAppBillingManager");
		androidJavaClass.CallStatic("Stop");
	}

	public static void StopUnityActivity()
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass(BillingCONST.PLUGIN_PKG + ".RRAndroidPluginActivity");
		androidJavaClass.CallStatic("CloseApplication");
	}
}
