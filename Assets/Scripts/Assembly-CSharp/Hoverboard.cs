using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Hoverboard : CharacterModifier
{
	public delegate void OnSwitchToHoverboardDelegate(GameObject hoverbooard);

	public delegate void OnSwitchToRunningDelegate();

	public delegate void OnHoverboardJumpDelegate();

	public delegate void OnRunDelegate();

	[Serializable]
	public class HoverboardSelection
	{
		public Hoverboards.BoardType boardType;

		public GameObject hoverboardPrefab;
	}

	public HoverboardSelection[] hoverboardSelector;

	public AudioClipInfo powerDownSound;

	public float cooldownDstance = 50f;

	public float slowMotionDistance = 90f;

	public float slowDownToScale = 0.3f;

	public bool isAllowed = true;

	private GameObject hoverboardRoot;

	public float WaitForParticlesDelay;

	public float RemoveObstaclesDistance = 250f;

	private Character character;

	private OnTriggerObject coinMagnetCollider;

	private CharacterRendering characterRendering;

	private Track track;

	private float lastEndActivationTime;

	[HideInInspector]
	public bool isActive;

	public AudioClipInfo CrashSound;

	public AudioClipInfo StartSound;

	public ActivePowerup Powerup;

	private static Hoverboard instance;

	private HoverboardManager hoverboardManager;

	private Dictionary<Hoverboards.BoardType, GameObject> hoverboardThatMatchBoardType = new Dictionary<Hoverboards.BoardType, GameObject>();

	public override bool ShouldPauseInJetpack
	{
		get
		{
			return true;
		}
	}

	public static Hoverboard Instance
	{
		get
		{
			return instance ?? (instance = UnityEngine.Object.FindObjectOfType(typeof(Hoverboard)) as Hoverboard);
		}
	}

	[method: MethodImpl(32)]
	public event OnSwitchToHoverboardDelegate OnSwitchToHoverboard;

	[method: MethodImpl(32)]
	public event OnSwitchToRunningDelegate OnSwitchToRunning;

	[method: MethodImpl(32)]
	public event OnHoverboardJumpDelegate OnJump;

	[method: MethodImpl(32)]
	public event OnRunDelegate OnRun;

	public void Awake()
	{
		character = Character.Instance;
		coinMagnetCollider = character.coinMagnetLongCollider;
		characterRendering = CharacterRendering.Instance;
		characterRendering.CharacterModelInitialized += CharacterModelInitialized;
		track = Track.Instance;
		hoverboardManager = HoverboardManager.Instance;
		HoverboardSelection[] array = hoverboardSelector;
		foreach (HoverboardSelection hoverboardSelection in array)
		{
			if (!hoverboardThatMatchBoardType.ContainsKey(hoverboardSelection.boardType))
			{
				hoverboardThatMatchBoardType.Add(hoverboardSelection.boardType, hoverboardSelection.hoverboardPrefab);
				continue;
			}
			throw new Exception("There are more hoverboards assigned to the hoverboard selection");
		}
	}

	public GameObject GetActiveHoverboard()
	{
		GameObject value;
		if (hoverboardThatMatchBoardType.TryGetValue(hoverboardManager.Hoverboard, out value))
		{
			return value;
		}
		Debug.Log("You need to make a hoverboard that matches the selection");
		return null;
	}

	private void CharacterModelInitialized(GameObject root)
	{
		hoverboardRoot = root;
	}

	public override void Reset()
	{
		character.immuneToCriticalHit = false;
		character.characterController.enabled = true;
		character.characterCollider.enabled = true;
		hoverboardRoot.SetActiveRecursively(false);
		isActive = false;
		Time.timeScale = 1f;
		character.hoverboardCrashParticleSystem.gameObject.SetActiveRecursively(false);
	}

	public override IEnumerator Begin()
	{
		float timeSinceLastActivation = Time.time - lastEndActivationTime;
		if (!isAllowed || timeSinceLastActivation < WaitForParticlesDelay + PlayerInfo.Instance.GetHoverBoardCoolDown())
		{
			yield break;
		}
		bool bouncerBoard = HoverboardManager.Instance.Hoverboard == Hoverboards.BoardType.bouncer;
		bool isLowrider = HoverboardManager.Instance.Hoverboard == Hoverboards.BoardType.lowrider;
		if (bouncerBoard)
		{
			character.superSneakers.SuperSneakersSuction.Add(this);
		}
		if (isLowrider)
		{
			character.SqueezeCollider.Add(this);
		}
		PlayerInfo.Instance.UseUpgrade(PowerupType.hoverboard);
		Missions.Instance.PlayerDidThis(Missions.MissionTarget.HoverBoard);
		Paused = false;
		if (character.IsStumbling)
		{
			character.StopStumble();
		}
		isActive = true;
		GameObject newBoard = UnityEngine.Object.Instantiate(GetActiveHoverboard()) as GameObject;
		newBoard.transform.parent = hoverboardRoot.transform;
		newBoard.transform.localPosition = Vector3.zero;
		newBoard.transform.localRotation = Quaternion.identity;
		if (this.OnSwitchToHoverboard != null)
		{
			this.OnSwitchToHoverboard(newBoard);
		}
		So.Instance.playSound(StartSound);
		character.CharacterPickupParticleSystem.PickedUpDefaultPowerUp();
		character.immuneToCriticalHit = true;
		stop = StopSignal.DONT_STOP;
		Powerup = GameStats.Instance.TriggerPowerup(PowerupType.hoverboard);
		while (Powerup.timeLeft > 0f && stop == StopSignal.DONT_STOP)
		{
			yield return null;
		}
		if (bouncerBoard)
		{
			character.superSneakers.SuperSneakersSuction.Remove(this);
		}
		if (isLowrider)
		{
			character.SqueezeCollider.Remove(this);
		}
		if (stop == StopSignal.DONT_STOP)
		{
			Missions.Instance.PlayerDidThis(Missions.MissionTarget.HoverBoardExpire);
			So.Instance.playSound(powerDownSound);
			if (this.OnSwitchToRunning != null)
			{
				this.OnSwitchToRunning();
			}
			if (character.IsFalling || character.IsJumping)
			{
				if (this.OnJump != null)
				{
					this.OnJump();
				}
			}
			else if (this.OnRun != null)
			{
				this.OnRun();
			}
			UnityEngine.Object.Destroy(newBoard);
		}
		character.immuneToCriticalHit = false;
		isActive = false;
		lastEndActivationTime = Time.time;
		if (stop != StopSignal.STOP)
		{
			yield break;
		}
		isActive = false;
		character.immuneToCriticalHit = false;
		character.hoverboardCrashParticleSystem.gameObject.SetActiveRecursively(true);
		character.hoverboardCrashParticleSystem.Play();
		PlayCrashSound();
		if (this.OnSwitchToRunning != null)
		{
			this.OnSwitchToRunning();
		}
		if (this.OnJump != null)
		{
			this.OnJump();
		}
		UnityEngine.Object.Destroy(newBoard);
		float timeLeft = WaitForParticlesDelay;
		while (timeLeft > 0f)
		{
			timeLeft -= Time.deltaTime;
			yield return null;
		}
		track.LayEmptyChunks(character.z, RemoveObstaclesDistance * Game.Instance.NormalizedGameSpeed);
		character.IsJumping = true;
		character.IsFalling = false;
		character.verticalSpeed = character.CalculateJumpVerticalSpeed(10f);
		float newSlowMotionDistance = slowMotionDistance * Game.Instance.NormalizedGameSpeed;
		float newCoolDownDist = cooldownDstance * Game.Instance.NormalizedGameSpeed;
		float distanceLeft = newSlowMotionDistance;
		bool didStopCooldown = false;
		while (distanceLeft > 0f)
		{
			distanceLeft -= Game.Instance.currentLevelSpeed * Time.deltaTime;
			newCoolDownDist -= Game.Instance.currentLevelSpeed * Time.deltaTime;
			if (newCoolDownDist < 0f && !didStopCooldown)
			{
				character.immuneToCriticalHit = false;
				didStopCooldown = true;
			}
			yield return null;
		}
		character.hoverboardCrashParticleSystem.gameObject.SetActiveRecursively(false);
	}

	public void PlayCrashSound()
	{
		So.Instance.playSound(CrashSound);
	}

	public override void Pause()
	{
		hoverboardRoot.SetActiveRecursively(false);
	}

	public override void Resume()
	{
		hoverboardRoot.SetActiveRecursively(true);
	}
}
