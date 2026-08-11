using System;
using System.Collections;
using UnityEngine;

public class MysteryBoxPopup : UIScreen
{
	private const string CONTINUELABEL_OPEN = "Tap to open";

	private const string CONTINUELABEL_CONTINUE = "Tap to continue";

	private const float MOVE_IN_NEXT_MYSTERYBOX_DURATION = 0.35f;

	private const float REMOVE_MYSTERYBOX_AFTER_OPEN = 0.7f;

	private const string POPUP_PATH = "Prefabs/Popups/";

	private const float buttonShowHideSpeed = 3.1f;

	private const float BOX_IDLE_ANIM_SPEED = 2f;

	private const float BOX_IDLE_ANIM_AMOUNT = 10f;

	private const float BOX_IDLE_ANIM_END_SPEED = 20f;

	public GameObject boxParent;

	public GameObject rewardLabelTemplate;

	private Vector3 _outOfScreenPosition = new Vector3(0f, -500f, 0f);

	[SerializeField]
	private GameObject prizeButtonNormal;

	[SerializeField]
	private GameObject prizeButtonSuper;

	private readonly Vector3 _ShowPrizeButtonPosition = new Vector3(-66f, -46f, 0f);

	private readonly Vector3 _HidePrizeButtonPosition = new Vector3(-66f, 46f, 0f);

	[SerializeField]
	private GameObject openButton;

	[SerializeField]
	private GameObject continueButton;

	public UILabel coinboxLabel;

	public UILabel newTrophyLabel;

	private static readonly Vector3 FIRST_SLOT_POSITION = new Vector3(0f, 0f, -50f);

	private static readonly Vector3 OTHER_SLOT_OFFSET = new Vector3(0f, 0f, 9370f);

	private static readonly Vector3 SUPERBOX_EFFECT_POSITION = new Vector3(0f, 0f, 180f);

	private GameObject[] slots;

	[SerializeField]
	private GameObject mysteryBoxPrefab;

	[SerializeField]
	private GameObject superMysteryBoxPrefab;

	[SerializeField]
	private GameObject superMysteryBoxEffectPrefab;

	public GameObject testRewardPrefab;

	public GameObject rewardCoins;

	[SerializeField]
	private GameObject rewardTokenTricky;

	[SerializeField]
	private GameObject rewardTokenFresh;

	[SerializeField]
	private GameObject rewardTokenSpike;

	[SerializeField]
	private GameObject rewardTokenYutani;

	[SerializeField]
	private GameObject trophyDiamond;

	[SerializeField]
	private GameObject trophyGoldBar;

	[SerializeField]
	private GameObject trophyChainClock;

	[SerializeField]
	private GameObject trophyChainDollar;

	[SerializeField]
	private GameObject trophyGoldSkull;

	[SerializeField]
	private GameObject trophyHeadphones;

	[SerializeField]
	private GameObject trophyLPBlack;

	[SerializeField]
	private GameObject trophyTapeBlack;

	[SerializeField]
	private GameObject rewardPowerupHoverboard;

	[SerializeField]
	private GameObject rewardPowerupHeadstart500;

	[SerializeField]
	private GameObject rewardPowerupHeadstart2000;

	[SerializeField]
	private GameObject trophyMedalSmart;

	public UILabel ContinueLabel;

	public AudioStateLoop audioStateLoop;

	private int _boxCurrent;

	private GameObject[] _boxes;

	private bool anotherBox = true;

	private Vector3 _boxScale = new Vector3(1150f, 1150f, 1150f);

	private Vector3 _boxRotation = new Vector3(0f, 250f, 20f);

	private Vector3 _rewardStartRotation = new Vector3(15f, -10.5f, 0f);

	private Vector3 _rewardStartScale = new Vector3(5f, 5f, 5f);

	private Vector3 _rewardEndScale_coin = new Vector3(10f, 10f, 10f);

	private Vector3 _rewardEndScale = new Vector3(20f, 20f, 20f);

	private GameObject _rewardMain;

	private UILabel _labelSingle;

	private UILabel _labelDouble1;

	private Vector3 _labelPosition = new Vector3(0f, 134f, -5f);

