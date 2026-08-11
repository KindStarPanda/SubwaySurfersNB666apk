using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Running : CharacterState
{
	public enum RunPositions
	{
		ground = 0,
		station = 1,
		train = 2,
		movingTrain = 3,
		air = 4
	}

	public float slowDownDuration = 0.625f;

	public float slowDownRatio = 1f;

	public float tunnelDelta = -20f;

	public Vector3 cameraOffset = new Vector3(0f, 33f, -33f);

	public float cameraOffsetSmoothDuration = 0.5f;

	public float cameraAimOffset = 20f;

	public float cameraFOV = 60f;

	public float smoothCameraXDuration = 0.05f;

	public float ySmoothDuration = 0.1f;

	public float characterChangeTrackLength = 30f;

	public AnimationCurve transitionFromJetpackCurve;

	private float tunnelStartZ;

	private Curve offsetDeltaCurve;

	private Game game;

	private Character character;

	private CharacterRendering characterRendering;

	private Transform characterTransform;

	private CharacterController characterController;

	private CharacterCamera characterCamera;

	private Transform characterCameraTransform;

	private Animation characterAnimation;

	public RunPositions currentRunPosition;

	private Queue<Collider> GrindedTrains = new Queue<Collider>();

	private int GrindedTrainsBufferSize = 5;

	private static Running instance;

	public static Running Instance
	{
		get
		{
			return instance ?? (instance = Object.FindObjectOfType(typeof(Running)) as Running);
		}
	}

	public void Awake()
	{
		game = Game.Instance;
		character = Character.Instance;
		characterRendering = CharacterRendering.Instance;
		characterTransform = character.transform;
		characterController = character.characterController;
		characterCamera = CharacterCamera.Instance;
		characterCameraTransform = characterCamera.transform;
		character.OnStumble += delegate
		{
			character.characterCamera.Shake();
		};
		character.OnLanding += UpdateGroundTag;
	}

	public override IEnumerator Begin()
	{
		Screen.sleepTimeout = -1;
		bool transitionFromJetpack = characterTransform.position.y > 70f;
		character.characterCollider.enabled = true;
		character.characterCamera.enabled = true;
		SmoothDampVector3 currentCameraOffset = new SmoothDampVector3(cameraOffset, cameraOffsetSmoothDuration)
		{
			Target = cameraOffset
		};
		SmoothDampFloat smoothCameraX = new SmoothDampFloat(characterCameraTransform.position.x, smoothCameraXDuration);
		SmoothDampFloat currentCameraAimOffset = new SmoothDampFloat(cameraAimOffset, cameraOffsetSmoothDuration);
		offsetDeltaCurve = new Curve();
		character.lastGroundedY = character.transform.position.y;
		AnimationEvent setAnimationSpeedEvent = new AnimationEvent
		{
			functionName = "SetAnimationSpeedEvent",
			time = 0.1f,
			messageOptions = SendMessageOptions.RequireReceiver
		};
		Globals.TryAddAnimationEvent(characterRendering.characterAnimation, "run", setAnimationSpeedEvent);
		Globals.TryAddAnimationEvent(characterRendering.characterAnimation, "run2", setAnimationSpeedEvent);
		Globals.TryAddAnimationEvent(characterRendering.characterAnimation, "run3", setAnimationSpeedEvent);
		Globals.TryAddAnimationEvent(characterRendering.characterAnimation, "run4_long", setAnimationSpeedEvent);
		ParticleSystem[] componentsInChildren = characterRendering.CharacterModel.sprayCanModel.GetComponentsInChildren<ParticleSystem>();
		foreach (ParticleSystem ps in componentsInChildren)
		{
			ps.enableEmission = false;
		}
		float transitionTimeMax = 2f;
		float transitionTime = 0f;
		Vector3 cameraPositionStart = characterCamera.position;
		Vector3 cameraTargetStart = characterCamera.target;
		characterController.Move(Vector3.down * 2f);
		SmoothDampFloat y = new SmoothDampFloat(characterTransform.position.y, ySmoothDuration);
		while (true)
		{
			game.LayTrackChunks();
			game.currentSpeed = game.currentLevelSpeed;
			game.HandleControls();
			character.ApplyGravity();
			character.MoveForward();
			Vector3 position = character.transform.position;
			if (character.IsJumpingHigher)
			{
				y.Target = 0.5f * (character.lastGroundedY + position.y);
			}
			else if (characterController.isGrounded)
			{
				y.Target = character.lastGroundedY;
			}
			else if (position.y < character.lastGroundedY)
			{
				y.Target = position.y;
			}
			y.Update();
			currentCameraOffset.Update();
			currentCameraAimOffset.Update();
			smoothCameraX.Update();
			Vector3 offsetDelta = offsetDeltaCurve.Evaluate(character.z - tunnelStartZ);
			// 摄像头 X 完全跟随玩家（原为 0.75 倍），保证玩家始终在屏幕中心，跑出三道也不会偏出画面。
			smoothCameraX.Target = position.x;
			Vector3 cameraPositionEnd = new Vector3(smoothCameraX.Value, y.Value, position.z) + currentCameraOffset.Value + offsetDelta;
			Vector3 cameraTargetEnd = new Vector3(smoothCameraX.Value, y.Value, position.z) + Vector3.up * currentCameraAimOffset.Value + offsetDelta * 0.5f;
			if (PlayerInfo.Instance != null && PlayerInfo.Instance.currentCharacter == (int)Characters.CharacterType.spike)
			{
				Vector3 guardCameraTarget = position + Vector3.forward * 6f + Vector3.up * 3f;
				cameraPositionEnd = Vector3.Lerp(cameraPositionEnd, position + Vector3.forward * 2f + Vector3.up * 2f, 0.25f);
				cameraTargetEnd = Vector3.Lerp(cameraTargetEnd, guardCameraTarget, 0.4f);
			}
			Vector3 offset = cameraTargetEnd - cameraPositionEnd;
			if (character.IsInsideSubway)
			{
				float cameraMaxYinSubway = character.subwayMaxY;
				cameraPositionEnd.y = Mathf.Min(cameraMaxYinSubway, cameraPositionEnd.y);
				cameraTargetEnd = cameraPositionEnd + offset;
				Debug.DrawLine(cameraPositionEnd, cameraTargetEnd, Color.blue, 10f);
			}
			if (transitionFromJetpack)
			{
				float transitionRatio = Mathf.Clamp01(transitionTime / transitionTimeMax);
				float warpedRatio = transitionFromJetpackCurve.Evaluate(transitionRatio);
				character.characterCamera.position = Vector3.Lerp(cameraPositionStart, cameraPositionEnd, warpedRatio);
				character.characterCamera.target = Vector3.Lerp(cameraTargetStart, cameraTargetEnd, warpedRatio);
				if (transitionRatio == 1f)
				{
					transitionFromJetpack = false;
				}
				transitionTime += Time.deltaTime;
			}
			else
			{
				character.characterCamera.position = cameraPositionEnd;
				character.characterCamera.target = cameraTargetEnd;
			}
			character.CheckInAirJump();
			game.UpdateMeters();
			UpdateInAirRunPosition();
			UpdateRunStateMeters();
			yield return null;
		}
	}

	private void UpdateRunStateMeters()
	{
		float num = game.currentSpeed * Time.deltaTime;
		GameStats gameStats = GameStats.Instance;
		if (currentRunPosition != RunPositions.air)
		{
			if (character.trackIndex == 0)
			{
				gameStats.metersRunLeftTrack += num;
			}
			if (character.trackIndex == 1)
			{
				gameStats.metersRunCenterTrack += num;
			}
			if (character.trackIndex == 2)
			{
				gameStats.metersRunRightTrack += num;
			}
		}
		if (currentRunPosition == RunPositions.ground)
		{
			GameStats.Instance.metersRunGround += num;
		}
		if (currentRunPosition == RunPositions.air)
		{
			GameStats.Instance.metersFly += num;
		}
		if (currentRunPosition == RunPositions.station)
		{
			GameStats.Instance.metersRunStation += num;
		}
		if (currentRunPosition == RunPositions.train)
		{
			GameStats.Instance.metersRunTrain += num;
		}
		if (currentRunPosition == RunPositions.movingTrain)
		{
			GameStats.Instance.metersRunTrain += num;
		}
	}

	private void UpdateInAirRunPosition()
	{
		if (!characterController.isGrounded)
		{
			currentRunPosition = RunPositions.air;
		}
	}

	private void LandedOnTrain(Collider trainCollider)
	{
		if (character.hoverboard.enabled && !GrindedTrains.Contains(trainCollider))
		{
			if (GrindedTrains.Count > GrindedTrainsBufferSize)
			{
				GrindedTrains.Dequeue();
			}
			GrindedTrains.Enqueue(trainCollider);
			GameStats.Instance.grindedTrains++;
		}
		Missions.Instance.PlayerDidThis(Missions.MissionTarget.LandOnTrainInRow);
	}

	private void UpdateGroundTag()
	{
		Ray ray = new Ray(character.characterRoot.position, -Vector3.up);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo))
		{
			switch (hitInfo.collider.tag)
			{
			case "Ground":
				currentRunPosition = RunPositions.ground;
				Missions.Instance.RemoveProgressForThis(Missions.MissionTarget.LandOnTrainInRow);
				break;
			case "HitTrain":
				currentRunPosition = RunPositions.train;
				LandedOnTrain(hitInfo.collider);
				break;
			case "HitMovingTrain":
				currentRunPosition = RunPositions.movingTrain;
				LandedOnTrain(hitInfo.collider);
				break;
			case "Station":
				currentRunPosition = RunPositions.station;
				break;
			}
		}
	}

	public override void HandleCriticalHit()
	{
		character.characterCamera.Shake();
		character.StopStumble();
		if (character.hoverboard.isActive)
		{
			character.hoverboard.Stop = CharacterModifier.StopSignal.STOP;
		}
	}

	public override void HandleSwipe(SwipeDir swipeDir)
	{
		// 玩家已接管保安：换道/跳跃作用于保安，原角色不受控继续向前跑
		if (game.controllingGuard)
		{
			FollowingGuard guard = FollowingGuard.Instance;
			switch (swipeDir)
			{
			case SwipeDir.Left:
				guard.MoveGuardTrack(-1);
				break;
			case SwipeDir.Right:
				guard.MoveGuardTrack(1);
				break;
			case SwipeDir.Up:
				guard.Jump(0f);
				break;
			case SwipeDir.Down:
				guard.GuardRoll();
				break;
			}
			return;
		}
		switch (swipeDir)
		{
		case SwipeDir.None:
			break;
		case SwipeDir.Left:
			character.ChangeTrack(-1, characterChangeTrackLength / game.currentSpeed);
			break;
		case SwipeDir.Right:
			character.ChangeTrack(1, characterChangeTrackLength / game.currentSpeed);
			break;
		case SwipeDir.Up:
			character.Jump();
			break;
		case SwipeDir.Down:
			character.Roll();
			break;
		}
	}

	public override void HandleDoubleTap()
	{
		int upgradeAmount = PlayerInfo.Instance.GetUpgradeAmount(PowerupType.hoverboard);
		if (upgradeAmount > 0 && !game.modifiers.IsActive(game.modifiers.Hoverboard))
		{
			game.Modifiers.Add(game.Modifiers.Hoverboard);
		}
	}

	public void StartTunnel(float tunnelLength)
	{
		tunnelStartZ = character.z;
		offsetDeltaCurve = new Curve();
		offsetDeltaCurve.AddKey(0f, Vector3.zero, -Vector3.up, -Vector3.up);
		offsetDeltaCurve.AddKey(tunnelLength / 2f, Vector3.up * tunnelDelta);
		offsetDeltaCurve.AddKey(tunnelLength, Vector3.zero, Vector3.up * 0.001f, Vector3.up * 0.001f);
	}

	public void EndTunnel()
	{
	}
}
