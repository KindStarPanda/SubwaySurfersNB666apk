using System;
using System.Collections;
using UnityEngine;

public class Jetpack : CharacterState
{
	public delegate void OnStartDelegate(bool isHeadStart);

	public delegate void OnStopDelegate();

	public delegate void OnFlyAheadStartDelegate();

	public delegate void OnFlyAheadUpdateDelegate(float ratio);

	[Serializable]
	public class FlyAheadInfo
	{
		public AnimationCurve cameraMovement;
	}

	public bool isActive;

	public AudioClipInfo powerDownSound;

	public Vector3 cameraOffset = new Vector3(0f, 33f, -33f);

	public float cameraOffsetSmoothDuration = 1f;

	public float cameraAimOffset = 20f;

	public float cameraFOV = 60f;

	public float ySmoothDuration = 0.5f;

	public float speedup = 2f;

	public float flyHeight = 95f;

	public float hitCeilingZPosition = 10f;

	public ParticleSystem ceilingBrickExpolsion;

	public float coinOffset = 200f;

	public float flyAheadDuration = 1.5f;

	private float flyingDuration;

	public float calmDownDuration = 2f;

	public float stopBeforeLandingChunkDistance = 50f;

	public float characterAngle = 45f;

	public float characterChangeTrackLength = 60f;

	public bool headStart;

	public float headStartDistance;

	public float headStartSpeed = 100f;

	public PowerupType powerType;

	public ActivePowerup Powerup;

	public FlyAheadInfo flyAhead;

	private Game game;

	private Track track;

	private Character character;

	private CharacterRendering characterRendering;

	private CharacterController characterController;

	private Transform characterTransform;

	private CharacterCamera characterCamera;

	private Transform characterCameraTransform;

	private Animation characterAnimation;

	public InAirCoinsManager coinsManager;

	private float landingZ;

	private float landingTime;

	public AnimationCurve fisso;

	private static Jetpack instance;

	public OnStartDelegate OnStart;

	public OnStopDelegate OnStop;

	public OnFlyAheadStartDelegate OnFlyAheadStart;

	public OnFlyAheadUpdateDelegate OnFlyAheadUpdate;

	private Vector3 jetpackParticlesInitialRotation;

	private Vector3 jetpackParticlesInitialScale;

	public override bool PauseActiveModifiers
	{
		get
		{
			return true;
		}
	}

	public float LandingZ
	{
		get
		{
			return landingZ;
		}
	}

	public float LandingTime
	{
		get
		{
			return landingTime;
		}
	}

	public static Jetpack Instance
	{
		get
		{
			return instance ?? (instance = UnityEngine.Object.FindObjectOfType(typeof(Jetpack)) as Jetpack);
		}
	}

	public void Awake()
	{
		game = Game.Instance;
		track = Track.Instance;
		character = Character.Instance;
		characterRendering = CharacterRendering.Instance;
		characterController = character.characterController;
		characterTransform = characterController.transform;
		characterCamera = CharacterCamera.Instance;
		characterCameraTransform = characterCamera.transform;
		coinsManager = this.FindObject<InAirCoinsManager>();
	}

