using System;
using System.Collections;
using UnityEngine;

public class FollowingGuard : MonoBehaviour
{
	public delegate void OnCatchPlayerDelegate(string currentChartacterCatch, float catchUpTime, float waitTimeBeforeScreen);

	[Serializable]
	public class CatchAnimationSet
	{
		public AnimationClip avatar;

		public AnimationClip guard;

		public AnimationClip dog;

		public float catchAvatarAnimationPlayOffset;

		public float waitTimeBeforeScreen;
	}

	public float distanceToCharacterMin = 10f;

	public float distanceToCharacterMax = 50f;

	public float catchUpDuration = 0.7f;

	public float resetCatchUpDuration = 1.5f;

	public float lastGroundedSmoothTime = 0.3f;

	public float xSmoothTime = 0.1f;

	public float gravity = 200f;

	public bool isShowing;

	public Animation guardAnimation;

	public Animation dogRightAnimation;

	public CatchAnimationSet[] caughtLeft;

	public CatchAnimationSet[] caughtRight;

	private string currentAvatarStumbleDeath;

	private string previusAvatarCaughtAnimLeft;

	private string previusAvatarCaughtAnimRight;

	public int debugCatchAnimationToPlay = -1;

	private Renderer[] enemyRenderers;

	public Transform[] enemies;

	private Vector3[] enemiesStartPos;

	private float y;

	private bool closeToCharacter;

	private float distanceToCharacter;

	private float lastGroundedSmooth;

	private float lastGroundedVelocity;

	private SmoothDampFloat x;

	private Game game;

	private Character character;

	private CharacterRendering characterRendering;

	private Transform characterTransform;

	private string previusAvatarStumbleDeath;

	public OnCatchPlayerDelegate OnCatchPlayer;

	private float verticalSpeed;

	public float guardProximityLoopVolume = 0.9f;

	private bool isPaused = true;

	// 玩家手动接管保安时为 true，此时保安横向不再硬跟随角色，而是按 manualTrackIndex 换道
	public bool manualControl;

	private int manualTrackIndex = 1;

	// 保安“角色化”所需的碰撞体与状态
	private OnTriggerObject guardTrigger;

	private BoxCollider guardCollider;

	private bool guardCrashed;

	// 保安独立前进的世界 z（不与玩家绑定）
	private float guardZ;

	// 保安下滚状态（无动画，仅缩小碰撞箱高度）
	private bool guardRolling;

	private static FollowingGuard instance;

	public static FollowingGuard Instance
	{
		get
		{
			return instance ?? (instance = UnityEngine.Object.FindObjectOfType(typeof(FollowingGuard)) as FollowingGuard);
		}
	}

	public void Initialize()
	{
		game = Game.Instance;
		character = Character.Instance;
		characterRendering = CharacterRendering.Instance;
		characterTransform = character.transform;
		enemyRenderers = base.gameObject.GetComponentsInChildren<Renderer>();
		enemiesStartPos = new Vector3[enemies.Length];
		for (int i = 0; i < enemies.Length; i++)
		{
			enemiesStartPos[i] = enemies[i].position;
		}
		x = new SmoothDampFloat(0f, xSmoothTime);
		base.GetComponent<AudioSource>().volume = guardProximityLoopVolume;
		Game obj = game;
		obj.OnPauseChange = (Game.OnPauseChangeDelegate)Delegate.Combine(obj.OnPauseChange, new Game.OnPauseChangeDelegate(HandleOnPauseChange));
		CatchAnimationSet[] array = caughtLeft;
		foreach (CatchAnimationSet catchAnimationSet in array)
		{
			SetupAvatarAnimationsStates(characterRendering.characterAnimation, catchAnimationSet.avatar);
			SetupDogGuardAnimationsStates(guardAnimation, catchAnimationSet.guard);
			SetupDogGuardAnimationsStates(dogRightAnimation, catchAnimationSet.dog);
		}
		CatchAnimationSet[] array2 = caughtRight;
		foreach (CatchAnimationSet catchAnimationSet2 in array2)
		{
			SetupAvatarAnimationsStates(characterRendering.characterAnimation, catchAnimationSet2.avatar);
			SetupDogGuardAnimationsStates(guardAnimation, catchAnimationSet2.guard);
			SetupDogGuardAnimationsStates(dogRightAnimation, catchAnimationSet2.dog);
		}
	}