	private bool _maySetTimeScale;

	public GameObject GlowEffect;

	private bool stopBoxIdleAnim;

	private int _numberOfBoxes;

	private MysteryBox.Type[] boxesToUnlock;

	private bool openingBoxNow;

	private SuperMysteryBoxEffect _superMysteryBoxEffect;

	private float _timeScaleBeforeMysteryBox = 1f;

	private GameObject prizeList;

	public override void Init()
	{
		base.Init();
		audioStateLoop = this.FindObject<AudioStateLoop>();
	}

	public override void Show()
	{
		base.Show();
		_timeScaleBeforeMysteryBox = Time.timeScale;
		Time.timeScale = 1f;
		SetupMysteryBoxScreen();
	}

	public override void Hide()
	{
		base.Hide();
		Time.timeScale = _timeScaleBeforeMysteryBox;
	}

	private void ShowPrizeButton(bool show)
	{
		if (show)
		{
			ShowPrizeButton(MysteryBox.Type.Normal);
			Debug.LogWarning("Show default");
		}
		else
		{
			StartCoroutine(PrizeButtonHide(prizeButtonNormal.transform));
			StartCoroutine(PrizeButtonHide(prizeButtonSuper.transform));
		}
	}

	private void ShowPrizeButton(MysteryBox.Type type)
	{
		if (type == MysteryBox.Type.Normal)
		{
			StartCoroutine(PrizeButtonShow(prizeButtonNormal.transform));
			StartCoroutine(PrizeButtonHide(prizeButtonSuper.transform));
		}
		else
		{
			StartCoroutine(PrizeButtonHide(prizeButtonNormal.transform));
			StartCoroutine(PrizeButtonShow(prizeButtonSuper.transform));
		}
	}

	private IEnumerator PrizeButtonShow(Transform toShow)
	{
		toShow.GetComponent<Collider>().enabled = true;
		float lerpTime = Vector3.Distance(toShow.localPosition, _HidePrizeButtonPosition) / Vector3.Distance(_HidePrizeButtonPosition, _ShowPrizeButtonPosition);
		while (lerpTime < 1f)
		{
			lerpTime += Time.deltaTime * 3.1f;
			toShow.localPosition = Vector3.Lerp(_HidePrizeButtonPosition, _ShowPrizeButtonPosition, lerpTime);
			yield return null;
		}
		toShow.localPosition = _ShowPrizeButtonPosition;
	}

	private IEnumerator PrizeButtonHide(Transform toHide)
	{
		toHide.GetComponent<Collider>().enabled = false;
		float lerpTime = Vector3.Distance(toHide.localPosition, _ShowPrizeButtonPosition) / Vector3.Distance(_HidePrizeButtonPosition, _ShowPrizeButtonPosition);
		while (lerpTime < 1f)
		{
			lerpTime += Time.deltaTime * 3.1f;
			toHide.localPosition = Vector3.Lerp(_ShowPrizeButtonPosition, _HidePrizeButtonPosition, lerpTime);
			yield return null;
		}
		toHide.localPosition = _HidePrizeButtonPosition;
	}

	private void ShowPrizeList(bool show)
	{
		if (show)
		{
			ShowPrizeList(MysteryBox.Type.Normal);
			Debug.LogWarning("Show default");
			return;
		}
		for (int i = 0; i < _numberOfBoxes; i++)
		{
			if (_boxes[i] != null)
			{
				NGUITools.SetActive(_boxes[i], true);
			}
		}
		if (_superMysteryBoxEffect != null)
		{
			_superMysteryBoxEffect.SetVisible(true);
		}
		if (prizeList != null)
		{
			UnityEngine.Object.Destroy(prizeList);
		}
		openButton.GetComponent<Collider>().enabled = true;
		continueButton.GetComponent<Collider>().enabled = true;
	}