	public override IEnumerator Begin()
	{
		isActive = true;
		character.IsGrounded.Value = false;
		GameStats.Instance.pickedUpPowerups++;
		Powerup = GameStats.Instance.TriggerPowerup(powerType);
		game.Modifiers.PauseInJetpackMode();
		if (character.IsStumbling)
		{
			character.StopStumble();
		}
		NotifyOnStart(headStart);
		Vector3 startCameraOffset = characterCameraTransform.position - characterTransform.position;
		float startCameraAimOffset = game.Running.cameraAimOffset;
		float startY = characterTransform.position.y;
		characterController.detectCollisions = false;
		character.characterCollider.enabled = false;
		SmoothDampFloat y = new SmoothDampFloat(characterTransform.position.y, ySmoothDuration)
		{
			Target = flyHeight
		};
		float jetpackSpeed = ((!headStart) ? (game.currentLevelSpeed * speedup) : headStartSpeed);
		float flyingDuration = ((!headStart) ? Powerup.timeLeft : (headStartDistance / headStartSpeed));
		float flyDistance2 = jetpackSpeed * flyingDuration;
		float flyAheadDistance = jetpackSpeed * flyAheadDuration;
		float jetpackDistance2 = flyAheadDistance + flyDistance2;
		jetpackDistance2 = track.LayJetpackChunks(character.z, jetpackDistance2) - stopBeforeLandingChunkDistance * Game.Instance.NormalizedGameSpeed;
		float extendedJetpackDuration = jetpackDistance2 / jetpackSpeed;
		float extendedFlyDuration = extendedJetpackDuration - flyAheadDuration;
		flyDistance2 = extendedFlyDuration * jetpackSpeed;
		if (!headStart)
		{
			float coinsDistance = flyDistance2 - coinOffset;
			float coinsStartZ = character.z + flyAheadDistance + coinOffset;
			coinsManager.Spawn(coinsStartZ, coinsDistance, flyHeight);
		}
		landingTime = Time.time + extendedJetpackDuration;
		landingZ = character.z + jetpackDistance2;
		float cameraZ = character.z;
		float startTime2 = Time.time;
		float ratio2 = (Time.time - startTime2) / flyAheadDuration;
		game.currentSpeed = jetpackSpeed;
		Vector3 cameraPositionStart = characterCamera.position;
		Vector3 cameraTargetStart = characterCamera.target;
		if (OnFlyAheadStart != null)
		{
			OnFlyAheadStart();
		}
		bool hasExploded = false;
		while (ratio2 < 1f)
		{
			game.HandleControls();
			Vector3 currentCameraOffset = Vector3.Lerp(startCameraOffset, cameraOffset, Mathf.SmoothStep(0f, 1f, ratio2));
			float characterAimOffset = Mathf.Lerp(startCameraAimOffset, cameraAimOffset, Mathf.SmoothStep(0f, 1f, ratio2));
			character.z += jetpackSpeed * Time.deltaTime;
			Vector3 pivot = track.GetPosition(character.x, character.z) + Vector3.up * (startY + (flyHeight - startY) * Mathf.SmoothStep(0f, 1f, ratio2));
			characterTransform.position = pivot;
			cameraZ += ((!headStart) ? game.currentLevelSpeed : jetpackSpeed) * Time.deltaTime;
			Vector3 cameraPositionEnd = new Vector3(track.GetPosition(character.x, character.z).x, pivot.y, character.z) + currentCameraOffset;
			Vector3 cameraTargetEnd = pivot + Vector3.up * characterAimOffset;
			float warpedRatio = flyAhead.cameraMovement.Evaluate(ratio2);
			Vector3 cameraPositionNew = Vector3.Lerp(cameraPositionStart, cameraPositionEnd, warpedRatio);
			Vector3 cameraTargetNew = Vector3.Lerp(cameraTargetStart, cameraTargetEnd, warpedRatio);
			cameraPositionNew.x = cameraPositionEnd.x;
			cameraTargetNew.x = cameraPositionEnd.x;
			characterCamera.position = cameraPositionNew;
			characterCamera.target = cameraTargetNew;
			if (OnFlyAheadUpdate != null)
			{
				OnFlyAheadUpdate(ratio2);
			}
			if (!hasExploded && pivot.y > hitCeilingZPosition && character.IsInsideSubway)
			{
				ceilingBrickExpolsion.gameObject.active = true;
				ceilingBrickExpolsion.Play();
				hasExploded = true;
				character.ForceLeaveSubway();
			}
			game.UpdateMeters();
			game.LayTrackChunks();
			yield return null;
			ratio2 = (Time.time - startTime2) / flyAheadDuration;
		}
		character.characterCollider.enabled = true;
		startTime2 = Time.time;
		for (ratio2 = (Time.time - startTime2) / extendedFlyDuration; ratio2 < 1f; ratio2 = (Time.time - startTime2) / extendedFlyDuration)
		{
			game.HandleControls();
			character.z += jetpackSpeed * Time.deltaTime;
			character.transform.position = track.GetPosition(character.x, character.z) + Vector3.up * flyHeight;
			Vector3 characterPosition = character.transform.position;
			characterCamera.position = characterPosition + cameraOffset;
			characterCamera.target = characterPosition + Vector3.up * cameraAimOffset;
			game.UpdateMeters();
			game.LayTrackChunks();
			yield return null;
		}
		isActive = false;
		NotifyOnStop();
		characterController.detectCollisions = true;
		coinsManager.ReleaseCoins();
		game.Modifiers.Resume();
		game.ChangeState(game.Running);
	}

	public override void HandleSwipe(SwipeDir swipeDir)
	{
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
		}
	}

	private void NotifyOnStart(bool isHeadstart)
	{
		if (OnStart != null)
		{
			OnStart(isHeadstart);
		}
	}

	private void NotifyOnStop()
	{
		if (OnStop != null)
		{
			OnStop();
		}
	}
}
