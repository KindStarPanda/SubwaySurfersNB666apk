using System;
using System.Collections;
using UnityEngine;

public class EarnCoinsScreen : UIScreen
{
	public GameObject WatchVideoPrefab;

	public GameObject coinEarnerPrefab;

	public UIFont headerFont;

	[SerializeField]
	private UITable _table;

	[SerializeField]
	private UIDraggablePanel _dragPanel;

	private bool firstRun = true;

	[NonSerialized]
	public int counterForElementsOnDisplay;

	public GameObject FooterPrefab;

	private UIFooterHandler _footerHandler;

	public int selectedButton;

	private GameObject go;

	public override void Init()
	{
		base.Init();
		FillTable();
		InitFooter();
	}

	public override void Show()
	{
		base.Show();
		_footerHandler.handleStickers();
		RefreshCurrencyEarners();
	}

	public void RefreshCurrencyEarners()
	{
		FillTable();
	}

	private void FillTable()
	{
		counterForElementsOnDisplay = 0;
		base.transform.parent.GetComponent<UIPanel>().widgetsAreStatic = false;
		foreach (Transform item in _table.transform)
		{
			NGUITools.SetActive(item.gameObject, false);
			UnityEngine.Object.Destroy(item.gameObject);
		}
		int num = 0;
		UILabel uILabel = NGUITools.AddWidget<UILabel>(_table.gameObject);
		uILabel.font = headerFont;
		uILabel.text = "Earn Coins";
		uILabel.color = new Color(0f, 0.2901961f, 0.5019608f, 1f);
		uILabel.name = string.Format("{0:000}", num);
		uILabel.MakePixelPerfect();
		if (DeviceInfo.isHighres)
		{
			uILabel.gameObject.transform.localScale = new Vector3(uILabel.gameObject.transform.localScale.x / 2f, uILabel.gameObject.transform.localScale.y / 2f, uILabel.gameObject.transform.localScale.z);
		}
		num++;
		for (int i = 0; i < EarnCurrencyInfo.profiles.Length; i++)
		{
			if (EarnCurrencyInfo.ShouldShowInGUI(i))
			{
				EarnCurrencyInfo.EarnCurrencyProfile earnCurrencyProfile = EarnCurrencyInfo.profiles[i];
				go = NGUITools.AddChild(_table.gameObject, coinEarnerPrefab);
				go.name = string.Format("{0:000}", num);
				string desc = string.Format(earnCurrencyProfile.desc, earnCurrencyProfile.GetAmountOfCoins());
				go.GetComponent<CoinEarnerButtonHelper>().Init(i, earnCurrencyProfile.title, desc, earnCurrencyProfile.iconName, RefreshCurrencyEarners);
				go.GetComponent<UIDragPanelContents>().draggablePanel = _dragPanel;
				NGUITools.AddWidgetCollider(go);
				num++;
			}
		}
		_table.gameObject.BroadcastMessage("CreatePanel", SendMessageOptions.DontRequireReceiver);
		if (_table.gameObject.active)
		{
			_table.Reposition();
			if (!firstRun)
			{
				_dragPanel.RestrictWithinBounds(true);
			}
			else
			{
				firstRun = false;
			}
			if (UIScreen.IsScreenHeightOutOfProportion())
			{
				_dragPanel.SendMessage("Update", SendMessageOptions.DontRequireReceiver);
				_dragPanel.SendMessage("LateUpdate", SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	private IEnumerator SetStatic()
	{
		yield return null;
		base.transform.parent.GetComponent<UIPanel>().widgetsAreStatic = true;
	}

	private void InitFooter()
	{
		GameObject gameObject = NGUITools.AddChild(base.gameObject, FooterPrefab);
		_footerHandler = gameObject.GetComponent<UIFooterHandler>();
		Color cOLOR_FOR_SELECTED_FOOTER = GlobalColors.COLOR_FOR_SELECTED_FOOTER;
		switch (selectedButton)
		{
		case 1:
			UnityEngine.Object.Destroy(_footerHandler.Button1.GetComponent<BoxCollider>());
			_footerHandler.Fill1.color = cOLOR_FOR_SELECTED_FOOTER;
			break;
		case 2:
			UnityEngine.Object.Destroy(_footerHandler.Button2.GetComponent<BoxCollider>());
			_footerHandler.Fill2.color = cOLOR_FOR_SELECTED_FOOTER;
			break;
		case 3:
			UnityEngine.Object.Destroy(_footerHandler.Button3.GetComponent<BoxCollider>());
			_footerHandler.Fill3.color = cOLOR_FOR_SELECTED_FOOTER;
			break;
		default:
			Debug.Log("No button was selected in the footer?", this);
			break;
		}
	}
}