	private void SetupAvatarAnimationsStates(Animation animation, AnimationClip animationClip)
	{
		AnimationClip clip = animation.GetClip(animationClip.name);
		if (clip == null)
		{
			animation.AddClip(animationClip, animationClip.name);
		}
		animation[animationClip.name].enabled = false;
		animation[animationClip.name].layer = 4;
	}

	private void SetupDogGuardAnimationsStates(Animation animation, AnimationClip animationClip)
	{
		AnimationClip clip = animation.GetClip(animationClip.name);
		if (clip == null)
		{
			animation.AddClip(animationClip, animationClip.name);
		}
	}

	private void HandleOnPauseChange(bool pause)
	{
		if (pause)
		{
			if (base.GetComponent<AudioSource>().isPlaying)
			{
				base.GetComponent<AudioSource>().Pause();
			}
			isPaused = true;
		}
		else
		{
			if (isPaused)
			{
				base.GetComponent<AudioSource>().Play();
			}
			isPaused = false;
		}
	}

	public void Restart(bool closeToCharacter)
	{
		StopAllCoroutines();
		this.closeToCharacter = closeToCharacter;
		distanceToCharacter = ((!closeToCharacter) ? distanceToCharacterMax : distanceToCharacterMin);
	}

	public void OnEnable()
	{
		lastGroundedSmooth = character.lastGroundedY;
		lastGroundedVelocity = 0f;
		y = character.lastGroundedY;
		x.Value = character.transform.position.x;
		distanceToCharacter = distanceToCharacterMin;
		closeToCharacter = true;
		verticalSpeed = 0f;
		character.OnJump += OnJump;
		character.OnRoll += OnRoll;
	}

	public void OnDisable()
	{
		character.OnJump -= OnJump;
		character.OnRoll -= OnRoll;
	}

	public void PlayMirrorAnimation(string clipName, bool queued = false, float fadeTime = 0.05f)
	{
		if (guardAnimation == null || guardAnimation.GetClip(clipName) == null)
		{
			return;
		}
		if (queued)
		{
			guardAnimation.PlayQueued(clipName);
		}
		else
		{
			guardAnimation.CrossFade(clipName, fadeTime);
		}
	}

	public void CatchUp()
	{
		CatchUp(catchUpDuration);
	}

	public void CatchUp(float duration)
	{
		if (!closeToCharacter)
		{
			float distanceFrom = distanceToCharacter;
			ShowEnemies(true);
			StopAllCoroutines();
			guardAnimation.Play("Guard_grap after");
			guardAnimation.PlayQueued("Guard_Run");
			base.GetComponent<AudioSource>().timeSamples = UnityEngine.Random.Range(0, base.GetComponent<AudioSource>().timeSamples);
			base.GetComponent<AudioSource>().Play();
			base.GetComponent<AudioSource>().pitch = UnityEngine.Random.Range(0.9f, 1.05f);
			StartCoroutine(pTween.To(duration, delegate(float t)
			{
				distanceToCharacter = Mathf.SmoothStep(distanceFrom, distanceToCharacterMin, t);
			}));
			StartCoroutine(pTween.To(duration, delegate(float t)
			{
				base.GetComponent<AudioSource>().volume = Mathf.SmoothStep(0f, guardProximityLoopVolume, t);
			}));
			closeToCharacter = true;
		}
	}

	public void ResetCatchUp()
	{
		ResetCatchUp(resetCatchUpDuration);
	}

	public void ResetCatchUp(float duration)
	{
		StartCoroutine(ResetCatchUpCoroutine(duration));
	}

