using System;
using System.Collections;
using UnityEngine;

public class Game : MonoBehaviour
{
	public delegate void OnStageMenuSequenceDelegate();

	public delegate void OnIntroRunDelegate();

	public delegate void OnPauseChangeDelegate(bool pause);

	[Serializable]
	public class SwipeInfo
	{
		public float distanceMin = 0.1f;

		public float doubleTapDuration = 0.3f;
	}

	[Serializable]
	public class SpeedInfo
	{
		public float min = 110f;

		public float max = 220f;

		public float rampUpDuration = 200f;
	}

	public delegate void OnGameOverDelegate(GameStats gameStats);

	public delegate void OnTopMenuDelegate();

	[HideInInspector]
	public bool isDead;

	public bool ingameTouchDetection = true;

	[HideInInspector]
	public float currentSpeed;

	public float currentLevelSpeed = 30f;

	public float distancePerMeter = 8f;

	public SwipeInfo swipe;

	public SpeedInfo speed;

	public float backToCheckpointDelayTime = 0.7f;

	public float backToCheckpointZoomTime = 1f;

	private bool goingBackToCheckpoint;

	public Transform introAnimation;

	private IEnumerator currentThread;

	private CharacterState characterState;

	[HideInInspector]
	public CharacterModifierCollection modifiers;

	private Swipe currentSwipe;

	private float lastTapTime = float.MinValue;

	public static bool HasLoaded;

	private static CharacterController characterController;

	public Character character;

	private CharacterRendering characterRendering;

	private Animation characterAnimation;

	public Track track;

	private CharacterCamera characterCamera;

	private Transform characterCameraTransform;

	private Distort distort;

	private FollowingGuard enemies;

	public Running running;

	private Jetpack jetpack;

	private static Game instance;

	private float startTime;

	private GameStats stats;

	public Action OnGameStarted;

	public Action OnGameEnded;

	public OnGameOverDelegate OnGameOver;

	public OnPauseChangeDelegate OnPauseChange;

	public OnStageMenuSequenceDelegate OnStageMenuSequence;

	public OnTopMenuDelegate OnTopMenu;

	public OnIntroRunDelegate OnIntroRun;

	private float waitTimeBeforeScreen;

	public Variable<bool> IsInGame;

	public Variable<bool> IsInTopMenu;

	public AudioStateLoop audioStateLoop;

	public AudioClipInfo DieSound;

	public bool awakeDone;

	// 视角/控制切换状态（V=第一人称切换，B=切换控制保安）
	[HideInInspector]
	public bool firstPersonView;

	[HideInInspector]
	public bool controllingGuard;

	public float firstPersonEyeHeight = 15f;

	public bool isReadyForHeadStart;

	private bool _paused;

	private float fovInMenuIphone5 = 70f;

	public bool isPaused
	{
		get
		{
			return _paused;
		}
	}

	public Character Character
	{
		get
		{
			return character;
		}
	}

	public CharacterState CharacterState
	{
		get
		{
			return characterState;
		}
	}

	public CharacterModifierCollection Modifiers
	{
		get
		{
			return modifiers;
		}
	}

	public Running Running
	{
		get
		{
			return running;
		}
	}

	public Jetpack Jetpack
	{
		get
		{
			return jetpack;
		}
	}

	public bool IsInJetpackMode
	{
		get
		{
			return characterState == Jetpack;
		}
	}

	public bool HasSuperSneakers
	{
		get
		{
			return modifiers.SuperSneakes.isActive;
		}
	}

	public float NormalizedGameSpeed
	{
		get
		{
			return currentSpeed / speed.min;
		}
	}

	public static Game Instance
	{
		get
		{
			return instance ?? (instance = Utils.FindObject<Game>());
		}
	}

	public static CharacterController Charactercontroller
	{
		get
		{
			return characterController ?? (characterController = UnityEngine.Object.FindObjectOfType(typeof(CharacterController)) as CharacterController);
		}
	}