	private void ShowPrizeList(MysteryBox.Type type)
	{
		if (prizeList != null)
		{
			UnityEngine.Object.Destroy(prizeList);
		}
		switch (type)
		{
		case MysteryBox.Type.Normal:
			Flurry.LogEventWithAParameter("Mysterybox view prices", "Screen Name", "ShowMysteryBox");
			prizeList = NGUITools.AddChild(base.gameObject, Resources.Load("Prefabs/Popups/PrizeMBPopup") as GameObject);
			break;
		case MysteryBox.Type.Super:
			Flurry.LogEventWithAParameter("Mysterybox view prices", "Screen Name", "ShowSuperMysteryBox");
			prizeList = NGUITools.AddChild(base.gameObject, Resources.Load("Prefabs/Popups/PrizeSMBPopup") as GameObject);
			break;
		default:
			Debug.LogError("Unhandeled mysterybox type " + type, this);
			return;
		}
		for (int i = 0; i < _numberOfBoxes; i++)
		{
			if (_boxes[i] != null)
			{
				NGUITools.SetActive(_boxes[i], false);
			}
		}
		if (_superMysteryBoxEffect != null)
		{
			_superMysteryBoxEffect.SetVisible(false);
		}
		prizeList.transform.localPosition = new Vector3(0f, 0f, -30f);
		openButton.GetComponent<Collider>().enabled = false;
		continueButton.GetComponent<Collider>().enabled = false;
	}

	private void SetupMysteryBoxScreen()
	{
		boxParent.transform.position = UIModelController.Instance.MysteryBoxAnchor.transform.position;
		boxesToUnlock = PlayerInfo.Instance.GetAndClearMysteryBoxesToUnlock();
		ShowPrizeButton(boxesToUnlock[0]);
		_numberOfBoxes = boxesToUnlock.Length;
		if (_numberOfBoxes <= 0)
		{
			anotherBox = false;
			Debug.LogError("You should not be here when do do not have any mysteryboxes");
			UIScreenController.Instance.ClosePopup();
			openButton.GetComponent<Collider>().enabled = false;
			return;
		}
		if (_numberOfBoxes == 1)
		{
			anotherBox = false;
		}
		else
		{
			anotherBox = true;
		}
		ContinueLabel.alpha = 1f;
		ContinueLabel.text = "Tap to open";
		newTrophyLabel.gameObject.active = false;
		slots = new GameObject[_numberOfBoxes];
		_boxes = new GameObject[_numberOfBoxes];
		for (int i = 0; i < _numberOfBoxes; i++)
		{
			slots[i] = NGUITools.AddChild(boxParent);
			slots[i].transform.localPosition = FIRST_SLOT_POSITION + OTHER_SLOT_OFFSET * ((i > 0) ? 1 : 0);
		}
		FillOutAllTheSlotsWithBoxes();
		_boxCurrent = 0;
		if (superMysteryBoxEffectPrefab != null)
		{
			GameObject gameObject = NGUITools.AddChild(slots[_boxCurrent], superMysteryBoxEffectPrefab);
			gameObject.transform.transform.localPosition = SUPERBOX_EFFECT_POSITION;
			_superMysteryBoxEffect = gameObject.GetComponent<SuperMysteryBoxEffect>();
			if (_superMysteryBoxEffect == null)
			{
				Debug.LogError("MysteryBoxHandler ERROR: unable to find SuperMysteryBoxEffect component on superMysteryBoxEffectPrefab", this);
			}
			else if (boxesToUnlock[_boxCurrent] == MysteryBox.Type.Super)
			{
				_superMysteryBoxEffect.StartEffect();
			}
			else
			{
				_superMysteryBoxEffect.StopEffect();
			}
		}
		else
		{
			Debug.LogError("MysteryBoxHandler ERROR: superMysteryBoxEffectPrefab is not assigned", this);
		}
		if (_boxes[_boxCurrent] != null)
		{
			StartCoroutine(BoxIdleAnimCoroutine(_boxes[_boxCurrent].transform));
		}
		openButton.GetComponent<Collider>().enabled = true;
	}

	public void SkipNow()
	{
		if (_maySetTimeScale)
		{
			Time.timeScale = 4f;
		}
	}

