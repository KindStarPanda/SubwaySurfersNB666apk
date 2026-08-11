using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CharacterRendering : MonoBehaviour
{
	public class Animations
	{
		public string[] TOP_MENU = new string[1] { "topMenu" };

		public string[] RUN = new string[1] { "run" };

		public string[] LAND = new string[1] { "land" };

		public string[] JUMP = new string[1] { "jump" };

		public string[] HANGTIME = new string[1] { "hangtime" };

		public string[] ROLL = new string[1] { "roll" };

		public string[] DODGE_LEFT = new string[1] { "dodgeLeft" };

		public string[] DODGE_RIGHT = new string[1] { "dodgeRight" };

		public string[] GRIND = new string[1] { "grind" };

		public string[] GET_ON_BOARD = new string[1] { "getOnBoard" };

		public string[] HIT_MID = new string[1] { "hitMid" };

		public string[] HIT_UPPER = new string[1] { "hitUpper" };

		public string[] HIT_LOWER = new string[1] { "hitLower" };

		public string[] HIT_MOVING = new string[1] { "hitMoving" };

		public string[] STUMBLE = new string[1] { "stumble_low" };

		public string[] STUMBLE_MIX = new string[1] { "stumble" };

		public string[] STUMBLE_LEFT_SIDE = new string[1] { "stumbleLeftSide" };

		public string[] STUMBLE_RIGHT_SIDE = new string[1] { "stumbleRightSide" };

		public string[] STUMBLE_LEFT_CORNER = new string[1] { "stumbleLeftCorner" };

		public string[] STUMBLE_RIGHT_CORNER = new string[1] { "stumbleRightCorner" };

		public string DEFAULT_HOVERBOARD_ANIMATION;

		public string TopMenu
		{
			get
			{
				return GetRandomAnimationName(TOP_MENU);
			}
		}

		public string Run
		{
			get
			{
				return GetRandomAnimationName(RUN);
			}
		}

		public string Land
		{
			get
			{
				return GetRandomAnimationName(LAND);
			}
		}

		public string Jump
		{
			get
			{
				return GetRandomAnimationName(JUMP);
			}
		}

		public string Hangtime
		{
			get
			{
				return GetRandomAnimationName(HANGTIME);
			}
		}

		public string[] HoverboardJump
		{
			get
			{
				return GetRandomHoverJumps(JUMP, HANGTIME);
			}
		}

		public string Roll
		{
			get
			{
				return GetRandomAnimationName(ROLL);
			}
		}

		public string DodgeLeft
		{
			get
			{
				return GetRandomAnimationName(DODGE_LEFT);
			}
		}

		public string DodgeRight
		{
			get
			{
				return GetRandomAnimationName(DODGE_RIGHT);
			}
		}

		public string Grind
		{
			get
			{
				return GetRandomAnimationName(GRIND);
			}
		}

		public string GetOnBoard
		{
			get
			{
				return GetRandomAnimationName(GET_ON_BOARD);
			}
		}

		public string HitMid
		{
			get
			{
				return GetRandomAnimationName(HIT_MID);
			}
		}

		public string HitUpper
		{
			get
			{
				return GetRandomAnimationName(HIT_UPPER);
			}
		}

		public string HitLower
		{
			get
			{
				return GetRandomAnimationName(HIT_LOWER);
			}
		}

		public string HitMoving
		{
			get
			{
				return GetRandomAnimationName(HIT_MOVING);
			}
		}

		public string Stumble
		{
			get
			{
				return GetRandomAnimationName(STUMBLE);
			}
		}

		public string StumbleMix
		{
			get
			{
				return GetRandomAnimationName(STUMBLE_MIX);
			}
		}

		public string StumbleLeftSide
		{
			get
			{
				return GetRandomAnimationName(STUMBLE_LEFT_SIDE);
			}
		}

		public string StumbleRightSide
		{
			get
			{
				return GetRandomAnimationName(STUMBLE_RIGHT_SIDE);
			}
		}

		public string StumbleLeftCorner
		{
			get
			{
				return GetRandomAnimationName(STUMBLE_LEFT_CORNER);
			}
		}

		public string StumbleRightCorner
		{
			get
			{
				return GetRandomAnimationName(STUMBLE_RIGHT_CORNER);
			}
		}

		private string GetRandomAnimationName(string[] animationsNames)
		{
			int num = UnityEngine.Random.Range(0, animationsNames.Length);
			return animationsNames[num];
		}

		private string[] GetRandomHoverJumps(string[] hoverboardJump, string[] hoverboardHangtime)
		{
			if (hoverboardJump.Length != hoverboardHangtime.Length)
			{
				Debug.LogWarning("hoverboardJump.Length (" + hoverboardJump.Length + ") != (" + hoverboardHangtime.Length + ") hoverboardHangtime.Length");
			}
			int num = UnityEngine.Random.Range(0, Mathf.Min(hoverboardJump.Length, hoverboardHangtime.Length));
			return new string[2]
			{
				hoverboardJump[num],
				hoverboardHangtime[num]
			};
		}
	}

	[Serializable]
	public class AnimationClipLists
	{
		public AnimationClip[] topMenu;

		public AnimationClip[] run;

		public AnimationClip[] jump;

		public AnimationClip[] hangtime;

		public AnimationClip[] landing;

		public AnimationClip[] dodgeLeft;

		public AnimationClip[] dodgeRight;

		public AnimationClip[] roll;

		public AnimationClip[] hitMid;

		public AnimationClip[] hitUpper;

		public AnimationClip[] hitLower;

		public AnimationClip[] hitMoving;

		public AnimationClip[] stumble;

		public AnimationClip[] stumbleMix;

		public AnimationClip[] stumbleDeath;

		public AnimationClip[] stumbleLeftSide;

		public AnimationClip[] stumbleRightSide;

		public AnimationClip[] stumbleLeftCorner;

		public AnimationClip[] stumbleRightCorner;
	}

	[Serializable]
	public class JetpackClips
	{
		public AnimationClip[] run;

		public AnimationClip[] dodgeLeft;

		public AnimationClip[] dodgeRight;
	}

	[Serializable]
	public class SuperSneaksClips
	{
		public AnimationClip[] run;
	}

	public delegate void CharacterModelInitializedDelegate(GameObject hoverboardRoot);

	[SerializeField]
	private AnimationClipLists defaultAnimations;

	[SerializeField]
	private JetpackClips jetpackAnimations;

	[SerializeField]
	private SuperSneaksClips SuperSneaksAnimations;

	public Animation characterAnimation;

	private Animation hoverboardAnimation;

	private HoverboardRendering hoverboardRendering;

	private List<AnimationClip> addedAnimClipsNames = new List<AnimationClip>();

	[SerializeField]
	private AnimationCurve jetpackParticleOffsetCurve;

	public Animations animations;

	[SerializeField]
	private GameObject characterModelPrefab;

	private GameObject currentHoverboard;

	[SerializeField]
	private GameObject characterRenderingEffectsPrefab;

	private Vector3 initRot;

	private Vector3 initScale;

	private ParticleSystem[] particleToKill;

	private AnimationState caught;

	private Game game;

	private Character character;

	private Hoverboard hoverboard;

	private SuperSneakers superSneakers;

	private Jetpack jetpack;

	private FollowingGuard followingGuard;

	private CharacterController characterController;

	private MeshRenderer shadow;

	private CharacterModel characterModel;

	private CharacterRenderingEffects characterRenderingEffects;

	private string jumpAnimation;

	private string hangtimeAnimation;

	private static CharacterRendering instance;

	public CharacterModel CharacterModel
	{
		get
		{
			return characterModel;
		}
	}

	public static CharacterRendering Instance
	{
		get
		{
			return instance ?? (instance = Utils.FindObject<CharacterRendering>());
		}
	}

	[method: MethodImpl(32)]
	public event CharacterModelInitializedDelegate CharacterModelInitialized;

	public void Initialize()
	{
		InitializeCharacterModel();
		InitializeCharacterRenderingEffects();
		InitializeAnimations();
		game = Game.Instance;
		character = Character.Instance;
		characterController = Game.Charactercontroller;
		hoverboard = Hoverboard.Instance;
		superSneakers = this.FindObject<SuperSneakers>();
		jetpack = Jetpack.Instance;
		followingGuard = FollowingGuard.Instance;
		Variable<bool> isInGame = game.IsInGame;
		isInGame.OnChange = (Variable<bool>.OnChangeDelegate)Delegate.Combine(isInGame.OnChange, new Variable<bool>.OnChangeDelegate(IsInGame_OnChange));
		character.OnChangeTrack += OnChangeTrack;
		character.OnStumble += OnStumble;
		character.OnTutorialMoveBackToCheckPoint += OnTutorialMoveBackToCheckPoint;
		character.OnTutorialStartFromCheckPoint += OnTutorialStartFromCheckPoint;
		character.OnHitByTrain += OnHitByTrain;
		character.OnJump += OnJump;
		character.OnRoll += OnRoll;
		character.OnLanding += OnLanding;
		character.OnHangtime += OnHangtime;
		Variable<bool> isGrounded = character.IsGrounded;
		isGrounded.OnChange = (Variable<bool>.OnChangeDelegate)Delegate.Combine(isGrounded.OnChange, new Variable<bool>.OnChangeDelegate(OnChangeIsGrounded));
		Game obj = game;
		obj.OnStageMenuSequence = (Game.OnStageMenuSequenceDelegate)Delegate.Combine(obj.OnStageMenuSequence, new Game.OnStageMenuSequenceDelegate(OnStageMenuSequence));
		Game obj2 = game;
		obj2.OnIntroRun = (Game.OnIntroRunDelegate)Delegate.Combine(obj2.OnIntroRun, new Game.OnIntroRunDelegate(OnIntroRun));
		hoverboard.OnSwitchToHoverboard += OnSwitchToHoverboard;
		hoverboard.OnSwitchToRunning += OnSwitchToRunning;
		hoverboard.OnJump += OnJump;
		hoverboard.OnRun += OnRun;
		Jetpack obj3 = jetpack;
		obj3.OnStart = (Jetpack.OnStartDelegate)Delegate.Combine(obj3.OnStart, new Jetpack.OnStartDelegate(OnSwitchToJetpack));
		Jetpack obj4 = jetpack;
		obj4.OnStop = (Jetpack.OnStopDelegate)Delegate.Combine(obj4.OnStop, new Jetpack.OnStopDelegate(JetpackOnStop));
		Jetpack obj5 = jetpack;
		obj5.OnFlyAheadStart = (Jetpack.OnFlyAheadStartDelegate)Delegate.Combine(obj5.OnFlyAheadStart, new Jetpack.OnFlyAheadStartDelegate(JetpackOnFlyAheadStart));
		Jetpack obj6 = jetpack;
		obj6.OnFlyAheadUpdate = (Jetpack.OnFlyAheadUpdateDelegate)Delegate.Combine(obj6.OnFlyAheadUpdate, new Jetpack.OnFlyAheadUpdateDelegate(JetpackOnFlyAheadUpdate));
		superSneakers.OnSwitchToSuperSneakers += OnSwitchToSuperSneakers;
		superSneakers.SuperSneakerOnStop += SuperSneakersOnStop;
		FollowingGuard obj7 = followingGuard;
		obj7.OnCatchPlayer = (FollowingGuard.OnCatchPlayerDelegate)Delegate.Combine(obj7.OnCatchPlayer, new FollowingGuard.OnCatchPlayerDelegate(OnCatchPlayer));
	}

	private void Start()
	{
		if (this.CharacterModelInitialized != null)
		{
			this.CharacterModelInitialized(characterModel.meshHoverboard.gameObject);
		}
		characterAnimation["jetpack_forward"].time = 1f;
	}

	private void InitializeCharacterModel()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(characterModelPrefab) as GameObject;
		gameObject.transform.parent = base.transform;
		gameObject.transform.localPosition = Vector3.zero;
		particleToKill = gameObject.GetComponentsInChildren<ParticleSystem>();
		characterModel = gameObject.GetComponent<CharacterModel>();
		shadow = characterModel.shadow;
		characterAnimation = characterModel.characterAnimation;
	}

	private void InitializeCharacterRenderingEffects()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(characterRenderingEffectsPrefab) as GameObject;
		characterRenderingEffects = gameObject.GetComponent<CharacterRenderingEffects>();
		characterRenderingEffects.Initialize(characterModel);
	}

	private void InitializeAnimations()
	{
		animations = new Animations();
		animations.TOP_MENU = InitializeClips(defaultAnimations.topMenu);
		animations.HIT_MID = InitializeClips(defaultAnimations.hitMid);
		animations.HIT_UPPER = InitializeClips(defaultAnimations.hitUpper);
		animations.HIT_LOWER = InitializeClips(defaultAnimations.hitLower);
		animations.HIT_MOVING = InitializeClips(defaultAnimations.hitMoving);
		animations.STUMBLE = InitializeClips(defaultAnimations.stumble);
		animations.STUMBLE_MIX = InitializeClips(defaultAnimations.stumbleMix);
		animations.STUMBLE_LEFT_SIDE = InitializeClips(defaultAnimations.stumbleLeftSide);
		animations.STUMBLE_RIGHT_SIDE = InitializeClips(defaultAnimations.stumbleRightSide);
		animations.STUMBLE_LEFT_CORNER = InitializeClips(defaultAnimations.stumbleLeftCorner);
		animations.STUMBLE_RIGHT_CORNER = InitializeClips(defaultAnimations.stumbleRightCorner);
		characterAnimation["caught"].layer = 4;
		characterAnimation["caught"].enabled = false;
		characterAnimation["caught2"].layer = 4;
		characterAnimation["caught2"].enabled = false;
		characterAnimation["stumble"].AddMixingTransform(characterModel.spineTransform);
		characterAnimation["stumble"].layer = 2;
		characterAnimation["stumble"].weight = 1f;
		characterAnimation["stumbleCornerLeft"].AddMixingTransform(characterModel.spineTransform);
		characterAnimation["stumbleCornerLeft"].layer = 2;
		characterAnimation["stumbleCornerLeft"].weight = 1f;
		characterAnimation["stumbleCornerRight"].AddMixingTransform(characterModel.spineTransform);
		characterAnimation["stumbleCornerRight"].layer = 2;
		characterAnimation["stumbleCornerRight"].weight = 1f;
	}

	private string[] InitializeClips(AnimationClip[] clips)
	{
		string[] array = new string[clips.Length];
		for (int i = 0; i < clips.Length; i++)
		{
			AnimationClip animationClip = clips[i];
			if (!addedAnimClipsNames.Contains(animationClip))
			{
				addedAnimClipsNames.Add(animationClip);
				characterAnimation.AddClip(animationClip, animationClip.name);
			}
			array[i] = animationClip.name;
		}
		return array;
	}

	private string GetHoverboardAnimationName(string animationName)
	{
		AnimationClip clip = hoverboardAnimation.GetClip(animationName);
		if (clip != null)
		{
			return animationName;
		}
		return animations.DEFAULT_HOVERBOARD_ANIMATION;
	}

	private bool IsSpikeCharacter()
	{
		return PlayerInfo.Instance != null && PlayerInfo.Instance.currentCharacter == (int)Characters.CharacterType.spike;
	}

	private void OnIntroRun()
	{
		ParticleSystem[] array = particleToKill;
		foreach (ParticleSystem particleSystem in array)
		{
			particleSystem.Stop();
		}
		OnSwitchToRunning();
		string run = animations.Run;
		characterAnimation.CrossFadeQueued(run, 0.2f);
		characterModel.sprayCanModel.SetActiveRecursively(false);
	}

	private void OnRun()
	{
		if (character.IsFalling || character.IsJumping)
		{
			return;
		}
		string run = animations.Run;
		if (characterController.isGrounded)
		{
			characterAnimation.CrossFade(run);
			if (IsSpikeCharacter() && followingGuard != null)
			{
				followingGuard.PlayMirrorAnimation("Guard_Run");
			}
			if (hoverboardAnimation != null)
			{
				hoverboardAnimation.CrossFade(GetHoverboardAnimationName(run));
			}
		}
	}

	private void OnChangeTrack(Character.OnChangeTrackDirection direction)
	{
		if (characterController.isGrounded)
		{
			string animationName = ((direction != 0) ? animations.DodgeRight : animations.DodgeLeft);
			if (!hoverboard.isActive && !jetpack.isActive)
			{
				characterAnimation[animationName].speed = Game.Instance.NormalizedGameSpeed;
			}
			else
			{
				characterAnimation[animationName].speed = 1f;
			}
			characterAnimation.CrossFade(animationName, 0.02f);
			if (IsSpikeCharacter() && followingGuard != null)
			{
				followingGuard.PlayMirrorAnimation((direction != Character.OnChangeTrackDirection.Left) ? "guard_changelane_right" : "guard_changelane_left", false, 0.02f);
			}
			if (hoverboard.isActive && hoverboardAnimation != null)
			{
				hoverboardAnimation.CrossFade(GetHoverboardAnimationName(animationName), 0.02f);
			}
		}
		if (!character.IsJumping)
		{
			string run = animations.Run;
			characterAnimation.CrossFadeQueued(run, (!game.Modifiers.IsActive(game.Modifiers.Hoverboard)) ? 0.02f : 0.4f);
			if (hoverboard.isActive && hoverboardAnimation != null)
			{
				hoverboardAnimation.CrossFadeQueued(GetHoverboardAnimationName(run), 0.02f);
			}
		}
	}

	private void OnStumble(Character.StumbleType stumbleType, Character.StumbleHorizontalHit horizontalHit, Character.StumbleVerticalHit verticalHit, string colliderName)
	{
		if (stumbleType == Character.StumbleType.Bush || colliderName == "lightSignal" || colliderName == "powerbox")
		{
			characterAnimation.CrossFade(animations.StumbleMix, 0.05f);
			characterAnimation.CrossFadeQueued(animations.Run, 0.5f);
			return;
		}
		if (stumbleType == Character.StumbleType.Side)
		{
			if (!game.Modifiers.IsActive(game.Modifiers.Hoverboard) && !game.IsInJetpackMode)
			{
				if (horizontalHit == Character.StumbleHorizontalHit.LeftCorner || horizontalHit == Character.StumbleHorizontalHit.Left)
				{
					characterAnimation.CrossFade(animations.StumbleLeftSide, 0.2f);
				}
				if (horizontalHit == Character.StumbleHorizontalHit.RightCorner || horizontalHit == Character.StumbleHorizontalHit.Right)
				{
					characterAnimation.CrossFade(animations.StumbleRightSide, 0.2f);
				}
			}
			if (!character.IsJumping)
			{
				characterAnimation.CrossFadeQueued(animations.Run, (!game.Modifiers.IsActive(game.Modifiers.Hoverboard)) ? 0.02f : 0.4f);
			}
			return;
		}
		switch (horizontalHit)
		{
		case Character.StumbleHorizontalHit.Center:
			switch (verticalHit)
			{
			case Character.StumbleVerticalHit.Lower:
				if (game.Modifiers.IsActive(game.Modifiers.Hoverboard))
				{
					characterAnimation.CrossFade(animations.StumbleMix, 0.05f);
				}
				else
				{
					characterAnimation.CrossFade(animations.Stumble, 0.05f);
				}
				characterAnimation.CrossFadeQueued(animations.Run, 0.5f);
				break;
			case Character.StumbleVerticalHit.Middle:
				characterAnimation.CrossFade(animations.HitMid, 0.07f);
				break;
			case Character.StumbleVerticalHit.Upper:
				characterAnimation.CrossFade(animations.HitUpper, 0.07f);
				break;
			}
			return;
		case Character.StumbleHorizontalHit.Left:
			characterAnimation.Play((!game.Modifiers.IsActive(game.Modifiers.Hoverboard)) ? animations.StumbleLeftSide : animations.StumbleLeftCorner);
			break;
		case Character.StumbleHorizontalHit.LeftCorner:
			characterAnimation.Play(animations.StumbleLeftCorner);
			break;
		case Character.StumbleHorizontalHit.Right:
			characterAnimation.Play((!game.Modifiers.IsActive(game.Modifiers.Hoverboard)) ? animations.StumbleRightSide : animations.StumbleRightCorner);
			break;
		case Character.StumbleHorizontalHit.RightCorner:
			characterAnimation.Play(animations.StumbleRightCorner);
			break;
		}
		characterAnimation.PlayQueued(animations.Run);
	}

	private void OnChangeIsGrounded(bool isGrounded)
	{
		shadow.enabled = isGrounded;
	}

	private void OnRoll()
	{
		StartCoroutine(OnRollPlayAnimation());
	}

	private IEnumerator OnRollPlayAnimation()
	{
		string rollAnimation = animations.Roll;
		string runAnimation = animations.Run;
		characterAnimation.CrossFade(rollAnimation, 0.1f);
		if (hoverboard.isActive)
		{
			characterAnimation.CrossFadeQueued(runAnimation, 0.2f);
			if (hoverboardAnimation != null)
			{
				hoverboardAnimation.CrossFade(GetHoverboardAnimationName(rollAnimation), 0.1f);
				hoverboardAnimation.CrossFadeQueued(GetHoverboardAnimationName(runAnimation), (!hoverboard.isActive) ? 0f : 0.2f);
			}
		}
		else
		{
			characterAnimation.PlayQueued(runAnimation);
		}
		float endTime = Time.time + characterAnimation[rollAnimation].length;
		while (Time.time < endTime && characterAnimation[rollAnimation].enabled)
		{
			yield return null;
		}
		character.EndRoll();
	}

	private void OnHitByTrain()
	{
		characterAnimation.Play(animations.HitMoving);
		Vector3 currentPos = character.transform.position;
		Vector3 camPos = character.characterCamera.transform.position;
		StartCoroutine(pTween.To(0.5f, delegate(float t)
		{
			character.transform.position = Vector3.Lerp(currentPos, new Vector3(camPos.x, camPos.y - 33f, currentPos.z), t);
		}));
	}

	private void OnJump()
	{
		if (hoverboard.isActive)
		{
			string[] hoverboardJump = animations.HoverboardJump;
			jumpAnimation = hoverboardJump[0];
			hangtimeAnimation = hoverboardJump[1];
		}
		else
		{
			jumpAnimation = animations.Jump;
		}
		characterAnimation.CrossFade(jumpAnimation, 0.05f);
		if (IsSpikeCharacter() && followingGuard != null)
		{
			followingGuard.PlayMirrorAnimation("Guard_jump", false, 0.05f);
		}
		if (hoverboard.isActive && hoverboardAnimation != null)
		{
			hoverboardAnimation.CrossFade(GetHoverboardAnimationName(jumpAnimation), 0.05f);
		}
	}

	private void OnHangtime()
	{
		if (!character.IsRolling)
		{
			if (!hoverboard.isActive || hangtimeAnimation == null)
			{
				hangtimeAnimation = animations.Hangtime;
			}
			characterAnimation.CrossFade(hangtimeAnimation, 0.2f);
			if (IsSpikeCharacter() && followingGuard != null)
			{
				followingGuard.PlayMirrorAnimation("Guard_hangtime", false, 0.2f);
			}
			if (hoverboardAnimation != null)
			{
				hoverboardAnimation.CrossFade(GetHoverboardAnimationName(hangtimeAnimation), 0.2f);
			}
		}
	}

	private void OnLanding()
	{
		string text = animations.Run;
		if (character.IsRolling)
		{
			return;
		}
		if (hoverboard.isActive)
		{
			string animationName;
			if (character.IsAboveGround)
			{
				text = animations.Grind;
				animationName = text + "_land";
			}
			else
			{
				animationName = animations.Land;
			}
			characterAnimation.CrossFade(animationName, 0.05f);
			characterAnimation.CrossFadeQueued(text, 0.1f);
			if (IsSpikeCharacter() && followingGuard != null)
			{
				followingGuard.PlayMirrorAnimation("Guard_landing", false, 0.05f);
			}
			if (hoverboardAnimation != null)
			{
				hoverboardAnimation.CrossFade(GetHoverboardAnimationName(animationName), 0.05f);
				hoverboardAnimation.CrossFadeQueued(GetHoverboardAnimationName(text), 0.1f);
			}
		}
		else
		{
			string land = animations.Land;
			characterAnimation.CrossFade(land, 0.05f);
			characterAnimation.CrossFadeQueued(text, 0.1f);
			if (IsSpikeCharacter() && followingGuard != null)
			{
				followingGuard.PlayMirrorAnimation("Guard_landing", false, 0.05f);
			}
		}
	}

	private void OnTutorialMoveBackToCheckPoint(float duration)
	{
		characterAnimation.CrossFade(animations.Run, duration);
	}

	private void OnTutorialStartFromCheckPoint()
	{
		characterAnimation.Play(animations.Run);
	}

	private void OnCatchPlayer(string currentCharacterCaught, float catchUpTime, float waitTimeBeforeScreen)
	{
		caught = characterAnimation[currentCharacterCaught];
		caught.weight = 0f;
		caught.normalizedTime = 0f;
		caught.enabled = true;
		StartCoroutine(CatchPlayerAnimStarter(caught, catchUpTime));
	}

	private IEnumerator CatchPlayerAnimStarter(AnimationState caught, float delay)
	{
		yield return new WaitForSeconds(delay);
		AnimationState caught2 = default(AnimationState);
		StartCoroutine(pTween.To(0.2f, delegate(float t)
		{
			caught2.weight = Mathf.Lerp(0f, 1f, t);
		}));
	}

	private void OnStageMenuSequence()
	{
		if (characterAnimation != null)
		{
			if (caught != null)
			{
				caught.enabled = false;
			}
			characterAnimation.transform.rotation = Quaternion.identity;
			characterAnimation.Play(animations.TopMenu);
		}
		characterModel.sprayCanModel.SetActiveRecursively(true);
	}

	public void OnSwitchToRunning()
	{
		if (character.superSneakers.isActive || game.HasSuperSneakers)
		{
			animations.RUN = InitializeClips(SuperSneaksAnimations.run);
		}
		else
		{
			animations.RUN = InitializeClips(defaultAnimations.run);
		}
		animations.LAND = InitializeClips(defaultAnimations.landing);
		animations.JUMP = InitializeClips(defaultAnimations.jump);
		animations.HANGTIME = InitializeClips(defaultAnimations.hangtime);
		animations.ROLL = InitializeClips(defaultAnimations.roll);
		animations.DODGE_LEFT = InitializeClips(defaultAnimations.dodgeLeft);
		animations.DODGE_RIGHT = InitializeClips(defaultAnimations.dodgeRight);
		ToggleCustomHoverboard(null);
	}

	private void OnSwitchToHoverboard(GameObject hoverBoard)
	{
		ToggleCustomHoverboard(hoverBoard);
		string getOnBoard = animations.GetOnBoard;
		hangtimeAnimation = animations.Hangtime;
		characterAnimation.CrossFade(getOnBoard, 0.1f);
		if (hoverboardAnimation != null)
		{
			hoverboardAnimation.CrossFade(GetHoverboardAnimationName(getOnBoard), 0.1f);
		}
		if (!character.IsFalling && !character.IsJumping)
		{
			string run = animations.Run;
			characterAnimation.CrossFadeQueued(run, 0.2f);
			if (hoverboardAnimation != null)
			{
				hoverboardAnimation.CrossFadeQueued(GetHoverboardAnimationName(run), 0.2f);
			}
		}
		else
		{
			characterAnimation.CrossFade(hangtimeAnimation, 0.2f);
			if (hoverboardAnimation != null)
			{
				hoverboardAnimation.CrossFadeQueued(GetHoverboardAnimationName(hangtimeAnimation), 0.2f);
			}
		}
	}

	private void OnSwitchToJetpack(bool isHeadStart)
	{
		animations.RUN = InitializeClips(jetpackAnimations.run);
		animations.DODGE_LEFT = InitializeClips(jetpackAnimations.dodgeLeft);
		animations.DODGE_RIGHT = InitializeClips(jetpackAnimations.dodgeRight);
		characterModel.meshJetpack.enabled = true;
		characterRenderingEffects.JetpackParticles.SetActiveRecursively(true);
		string run = animations.Run;
		characterAnimation.CrossFade(run);
		if (hoverboardAnimation != null)
		{
			hoverboardAnimation.CrossFade(GetHoverboardAnimationName(run));
		}
	}

	private void ToggleCustomHoverboard(GameObject newHoverboard)
	{
		if (currentHoverboard != null && currentHoverboard != newHoverboard)
		{
			UnityEngine.Object.Destroy(currentHoverboard);
		}
		currentHoverboard = newHoverboard;
		if (newHoverboard != null)
		{
			hoverboardAnimation = newHoverboard.GetComponent<Animation>();
			hoverboardRendering = newHoverboard.GetComponent<HoverboardRendering>();
			if (hoverboardRendering != null)
			{
				hoverboardRendering.Initialize(characterAnimation, hoverboardAnimation, addedAnimClipsNames);
				return;
			}
			hoverboardRendering = Hoverboard.Instance.hoverboardSelector[0].hoverboardPrefab.GetComponent<HoverboardRendering>();
			hoverboardRendering.Initialize(characterAnimation, hoverboardAnimation, addedAnimClipsNames);
		}
		else
		{
			hoverboardAnimation = null;
			hoverboardRendering = null;
		}
	}

	private void JetpackOnFlyAheadStart()
	{
		initRot = characterRenderingEffects.JetpackParticles.transform.rotation.eulerAngles;
		initScale = characterRenderingEffects.JetpackParticles.transform.localScale;
	}

	private void JetpackOnFlyAheadUpdate(float ratio)
	{
		float num = Mathf.Lerp(0f, 1f, jetpackParticleOffsetCurve.Evaluate(ratio));
		characterRenderingEffects.JetpackParticles.transform.rotation = Quaternion.Euler(initRot - new Vector3(num, 0f, 0f));
		characterRenderingEffects.JetpackParticles.transform.localScale = initScale + new Vector3(0f, 0f, num * 2f);
	}

	private void JetpackOnStop()
	{
		characterRenderingEffects.JetpackParticles.SetActiveRecursively(false);
		characterModel.meshJetpack.enabled = false;
		if (hoverboard.isActive)
		{
			OnSwitchToHoverboard(currentHoverboard);
		}
		else if (superSneakers.isActive)
		{
			OnSwitchToSuperSneakers();
			OnHangtime();
		}
		else
		{
			OnSwitchToRunning();
			OnHangtime();
		}
	}

	private void IsInGame_OnChange(bool isInGame)
	{
		if (!isInGame)
		{
			JetpackOnStop();
		}
	}

	private void OnSwitchToSuperSneakers()
	{
		if (hoverboardRendering == null)
		{
			OnSwitchToRunning();
			OnRun();
		}
	}

	private void SuperSneakersOnStop()
	{
		if (hoverboardRendering == null)
		{
			OnSwitchToRunning();
			OnRun();
		}
	}
}
