using System;
using UnityEngine;

public class RestorePurchaseHelper : MonoBehaviour
{
	private Action _onFinishDelegate;

	public void setOnClickDelegate(Action onFinishDelegate)
	{
		_onFinishDelegate = onFinishDelegate;
	}

	private void OnClick()
	{
		InAppManager.Instance.StartRestoreAndroid(_onFinishDelegate);
	}
}