	private void FillOutAllTheSlotsWithBoxes()
	{
		for (int i = 0; i < _numberOfBoxes; i++)
		{
			GameObject gameObject = ((boxesToUnlock[i] != 0) ? NGUITools.AddChild(slots[i], superMysteryBoxPrefab) : NGUITools.AddChild(slots[i], mysteryBoxPrefab));
			gameObject.transform.localScale = _boxScale;
			gameObject.transform.localRotation = Quaternion.Euler(_boxRotation);
			Utility.SetLayerRecursively(gameObject.transform, boxParent.layer);
			_boxes[i] = gameObject;
		}
		GlowEffect.GetComponent<MeshRenderer>().material.SetColor("_MainColor", Color.black);
	}

	private IEnumerator BoxIdleAnimCoroutine(Transform boxTrans)
	{
		Vector3 baseLocalPos = boxTrans.parent.localPosition;
		stopBoxIdleAnim = false;
		float t = UnityEngine.Random.Range(0f, 6f);
		Vector3 newLocalPos = baseLocalPos;
		while (!stopBoxIdleAnim)
		{
			t += Time.deltaTime;
			newLocalPos.y = baseLocalPos.y + Mathf.Sin(t * 2f) * 10f;
			boxTrans.parent.localPosition = newLocalPos;
			yield return null;
		}
		bool doneResetting = false;
		while (!doneResetting)
		{
			newLocalPos.y = Mathf.MoveTowards(newLocalPos.y, baseLocalPos.y, Time.deltaTime * 20f);
			if (Mathf.Approximately(newLocalPos.y, baseLocalPos.y))
			{
				doneResetting = true;
			}
			boxTrans.parent.localPosition = newLocalPos;
			yield return null;
		}
	}

	private IEnumerator MoveNextBoxToFront()
	{
		_boxCurrent++;
		if (_boxCurrent >= _numberOfBoxes - 1)
		{
			anotherBox = false;
		}
		GlowEffect.GetComponent<MeshRenderer>().material.SetColor("_MainColor", Color.black);
		_boxes[_boxCurrent].transform.parent = slots[0].transform;
		StartCoroutine(MoveGameObject(_boxes[_boxCurrent].transform, 0.35f, Vector3.zero));
		StartCoroutine(BoxIdleAnimCoroutine(_boxes[_boxCurrent].transform));
		yield return new WaitForSeconds(0.35f);
		openButton.GetComponent<Collider>().enabled = true;
		if (boxesToUnlock[_boxCurrent] == MysteryBox.Type.Normal)
		{
			ShowPrizeButton(MysteryBox.Type.Normal);
			if (_superMysteryBoxEffect != null)
			{
				_superMysteryBoxEffect.StopEffect();
			}
		}
		else
		{
			ShowPrizeButton(MysteryBox.Type.Super);
			if (_superMysteryBoxEffect != null)
			{
				_superMysteryBoxEffect.StartEffect();
			}
		}
	}

	public void TestDown()
	{
		stopBoxIdleAnim = true;
		Animation componentInChildren = _boxes[_boxCurrent].GetComponentInChildren<Animation>();
		componentInChildren.Play("down");
	}

	public void TestUp()
	{
		openButton.GetComponent<Collider>().enabled = false;
		if (!openingBoxNow)
		{
			ShowPrizeButton(false);
			MysteryBoxReward reward = MysteryBox.Roll(boxesToUnlock[_boxCurrent]);
			if (_superMysteryBoxEffect != null)
			{
				_superMysteryBoxEffect.StopEffect();
			}
			StartCoroutine(AnimateAlpha(ContinueLabel, 0.5f, 0f));
			StartCoroutine(_ShowReward(reward, _boxes[_boxCurrent]));
			openingBoxNow = true;
		}
	}

	private void ClosePrizeList()
	{
		ShowPrizeList(false);
	}

	private void PrizeListNormalClicked()
	{
		ShowPrizeList(MysteryBox.Type.Normal);
	}

	private void PrizeListSuperClicked()
	{
		ShowPrizeList(MysteryBox.Type.Super);
	}