	public Game()
	{
		IsInGame = new Variable<bool>(false);
		IsInTopMenu = new Variable<bool>(false);
	}

	public void Awake()
	{
		HasLoaded = true;
		character = Character.Instance;
		AttachLightSignalSafeSurfaces();
		character.Initialize();
		characterRendering = CharacterRendering.Instance;
		characterAnimation = characterRendering.characterAnimation;
		track = Track.Instance;
		characterCamera = CharacterCamera.Instance;
		characterCameraTransform = characterCamera.transform;
		distort = this.FindObject<Distort>();
		running = Running.Instance;
		jetpack = Jetpack.Instance;
		enemies = FollowingGuard.Instance;
		enemies.Initialize();
		FollowingGuard followingGuard = enemies;
		followingGuard.OnCatchPlayer = (FollowingGuard.OnCatchPlayerDelegate)Delegate.Combine(followingGuard.OnCatchPlayer, new FollowingGuard.OnCatchPlayerDelegate(OnCatchPlayer));
		modifiers = new CharacterModifierCollection();
		character.OnStumble += OnStumble;
		character.OnCriticalHit += OnCriticalHit;
		currentLevelSpeed = Speed(0f);
		stats = GameStats.Instance;
		awakeDone = true;
		Variable<bool> isInTopMenu = IsInTopMenu;
		isInTopMenu.OnChange = (Variable<bool>.OnChangeDelegate)Delegate.Combine(isInTopMenu.OnChange, (Variable<bool>.OnChangeDelegate)delegate(bool value)
		{
			if (value)
			{
				characterRendering.CharacterModel.StartEyeAnimations();
			}
			else
			{
				characterRendering.CharacterModel.StartEyeAnimations();
			}
		});
	}

	private void AttachLightSignalSafeSurfaces()
	{
		foreach (GameObject gameObject in GameObject.FindObjectsOfType<GameObject>())
		{
			if (gameObject.name.Equals("lightSignal", StringComparison.OrdinalIgnoreCase) && gameObject.GetComponent<LightSignalSafeSurface>() == null)
			{
				gameObject.AddComponent<LightSignalSafeSurface>();
			}
		}
	}

	public void Start()
	{
		track.Restart();
		currentThread = GameIntro();
		currentThread.MoveNext();
	}

	public void StartNewRun()
	{
		IsInTopMenu.Value = false;
		IsInGame.Value = true;
		// 每局开始重置视角/保安控制状态，避免上一局残留
		firstPersonView = false;
		controllingGuard = false;
		lastChaosStage = 0;
		bossBannerHideTime = 0f;
		if (enemies != null)
		{
			enemies.SetManualControl(false);
		}
		ChangeState(null, Intro());
		Action onGameStarted = OnGameStarted;
		if (onGameStarted != null)
		{
			onGameStarted();
		}
	}

	public void Update()
	{
		HandleDebugControls();
		float t = Time.time - startTime;
		currentLevelSpeed = Speed(t);
		currentThread.MoveNext();
		if (characterState != null)
		{
			modifiers.Update();
		}
		GameStats.Instance.UpdatePowerupTimes(Time.deltaTime);
		// 检测混乱度跨越整数阶段，触发大屏 BOSS 阶段横幅
		if (IsInGame.Value && character != null)
		{
			int stage = Mathf.FloorToInt(character.z / distancePerMeter / 1000f);
			if (stage > lastChaosStage)
			{
				lastChaosStage = stage;
				bossBannerText = "BOSS进入第" + stage + "阶段！";
				bossBannerHideTime = Time.time + 2.5f;
			}
		}
	}

	// BOSS 阶段横幅
	private int lastChaosStage;

	private string bossBannerText = "";

	private float bossBannerHideTime;

	private static Texture2D _chaosBarTex;

