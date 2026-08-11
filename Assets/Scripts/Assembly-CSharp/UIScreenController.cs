using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScreenController : MonoBehaviour
{
	public enum SlideInType
	{
		Mission = 0,
		MissionSet = 1,
		Letters = 2,
		LettersCompleteMysteryBox = 3,
		LettersCompleteCoins = 4,
		Unlock = 5,
		ErrorMessage = 6
	}

	private class SlideIn
	{
		public SlideInType type;

		public string payload = string.Empty;
	}

	private const float CHARTBOOST_DELAY_SECONDS = 0.1f;

	private const string SCREEN_RESOURCE_PATH = "Prefabs/Screens/";

	private const string POPUP_RESOURCE_PATH = "Prefabs/Popups/";

	private static UIScreenController _instance;

	public AnimationCurve guidelineAnimation;

	public GameObject backgroundAnchor;

	public GameObject screenAnchor;

	public GameObject popupAnchor;

	public GameObject superPopupAnchor;

	public Camera Camera3D;

	public UIRoot root;

	public GameObject MenuElements3D;

	public bool LoadMenuOnStart;

	public UIFont FloatingTextFont;

	private Camera mainCamera;

	public Action<string> OnChangedScreen;

	private static readonly List<string> PAYOUT_DISALLOWED_SCREENS = new List<string> { "IngameUI" };

	private static bool _facebookPayoutPopupQueued = false;

	private static bool _gameCenterPayoutPopupQueued = false;

	private bool _runningChartboostDelayCoroutine;

	private float _chartboostDelaySecondsLeft;

	private Dictionary<string, UIScreen> _cachedScreens = new Dictionary<string, UIScreen>();

	private List<string> _screenStack = new List<string>();

	private List<string> _popupQueue = new List<string>();

	private bool _popupActive;

	private List<string> _screenNamesWithoutBackground = new List<string> { "FrontUI", "IngameUI" };

	private List<string> _screenNamesWithOnlineVersion = new List<string> { "LeaderboardUI", "FriendsUI" };

	public bool stoppingFromEditor;

	[SerializeField]
	private UISlideInMissionHelper missionSlideIn;

	[SerializeField]
	private UISlideInMissionSetHelper missionSetSlideIn;

	[SerializeField]
	private UISlideInLettersHelper lettersSlideIn;

	[SerializeField]
	private UISlideInUnlock unlockSlideIn;

	[SerializeField]
	private UISlideInErrorMessage errorSlideIn;

	public CoinLabelSizer coinReward;

	private Queue<SlideIn> _slideInQueue = new Queue<SlideIn>(15);

	private bool slideInActive;

	[SerializeField]
	private AudioClipInfo slideInSound;

	[SerializeField]
	private AudioClipInfo slideInFanfare;

	private bool slideHasPreloaded;

	public UIMessageHelper messageHelper;

	private Queue<string> _messageQueue = new Queue<string>();

	private bool messageShowing;

	public GameObject inAppPurchaseOverlay;

	public static bool isInstanced
	{
		get
		{
			if (_instance == null)
			{
				_instance = UnityEngine.Object.FindObjectOfType(typeof(UIScreenController)) as UIScreenController;
			}
			return _instance != null;
		}
	}

	public static UIScreenController Instance
	{
		get
		{
			return _instance ?? (_instance = UnityEngine.Object.FindObjectOfType(typeof(UIScreenController)) as UIScreenController);
		}
	}

	public bool isShowingPopup
	{
		get
		{
			return _popupQueue != null && _popupQueue.Count > 0;
		}
	}

	private void Awake()
	{
		Missions instance = Missions.Instance;
		instance.onMissionComplete = (Missions.MissionCompleteHandler)Delegate.Combine(instance.onMissionComplete, new Missions.MissionCompleteHandler(OnMissionCompleted));
		Missions instance2 = Missions.Instance;
		instance2.onMissionSetComplete = (Missions.MissionSetCompleteHandler)Delegate.Combine(instance2.onMissionSetComplete, new Missions.MissionSetCompleteHandler(OnMissionSetCompleted));
		PlayerInfo instance3 = PlayerInfo.Instance;
		instance3.OnPickedUpLetter = (Action)Delegate.Combine(instance3.OnPickedUpLetter, new Action(OnLetterPickedUp));
		PlayerInfo instance4 = PlayerInfo.Instance;
		instance4.OnTokenCollected = (Action<Characters.CharacterType>)Delegate.Combine(instance4.OnTokenCollected, new Action<Characters.CharacterType>(OnTokenPickUp));
		if (mainCamera == null)
		{
			mainCamera = Camera.main;
		}
	}

	private void OnApplicationQuit()
	{
	}

	private void OnDestroy()
	{
		if (!stoppingFromEditor)
		{
			Missions instance = Missions.Instance;
			instance.onMissionComplete = (Missions.MissionCompleteHandler)Delegate.Remove(instance.onMissionComplete, new Missions.MissionCompleteHandler(OnMissionCompleted));
			Missions instance2 = Missions.Instance;
			instance2.onMissionSetComplete = (Missions.MissionSetCompleteHandler)Delegate.Remove(instance2.onMissionSetComplete, new Missions.MissionSetCompleteHandler(OnMissionSetCompleted));
			PlayerInfo instance3 = PlayerInfo.Instance;
			instance3.OnPickedUpLetter = (Action)Delegate.Remove(instance3.OnPickedUpLetter, new Action(OnLetterPickedUp));
			PlayerInfo instance4 = PlayerInfo.Instance;
			instance4.OnTokenCollected = (Action<Characters.CharacterType>)Delegate.Remove(instance4.OnTokenCollected, new Action<Characters.CharacterType>(OnTokenPickUp));
		}
	}

	private void Start()
	{
		HideInAppPurchaseOverlay();
		if (LoadMenuOnStart)
		{
			ShowMainMenu();
		}
		PlayerInfo.Instance.BragCompleted();
		MissionInfo[] missionInfo = Missions.Instance.GetMissionInfo();
		if (missionInfo[0].complete && missionInfo[1].complete && missionInfo[2].complete)
		{
			Missions.Instance.currentMissionSet++;
			Debug.LogWarning("you completed all missions but was not sent to next mission set, this should never happen, but this fixes it");
		}
		PreloadAllSlideIn();
	}

	private void OnApplicationPause(bool paused)
	{
		if (paused)
		{
			if (_screenStack.Count > 0 && Peek(_screenStack) == "IngameUI" && (!(Game.Instance != null) || !Game.Instance.isDead))
			{
				PushScreen(null, "PauseUI");
			}
			PlayerInfo.Instance.SaveIfDirty();
			UpdateApp.AllowAgainThisSession();
			LogFlurry();
		}
	}

	private void LogFlurry()
	{
		string currentPopupName = GetCurrentPopupName();
		string topScreenName = GetTopScreenName();
		if (!string.IsNullOrEmpty(currentPopupName))
		{
			if (!string.IsNullOrEmpty(currentPopupName) && !string.IsNullOrEmpty(topScreenName))
			{
				Flurry.LogEventWithAParameter("Back button pressed", "Screen Name", topScreenName + "_" + currentPopupName);
			}
			else if (!string.IsNullOrEmpty(currentPopupName))
			{
				Flurry.LogEventWithAParameter("Back button pressed", "Screen Name", currentPopupName);
			}
			else if (!string.IsNullOrEmpty(topScreenName))
			{
				Flurry.LogEventWithAParameter("Back button pressed", "Screen Name", topScreenName);
			}
			else
			{
				Flurry.LogEventWithAParameter("Back button pressed", "Screen Name", "NO SCREEN");
			}
		}
	}

	public void FacebookLogIn(bool loggedIn)
	{
		if (loggedIn)
		{
			Missions.Instance.PlayerDidThis(Missions.MissionTarget.FacebookLoggedIn);
		}
		if (_screenStack.Count <= 0)
		{
			return;
		}
		string text = Peek(_screenStack);
		string text2 = text;
		foreach (string item in _screenNamesWithOnlineVersion)
		{
			if (text.StartsWith(item))
			{
				text2 = _CheckOnlineOfflineScreens(item);
				break;
			}
		}
		if (text2 != text)
		{
			_SwitchScreen(text2);
		}
		if (text == "GameoverUI" && loggedIn)
		{
			_cachedScreens["GameoverUI"].GetComponent<GameOverScreen>().FacebookLoggedIn();
		}
	}

	public void ShowMainMenu()
	{
		StartCoroutine(ShowMainMenuCoroutine());
	}

	private IEnumerator ShowMainMenuCoroutine()
	{
		while (Time.realtimeSinceStartup < LoadLevelCtrl.continueTime)
		{
			yield return null;
		}
		ThemeManager.Instance.ForceRefresh();
		_ActivateScreen("FrontUI");
	}

	public void GameOverTriggered()
	{
		PlayerInfo.Instance.RunCompleted();
		Missions.Instance.inRun = false;
		_ActivateScreen("GameoverUI");
	}

	public void QueueMessage(string message)
	{
		Debug.Log("Showing message: " + message);
		_QueueMessage(message);
	}

	public void GoToMainMenuFromGame(GameObject sender)
	{
		if (Game.Instance != null)
		{
			Missions.Instance.inRun = false;
			Game.Instance.StartTopMenu();
			Game.Instance.TriggerPause(false);
		}
		_ActivateScreen("FrontUI");
	}

	public string GetTopScreenName()
	{
		if (_screenStack != null && _screenStack.Count > 0)
		{
			return Peek(_screenStack);
		}
		return null;
	}

	public void PushScreen(string screenName)
	{
		PushScreen(null, screenName);
	}

	public void PushScreen(GameObject sender)
	{
		PushScreen(sender, string.Empty);
	}

	public void PushScreen(GameObject sender, string screenOverride)
	{
		string screenName = string.Empty;
		if (screenOverride != string.Empty)
		{
			screenName = screenOverride;
		}
		else
		{
			UIButtonChangeScreen component = sender.GetComponent<UIButtonChangeScreen>();
			if (component != null)
			{
				screenName = component.ScreenNameToOpen;
			}
			BackBtnBehaviourAndroid component2 = sender.GetComponent<BackBtnBehaviourAndroid>();
			if (component2 != null)
			{
				screenName = component2.ScreenNameToOpen;
			}
		}
		screenName = _CheckOnlineOfflineScreens(screenName);
		if ((screenName == "PauseUI" && (!(Game.Instance != null) || Game.Instance.isDead)) || (Peek(_screenStack) == "PauseUI" && screenName == "IngameUI" && _cachedScreens["PauseUI"].GetComponent<PauseScreen>().IsWaiting()))
		{
			return;
		}
		if (screenName == "FrontUI")
		{
			string topScreenName = GetTopScreenName();
			Debug.Log("Going to front screen! Currentscreen: " + topScreenName);
			if (!string.IsNullOrEmpty(topScreenName))
			{
				switch (topScreenName)
				{
				case "FriendsUI_offline":
				case "FriendsUI_online":
					Debug.Log("Saving screen" + topScreenName);
					UIScreens.friendsMenu_lastScreen = "FriendsUI";
					break;
				case "LeaderboardUI_offline":
				case "LeaderboardUI_online":
					Debug.Log("Saving screen" + topScreenName);
					UIScreens.friendsMenu_lastScreen = "LeaderboardUI";
					break;
				case "CharacterScreen":
				case "BoardScreen":
				case "TrophiesScreen":
					Debug.Log("Saving screen" + topScreenName);
					UIScreens.meMenu_lastScreen = topScreenName;
					break;
				case "UpgradesUI_shop":
				case "EarnCoinsScreen":
				case "CoinsUI_shop":
					Debug.Log("Saving screen" + topScreenName);
					UIScreens.shopMenu_lastScreen = topScreenName;
					break;
				}
			}
		}
		_ActivateScreen(screenName);
	}

	public void SwitchScreen(GameObject sender)
	{
		string empty = string.Empty;
		empty = sender.GetComponent<UIButtonChangeScreen>().ScreenNameToOpen;
		empty = _CheckOnlineOfflineScreens(empty);
		_SwitchScreen(empty);
	}

	public void BackToPrevious()
	{
		_BackToPreviousScreen();
	}

	private UIScreen _ActivateScreen(string screenName)
	{
		bool flag = true;
		UIScreen uIScreen;
		if (_cachedScreens.ContainsKey(screenName))
		{
			if (_screenStack.Count > 0)
			{
				int num = _screenStack.LastIndexOf(screenName);
				if (num >= 0)
				{
					flag = false;
					if (num < _screenStack.Count - 1)
					{
						int num2 = num + 1;
						int num3 = _screenStack.Count - num2;
						for (int i = num2; i < num2 + num3; i++)
						{
							_cachedScreens[_screenStack[i]].Hide();
						}
						_screenStack.RemoveRange(num2, num3);
					}
				}
			}
			uIScreen = _cachedScreens[screenName];
		}
		else
		{
			uIScreen = _LoadScreenToCache(screenName);
		}
		AddBackButtonBehaviorForAndroid(screenName);
		if (flag)
		{
			if (_screenStack.Count > 0)
			{
				_cachedScreens[_screenStack[_screenStack.Count - 1]].Hide();
			}
			_screenStack.Add(screenName);
		}
		_ShowScreen(screenName, uIScreen);
		return uIScreen;
	}

	private void _ShowScreen(string screenName, UIScreen screen)
	{
		_SetBackground(!_screenNamesWithoutBackground.Contains(screenName));
		screen.Show();
		screenAnchor.gameObject.BroadcastMessage("Refresh", SendMessageOptions.DontRequireReceiver);
		if (screenName == "GameoverUI")
		{
			_cachedScreens[screenName].GetComponent<GameOverScreen>().SetupBeforeMysteryBox();
			if (PlayerInfo.Instance.mysteryBoxesToUnlockCount > 0)
			{
				_QueuePopup("MysteryBoxPopup");
			}
			else
			{
				_cachedScreens[screenName].GetComponent<GameOverScreen>().SetupAfterMysteryBox();
			}
		}
		Action<string> onChangedScreen = OnChangedScreen;
		if (onChangedScreen != null)
		{
			onChangedScreen(screenName);
		}
		ScreenDidChange(screenName);
	}

	private void _SwitchScreen(string screenName)
	{
		string key = Pop(_screenStack);
		_cachedScreens[key].Hide();
		_ActivateScreen(screenName);
	}

	private void _BackToPreviousScreen()
	{
		if (_screenStack.Count > 1)
		{
			string key = Pop(_screenStack);
			_cachedScreens[key].Hide();
			key = Peek(_screenStack);
			_cachedScreens[key].Show();
			_SetBackground(!_screenNamesWithoutBackground.Contains(key));
			ScreenDidChange(key);
		}
		else
		{
			Debug.LogError("Tried to remove the only screen in the stack. You dun goofed.", this);
		}
	}

	private void ScreenDidChange(string newScreenName)
	{
		messageHelper.SetTemporaryHidden(newScreenName != "IngameUI");
		InvokeChartboostDelayed(0.1f);
		TryQueuePayoutPopups();
		if (newScreenName == "FrontUI" || newScreenName == "GameoverUI")
		{
			HouseKeeper.RefreshOnlineSettingsAndInappsIfNeeded();
		}
		if (newScreenName == "FrontUI" && _cachedScreens[GetTopScreenName()].GetComponent<FrontScreen>().buttonsHaveTweened && PlayerInfo.Instance.shouldShowMission2Popup && !PlayerInfo.Instance.hasShownMission2Popup)
		{
			PlayerInfo.Instance.shouldShowMission2Popup = false;
			PlayerInfo.Instance.hasShownMission2Popup = true;
			_QueuePopup("TutorialEndGameMissionsPopup");
		}
	}

	private void _SetBackground(bool state)
	{
		string text = "NotebookPanel2";
		if (state)
		{
			if (!_cachedScreens.ContainsKey(text))
			{
				GameObject prefab = Resources.Load("Prefabs/Screens/" + text, typeof(GameObject)) as GameObject;
				GameObject gameObject = NGUITools.AddChild(backgroundAnchor, prefab);
				_cachedScreens.Add(text, gameObject.GetComponent<UIScreen>());
				_cachedScreens[text].Init();
			}
			_cachedScreens[text].Show();
			_cachedScreens[text].gameObject.BroadcastMessage("CreatePanel", SendMessageOptions.DontRequireReceiver);
			_cachedScreens[text].gameObject.BroadcastMessage("Refresh", SendMessageOptions.DontRequireReceiver);
			mainCamera.enabled = false;
		}
		else
		{
			if (_cachedScreens.ContainsKey(text))
			{
				_cachedScreens[text].Hide();
			}
			mainCamera.enabled = true;
		}
	}

	private UIScreen _LoadScreenToCache(string screenName, bool isPopup = false)
	{
		GameObject gameObject;
		if (!isPopup)
		{
			GameObject prefab = Resources.Load("Prefabs/Screens/" + screenName, typeof(GameObject)) as GameObject;
			gameObject = NGUITools.AddChild(screenAnchor, prefab);
		}
		else
		{
			GameObject prefab2 = Resources.Load("Prefabs/Popups/" + screenName, typeof(GameObject)) as GameObject;
			gameObject = NGUITools.AddChild(popupAnchor, prefab2);
		}
		UIScreen component = gameObject.GetComponent<UIScreen>();
		_cachedScreens.Add(screenName, component);
		component.Init();
		return component;
	}

	private string _CheckOnlineOfflineScreens(string screenName)
	{
		if (_screenNamesWithOnlineVersion.Contains(screenName))
		{
			screenName = ((!Social.localUser.authenticated && !SocialManager.instance.facebookIsLoggedIn) ? (screenName + "_offline") : (screenName + "_online"));
		}
		return screenName;
	}

	private string Peek(List<string> list)
	{
		if (list.Count > 0)
		{
			return list[list.Count - 1];
		}
		return string.Empty;
	}

	private string Pop(List<string> list)
	{
		string result = string.Empty;
		if (list.Count > 0)
		{
			result = list[list.Count - 1];
			list.RemoveAt(list.Count - 1);
		}
		return result;
	}

	private string QueuePeek(List<string> list)
	{
		string result = string.Empty;
		if (list.Count > 0)
		{
			result = list[0];
		}
		return result;
	}

	private string Dequeue(List<string> list)
	{
		string result = string.Empty;
		if (list.Count > 0)
		{
			result = list[0];
			list.RemoveAt(0);
		}
		return result;
	}

	public string GetCurrentPopupName()
	{
		return QueuePeek(_popupQueue);
	}

	public bool IsPopupQueueEmpty()
	{
		return _popupQueue.Count <= 0;
	}

	private void _QueuePopup(string name)
	{
		_popupQueue.Add(name);
		if (!_popupActive)
		{
			_ActivateNextPopup();
		}
	}

	private void _ActivateNextPopup()
	{
		if (_popupQueue.Count > 0)
		{
			_PauseAnimations(true, MenuElements3D.transform);
			NGUITools.SetActive(UIModelController.Instance.CharacterAnchor, false);
			NGUITools.SetActive(UIModelController.Instance.GameOverAnchor, false);
			NGUITools.SetActive(UIModelController.Instance.MysteryBoxAnchor, false);
			string text = QueuePeek(_popupQueue);
			if (!_cachedScreens.ContainsKey(text))
			{
				_LoadScreenToCache(text, true);
			}
			_cachedScreens[text].Show();
			_cachedScreens[text].BroadcastMessage("CreatePanel", SendMessageOptions.DontRequireReceiver);
			_popupActive = true;
			BackBtnBehaviourAndroid component = _cachedScreens[text].gameObject.GetComponent<BackBtnBehaviourAndroid>();
			if (text != "MysteryBoxPopup" && _cachedScreens[text].gameObject.GetComponent<BackBtnBehaviourAndroid>() == null)
			{
				_cachedScreens[text].gameObject.AddComponent<BackBtnBehaviourAndroid>().screenChangeType = BackBtnBehaviourAndroid.ScreenChangeType.ClosePopup;
			}
			Action<string> onChangedScreen = OnChangedScreen;
			if (onChangedScreen != null)
			{
				onChangedScreen(text);
			}
		}
		else
		{
			NGUITools.SetActive(UIModelController.Instance.CharacterAnchor, true);
			NGUITools.SetActive(UIModelController.Instance.GameOverAnchor, true);
			NGUITools.SetActive(UIModelController.Instance.MysteryBoxAnchor, true);
			_PauseAnimations(false, MenuElements3D.transform);
		}
	}

	public static void QueueFacebookPayoutPopup()
	{
		_facebookPayoutPopupQueued = true;
		if (isInstanced)
		{
			Instance.TryQueuePayoutPopups();
		}
	}

	public static void QueueGameCenterPayoutPopup()
	{
		_gameCenterPayoutPopupQueued = true;
		if (isInstanced)
		{
			Instance.TryQueuePayoutPopups();
		}
	}

	private void TryQueuePayoutPopups()
	{
		if ((!_facebookPayoutPopupQueued && !_gameCenterPayoutPopupQueued) || _screenStack == null || _screenStack.Count <= 0)
		{
			return;
		}
		string item = Peek(_screenStack);
		if (!PAYOUT_DISALLOWED_SCREENS.Contains(item))
		{
			if (_facebookPayoutPopupQueued)
			{
				QueuePopup("FacebookPayoutPopup");
				_facebookPayoutPopupQueued = false;
			}
			if (_gameCenterPayoutPopupQueued)
			{
				QueuePopup("GameCenterPayoutPopup");
				_gameCenterPayoutPopupQueued = false;
			}
		}
		else
		{
			Debug.Log("Cannot show payout popup on this screen");
		}
	}

	public void QueuePopup(string popupName)
	{
		_QueuePopup(popupName);
	}

	public void QueuePopup(GameObject sender)
	{
		string screenNameToOpen = sender.GetComponent<UIButtonChangeScreen>().ScreenNameToOpen;
		_QueuePopup(screenNameToOpen);
	}

	public void QueueMysteryBox()
	{
		string text = string.Empty;
		if (_popupQueue.Count > 0)
		{
			text = QueuePeek(_popupQueue);
			if (text == "MysteryBoxPopup")
			{
				return;
			}
			_RemovePopup();
		}
		_QueuePopup("MysteryBoxPopup");
		if (text != string.Empty)
		{
			_QueuePopup(text);
		}
	}

	public void ClosePopup(GameObject go = null)
	{
		_RemovePopup();
	}

	private void _PauseAnimations(bool pause, Transform trans)
	{
		foreach (Transform tran in trans)
		{
			_PauseAnimations(pause, tran);
		}
		if (trans.GetComponent<CharacterModel>() != null)
		{
			if (pause)
			{
				trans.GetComponent<CharacterModel>().StopIdleAnimations();
			}
			else
			{
				trans.GetComponent<CharacterModel>().StartIdleAnimations();
			}
		}
	}

	private void _RemovePopup()
	{
		if (_popupQueue.Count < 1)
		{
			return;
		}
		string text = Dequeue(_popupQueue);
		_cachedScreens[text].Hide();
		_popupActive = false;
		if (_popupQueue.Count == 0)
		{
			Action<string> onChangedScreen = OnChangedScreen;
			if (onChangedScreen != null)
			{
				onChangedScreen(GetTopScreenName());
			}
		}
		_ActivateNextPopup();
		if (text == "MysteryBoxPopup" && GetTopScreenName() == "GameoverUI")
		{
			_cachedScreens["GameoverUI"].GetComponent<GameOverScreen>().SetupAfterMysteryBox();
		}
		if (_popupQueue.Count == 0)
		{
			ChartBoostManager.instance.LastQueuedPopupsClosed(GetTopScreenName());
		}
	}

	private void InvokeChartboostDelayed(float delay)
	{
		_chartboostDelaySecondsLeft = delay;
		if (!_runningChartboostDelayCoroutine)
		{
			StartCoroutine(DelayedChartboostNotifyCoroutine());
		}
	}

	private IEnumerator DelayedChartboostNotifyCoroutine()
	{
		if (!_runningChartboostDelayCoroutine)
		{
			_runningChartboostDelayCoroutine = true;
			float lastRealTime = Time.realtimeSinceStartup;
			while (_chartboostDelaySecondsLeft > 0f)
			{
				float realTime = Time.realtimeSinceStartup;
				float deltaRealTime = realTime - lastRealTime;
				lastRealTime = realTime;
				_chartboostDelaySecondsLeft -= deltaRealTime;
				yield return null;
			}
			string screenName = GetTopScreenName();
			if (!string.IsNullOrEmpty(screenName))
			{
				ChartBoostManager.instance.GameScreenChanged(screenName);
			}
			_runningChartboostDelayCoroutine = false;
		}
	}

	public void SpawnCollectText(Vector3 startPosition, string text)
	{
		UILabel uILabel = NGUITools.AddWidget<UILabel>(superPopupAnchor);
		uILabel.text = text;
		uILabel.transform.position = new Vector3(startPosition.x, startPosition.y, uILabel.cachedTransform.position.z);
		uILabel.font = FloatingTextFont;
		uILabel.color = new Color(50f / 51f, 66f / 85f, 0.23529412f, 0f);
		uILabel.cachedTransform.localScale = new Vector3(17f, 17f, 1f);
		StartCoroutine(AnimateCollectText(uILabel));
	}

	private IEnumerator AnimateCollectText(UILabel collectText)
	{
		Vector3 fromLocalPosition = collectText.transform.localPosition;
		Vector3 toLocalPosition = new Vector3(fromLocalPosition.x, fromLocalPosition.y + 50f, fromLocalPosition.z);
		yield return StartCoroutine(AnimateAlpha(collectText, 0.1f, 1f));
		StartCoroutine(MoveTransform(collectText.cachedTransform, 1f, toLocalPosition));
		yield return new WaitForSeconds(0.8f);
		StartCoroutine(AnimateAlpha(collectText, 0.2f, 0f));
		yield return new WaitForSeconds(0.25f);
		UnityEngine.Object.Destroy(collectText.gameObject);
	}

	private IEnumerator AnimateAlpha(UILabel label, float duration, float toAlpha)
	{
		float fromAlpha = label.alpha;
		float factor2 = 0f;
		while (factor2 < 1f)
		{
			factor2 += Time.deltaTime / duration;
			factor2 = Mathf.Clamp01(factor2);
			label.alpha = Mathf.Lerp(fromAlpha, toAlpha, factor2);
			yield return null;
		}
	}

	private IEnumerator MoveTransform(Transform trans, float duration, Vector3 toPos)
	{
		Vector3 fromPos = trans.localPosition;
		float factor2 = 0f;
		while (factor2 < 1f)
		{
			factor2 += Time.deltaTime / duration;
			factor2 = Mathf.Clamp01(factor2);
			trans.localPosition = Vector3.Lerp(fromPos, toPos, factor2);
			yield return null;
		}
	}

	private void PreloadAllSlideIn()
	{
		if (!slideHasPreloaded)
		{
			slideHasPreloaded = true;
			StartCoroutine(missionSlideIn.PreloadSlideIn());
			StartCoroutine(missionSetSlideIn.PreloadSlideIn());
			StartCoroutine(lettersSlideIn.PreloadSlideIn());
			StartCoroutine(unlockSlideIn.PreloadSlideIn());
		}
	}

	private void OnMissionCompleted(string message)
	{
		QueueSlideIn(SlideInType.Mission, message);
	}

	private void OnMissionSetCompleted()
	{
		Missions.Instance.PlayerDidThis(Missions.MissionTarget.ReachMissionSet);
		QueueSlideIn(SlideInType.MissionSet, string.Empty);
	}

	private void OnLetterPickedUp()
	{
		QueueSlideIn(SlideInType.Letters, string.Empty);
	}

	private void OnTokenPickUp(Characters.CharacterType type)
	{
		if (Characters.characterData[type].Price <= PlayerInfo.Instance.GetCollectedTokens(type))
		{
			QueueSlideIn(SlideInType.Unlock, Characters.characterData[type].modelName);
			Flurry.LogEventWithAParameter("Character unlocked", "Id", Characters.characterData[type].modelName.ToLower());
		}
	}

	public void QueueErrorMessageSlidein(string message)
	{
		QueueSlideIn(SlideInType.ErrorMessage, message);
	}

	public void QueueSlideIn(SlideInType type, string payload = "")
	{
		SlideIn slideIn = new SlideIn();
		slideIn.type = type;
		slideIn.payload = payload;
		_slideInQueue.Enqueue(slideIn);
		if (!slideInActive)
		{
			_ShowSlideIn();
		}
	}

	public void ReadyForNextSlide()
	{
		slideInActive = false;
		if (!slideInActive)
		{
			_ShowSlideIn();
		}
	}

	private void _ShowSlideIn()
	{
		if (_slideInQueue.Count > 0)
		{
			SlideIn slideIn = _slideInQueue.Dequeue();
			if (slideIn.type == SlideInType.Mission)
			{
				So.Instance.playSound(slideInFanfare);
				missionSlideIn.SetupSlideInMission(slideIn.payload);
			}
			else if (slideIn.type == SlideInType.MissionSet)
			{
				So.Instance.playSound(slideInFanfare);
				missionSetSlideIn.SetupSlideInMissionSet(PlayerInfo.Instance.rawMultiplier);
			}
			else if (slideIn.type == SlideInType.Letters)
			{
				So.Instance.playSound(slideInSound);
				lettersSlideIn.SetupLetters();
			}
			else if (slideIn.type == SlideInType.LettersCompleteMysteryBox)
			{
				So.Instance.playSound(slideInFanfare);
				missionSetSlideIn.SetupMysteryBox();
			}
			else if (slideIn.type == SlideInType.LettersCompleteCoins)
			{
				So.Instance.playSound(slideInFanfare);
				missionSetSlideIn.SetupCoin();
			}
			else if (slideIn.type == SlideInType.Unlock)
			{
				So.Instance.playSound(slideInFanfare);
				unlockSlideIn.SetupSlideInUnlock(slideIn.payload);
			}
			else if (slideIn.type == SlideInType.ErrorMessage)
			{
				So.Instance.playSound(slideInSound);
				errorSlideIn.SetupErrorMessage(slideIn.payload);
			}
			slideInActive = true;
		}
	}

	public void ReadyForNextMessage()
	{
		messageShowing = false;
		_ShowNextMessage();
	}

	private void _QueueMessage(string message)
	{
		_messageQueue.Enqueue(message);
		if (!messageShowing)
		{
			_ShowNextMessage();
		}
		if (!slideInActive)
		{
			_ShowSlideIn();
		}
	}

	private void _ShowNextMessage()
	{
		if (_messageQueue.Count > 0)
		{
			string message = _messageQueue.Dequeue();
			messageHelper.ShowMessage(message);
			messageShowing = true;
		}
	}

	public void ShowInAppPurchaseOverlay()
	{
		inAppPurchaseOverlay.SetActiveRecursively(true);
		Camera3D.enabled = false;
	}

	public void HideInAppPurchaseOverlay()
	{
		inAppPurchaseOverlay.SetActiveRecursively(false);
		Camera3D.enabled = true;
	}

	public bool IsInAppPurchaseOverlayVisible()
	{
		return inAppPurchaseOverlay.active;
	}

	private void AddBackButtonBehaviorForAndroid(string screenName)
	{
		Debug.Log("screenName : " + screenName);
		BackBtnBehaviourAndroid component = _cachedScreens[screenName].gameObject.GetComponent<BackBtnBehaviourAndroid>();
		if (component == null)
		{
			_cachedScreens[screenName].gameObject.AddComponent<BackBtnBehaviourAndroid>();
			switch (screenName)
			{
			case "FrontUI":
				_cachedScreens[screenName].gameObject.GetComponent<BackBtnBehaviourAndroid>().screenChangeType = BackBtnBehaviourAndroid.ScreenChangeType.ExitGame;
				break;
			case "FriendsUI_offline":
			case "FriendsUI_online":
			case "TrophiesScreen":
			case "LeaderboardUI_offline":
			case "LeaderboardUI_online":
			case "CharacterScreen":
			case "UpgradesUI_shop":
			case "CoinsUI_shop":
			case "GameoverUI":
			case "BoardScreen":
			case "EarnCoinsScreen":
				_cachedScreens[screenName].gameObject.GetComponent<BackBtnBehaviourAndroid>().screenChangeType = BackBtnBehaviourAndroid.ScreenChangeType.PushScreen;
				_cachedScreens[screenName].gameObject.GetComponent<BackBtnBehaviourAndroid>().ScreenNameToOpen = "FrontUI";
				break;
			case "PauseUI":
				_cachedScreens[screenName].gameObject.GetComponent<BackBtnBehaviourAndroid>().screenChangeType = BackBtnBehaviourAndroid.ScreenChangeType.PushScreen;
				_cachedScreens[screenName].gameObject.GetComponent<BackBtnBehaviourAndroid>().ScreenNameToOpen = "IngameUI";
				break;
			case "IngameUI":
				_cachedScreens[screenName].gameObject.GetComponent<BackBtnBehaviourAndroid>().screenChangeType = BackBtnBehaviourAndroid.ScreenChangeType.PushScreen;
				_cachedScreens[screenName].gameObject.GetComponent<BackBtnBehaviourAndroid>().ScreenNameToOpen = "PauseUI";
				break;
			}
		}
	}
}