	private IEnumerator _ShowReward(MysteryBoxReward reward, GameObject box)
	{
		GameObject rewardGo = NGUITools.AddChild(prefab: ChooseRewardPrefab(reward), parent: slots[0]);
		rewardGo.transform.localScale = _rewardStartScale;
		rewardGo.transform.localRotation = Quaternion.Euler(_rewardStartRotation);
		Utility.SetLayerRecursively(rewardGo.transform, boxParent.layer);
		if (audioStateLoop != null)
		{
			audioStateLoop.PlayMysteryBoxOpenSound();
		}
		Animation animation = box.GetComponentInChildren<Animation>();
		animation.Play("up");
		while (animation["up"].normalizedTime < 0.5f)
		{
			yield return null;
		}
		_maySetTimeScale = true;
		if (reward.type == MysteryBoxRewardType.coins)
		{
			StartCoroutine(ScaleGameObject(rewardGo.transform, 2f, _rewardEndScale_coin));
		}
		else
		{
			StartCoroutine(ScaleGameObject(rewardGo.transform, 2f, _rewardEndScale));
		}
		StartCoroutine(MoveGameObject(_boxes[_boxCurrent].transform, 0.7f, _outOfScreenPosition));
		StartCoroutine(RotateGameObject(rewardGo.transform, 4f, new Vector3(0f, 1500f, 0f)));
		yield return new WaitForSeconds(0.5f);
		StartCoroutine(AnimateColor(GlowEffect.GetComponent<MeshRenderer>().material, 1.5f, Color.white));
		StartCoroutine(RotateGameObject(GlowEffect.transform, 3f, new Vector3(0f, 0f, -270f)));
		yield return new WaitForSeconds(1.3f);
		GameObject labelGo = NGUITools.AddChild(base.gameObject, rewardLabelTemplate);
		labelGo.transform.localPosition = _labelPosition;
		MysteryBoxRewardLabelTemplate template = labelGo.GetComponent<MysteryBoxRewardLabelTemplate>();
		bool showNewTrophyLabel = false;
		if (reward.type == MysteryBoxRewardType.coins)
		{
			template.SetupCoins(reward.amount);
		}
		else if (reward.type == MysteryBoxRewardType.powerup)
		{
			template.SetupPowerup(reward.powerupType, reward.amount);
		}
		else if (reward.type == MysteryBoxRewardType.token)
		{
			template.SetupToken(reward.characterType, reward.amount);
			Missions.Instance.PlayerDidThis(Missions.MissionTarget.Tokens, reward.amount);
		}
		else if (reward.type == MysteryBoxRewardType.trophy)
		{
			showNewTrophyLabel = true;
			template.SetupTrophy(reward.trophyType);
		}
		else if (reward.type == MysteryBoxRewardType.medal)
		{
			showNewTrophyLabel = true;
			template.SetupMedal();
		}
		newTrophyLabel.gameObject.active = showNewTrophyLabel;
		StartCoroutine(AnimateAlpha(template, 0.2f, 1f));
		yield return new WaitForSeconds(0.5f);
		if (reward.type == MysteryBoxRewardType.coins)
		{
			StartCoroutine(CountUpCoins(reward.amount, template));
			yield return new WaitForSeconds(2.5f);
		}
		else
		{
			yield return new WaitForSeconds(1f);
		}
		StartCoroutine(AnimateColor(GlowEffect.GetComponent<MeshRenderer>().material, 0.5f, Color.black));
		if (reward.type == MysteryBoxRewardType.powerup)
		{
			PlayerInfo.Instance.IncreaseUpgradeAmount(reward.powerupType, reward.amount);
		}
		else if (reward.type == MysteryBoxRewardType.token)
		{
			PlayerInfo.Instance.CollectToken(reward.characterType, reward.amount);
		}
		else if (reward.type == MysteryBoxRewardType.trophy)
		{
			PlayerInfo.Instance.UnlockTrophy(reward.trophyType);
		}
		PlayerInfo.Instance.SaveIfDirty();
		Flurry.LogEvent("Mystery Box opened");
		yield return new WaitForSeconds(1f);
		ContinueLabel.text = "Tap to continue";
		StartCoroutine(AnimateAlpha(ContinueLabel, 0.5f, 1f));
		while (!Input.GetMouseButtonUp(0))
		{
			yield return null;
		}
		Time.timeScale = 1f;
		_maySetTimeScale = false;
		UnityEngine.Object.Destroy(rewardGo);
		UnityEngine.Object.Destroy(labelGo);
		UnityEngine.Object.Destroy(box);
		_FinishOpening();
	}

