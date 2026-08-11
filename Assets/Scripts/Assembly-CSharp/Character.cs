using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Character : MonoBehaviour
{
	public enum StumbleType
	{
		Normal = 0,
		Bush = 1,
		Side = 2
	}

	public enum OnChangeTrackDirection
	{
		Left = 0,
		Right = 1
	}

	public delegate void OnChangeTrackDelegate(OnChangeTrackDirection direction);

	public enum StumbleHorizontalHit
	{
		Left = 0,
		LeftCorner = 1,
		Center = 2,
		RightCorner = 3,
		Right = 4
	}

	public enum StumbleVerticalHit
	{
		Upper = 0,
		Middle = 1,
		Lower = 2
	}

	public delegate void OnStumbleDelegate(StumbleType stumbleType, StumbleHorizontalHit horizontalHit, StumbleVerticalHit verticalHit, string colliderName);

	private struct SuperSneakersJump
	{
		public float z_start;

		public float z_length;

		public float z_end;

		public float y_start;
	}

	public enum CriticalHitType
	{
		Train = 0,
		Barrier = 1,
		MovingTrain = 2,
		None = 3
	}

	public enum ObstacleType
	{
		JumpHighBarrier = 0,
		JumpTrain = 1,
		RollBarrier = 2,
		JumpBarrier = 3,
		None = 4
	}

	private enum ImpactX
	{
		Left = 0,
		Middle = 1,
		Right = 2
	}

	private enum ImpactY
	{
		Upper = 0,
		Middle = 1,
		Lower = 2
	}

	private enum ImpactZ
	{
		Before = 0,
		Middle = 1,
		After = 2
	}

	public delegate void OnCriticalHitDelegate(CriticalHitType type);

	public delegate void OnJumpDelegate();

	public delegate void OnLandingDelegate();

	public delegate void OnHangtimeDelegate();

	public delegate void OnRollDelegate();

	public delegate void OnHitByTrainDelegate();

	public delegate void OnTutorialMoveBackToCheckPointDelegate(float duration);

	public delegate void OnTutorialStartFromCheckPointDelegate();

	public delegate void OnPassedObstacleDelegate(ObstacleType type);

	public delegate void OnJumpOverTrainDelegate();

	public Transform characterRoot;

	public CapsuleCollider characterCollider;

	public OnTriggerObject coinMagnetCollider;

	public OnTriggerObject coinMagnetLongCollider;

	public float characterAngle = 45f;

	public ParticleSystem hoverboardCrashParticleSystem;

	public CharacterPickupParticles CharacterPickupParticleSystem;

	public float ColliderTrackWidth = 17f;

	[HideInInspector]
	public CharacterController characterController;

	[HideInInspector]
	public OnTriggerObject characterColliderTrigger;

	[HideInInspector]
	public CharacterModel characterModel;

	[HideInInspector]
	public CharacterCamera characterCamera;

	[HideInInspector]
	public Hoverboard hoverboard;

	[HideInInspector]
	public SuperSneakers superSneakers;

	[HideInInspector]
	public Running running;

	[HideInInspector]
	public bool immuneToCriticalHit;

	// 幽灵穿火车状态：为 true 时主角直接穿过火车、不被撞
	[HideInInspector]
	public bool ghostThroughTrains;

	private bool ghostIgnoredPhysics;

	private int ghostTrainLayer = -1;

	private int ghostPlayerLayer;

	[HideInInspector]
	public int trackIndex;

	[HideInInspector]
	public float x;

	public float z;

	public float verticalSpeed;

	[HideInInspector]
	public float lastGroundedY;

	private bool isInsideSubway;

	[HideInInspector]
	public float subwayMaxY;

	private Vector3 characterControllerCenter;

	private float characterControllerHeight;

	private Vector3 characterColliderCenter;

	private float characterColliderHeight;

	private int initialTrackIndex = 1;

	private int trackMovement;

	private int trackMovementNext;

	private float characterRotation;

	private int trackIndexTarget;

	private float trackIndexPosition;

	private Game game;

	private Track track;

	private FollowingGuard guard;

	[HideInInspector]
	public float jumpHeight;

	public float gravity = 200f;

	public float jumpHeightNormal = 20f;

	public float jumpHeightSuperSneakers = 40f;

	public float verticalFallSpeedLimit = -1f;

	public float stumbleCornerTolerance = 15f;

	public float stumbleDecayTime = 5f;

	private bool isJumping;

	private bool isRolling;

	private bool isFalling;

	private bool isStumbling;

	private bool inAirJump;

	private bool isJumpingHigher;

	public Variable<bool> IsGrounded = new Variable<bool>(false);

	private HashSet<Collider> subwayColliders = new HashSet<Collider>();

	private VariableBool squeezeCollider = new VariableBool();

	private SuperSneakersJump? superSneakersJump;

	public AnimationCurve superSneakersJumpCurve;

	public float superSneakersJumpApexRatio = 0.5f;

	private string lastHitTag;

	[HideInInspector]
	public bool stopColliding;

	private bool startedJumpFromGround;

	private float trainJumpSampleZ;

	private float trainJumpSampleLength = 10f;

	private bool trainJump;

	private float verticalSpeed_jumpTolerance = -30f;

	private Layers layers;

	private ObstacleType lastObstacleTriggerType;

	private int lastObstacleTriggerTrackIndex;

	public float sameLaneTimeStamp;

	private static Character instance;

	public bool IsStumbling
	{
		get
		{
			return isStumbling;
		}
		set
		{
			isStumbling = value;
		}
	}

	public bool IsFalling
	{
		get
		{
			return isFalling;
		}
		set
		{
			isFalling = value;
		}
	}

	public bool IsJumping
	{
		get
		{
			return isJumping;
		}
		set
		{
			isJumping = value;
		}
	}

	public bool IsRolling
	{
		get
		{
			return isRolling;
		}
	}

	public bool IsInsideSubway
	{
		get
		{
			return isInsideSubway;
		}
	}

	public int TrackIndex
	{
		get
		{
			return trackIndex;
		}
	}

	public bool IsAboveGround
	{
		get
		{
			return base.transform.position.y > 20f;
		}
	}

	public bool IsJumpingHigher
	{
		get
		{
			return isJumpingHigher || (hoverboard.isActive && HoverboardManager.Instance.Hoverboard == Hoverboards.BoardType.bouncer);
		}
		set
		{
			isJumpingHigher = value;
		}
	}

	public VariableBool SqueezeCollider
	{
		get
		{
			return squeezeCollider;
		}
	}

	public static Character Instance
	{
		get
		{
			if (instance == null)
			{
				instance = UnityEngine.Object.FindObjectOfType(typeof(Character)) as Character;
			}
			return instance;
		}
	}

	[method: MethodImpl(32)]
	public event OnStumbleDelegate OnStumble;

	[method: MethodImpl(32)]
	public event OnCriticalHitDelegate OnCriticalHit;

	[method: MethodImpl(32)]
	public event OnJumpDelegate OnJump;

	[method: MethodImpl(32)]
	public event OnLandingDelegate OnLanding;

	[method: MethodImpl(32)]
	public event OnHangtimeDelegate OnHangtime;

	[method: MethodImpl(32)]
	public event OnRollDelegate OnRoll;

	[method: MethodImpl(32)]
	public event OnHitByTrainDelegate OnHitByTrain;

	[method: MethodImpl(32)]
	public event OnChangeTrackDelegate OnChangeTrack;

	[method: MethodImpl(32)]
	public event OnTutorialMoveBackToCheckPointDelegate OnTutorialMoveBackToCheckPoint;

	[method: MethodImpl(32)]
	public event OnTutorialStartFromCheckPointDelegate OnTutorialStartFromCheckPoint;

	[method: MethodImpl(32)]
	public event OnPassedObstacleDelegate OnPassedObstacle;

	[method: MethodImpl(32)]
	public event OnJumpOverTrainDelegate OnJumpOverTrain;

	public void Initialize()
	{
		layers = Layers.Instance;
		game = Game.Instance;
		Variable<bool> isInGame2 = game.IsInGame;
		isInGame2.OnChange = (Variable<bool>.OnChangeDelegate)Delegate.Combine(isInGame2.OnChange, (Variable<bool>.OnChangeDelegate)delegate(bool isInGame)
		{
			if (!isInGame)
			{
				StopAllCoroutines();
				immuneToCriticalHit = false;
				characterController.enabled = true;
				stopColliding = false;
			}
		});
		VariableBool variableBool = squeezeCollider;
		variableBool.OnChange = (VariableBool.OnChangeDelegate)Delegate.Combine(variableBool.OnChange, (VariableBool.OnChangeDelegate)delegate(bool squeeze)
		{
			if (squeeze)
			{
				characterController.height = 4f;
				characterController.center = new Vector3(0f, 2f, characterControllerCenter.z);
				characterCollider.height = 4f;
				characterCollider.center = new Vector3(0f, 4f, characterColliderCenter.z);
			}
			else
			{
				characterController.center = characterControllerCenter;
				characterController.height = characterControllerHeight;
				characterCollider.center = characterColliderCenter;
				characterCollider.height = characterColliderHeight;
			}
		});
		track = Track.Instance;
		characterController = Game.Charactercontroller;
		hoverboard = Hoverboard.Instance;
		running = Running.Instance;
		CharacterRendering component = GetComponent<CharacterRendering>();
		component.Initialize();
		superSneakers = this.FindObject<SuperSneakers>();
		characterModel = GetComponentInChildren<CharacterModel>();
		characterRoot = characterModel.transform;
		characterCamera = CharacterCamera.Instance;
		guard = FollowingGuard.Instance;
		CharacterPickupParticleSystem = GetComponentInChildren<CharacterPickupParticles>();
		characterColliderTrigger = characterCollider.GetComponent<OnTriggerObject>();
		OnTriggerObject onTriggerObject = characterColliderTrigger;
		onTriggerObject.OnEnter = (OnTriggerObject.OnEnterDelegate)Delegate.Combine(onTriggerObject.OnEnter, new OnTriggerObject.OnEnterDelegate(OnCharacterColliderEnter));
		OnTriggerObject onTriggerObject2 = characterColliderTrigger;
		onTriggerObject2.OnExit = (OnTriggerObject.OnExitDelegate)Delegate.Combine(onTriggerObject2.OnExit, new OnTriggerObject.OnExitDelegate(OnCharacterColliderExit));
		characterControllerCenter = characterController.center;
		characterControllerHeight = characterController.height;
		characterColliderCenter = characterCollider.center;
		characterColliderHeight = characterCollider.height;
	}

	public void Restart()
	{
		trackIndex = initialTrackIndex;
		trackIndexTarget = initialTrackIndex;
		x = track.GetTrackX(trackIndex);
		trackIndexPosition = trackIndex;
		characterModel.ResetBlink();
		z = 0f;
		trackMovement = 0;
		trackMovementNext = 0;
		squeezeCollider.Clear();
		characterController.transform.position = track.GetPosition(x, z) + Vector3.up * 5f;
		characterController.Move(-5f * Vector3.up);
		verticalSpeed = 0f;
		superSneakersJump = null;
		jumpHeight = jumpHeightNormal;
		inAirJump = false;
		isJumping = false;
		isRolling = false;
		IsGrounded.Value = false;
		lastGroundedY = 0f;
		guard.Restart(true);
		StartStumble();
		startedJumpFromGround = false;
		sameLaneTimeStamp = Time.time;
		subwayColliders.Clear();
		isInsideSubway = false;
	}

	public void ChangeTrack(int movement, float duration)
	{
		Missions.Instance.PlayerDidThis(Missions.MissionTarget.StayInOneLane, (int)(Time.time - sameLaneTimeStamp));
		Missions.Instance.RemoveProgressForThis(Missions.MissionTarget.StayInOneLane);
		sameLaneTimeStamp = Time.time;
		if (trackMovement != movement)
		{
			ForceChangeTrack(movement, duration);
		}
		else
		{
			trackMovementNext = movement;
		}
	}

	public void ForceChangeTrack(int movement, float duration)
	{
		StopAllCoroutines();
		StartCoroutine(ChangeTrackCoroutine(movement, duration));
	}

	private IEnumerator ChangeTrackCoroutine(int move, float duration)
	{
		trackMovement = move;
		trackMovementNext = 0;
		int newTrackIndex = trackIndexTarget + move;
		float trackChangeIndexDistance = Mathf.Abs((float)newTrackIndex - trackIndexPosition);
		float trackIndexPositionBegin = trackIndexPosition;
		float startX = x;
		float endX = track.GetTrackX(newTrackIndex);
		float dir = Mathf.Sign(newTrackIndex - trackIndexTarget);
		float startRotation = characterRotation;
		// 空气墙已移除：不再把 newTrackIndex 夹回到 0 ~ numberOfTracks-1，玩家可自由移动到三道之外。
		if (this.OnChangeTrack != null)
		{
			this.OnChangeTrack((move >= 0) ? OnChangeTrackDirection.Right : OnChangeTrackDirection.Left);
		}
		trackIndexTarget = newTrackIndex;
		yield return StartCoroutine(pTween.To(trackChangeIndexDistance * duration, delegate(float t)
		{
			trackIndexPosition = Mathf.Lerp(trackIndexPositionBegin, newTrackIndex, t);
			x = Mathf.Lerp(startX, endX, t);
			characterRotation = pMath.Bell(t) * dir * characterAngle + Mathf.Lerp(startRotation, 0f, t);
			characterRoot.localRotation = Quaternion.Euler(0f, characterRotation, 0f);
		}));
		trackIndex = newTrackIndex;
		trackMovement = 0;
		if (trackMovementNext != 0)
		{
			StartCoroutine(ChangeTrackCoroutine(trackMovementNext, duration));
		}
	}

	public void SetBackToCheckPoint(float zoomTime)
	{
		float lastCheckPoint = track.GetLastCheckPoint(z);
		trackIndex = initialTrackIndex;
		trackIndexTarget = initialTrackIndex;
		float trackX = track.GetTrackX(trackIndex);
		trackIndexPosition = trackIndex;
		trackMovement = 0;
		trackMovementNext = 0;
		StartCoroutine(MoveCharacterToPosition(trackX, lastCheckPoint, zoomTime));
	}

	private IEnumerator MoveCharacterToPosition(float newX, float newZ, float time)
	{
		float oldX = x;
		float oldZ = z;
		game.ChangeState(null);
		immuneToCriticalHit = true;
		stopColliding = true;
		characterController.enabled = false;
		NotifyOnTutorialMoveBackToCheckPoint(time);
		float newX2 = default(float);
		float newZ2 = default(float);
		yield return StartCoroutine(pTween.To(time, delegate(float t)
		{
			x = Mathf.SmoothStep(oldX, newX2, t);
			z = Mathf.SmoothStep(oldZ, newZ2, t);
		}));
		immuneToCriticalHit = false;
		characterController.enabled = true;
		NotifyOnTutorialStartFromCheckPoint();
		stopColliding = false;
		game.ChangeState(game.Running);
	}

	private ObstacleType ObstacleTagToType(string tag)
	{
		switch (tag)
		{
		case "JumpTrain":
			return ObstacleType.JumpTrain;
		case "RollBarrier":
			return ObstacleType.RollBarrier;
		case "JumpBarrier":
			return ObstacleType.JumpBarrier;
		case "JumpHighBarrier":
			return ObstacleType.JumpHighBarrier;
		default:
			return ObstacleType.None;
		}
	}

	public void ForceLeaveSubway()
	{
		subwayColliders.Clear();
		isInsideSubway = false;
	}

	private void OnCharacterColliderExit(Collider collider)
	{
		if (collider.CompareTag("Subway"))
		{
			if (subwayColliders.Contains(collider))
			{
				subwayColliders.Remove(collider);
				isInsideSubway = subwayColliders.Count > 0;
			}
		}
		else
		{
			ObstacleType obstacleType = ObstacleTagToType(collider.tag);
			if (obstacleType == lastObstacleTriggerType && lastObstacleTriggerTrackIndex == trackIndex && this.OnPassedObstacle != null)
			{
				this.OnPassedObstacle(obstacleType);
			}
		}
	}

	private bool IsSafeSurfaceCollider(Collider collider)
	{
		if (collider == null)
		{
			return false;
		}
		if (collider.name.Equals("lightSignal", StringComparison.OrdinalIgnoreCase))
		{
			return true;
		}
		return collider.GetComponent<LightSignalSafeSurface>() != null || collider.GetComponentInParent<LightSignalSafeSurface>() != null;
	}

	private void OnCharacterColliderEnter(Collider collider)
	{
		if (!game.IsInGame.Value)
		{
			return;
		}
		if (IsSafeSurfaceCollider(collider))
		{
			return;
		}
		if (collider.CompareTag("Subway"))
		{
			subwayColliders.Add(collider);
			isInsideSubway = subwayColliders.Count > 0;
			subwayMaxY = collider.bounds.max.y - 3f;
		}
		else
		{
			if (stopColliding || collider.gameObject.layer == layers.KeepOnHoverboard)
			{
				return;
			}
			// 幽灵穿火车：碰到火车直接忽略（不踉跄、不死亡），像保安一样穿过
			if (ghostThroughTrains && (collider.CompareTag("HitMovingTrain") || collider.CompareTag("HitTrain")))
			{
				return;
			}
			Pickup componentInChildren = collider.GetComponentInChildren<Pickup>();
			if (componentInChildren != null)
			{
				NotifyPickup(componentInChildren);
				return;
			}
			if (collider.gameObject.layer == layers.Default)
			{
				if (collider.isTrigger && characterController.isGrounded)
				{
					IsGrounded.Value = true;
				}
				if (collider.isTrigger)
				{
					ObstacleType obstacleType = ObstacleTagToType(collider.tag);
					if (obstacleType != ObstacleType.None)
					{
						lastObstacleTriggerType = obstacleType;
						lastObstacleTriggerTrackIndex = trackIndex;
					}
				}
				return;
			}
			if (collider.isTrigger)
			{
				if (collider.name == "bush")
				{
					HandleStumble(StumbleType.Bush, StumbleHorizontalHit.Center, StumbleVerticalHit.Lower, collider.name);
				}
				else
				{
					HandleStumble(StumbleType.Normal, StumbleHorizontalHit.Center, StumbleVerticalHit.Middle, collider.name);
				}
				return;
			}
			lastHitTag = collider.tag;
			ImpactX impactX = GetImpactX(collider);
			ImpactY impactY = GetImpactY(collider);
			ImpactZ impactZ = GetImpactZ(collider);
			float num = (collider.bounds.min.x + collider.bounds.max.x) / 2f;
			float num2 = base.transform.position.x;
			int num3 = ((num2 < num) ? 1 : ((num2 > num) ? (-1) : 0));
			bool flag = num3 == 0 || trackMovement == num3;
			bool flag2 = characterCollider.bounds.center.z < collider.bounds.min.z;
			bool flag3 = impactZ == ImpactZ.Before && !flag2 && flag;
			if (impactZ == ImpactZ.Middle || flag3)
			{
				if (trackMovement != 0)
				{
					float duration = 0.5f;
					if (track.IsRunningOnTutorialTrack)
					{
						duration = 0.2f;
					}
					ChangeTrack(-trackMovement, duration);
				}
				switch (impactX)
				{
				case ImpactX.Left:
					HandleStumble(StumbleType.Normal, StumbleHorizontalHit.Left, StumbleVerticalHit.Middle, collider.name);
					break;
				case ImpactX.Right:
					HandleStumble(StumbleType.Normal, StumbleHorizontalHit.Right, StumbleVerticalHit.Middle, collider.name);
					break;
				}
				return;
			}
			if (impactX == ImpactX.Middle)
			{
				if (impactY == ImpactY.Lower)
				{
					verticalSpeed = CalculateJumpVerticalSpeed(8f);
					HandleStumble(StumbleType.Normal, StumbleHorizontalHit.Center, StumbleVerticalHit.Lower, collider.name);
				}
				else if (collider.gameObject.CompareTag("HitMovingTrain"))
				{
					HitByTrainSequence();
					NotifyCriticalHit();
				}
				else if (impactY == ImpactY.Middle)
				{
					HandleStumble(StumbleType.Normal, StumbleHorizontalHit.Center, StumbleVerticalHit.Middle, collider.name);
					NotifyCriticalHit();
				}
				else
				{
					HandleStumble(StumbleType.Normal, StumbleHorizontalHit.Center, StumbleVerticalHit.Upper, collider.name);
					NotifyCriticalHit();
				}
				return;
			}
			if (impactZ == ImpactZ.Before && flag)
			{
				if (collider.gameObject.CompareTag("HitMovingTrain"))
				{
					HitByTrainSequence();
					NotifyCriticalHit();
				}
				else if (collider.gameObject.layer == layers.HitBounceOnly)
				{
					HandleStumble(StumbleType.Normal, StumbleHorizontalHit.Center, StumbleVerticalHit.Lower, collider.name);
				}
				else
				{
					ForceChangeTrack(-trackMovement, 0.5f);
				}
			}
			else if (collider.gameObject.layer == layers.HitBounceOnly)
			{
				ForceChangeTrack(-trackMovement, 0.5f);
			}
			switch (impactX)
			{
			case ImpactX.Left:
				HandleStumble(StumbleType.Normal, StumbleHorizontalHit.LeftCorner, StumbleVerticalHit.Middle, collider.name);
				break;
			case ImpactX.Right:
				HandleStumble(StumbleType.Normal, StumbleHorizontalHit.RightCorner, StumbleVerticalHit.Middle, collider.name);
				break;
			}
		}
	}

	private void HitByTrainSequence()
	{
		if (hoverboard.isActive)
		{
			NotifyOnJump();
		}
		else
		{
			NotifyOnHitByTrain();
		}
	}

	private ImpactX GetImpactX(Collider collider)
	{
		Bounds bounds = characterCollider.bounds;
		Bounds bounds2 = collider.bounds;
		float num = Mathf.Max(bounds.min.x, bounds2.min.x);
		float num2 = Mathf.Min(bounds.max.x, bounds2.max.x);
		float num3 = (num + num2) * 0.5f;
		float num4 = num3 - bounds2.min.x;
		if ((double)num4 > (double)bounds2.size.x - (double)ColliderTrackWidth * 0.33)
		{
			return ImpactX.Right;
		}
		if ((double)num4 < (double)ColliderTrackWidth * 0.33)
		{
			return ImpactX.Left;
		}
		return ImpactX.Middle;
	}

	private ImpactZ GetImpactZ(Collider collider)
	{
		Vector3 position = base.transform.position;
		Bounds bounds = collider.bounds;
		if (position.z > bounds.max.z - ((!(bounds.max.z - bounds.min.z > 30f)) ? ((bounds.max.z - bounds.min.z) * 0.5f) : stumbleCornerTolerance))
		{
			return ImpactZ.After;
		}
		if (position.z < bounds.min.z + stumbleCornerTolerance)
		{
			return ImpactZ.Before;
		}
		return ImpactZ.Middle;
	}

	private ImpactY GetImpactY(Collider collider)
	{
		Bounds bounds = characterCollider.bounds;
		Bounds bounds2 = collider.bounds;
		float num = Mathf.Max(bounds.min.y, bounds2.min.y);
		float num2 = Mathf.Min(bounds.max.y, bounds2.max.y);
		float num3 = (num + num2) * 0.5f;
		float num4 = (num3 - bounds.min.y) / bounds.size.y;
		if (num4 < 0.33f)
		{
			return ImpactY.Lower;
		}
		if (num4 < 0.66f)
		{
			return ImpactY.Middle;
		}
		return ImpactY.Upper;
	}

	public void Update()
	{
		Vector3 position = base.transform.position;
		if (position.y < 0f)
		{
			position.y = 1f;
			base.transform.position = position;
			Debug.Log("Character y-position has been clamped to avoid fallthrough.");
		}
	}

	public float GetTrackX()
	{
		return track.GetPosition(track.GetTrackX(trackIndex), 0f).x;
	}

	public void Jump()
	{
		bool flag = !isJumping && verticalSpeed <= 0f && verticalSpeed > verticalSpeed_jumpTolerance;
		if (characterController.isGrounded || flag)
		{
			isJumping = true;
			isFalling = false;
			IsGrounded.Value = false;
			NotifyOnJump();
			if (IsJumpingHigher)
			{
				Vector3 position = base.transform.position;
				SuperSneakersJump value = default(SuperSneakersJump);
				value.z_start = position.z;
				value.z_length = JumpLength(game.currentSpeed, jumpHeightSuperSneakers) * superSneakersJumpApexRatio;
				value.z_end = value.z_start + value.z_length;
				value.y_start = position.y;
				superSneakersJump = value;
				verticalSpeed = 0f;
			}
			else
			{
				verticalSpeed = CalculateJumpVerticalSpeed(jumpHeight);
			}
			if (IsRunningOnGround())
			{
				startedJumpFromGround = true;
				trainJump = false;
				trainJumpSampleZ = z + trainJumpSampleLength;
			}
		}
		else if (verticalSpeed < 0f)
		{
			inAirJump = true;
		}
	}

	private bool IsRunningFromTrain()
	{
		return running.currentRunPosition == Running.RunPositions.train || running.currentRunPosition == Running.RunPositions.movingTrain;
	}

	private bool IsRunningOnGround()
	{
		return running.currentRunPosition == Running.RunPositions.ground;
	}

	public void CheckInAirJump()
	{
		if (characterController.isGrounded && inAirJump)
		{
			Jump();
			inAirJump = false;
		}
	}

	public void Roll()
	{
		if (!isRolling)
		{
			SuperSneakersJump? superSneakersJump = this.superSneakersJump;
			if (superSneakersJump.HasValue)
			{
				this.superSneakersJump = null;
			}
			squeezeCollider.Add(this);
			verticalSpeed = 0f - CalculateJumpVerticalSpeed(jumpHeight);
			isRolling = true;
			NotifyOnRoll();
		}
	}

	public void ApplyGravity()
	{
		if (verticalSpeed < 0f && characterController.isGrounded)
		{
			if (startedJumpFromGround && trainJump && IsRunningOnGround())
			{
				NotifyOnJumpOverTrain();
			}
			if (running.currentRunPosition != Running.RunPositions.air)
			{
				startedJumpFromGround = false;
			}
			verticalSpeed = 0f;
			IsGrounded.Value = true;
			if (isJumping || isFalling)
			{
				isJumping = false;
				isFalling = false;
				IsGrounded.Value = true;
				NotifyOnLanding();
			}
		}
		else if (startedJumpFromGround && trainJumpSampleZ < z)
		{
			RaycastHit hitInfo;
			if (Physics.Raycast(new Ray(base.transform.position, -Vector3.up), out hitInfo) && (hitInfo.collider.CompareTag("HitMovingTrain") || hitInfo.collider.CompareTag("HitTrain")))
			{
				trainJump = true;
			}
			trainJumpSampleZ += trainJumpSampleLength;
		}
		verticalSpeed -= gravity * Time.deltaTime;
		if (!characterController.isGrounded && !isFalling && verticalSpeed < verticalFallSpeedLimit && !isRolling)
		{
			isFalling = true;
			NotifyOnHangtime();
			IsGrounded.Value = false;
		}
	}

	public void MoveWithGravity()
	{
		if (characterController.enabled)
		{
			verticalSpeed -= gravity * Time.deltaTime;
			if (verticalSpeed > 0f)
			{
				verticalSpeed = 0f;
			}
			Vector3 motion = verticalSpeed * Time.deltaTime * Vector3.up;
			characterController.Move(motion);
		}
	}

	public void MoveForward()
	{
		Vector3 position = base.transform.position;
		float num = z + game.currentSpeed * Time.deltaTime;
		Vector3 vector = verticalSpeed * Time.deltaTime * Vector3.up;
		Vector3 position2 = track.GetPosition(x, num);
		Vector3 vector2 = new Vector3(position.x, 0f, position.z);
		if (superSneakersJump.HasValue)
		{
			SuperSneakersJump value = superSneakersJump.Value;
			if (z < value.z_end)
			{
				float num2 = superSneakersJumpCurve.Evaluate((num - value.z_start) / value.z_length) * jumpHeightSuperSneakers + value.y_start;
				float num3 = num2 - position.y;
				vector = Vector3.up * num3;
			}
			else
			{
				superSneakersJump = null;
				verticalSpeed = 0f;
				vector = Vector3.zero;
			}
		}
		Vector3 vector3 = position2 - vector2;
		if (characterController.enabled)
		{
			characterController.Move(vector + vector3);
		}
		else
		{
			characterController.transform.position = characterController.transform.position + vector3;
		}
		z = base.transform.position.z;
		if (characterController.isGrounded)
		{
			lastGroundedY = position.y;
		}
	}

	public void EndRoll()
	{
		if (characterController.enabled)
		{
			characterController.Move(Vector3.up * 2f);
		}
		squeezeCollider.Remove(this);
		if (characterController.enabled)
		{
			characterController.Move(Vector3.down * 2f);
		}
		isRolling = false;
	}

	public float CalculateJumpVerticalSpeed(float jumpHeight)
	{
		return Mathf.Sqrt(2f * jumpHeight * gravity);
	}

	public float CalculateJumpVerticalSpeed()
	{
		return CalculateJumpVerticalSpeed(jumpHeight);
	}

	public float JumpLength(float speed, float jumpHeight)
	{
		return speed * 2f * CalculateJumpVerticalSpeed(jumpHeight) / gravity;
	}

	private void StartStumble()
	{
		isStumbling = true;
		if (!track.IsRunningOnTutorialTrack)
		{
			guard.CatchUp();
		}
		guard.StartCoroutine(StumbleDecay());
	}

	private IEnumerator StumbleDecay()
	{
		yield return new WaitForSeconds(stumbleDecayTime);
		StopStumble();
	}

	public void StopStumble()
	{
		guard.ResetCatchUp();
		isStumbling = false;
	}

	private void HandleStumble(StumbleType stumbleType, StumbleHorizontalHit horizontalHit, StumbleVerticalHit verticalHit, string colliderName)
	{
		if (!game.IsInJetpackMode)
		{
			NotifyOnStumble(stumbleType, horizontalHit, verticalHit, colliderName);
			StartStumble();
		}
	}

	private void NotifyCriticalHit()
	{
		if (this.OnCriticalHit != null)
		{
			CriticalHitType type;
			switch (lastHitTag)
			{
			case "HitTrain":
				type = CriticalHitType.Train;
				break;
			case "HitBarrier":
				type = CriticalHitType.Barrier;
				break;
			case "HitMovingTrain":
				type = CriticalHitType.MovingTrain;
				break;
			default:
				type = CriticalHitType.None;
				break;
			}
			this.OnCriticalHit(type);
		}
	}

	private bool IsFreshCharacter()
	{
		return PlayerInfo.Instance != null && PlayerInfo.Instance.currentCharacter == (int)Characters.CharacterType.fresh;
	}

	public void NotifyPickup(Pickup pickup)
	{
		if (pickup != null && pickup.GetComponent<Coin>() != null && IsFreshCharacter())
		{
			return;
		}
		pickup.NotifyPickup(CharacterPickupParticleSystem);
	}

	// 按 M 切换：未开启则进入幽灵（并启动 duration 秒自动恢复）；已开启则立即恢复
	public void GhostThroughTrains(float duration)
	{
		if (ghostThroughTrains)
		{
			SetGhostThroughTrains(false);
		}
		else
		{
			SetGhostThroughTrains(true);
			StartCoroutine(GhostAutoRecoverCoroutine(duration));
		}
	}

	private void SetGhostThroughTrains(bool on)
	{
		if (on)
		{
			if (ghostThroughTrains)
			{
				return;
			}
			ghostThroughTrains = true;
			immuneToCriticalHit = true;
			ghostPlayerLayer = characterController.gameObject.layer;
			ghostTrainLayer = FindLayerByTag("HitMovingTrain");
			if (ghostTrainLayer < 0)
			{
				ghostTrainLayer = FindLayerByTag("HitTrain");
			}
			int groundLayer = FindLayerByTag("Ground");
			ghostIgnoredPhysics = false;
			// 仅当火车层独立（不与玩家层或地面层相同）才关闭物理碰撞，避免连带穿过地面
			if (ghostTrainLayer >= 0 && ghostTrainLayer != ghostPlayerLayer && ghostTrainLayer != groundLayer)
			{
				Physics.IgnoreLayerCollision(ghostPlayerLayer, ghostTrainLayer, true);
				ghostIgnoredPhysics = true;
			}
		}
		else
		{
			if (!ghostThroughTrains)
			{
				return;
			}
			if (ghostIgnoredPhysics)
			{
				Physics.IgnoreLayerCollision(ghostPlayerLayer, ghostTrainLayer, false);
				ghostIgnoredPhysics = false;
			}
			ghostThroughTrains = false;
			immuneToCriticalHit = false;
		}
	}

	private IEnumerator GhostAutoRecoverCoroutine(float duration)
	{
		float endTime = Time.time + duration;
		while (Time.time < endTime)
		{
			// 若期间被再次按 M 手动关闭，则直接结束，不重复恢复
			if (!ghostThroughTrains)
			{
				yield break;
			}
			yield return null;
		}
		SetGhostThroughTrains(false);
	}

	private int FindLayerByTag(string tag)
	{
		try
		{
			GameObject[] array = GameObject.FindGameObjectsWithTag(tag);
			if (array != null && array.Length > 0)
			{
				return array[0].layer;
			}
		}
		catch
		{
		}
		return -1;
	}

	private void NotifyOnStumble(StumbleType stumbleType, StumbleHorizontalHit horizontalHit, StumbleVerticalHit verticalHit, string colliderName)
	{
		if (this.OnStumble != null)
		{
			this.OnStumble(stumbleType, horizontalHit, verticalHit, colliderName);
		}
	}

	private void NotifyOnCriticalHit(CriticalHitType type)
	{
		if (this.OnCriticalHit != null)
		{
			this.OnCriticalHit(type);
		}
	}

	private void NotifyOnJump()
	{
		if (this.OnJump != null)
		{
			this.OnJump();
		}
	}

	private void NotifyOnLanding()
	{
		if (this.OnLanding != null)
		{
			this.OnLanding();
		}
	}

	private void NotifyOnHangtime()
	{
		if (this.OnHangtime != null)
		{
			this.OnHangtime();
		}
	}

	private void NotifyOnRoll()
	{
		if (this.OnRoll != null)
		{
			this.OnRoll();
		}
	}

	private void NotifyOnHitByTrain()
	{
		if (this.OnHitByTrain != null)
		{
			this.OnHitByTrain();
		}
	}

	private void NotifyOnChangeTrack(OnChangeTrackDirection direction)
	{
		if (this.OnChangeTrack != null)
		{
			this.OnChangeTrack(direction);
		}
	}

	private void NotifyOnTutorialMoveBackToCheckPoint(float duration)
	{
		if (this.OnTutorialMoveBackToCheckPoint != null)
		{
			this.OnTutorialMoveBackToCheckPoint(duration);
		}
	}

	private void NotifyOnTutorialStartFromCheckPoint()
	{
		if (this.OnTutorialStartFromCheckPoint != null)
		{
			this.OnTutorialStartFromCheckPoint();
		}
	}

	private void NotifyOnJumpOverTrain()
	{
		if (this.OnJumpOverTrain != null)
		{
			this.OnJumpOverTrain();
		}
	}
}