	private static Texture2D ChaosBarTex
	{
		get
		{
			if (_chaosBarTex == null)
			{
				_chaosBarTex = new Texture2D(1, 1);
				_chaosBarTex.SetPixel(0, 0, Color.white);
				_chaosBarTex.Apply();
			}
			return _chaosBarTex;
		}
	}

	// 屏幕上方进度条：显示混乱度 chaos 到达下一个整数的进度
	public void OnGUI()
	{
		if (!IsInGame.Value || character == null)
		{
			return;
		}
		float meters = character.z / distancePerMeter;
		float chaos = meters / 1000f;
		float floor = Mathf.Floor(chaos);
		float progress = chaos - floor;
		float w = (float)Screen.width * 0.6f;
		float h = 20f;
		float x = ((float)Screen.width - w) * 0.5f;
		float y = 12f;
		Color old = GUI.color;
		// 背景
		GUI.color = new Color(0f, 0f, 0f, 0.55f);
		GUI.DrawTexture(new Rect(x, y, w, h), ChaosBarTex);
		// 填充（越接近下一整数越满）
		GUI.color = new Color(1f, 0.35f, 0.1f, 0.95f);
		GUI.DrawTexture(new Rect(x, y, w * progress, h), ChaosBarTex);
		GUI.color = old;
		GUI.Label(new Rect(x + 6f, y + 1f, w, h), "boss即将进入第" + (floor + 1f).ToString("F0") + "阶段   " + (progress * 100f).ToString("F0") + "%");
		// 大屏 BOSS 阶段横幅
		if (Time.time < bossBannerHideTime && !string.IsNullOrEmpty(bossBannerText))
		{
			GUIStyle style = new GUIStyle(GUI.skin.label);
			style.fontSize = Mathf.Max(24, (int)((float)Screen.height * 0.07f));
			style.fontStyle = FontStyle.Bold;
			style.alignment = TextAnchor.MiddleCenter;
			style.normal.textColor = new Color(1f, 0.2f, 0.1f, 1f);
			GUI.Label(new Rect(0f, (float)Screen.height * 0.32f, (float)Screen.width, (float)Screen.height * 0.15f), bossBannerText, style);
		}
	}

	public void LayTrackChunks()
	{
		float z = character.z;
		// 玩家接管保安时，用主角与保安中较靠前的 z 铺设地形，保证保安独立前进时脚下始终有轨道
		if (enemies != null && enemies.manualControl)
		{
			z = Mathf.Max(z, enemies.GuardZ);
		}
		track.LayTrackChunks(z);
	}

	private void HandleDebugControls()
	{
		if (Input.GetKeyDown(KeyCode.F))
		{
			modifiers.Add(modifiers.SuperSneakes);
		}
		if (Input.GetKeyDown(KeyCode.M))
		{
			// 主角临时穿过火车（幽灵状态），持续 5 秒后恢复
			character.GhostThroughTrains(5f);
		}
		if (Input.GetKeyDown(KeyCode.X))
		{
			modifiers.Stop();
		}
		// V：切换当前控制目标的第一/第三人称视角
		if (Input.GetKeyDown(KeyCode.V))
		{
			firstPersonView = !firstPersonView;
		}
		// B：在“控制玩家角色”与“控制保安”之间切换；控制保安时原角色自动向前跑
		if (Input.GetKeyDown(KeyCode.B))
		{
			controllingGuard = !controllingGuard;
			if (enemies != null)
			{
				enemies.SetManualControl(controllingGuard);
			}
		}
		if (characterState != null)
		{
			if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
			{
				characterState.HandleSwipe(SwipeDir.Up);
			}
			if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
			{
				characterState.HandleSwipe(SwipeDir.Down);
			}
			if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
			{
				characterState.HandleSwipe(SwipeDir.Left);
			}
			if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
			{
				characterState.HandleSwipe(SwipeDir.Right);
			}
		}
	}