	public IEnumerator ResetCatchUpCoroutine(float duration)
	{
		if (closeToCharacter)
		{
			closeToCharacter = false;
			// 保安永久贴身显示：不再拉远距离、不再隐藏，仅淡出追逐音效
			yield return StartCoroutine(pTween.To(duration * 2f, delegate(float t)
			{
				base.GetComponent<AudioSource>().volume = Mathf.SmoothStep(guardProximityLoopVolume, 0f, t);
			}));
			base.GetComponent<AudioSource>().Stop();
		}
	}

	public void MuteProximityLoop()
	{
		base.GetComponent<AudioSource>().Stop();
	}

	public void PlayIntro()
	{
		base.gameObject.transform.position = new Vector3(0f, 0f, -10f);
		for (int i = 0; i < enemies.Length; i++)
		{
			enemies[i].position = enemiesStartPos[i];
			enemies[i].rotation = Quaternion.Euler(0f, 0f, 0f);
		}
		guardAnimation.Play("playIntro");
		dogRightAnimation.Play("playIntro");
		guardAnimation.CrossFadeQueued("Guard_Run", 0.2f);
		dogRightAnimation.CrossFadeQueued("Dog_Fast Run", 0.2f);
	}

	public void CatchPlayer(float pos)
	{
		base.GetComponent<AudioSource>().Stop();
		StopAllCoroutines();
		int num = ((debugCatchAnimationToPlay <= -1 || debugCatchAnimationToPlay >= caughtLeft.Length) ? UnityEngine.Random.Range(0, caughtLeft.Length) : debugCatchAnimationToPlay);
		int num2 = ((debugCatchAnimationToPlay <= -1 || debugCatchAnimationToPlay >= caughtRight.Length) ? UnityEngine.Random.Range(0, caughtRight.Length) : debugCatchAnimationToPlay);
		float num3;
		if (pos < 20f)
		{
			guardAnimation.CrossFade(caughtLeft[num].guard.name, 0.2f);
			dogRightAnimation.CrossFade(caughtLeft[num].dog.name, 0.2f);
			num3 = caughtLeft[num].catchAvatarAnimationPlayOffset / 25f;
			if (OnCatchPlayer != null)
			{
				OnCatchPlayer(caughtLeft[num].avatar.name, num3, caughtLeft[num].waitTimeBeforeScreen);
			}
		}
		else
		{
			guardAnimation.CrossFade(caughtRight[num2].guard.name, 0.2f);
			dogRightAnimation.CrossFade(caughtRight[num2].dog.name, 0.2f);
			num3 = caughtRight[num2].catchAvatarAnimationPlayOffset / 25f;
			if (OnCatchPlayer != null)
			{
				OnCatchPlayer(caughtRight[num2].avatar.name, num3, caughtRight[num2].waitTimeBeforeScreen);
			}
		}
		StartCoroutine(pTween.To(num3, delegate(float t)
		{
			for (int i = 0; i < enemies.Length; i++)
			{
				enemies[i].position = Vector3.Lerp(enemies[i].position, character.transform.position, t);
			}
		}));
	}

	public void HitByTrainSequence()
	{
		base.GetComponent<AudioSource>().Stop();
		StartCoroutine(HitByTrainSequenceCoroutine());
	}

	public IEnumerator HitByTrainSequenceCoroutine()
	{
		GameStats.Instance.guardHitScreen++;
		float catchUpTime = 0.2f;
		yield return StartCoroutine(pTween.To(catchUpTime, delegate(float t)
		{
			for (int i = 0; i < enemies.Length; i++)
			{
				enemies[i].position = Vector3.Lerp(enemies[i].position, character.transform.position, t);
			}
		}));
		dogRightAnimation.Play("Dog_death_movingTrain");
		yield return new WaitForSeconds(0.4f);
		Vector3 charPos = characterTransform.position;
		StartCoroutine(pTween.To(1f, delegate(float t)
		{
			characterTransform.position = Vector3.Lerp(charPos, new Vector3(charPos.x, -5f, charPos.z), t);
		}));
		yield return new WaitForSeconds(0.2f);
		guardAnimation.Play("Guard_death_movingTrain");
	}

