using System;
using System.Collections;
using UnityEngine;

public class CoinScreen : UIScreen
{
	public GameObject coinPrefab;

	public GameObject coinEarnerPrefab;

	public GameObject restorePrefab;

	public UIFont headerFont;

	[SerializeField]
	private UITable _table;

	[SerializeField]
	private UIDraggablePanel _parentDragPanel;

	private bool firstRun = true;

	[SerializeField]
	private bool shouldShowEarners;

	[NonSerialized]
	public int counterForElementsOnDisplay;

	public GameObject FooterPrefab;

	private UIFooterHandler _footerHandler;

	public int selectedButton;

	public bool isOnScreenLayer = true;

	private GameObject go;

	public override void Init()
	{
		base.Init();
		FillTable();
		if (UIScreen.IsScreenHeightOutOfProportion())
		{
			Vector3 localPosition = _parentDragPanel.transform.parent.localPosition;
			localPosition.y = 0f;
			_parentDragPanel.transform.parent.localPosition = localPosition;
		}
		if (isOnScreenLayer)
		{
			InitFooter();
		}
	}

	public override void Show()
	{
		base.Show();
		if (isOnScreenLayer)
		{
			_footerHandler.handleStickers();
		}
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
		GameObject gameObject = NGUITools.AddChild(_table.gameObject);
		gameObject.name = string.Format("{0:000}", num);
		UILabel uILabel = NGUITools.AddWidget<UILabel>(_table.gameObject);
		uILabel.cachedTransform.parent = gameObject.transform;
		uILabel.font = headerFont;
		uILabel.text = "Coin Shop";
		uILabel.color = new Color(0f, 0.2901961f, 0.5019608f, 1f);
		uILabel.MakePixelPerfect();
		if (DeviceInfo.isHighres)
		{
			uILabel.gameObject.transform.localScale = new Vector3(uILabel.gameObject.transform.localScale.x / 2f, uILabel.gameObject.transform.localScale.y / 2f, uILabel.gameObject.transform.localScale.z);
		}
		num++;
		for (int i = 0; i < InAppData.inAppTiersAndInAppTiersDiscount.Length / 2; i++)
		{
			go = NGUITools.AddChild(_table.gameObject, coinPrefab);
			go.name = string.Format("{0:000}", num);
			go.GetComponent<CoinButtonHelper>().Init(i);
			go.GetComponent<UIDragPanelContents>().draggablePanel = _parentDragPanel;
			NGUITools.AddWidgetCollider(go);
			num++;
		}
		if (!PlayerInfo.Instance.hasDoubleCoins)
		{
			go = NGUITools.AddChild(_table.gameObject, restorePrefab);
			go.name = string.Format("{0:000}", num);
			go.GetComponent<UIDragPanelContents>().draggablePanel = _parentDragPanel;
			go.GetComponent<RestorePurchaseHelper>().setOnClickDelegate(RefreshCurrencyEarners);
			NGUITools.AddWidgetCollider(go);
			num++;
		}
		if (shouldShowEarners)
		{
			UILabel uILabel2 = NGUITools.AddWidget<UILabel>(_table.gameObject);
			uILabel2.font = headerFont;
			uILabel2.text = "Earn Coins";
			uILabel2.color = new Color(0f, 0.2901961f, 0.5019608f, 1f);
			uILabel2.name = string.Format("{0:000}", num);
			uILabel2.MakePixelPerfect();
			if (DeviceInfo.isHighres)
			{
				uILabel2.gameObject.transform.localScale = new Vector3(uILabel2.gameObject.transform.localScale.x / 2f, uILabel2.gameObject.transform.localScale.y / 2f, uILabel2.gameObject.transform.localScale.z);
			}
			num++;
			for (int j = 0; j < EarnCurrencyInfo.profiles.Length; j++)
			{
				if (EarnCurrencyInfo.ShouldShowInGUI(j))
				{
					EarnCurrencyInfo.EarnCurrencyProfile earnCurrencyProfile = EarnCurrencyInfo.profiles[j];
					go = NGUITools.AddChild(_table.gameObject, coinEarnerPrefab);
					go.name = string.Format("{0:000}", num);
					string desc = string.Format(earnCurrencyProfile.desc, earnCurrencyProfile.GetAmountOfCoins());
					go.GetComponent<CoinEarnerButtonHelper>().Init(j, earnCurrencyProfile.title, desc, earnCurrencyProfile.iconName, RefreshCurrencyEarners);
					go.GetComponent<UIDragPanelContents>().draggablePanel = _parentDragPanel;
					NGUITools.AddWidgetCollider(go);
					num++;
				}
			}
		}
		_table.gameObject.BroadcastMessage("CreatePanel", SendMessageOptions.DontRequireReceiver);
		if (_table.gameObject.active)
		{
			_table.Reposition();
			if (!firstRun)
			{
				_parentDragPanel.RestrictWithinBounds(true);
			}
			else
			{
				firstRun = false;
			}
		}
		if (UIScreen.IsScreenHeightOutOfProportion())
		{
			_parentDragPanel.SendMessage("Update", SendMessageOptions.DontRequireReceiver);
			_parentDragPanel.SendMessage("LateUpdate", SendMessageOptions.DontRequireReceiver);
		}
	}

	private IEnumerator SetStatic()
	{
		yield return null;
		base.transform.parent.GetComponent<UIPanel>().widgetsAreStatic = true;
	}

	public IEnumerator MoveScrollPanelAtTop(int delayFrames)
	{
		int num = 0;
		while (num < delayFrames)
		{
			num++;
			yield return null;
		}
		if (_parentDragPanel != null)
		{
			Vector3 panelTrans = _parentDragPanel.transform.localPosition;
			panelTrans.y = 0f;
			_parentDragPanel.transform.localPosition = panelTrans;
		}
		else
		{
			Debug.Log("Scroll panel not set in CoinsUI_shop prefab");
		}
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