	public void UpdateMeters()
	{
		stats.meters = Mathf.RoundToInt(character.z / distancePerMeter);
	}

	public float CalcTime(float z)
	{
		if (z <= Position(speed.rampUpDuration))
		{
			float f = speed.min * speed.min + 2f * ((speed.max - speed.min) / speed.rampUpDuration) * z;
			return (0f - speed.min + Mathf.Sqrt(f)) / ((speed.max - speed.min) / speed.rampUpDuration);
		}
		return (z - Position(speed.rampUpDuration)) * 1f / speed.max + speed.rampUpDuration;
	}

	public void ChangeState(CharacterState state)
	{
		characterState = state;
		if (state != null)
		{
			currentThread = state.Begin();
		}
	}

	public void ChangeState(CharacterState state, IEnumerator thread)
	{
		characterState = state;
		currentThread = thread;
	}

	public void ActivateJetpack()
	{
		if (characterState != Jetpack)
		{
			ChangeState(Jetpack);
		}
	}

	private float Speed(float t)
	{
		if (t < speed.rampUpDuration)
		{
			return t * (speed.max - speed.min) / speed.rampUpDuration + speed.min;
		}
		return speed.max;
	}

	private float Position(float t)
	{
		if (t < speed.rampUpDuration)
		{
			return 0.5f * ((speed.max - speed.min) / speed.rampUpDuration) * t * t + speed.min * t;
		}
		return (t - speed.rampUpDuration) * speed.max + 0.5f * (speed.max - speed.min) * speed.rampUpDuration + speed.min * speed.rampUpDuration;
	}

	public void Die()
	{
		if (modifiers.IsActive(modifiers.Hoverboard))
		{
			enemies.MuteProximityLoop();
			enemies.ResetCatchUp();
			if (character.IsStumbling)
			{
				character.StopStumble();
			}
			enemies.Restart(false);
			modifiers.Hoverboard.Stop = CharacterModifier.StopSignal.STOP;
			GameStats.Instance.RemoveHoverBoardPowerup();
			return;
		}
		if (track.IsRunningOnTutorialTrack)
		{
			if (!goingBackToCheckpoint)
			{
				StartCoroutine(BackToCheckPointSequence());
			}
			return;
		}
		Screen.sleepTimeout = -2;
		GameStats.Instance.ClearPowerups();
		isDead = true;
		MovingTrain.ActivateAutoPilot();
		MovingCoin.ActivateAutoPilot();
		if (enemies.isShowing)
		{
			if (characterAnimation["death_movingTrain"].enabled)
			{
				enemies.HitByTrainSequence();
			}
			else
			{
				enemies.CatchPlayer(character.x - character.GetTrackX());
			}
		}
		stats.duration = GetDuration();
		Missions.Instance.PlayerDidThis(Missions.MissionTarget.TimeDeath, Mathf.FloorToInt(GameStats.Instance.duration));
		enemies.enabled = false;
		if (OnGameOver != null)
		{
			OnGameOver(stats);
		}
		Action onGameEnded = OnGameEnded;
		if (onGameEnded != null)
		{
			onGameEnded();
		}
		StopAllCoroutines();
		ChangeState(null, SwitchToDieStateWhenGrounded());
	}

	public float GetDuration()
	{
		return Time.time - startTime;
	}

	private IEnumerator SwitchToDieStateWhenGrounded()
	{
		while (!character.characterController.isGrounded)
		{
			character.MoveWithGravity();
			yield return null;
		}
		ChangeState(null, DieSequence());
	}

	public void OnCriticalHit(Character.CriticalHitType type)
	{
		if (characterState != null)
		{
			if (type == Character.CriticalHitType.Train || type == Character.CriticalHitType.MovingTrain)
			{
				characterState.HandleCriticalHit();
				return;
			}
			So.Instance.playSound(DieSound);
			characterState.HandleCriticalHit();
		}
	}