	public void ShowEnemies(bool vis)
	{
		// 游戏进行中保安永久显示，忽略隐藏请求（避免因离玩家远/太久没受伤而消失）
		if (game != null && game.IsInGame.Value)
		{
			vis = true;
		}
		isShowing = vis;
		Renderer[] array = enemyRenderers;
		foreach (Renderer renderer in array)
		{
			renderer.gameObject.active = vis;
		}
	}

	// 由玩家接管/释放对保安的控制
	public void SetManualControl(bool on)
	{
		manualControl = on;
		if (on)
		{
			// 从角色当前所在车道开始，拉近并显示保安
			manualTrackIndex = character.trackIndex;
			distanceToCharacter = distanceToCharacterMin;
			closeToCharacter = true;
			guardCrashed = false;
			guardZ = character.z;
			ShowEnemies(true);
			EnsureGuardCollider();
			if (guardCollider != null)
			{
				guardCollider.enabled = true;
			}
			// 原玩家角色免疫碰撞、持续向前奔跑，作为世界驱动器（轨道/金币不断生成），
			// 因此玩家角色不会死亡、世界不停，保安得以一直自己跑自己的。
			character.stopColliding = true;
		}
		else
		{
			if (guardCollider != null)
			{
				guardCollider.enabled = false;
			}
			character.stopColliding = false;
		}
	}

	// 运行时为保安创建触发碰撞体，使其能像玩家一样检测金币/道具/障碍/火车
	private void EnsureGuardCollider()
	{
		if (guardTrigger != null)
		{
			return;
		}
		if (base.GetComponent<Rigidbody>() == null)
		{
			Rigidbody rb = base.gameObject.AddComponent<Rigidbody>();
			rb.isKinematic = true;
			rb.useGravity = false;
		}
		guardCollider = base.gameObject.AddComponent<BoxCollider>();
		guardCollider.isTrigger = true;
		guardCollider.center = new Vector3(0f, 8f, 0f);
		guardCollider.size = new Vector3(15f, 16f, 8f);
		// 与玩家角色同层，最大程度复用玩家的碰撞层行为
		base.gameObject.layer = character.characterCollider.gameObject.layer;
		guardTrigger = base.gameObject.AddComponent<OnTriggerObject>();
		OnTriggerObject onTriggerObject = guardTrigger;
		onTriggerObject.OnEnter = (OnTriggerObject.OnEnterDelegate)Delegate.Combine(onTriggerObject.OnEnter, new OnTriggerObject.OnEnterDelegate(OnGuardTriggerEnter));
	}

	private void OnGuardTriggerEnter(Collider collider)
	{
		if (!manualControl || guardCrashed || collider == null || !game.IsInGame.Value)
		{
			return;
		}
		if (collider.CompareTag("Subway"))
		{
			return;
		}
		// 捡道具（金币、磁铁、喷气背包等）——与玩家完全一致
		Pickup pickup = collider.GetComponentInChildren<Pickup>();
		if (pickup != null)
		{
			character.NotifyPickup(pickup);
			return;
		}
		// 保安不受伤、不死亡：碰到火车/障碍不做任何处理
	}

	// 玩家控制保安下滚（按 S）：不播放动画，仅临时缩小碰撞箱高度以滚过障碍
	public void GuardRoll()
	{
		if (guardCollider == null || guardRolling)
		{
			return;
		}
		StartCoroutine(GuardRollCoroutine());
	}

	private IEnumerator GuardRollCoroutine()
	{
		guardRolling = true;
		// 缩小碰撞箱高度（贴地），可滚过上方障碍
		guardCollider.center = new Vector3(0f, 4f, 0f);
		guardCollider.size = new Vector3(15f, 8f, 8f);
		yield return new WaitForSeconds(0.6f);
		// 恢复原碰撞箱高度
		guardCollider.center = new Vector3(0f, 8f, 0f);
		guardCollider.size = new Vector3(15f, 16f, 8f);
		guardRolling = false;
	}

