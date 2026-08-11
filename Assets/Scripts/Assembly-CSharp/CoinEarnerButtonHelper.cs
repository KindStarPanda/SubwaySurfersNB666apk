using System;
using UnityEngine;

public class CoinEarnerButtonHelper : MonoBehaviour
{
	public UISprite icon;

	public UILabel title;

	public UILabel description;

	private int earnCurrencyProfileIndex;

	private Action _onClickDelegate;

	public void Init(int earnCurrencyProfileIndex, string title, string desc, string iconName, Action onClickDelegate)
	{
		this.earnCurrencyProfileIndex = earnCurrencyProfileIndex;
		this.title.text = title;
		_onClickDelegate = onClickDelegate;
		icon.spriteName = iconName;
		description.text = desc;
		icon.MakePixelPerfect();
	}

	private void OnClick()
	{
		EarnCurrencyInfo.Trigger(earnCurrencyProfileIndex);
		if (_onClickDelegate != null)
		{
			_onClickDelegate();
		}
	}
}