	private IEnumerator StumbleDeathSequence()
	{
		currentSpeed = speed.min;
		yield return new WaitForSeconds(0.2f);
		if (characterState != Jetpack)
		{
			characterAnimation.CrossFade("stumbleFall", 0.2f);
			if (characterState != null)
			{
				characterState.HandleCriticalHit();
			}
		}
	}

	public void OnStumble(Character.StumbleType stumbleType, Character.StumbleHorizontalHit horizontalHit, Character.StumbleVerticalHit verticalHit, string colliderName)
	{
		if (character.IsStumbling && characterState != null)
		{
			StartCoroutine(StumbleDeathSequence());
		}
	}

	public void StartJetpack()
	{
		Jetpack.headStart = false;
		Jetpack.powerType = PowerupType.jetpack;
		ChangeState(Jetpack);
	}

	public void PickupJetpack()
	{
		Instance.StartJetpack();
		GameStats.Instance.jetpackPickups++;
	}

	public void StartTopMenu()
	{
		ChangeState(null, TopMenu());
	}

	public void StartHeadStart2000()
	{
		if (!isDead)
		{
			float powerupDuration = PlayerInfo.Instance.GetPowerupDuration(PowerupType.headstart2000);
			Jetpack.headStart = true;
			Jetpack.powerType = PowerupType.headstart2000;
			Jetpack.headStartDistance = powerupDuration * distancePerMeter;
			Jetpack.headStartSpeed = 1000f;
			ChangeState(Jetpack);
			PlayerInfo.Instance.UseUpgrade(PowerupType.headstart2000);
			Missions.Instance.PlayerDidThis(Missions.MissionTarget.Headstart);
			Missions.Instance.PlayerDidThis(Missions.MissionTarget.HaveHeadStartLarge, -1);
		}
	}

	public void StartHeadStart500()
	{
		if (!isDead)
		{
			float powerupDuration = PlayerInfo.Instance.GetPowerupDuration(PowerupType.headstart500);
			Jetpack.headStart = true;
			Jetpack.powerType = PowerupType.headstart500;
			Jetpack.headStartDistance = powerupDuration * distancePerMeter;
			Jetpack.headStartSpeed = 1000f;
			ChangeState(Jetpack);
			PlayerInfo.Instance.UseUpgrade(PowerupType.headstart500);
			Missions.Instance.PlayerDidThis(Missions.MissionTarget.Headstart);
		}
	}

	private void OnCatchPlayer(string currentCharacterCaught, float catchUpTime, float waitTime)
	{
		waitTimeBeforeScreen = waitTime;
	}

	private IEnumerator DieSequence()
	{
		float wait = Time.time + waitTimeBeforeScreen;
		float skipTime = Time.time + 0.5f;
		while (Time.time < skipTime)
		{
			yield return null;
		}
		while (Time.time < wait && !Input.GetMouseButtonUp(0))
		{
			if (Input.touchCount > 0)
			{
				Touch touch = Input.touches[0];
				if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
				{
					break;
				}
			}
			yield return null;
		}
		ingameTouchDetection = false;
		UIScreenController.Instance.GameOverTriggered();
		ChangeState(null, TopMenu());
	}

	private void StageMenuSequence()
	{
		enemies.enabled = false;
		enemies.ShowEnemies(false);
		enemies.StopAllCoroutines();
		character.StopAllCoroutines();
		character.transform.position = Vector3.zero + new Vector3(0f, 0f, 0f);
		characterCamera.enabled = false;
		characterCamera.GetComponent<Camera>().fieldOfView = 43f / characterCamera.GetComponent<Camera>().aspect;
		characterCameraTransform.localPosition = character.transform.position + Running.cameraOffset + Vector3.up * 0.8f;
		characterCameraTransform.localRotation = Quaternion.Euler(21.50143f, 0f, 0f);
		if (OnStageMenuSequence != null)
		{
			OnStageMenuSequence();
		}
	}