	private void _FinishOpening()
	{
		openingBoxNow = false;
		if (anotherBox)
		{
			StartCoroutine(MoveNextBoxToFront());
			return;
		}
		GameObject[] array = slots;
		foreach (GameObject obj in array)
		{
			UnityEngine.Object.Destroy(obj);
		}
		UIScreenController.Instance.ClosePopup();
		continueButton.GetComponent<Collider>().enabled = true;
		openButton.GetComponent<Collider>().enabled = false;
		if (PlayerInfo.Instance.mysteryBoxesToUnlockCount > 0)
		{
			UIScreenController.Instance.QueueMysteryBox();
		}
	}

	private IEnumerator RotateGameObject(Transform trans, float duration, Vector3 angleToRotate)
	{
		Quaternion fromRotation = trans.localRotation;
		float factor2 = 0f;
		while (factor2 < 1f)
		{
			factor2 += Time.deltaTime / duration;
			factor2 = Mathf.Clamp01(factor2);
			float angle2 = 0f;
			angle2 = Mathf.Lerp(4.712389f, (float)Math.PI * 2f, factor2);
			float cosFactor = Mathf.Cos(angle2) * 0.5f + 0.5f;
			trans.localRotation = fromRotation;
			trans.Rotate(angleToRotate * cosFactor, Space.World);
			yield return null;
		}
	}

