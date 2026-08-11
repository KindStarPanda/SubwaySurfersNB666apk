using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeScreen : UIScreen
{
	public GameObject DoubleCoinsPrefab;

	public GameObject ConsumablePrefab;

	public GameObject PermanentPrefab;

	public UIFont headerFont;

	[SerializeField]
	private UITable _table;

	[SerializeField]
	private UIDraggablePanel _parentDragPanel;

	[SerializeField]
	private UIScrollBar _scrollBar;

	private GameObject[] skipMissions = new GameObject[3];

	private string[] skipMissionNames = new string[3];

	public List<UpgradeHelper> cachedUpgradeHelpers = new List<UpgradeHelper>(11);

	private bool _hasStartedToFillTable;

	private bool _hasFilledTableCompletely;

	public GameObject FooterPrefab;

	private UIFooterHandler _footerHandler;

	public int selectedButton;

	public bool isOnScreenLayer = true;

	[NonSerialized]
	public PowerupType[] powerupSingleUse = new PowerupType[4]
	{
		PowerupType.hoverboard,
		PowerupType.mysterybox,
		PowerupType.headstart500,
		PowerupType.headstart2000
	};

	[NonSerialized]
	public PowerupType[] powerupSkipMission = new PowerupType[3]
	{
		PowerupType.skipmission1,
		PowerupType.skipmission2,
		PowerupType.skipmission3
	};

	[NonSerialized]
	public PowerupType[] powerupPermanent = new PowerupType[4]
	{
		PowerupType.jetpack,
		PowerupType.supersneakers,
		PowerupType.coinmagnet,
		PowerupType.doubleMultiplier
	};

	private float _timescaleBeforeUpgradeUI = 1f;

	private int numberOfObjects;

	public override void Init()
	{
		Missions instance = Missions.Instance;
		instance.onMissionComplete = (Missions.MissionCompleteHandler)Delegate.Combine(instance.onMissionComplete, new Missions.MissionCompleteHandler(OnMissionComplete));
		Missions instance2 = Missions.Instance;
		instance2.onMissionSetComplete = (Missions.MissionSetCompleteHandler)Delegate.Combine(instance2.onMissionSetComplete, new Missions.MissionSetCompleteHandler(OnMissionSetComplete));
		numberOfObjects = 0;
		FillTopTable();
		VideoAdsManager instance3 = VideoAdsManager.instance;
		if (!instance3.isInitialized)
		{
			instance3.Init();
		}
		if (isOnScreenLayer)
		{
			InitFooter();
		}
	}

	public override void Show()
	{
		base.Show();
		_table.repositionNow = true;
		if (_hasStartedToFillTable && !_hasFilledTableCompletely)
		{
			FillRemainingTableNow();
		}
		_timescaleBeforeUpgradeUI = Time.timeScale;
		Time.timeScale = 0f;
		if (isOnScreenLayer)
		{
			_footerHandler.handleStickers();
		}
	}

	public override void Hide()
	{
		base.Hide();
		Time.timeScale = _timescaleBeforeUpgradeUI;
	}

	private void OnDestroy()
	{
		if (!UIScreenController.Instance.stoppingFromEditor)
		{
			Missions instance = Missions.Instance;
			instance.onMissionComplete = (Missions.MissionCompleteHandler)Delegate.Remove(instance.onMissionComplete, new Missions.MissionCompleteHandler(OnMissionComplete));
			Missions instance2 = Missions.Instance;
			instance2.onMissionSetComplete = (Missions.MissionSetCompleteHandler)Delegate.Remove(instance2.onMissionSetComplete, new Missions.MissionSetCompleteHandler(OnMissionSetComplete));
		}
	}

	private void FillTopTable()
	{
		_hasStartedToFillTable = true;
		GameObject gameObject = NGUITools.AddChild(_table.gameObject, DoubleCoinsPrefab);
		gameObject.name = string.Format("{0:000}", numberOfObjects);
		gameObject.GetComponent<DoubleCoinUpgradeHelper>().Init();
		gameObject.GetComponent<UIDragPanelContents>().draggablePanel = _parentDragPanel;
		numberOfObjects++;
		UILabel uILabel = NGUITools.AddWidget<UILabel>(_table.gameObject);
		uILabel.font = headerFont;
		uILabel.text = "Single Use";
		uILabel.color = new Color(0f, 0.2901961f, 0.5019608f, 1f);
		uILabel.name = string.Format("{0:000}", numberOfObjects);
		uILabel.supportEncoding = false;
		uILabel.multiLine = false;
		uILabel.MakePixelPerfect();
		if (DeviceInfo.isHighres)
		{
			uILabel.gameObject.transform.localScale = new Vector3(uILabel.gameObject.transform.localScale.x / 2f, uILabel.gameObject.transform.localScale.y / 2f, uILabel.gameObject.transform.localScale.z);
		}
		numberOfObjects++;
		PowerupType[] array = powerupSingleUse;
		foreach (PowerupType powerupType in array)
		{
			MakeBuyable(powerupType, false);
			numberOfObjects++;
		}
		base.gameObject.BroadcastMessage("CreatePanel", SendMessageOptions.DontRequireReceiver);
		StartCoroutine(FillRemaingTableDelayed());
	}

	private IEnumerator FillRemaingTableDelayed()
	{
		yield return null;
		FillRemainingTableNow();
		_table.Reposition();
		base.gameObject.BroadcastMessage("CreatePanel", SendMessageOptions.DontRequireReceiver);
		if (UIScreen.IsScreenHeightOutOfProportion())
		{
			StartCoroutine(MoveScrollPanelAtTop(0));
		}
	}

	private void FillRemainingTableNow()
	{
		if (_hasFilledTableCompletely)
		{
			Debug.Log("Table was already filled!", base.gameObject);
			return;
		}
		for (int i = 0; i < powerupSkipMission.Length; i++)
		{
			string text = string.Format("{0:000}", numberOfObjects);
			skipMissionNames[i] = text;
			if (!Missions.Instance.GetMissionInfo(i).complete)
			{
				skipMissions[i] = MakeBuyable(powerupSkipMission[i], false);
			}
			numberOfObjects++;
		}
		UILabel uILabel = NGUITools.AddWidget<UILabel>(_table.gameObject);
		uILabel.font = headerFont;
		uILabel.text = "Upgrades";
		uILabel.color = new Color(0f, 0.2901961f, 0.5019608f, 1f);
		uILabel.name = string.Format("{0:000}", numberOfObjects);
		uILabel.supportEncoding = false;
		uILabel.multiLine = false;
		uILabel.MakePixelPerfect();
		if (DeviceInfo.isHighres)
		{
			uILabel.gameObject.transform.localScale = new Vector3(uILabel.gameObject.transform.localScale.x / 2f, uILabel.gameObject.transform.localScale.y / 2f, uILabel.gameObject.transform.localScale.z);
		}
		numberOfObjects++;
		PowerupType[] array = powerupPermanent;
		foreach (PowerupType powerupType in array)
		{
			MakeBuyable(powerupType, true);
			numberOfObjects++;
		}
		_hasFilledTableCompletely = true;
	}

	private GameObject MakeBuyable(PowerupType powerupType, bool permanent)
	{
		GameObject gameObject;
		if (permanent)
		{
			gameObject = NGUITools.AddChild(_table.gameObject, PermanentPrefab);
			gameObject.GetComponent<UpgradeHelper>().InitPermanent(powerupType);
		}
		else
		{
			gameObject = NGUITools.AddChild(_table.gameObject, ConsumablePrefab);
			gameObject.GetComponent<UpgradeHelper>().InitSingle(powerupType);
		}
		gameObject.GetComponent<UIDragPanelContents>().draggablePanel = _parentDragPanel;
		gameObject.name = string.Format("{0:000}", numberOfObjects);
		NGUITools.AddWidgetCollider(gameObject);
		cachedUpgradeHelpers.Add(gameObject.GetComponent<UpgradeHelper>());
		return gameObject;
	}

	private void OnMissionComplete(string payload)
	{
		for (int i = 0; i < skipMissions.Length; i++)
		{
			if (skipMissions[i] != null && Missions.Instance.GetMissionInfo(i).complete)
			{
				cachedUpgradeHelpers.Remove(skipMissions[i].GetComponent<UpgradeHelper>());
				NGUITools.SetActive(skipMissions[i], false);
				UnityEngine.Object.Destroy(skipMissions[i]);
			}
		}
		_table.repositionNow = true;
	}

	private void OnMissionSetComplete()
	{
		for (int i = 0; i < skipMissions.Length; i++)
		{
			if (skipMissions[i] != null)
			{
				cachedUpgradeHelpers.Remove(skipMissions[i].GetComponent<UpgradeHelper>());
				UnityEngine.Object.Destroy(skipMissions[i]);
			}
		}
		bool active = base.gameObject.active;
		for (int j = 0; j < powerupSkipMission.Length; j++)
		{
			if (!Missions.Instance.GetMissionInfo(j).complete)
			{
				GameObject gameObject = NGUITools.AddChild(_table.gameObject, ConsumablePrefab);
				gameObject.name = skipMissionNames[j];
				gameObject.GetComponent<UpgradeHelper>().InitSingle(powerupSkipMission[j]);
				gameObject.GetComponent<UIDragPanelContents>().draggablePanel = _parentDragPanel;
				NGUITools.AddWidgetCollider(gameObject);
				skipMissions[j] = gameObject;
				NGUITools.SetActive(gameObject, active);
				cachedUpgradeHelpers.Add(gameObject.GetComponent<UpgradeHelper>());
			}
		}
		if (base.gameObject.active)
		{
			_table.repositionNow = true;
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
		if (!(_parentDragPanel != null))
		{
			Debug.Log("Scroll panel not set in UpgradesUI_shop prefab");
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