	private IEnumerator GameIntro()
	{
		if (UIScreenController.isInstanced)
		{
			UIScreenController.Instance.ShowMainMenu();
		}
		ChangeState(null, TopMenu());
		yield break;
	}

	private IEnumerator TopMenu()
	{
		IsInGame.Value = false;
		IsInTopMenu.Value = true;
		audioStateLoop.ChangeLoop(AudioState.Menu);
		enemies.MuteProximityLoop();
		track.DeactivateTrackChunks();
		modifiers.StopWithNoEnding();
		modifiers.Update();
		GameStats.Instance.ClearPowerups();
		jetpack.coinsManager.ReleaseCoins();
		distort.Reset();
		enemies.ShowEnemies(false);
		StageMenuSequence();
		characterCamera.transform.parent.GetComponent<Animation>().CrossFade("menuIdle", 0.1f);
		if (OnTopMenu != null)
		{
			OnTopMenu();
		}
		yield return null;
	}

	private IEnumerator Intro()
	{
		stats.Reset();
		modifiers.Stop();
		modifiers.Update();
		audioStateLoop.ChangeLoop(AudioState.Ingame);
		enemies.MuteProximityLoop();
		isDead = false;
		ingameTouchDetection = true;
		character.CharacterPickupParticleSystem.CoinEFX.transform.localPosition = CharacterPickupParticles.coinEfxOffset;
		StageMenuSequence();
		enemies.ShowEnemies(true);
		enemies.PlayIntro();
		currentLevelSpeed = Speed(0f);
		startTime = Time.time;
		character.Restart();
		SpawnPointManager.Instance.Restart();
		track.Restart();
		track.LayTrackChunks(0f);
		distort.Reset();
		characterCamera.transform.parent.GetComponent<Animation>().CrossFade("startPan", 0.2f);
		characterAnimation.CrossFade("introRun", 0.2f);
		if (OnIntroRun != null)
		{
			OnIntroRun();
		}
		IEnumerator cameraMovement = pTween.To(characterAnimation.GetComponent<Animation>()["introRun"].length, delegate
		{
		});
		float time = Time.time;
		float fov_start = characterCamera.GetComponent<Camera>().fieldOfView;
		float fov_end = Running.cameraFOV / characterCamera.GetComponent<Camera>().aspect;
		while (cameraMovement.MoveNext())
		{
			characterCamera.GetComponent<Camera>().fieldOfView = Mathf.Lerp(fov_start, fov_end, (Time.time - time) * 0.75f);
			yield return null;
		}
		characterCamera.GetComponent<Camera>().fieldOfView = fov_end;
		stats.Reset();
		enemies.enabled = true;
		if (track.IsRunningOnTutorialTrack)
		{
			enemies.ResetCatchUp();
		}
		isReadyForHeadStart = true;
		ChangeState(Running);
		yield return null;
	}

	public void TriggerPause(bool pauseGame)
	{
		_paused = pauseGame;
		if (pauseGame)
		{
			ingameTouchDetection = false;
			Time.timeScale = 0f;
		}
		else
		{
			ingameTouchDetection = true;
			Time.timeScale = 1f;
		}
		if (OnPauseChange != null)
		{
			OnPauseChange(_paused);
		}
	}

	private bool HandleTap()
	{
		bool result = false;
		if (Time.time < lastTapTime + swipe.doubleTapDuration && characterState != null)
		{
			characterState.HandleDoubleTap();
			result = true;
		}
		lastTapTime = Time.time;
		return result;
	}