	// 玩家控制保安左右换道（dir = -1 左，1 右）
	public void MoveGuardTrack(int dir)
	{
		manualTrackIndex = Mathf.Clamp(manualTrackIndex + dir, 0, Track.Instance.numberOfTracks - 1);
	}

	// 保安当前世界 z（供地形加载使用）
	public float GuardZ
	{
		get
		{
			return guardZ;
		}
	}

	public void LateUpdate()
	{
		Vector3 position;
		if (manualControl)
		{
			// 玩家接管保安：横向目标为玩家选择的车道位置
			x.Target = Track.Instance.GetPosition(Track.Instance.GetTrackX(manualTrackIndex), 0f).x;
			x.Update();
			// 保安独立向前跑，不与玩家 z 绑定
			guardZ += game.currentSpeed * Time.deltaTime;
			// 独立检测脚下地面（含火车顶），使保安能自己踩上火车而不依赖主角 y
			float targetGroundY = GetGuardGroundY();
			if (y > targetGroundY + 0.1f)
			{
				verticalSpeed -= gravity * Time.deltaTime;
			}
			y += verticalSpeed * Time.deltaTime;
			if (y <= targetGroundY)
			{
				y = targetGroundY;
				if (verticalSpeed < 0f)
				{
					verticalSpeed = 0f;
				}
			}
			position = new Vector3(x.Value, y, guardZ);
		}
		else
		{
			x.Target = character.transform.position.x;
			x.Update();
			lastGroundedSmooth = Mathf.SmoothDamp(lastGroundedSmooth, character.lastGroundedY, ref lastGroundedVelocity, lastGroundedSmoothTime);
			if (y > lastGroundedSmooth)
			{
				verticalSpeed -= gravity * Time.deltaTime;
			}
			y += verticalSpeed * Time.deltaTime;
			y = Mathf.Max(y, lastGroundedSmooth);
			position = characterTransform.position - Vector3.forward * distanceToCharacter;
			position.y = y;
			position.x = x.Value;
		}
		base.transform.position = position;
	}

	// 从高空向下射线检测保安脚下可站立表面（地面/火车顶/站台）的高度
	private float GetGuardGroundY()
	{
		Ray ray = new Ray(new Vector3(x.Value, 300f, guardZ), Vector3.down);
		RaycastHit hit;
		// 忽略触发器（保安自身碰撞箱、金币等都是 trigger），只命中地面/火车等实体
		if (Physics.Raycast(ray, out hit, 600f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
		{
			switch (hit.collider.tag)
			{
			case "Ground":
			case "HitTrain":
			case "HitMovingTrain":
			case "Station":
				return hit.point.y;
			}
		}
		return 0f;
	}

	private void OnRoll()
	{
		StartCoroutine(RollCoroutine(distanceToCharacter / game.currentSpeed));
	}

	private IEnumerator RollCoroutine(float delay)
	{
		yield return new WaitForSeconds(delay);
		verticalSpeed = 0f - character.CalculateJumpVerticalSpeed();
	}

	private void OnJump()
	{
		Jump(distanceToCharacter / game.currentSpeed);
	}

	public void Jump(float delay)
	{
		if (distanceToCharacter <= distanceToCharacterMin)
		{
			Missions.Instance.PlayerDidThis(Missions.MissionTarget.GuardJump);
		}
		StartCoroutine(JumpCoroutine(delay));
	}

	private IEnumerator JumpCoroutine(float delay)
	{
		yield return new WaitForSeconds(delay);
		guardAnimation.Play("Guard_jump");
		guardAnimation.CrossFadeQueued("Guard_Run", 0.2f);
		dogRightAnimation.Play("Dog_jump");
		dogRightAnimation.CrossFadeQueued("Dog_Fast Run", 0.2f);
		verticalSpeed = character.CalculateJumpVerticalSpeed() * 0.7f;
	}
}