	private IEnumerator CountUpCoins(int amount, MysteryBoxRewardLabelTemplate rewardTemplate)
	{
		float countFactor = 0f;
		float countTime = Mathf.Lerp(0.3f, 2f, (float)amount / 100f);
		int coinboxFrom = PlayerInfo.Instance.amountOfCoins;
		int coinboxTo = coinboxFrom + amount;
		int rewardLabelTo = 0;
		yield return new WaitForSeconds(0.5f);
		while (countFactor < 1f)
		{
			countFactor += Time.deltaTime / countTime;
			rewardTemplate.UpdateCoins(Mathf.RoundToInt(Mathf.SmoothStep(amount, rewardLabelTo, countFactor)));
			coinboxLabel.text = string.Empty + Mathf.RoundToInt(Mathf.SmoothStep(coinboxFrom, coinboxTo, countFactor));
			yield return null;
		}
		Missions.Instance.PlayerDidThis(Missions.MissionTarget.EarnCoin, amount);
		PlayerInfo.Instance.amountOfCoins = coinboxTo;
		PlayerInfo.Instance.SaveIfDirty();
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

	private IEnumerator AnimateAlpha(MysteryBoxRewardLabelTemplate template, float duration, float toAlpha)
	{
		float fromAlpha = template.Alpha;
		float factor2 = 0f;
		while (factor2 < 1f)
		{
			factor2 += Time.deltaTime / duration;
			factor2 = Mathf.Clamp01(factor2);
			template.Alpha = Mathf.Lerp(fromAlpha, toAlpha, factor2);
			yield return null;
		}
	}

	private IEnumerator AnimateColor(UIWidget widget, float duration, Color toColor)
	{
		Color fromColor = widget.color;
		float factor2 = 0f;
		while (factor2 < 1f)
		{
			factor2 += Time.deltaTime / duration;
			factor2 = Mathf.Clamp01(factor2);
			widget.color = Color.Lerp(fromColor, toColor, factor2);
			yield return null;
		}
	}

	private IEnumerator AnimateColor(Material material, float duration, Color toColor)
	{
		Color fromColor = material.GetColor("_MainColor");
		float factor2 = 0f;
		while (factor2 < 1f)
		{
			factor2 += Time.deltaTime / duration;
			factor2 = Mathf.Clamp01(factor2);
			material.SetColor("_MainColor", Color.Lerp(fromColor, toColor, factor2));
			yield return null;
		}
	}

	private IEnumerator MoveGameObject(Transform trans, float duration, Vector3 toPos)
	{
		Vector3 fromPos = trans.localPosition;
		float factor2 = 0f;
		while (factor2 < 1f)
		{
			factor2 += Time.deltaTime / duration;
			factor2 = Mathf.Clamp01(factor2);
			trans.localPosition = Vector3.Lerp(fromPos, toPos, 0.5f * (Mathf.Sin((factor2 - 0.5f) * (float)Math.PI) + 1f));
			yield return null;
		}
		trans.localPosition = toPos;
	}

	private IEnumerator ScaleGameObject(Transform trans, float duration, Vector3 toScale)
	{
		float factor2 = 0f;
		Vector3 fromScale = trans.localScale;
		while (factor2 < 1f)
		{
			factor2 += Time.deltaTime / duration;
			factor2 = Mathf.Clamp01(factor2);
			float angle2 = 0f;
			angle2 = Mathf.Lerp((float)Math.PI, (float)Math.PI * 2f, factor2);
			float cosFactor = Mathf.Cos(angle2) * 0.5f + 0.5f;
			trans.localScale = Vector3.Lerp(fromScale, toScale, cosFactor);
			yield return null;
		}
	}

	private GameObject ChooseRewardPrefab(MysteryBoxReward reward)
	{
		GameObject result = rewardCoins;
		switch (reward.type)
		{
		case MysteryBoxRewardType.coins:
			result = rewardCoins;
			break;
		case MysteryBoxRewardType.powerup:
			switch (reward.powerupType)
			{
			case PowerupType.hoverboard:
				result = rewardPowerupHoverboard;
				break;
			case PowerupType.headstart500:
				result = rewardPowerupHeadstart500;
				break;
			case PowerupType.headstart2000:
				result = rewardPowerupHeadstart2000;
				break;
			}
			break;
		case MysteryBoxRewardType.token:
			switch (reward.characterType)
			{
			case Characters.CharacterType.fresh:
				result = rewardTokenFresh;
				break;
			case Characters.CharacterType.tricky:
				result = rewardTokenTricky;
				break;
			case Characters.CharacterType.spike:
				result = rewardTokenSpike;
				break;
			case Characters.CharacterType.yutani:
				result = rewardTokenYutani;
				Missions.Instance.PlayerDidThis(Missions.MissionTarget.HaveYutani);
				break;
			}
			break;
		case MysteryBoxRewardType.trophy:
			switch (reward.trophyType)
			{
			case Trophies.Trophy.diamond:
				result = trophyDiamond;
				Missions.Instance.PlayerDidThis(Missions.MissionTarget.HaveTrophiesFirstAchievements);
				break;
			case Trophies.Trophy.goldbar:
				result = trophyGoldBar;
				Missions.Instance.PlayerDidThis(Missions.MissionTarget.HaveTrophiesFirstAchievements);
				break;
			case Trophies.Trophy.goldChainClock:
				result = trophyChainClock;
				Missions.Instance.PlayerDidThis(Missions.MissionTarget.HaveTrophiesFirstAchievements);
				break;
			case Trophies.Trophy.goldChainDollar:
				result = trophyChainDollar;
				Missions.Instance.PlayerDidThis(Missions.MissionTarget.HaveTrophiesFirstAchievements);
				break;
			case Trophies.Trophy.goldSkull:
				result = trophyGoldSkull;
				Missions.Instance.PlayerDidThis(Missions.MissionTarget.HaveTrophiesFirstAchievements);
				break;
			case Trophies.Trophy.headphones:
				result = trophyHeadphones;
				Missions.Instance.PlayerDidThis(Missions.MissionTarget.HaveTrophiesFirstAchievements);
				break;
			case Trophies.Trophy.lpBlack:
				result = trophyLPBlack;
				Missions.Instance.PlayerDidThis(Missions.MissionTarget.HaveTrophiesFirstAchievements);
				break;
			case Trophies.Trophy.tapeBlack:
				result = trophyTapeBlack;
				Missions.Instance.PlayerDidThis(Missions.MissionTarget.HaveTrophiesFirstAchievements);
				break;
			default:
				Debug.LogError(string.Concat(reward.trophyType, string.Empty));
				break;
			}
			break;
		case MysteryBoxRewardType.medal:
			result = trophyMedalSmart;
			break;
		}
		return result;
	}
}