	public void HandleControls()
	{
		if (_paused)
		{
			return;
		}
		if (Input.touchCount <= 0)
		{
			// 无触摸设备（如 PC）时，用鼠标按住拖动来模拟屏幕滑动操控
			HandleMouseSwipe();
			return;
		}
		Touch touch = Input.touches[0];
		if (touch.phase == TouchPhase.Began)
		{
			currentSwipe = new Swipe();
			currentSwipe.start = touch.position;
			currentSwipe.startTime = Time.time;
		}
		if ((touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) && currentSwipe != null)
		{
			currentSwipe.endTime = Time.time;
			currentSwipe.end = touch.position;
			SwipeDir swipeDir = AnalyzeSwipe(currentSwipe);
			if (swipeDir != SwipeDir.None)
			{
				if (characterState != null)
				{
					characterState.HandleSwipe(swipeDir);
				}
				currentSwipe = null;
			}
		}
		if (touch.phase == TouchPhase.Ended && currentSwipe != null)
		{
			currentSwipe.endTime = Time.time;
			currentSwipe.end = touch.position;
			SwipeDir swipeDir2 = AnalyzeSwipe(currentSwipe);
			if (swipeDir2 == SwipeDir.None && characterState != null)
			{
				HandleTap();
			}
		}
	}

	// 用鼠标按住拖动模拟屏幕滑动：按下=开始，拖动足够距离=触发方向，松开无滑动=点按
	private void HandleMouseSwipe()
	{
		if (Input.GetMouseButtonDown(0))
		{
			currentSwipe = new Swipe();
			currentSwipe.start = Input.mousePosition;
			currentSwipe.startTime = Time.time;
		}
		else if (Input.GetMouseButton(0) && currentSwipe != null)
		{
			currentSwipe.endTime = Time.time;
			currentSwipe.end = Input.mousePosition;
			SwipeDir swipeDir = AnalyzeSwipe(currentSwipe);
			if (swipeDir != SwipeDir.None)
			{
				if (characterState != null)
				{
					characterState.HandleSwipe(swipeDir);
				}
				currentSwipe = null;
			}
		}
		else if (Input.GetMouseButtonUp(0) && currentSwipe != null)
		{
			currentSwipe.endTime = Time.time;
			currentSwipe.end = Input.mousePosition;
			SwipeDir swipeDir = AnalyzeSwipe(currentSwipe);
			if (swipeDir == SwipeDir.None && characterState != null)
			{
				HandleTap();
			}
			currentSwipe = null;
		}
	}

	private SwipeDir AnalyzeSwipe(Swipe swipe)
	{
		Vector3 b = Camera.main.ScreenToWorldPoint(new Vector3(swipe.start.x, swipe.start.y, 2f));
		Vector3 a = Camera.main.ScreenToWorldPoint(new Vector3(swipe.end.x, swipe.end.y, 2f));
		float num = Vector3.Distance(a, b);
		if (num < this.swipe.distanceMin)
		{
			return SwipeDir.None;
		}
		Vector3 lhs = swipe.end - swipe.start;
		SwipeDir result = SwipeDir.None;
		float num2 = 0f;
		float num3 = Vector3.Dot(lhs, Vector3.up);
		if (num3 > num2)
		{
			num2 = num3;
			result = SwipeDir.Up;
		}
		num3 = Vector3.Dot(lhs, Vector3.down);
		if (num3 > num2)
		{
			num2 = num3;
			result = SwipeDir.Down;
		}
		num3 = Vector3.Dot(lhs, Vector3.left);
		if (num3 > num2)
		{
			num2 = num3;
			result = SwipeDir.Left;
		}
		num3 = Vector3.Dot(lhs, Vector3.right);
		if (num3 > num2)
		{
			num2 = num3;
			result = SwipeDir.Right;
		}
		return result;
	}

	private IEnumerator BackToCheckPointSequence()
	{
		goingBackToCheckpoint = true;
		ChangeState(null);
		yield return new WaitForSeconds(backToCheckpointDelayTime);
		character.SetBackToCheckPoint(backToCheckpointZoomTime);
		yield return new WaitForSeconds(backToCheckpointZoomTime);
		goingBackToCheckpoint = false;
	}
}
