using System;
using UnityEngine;

public class IngameScreen : UIScreen
{
	public UILabel scoreLabel;

	public UILabel multiplierLabel;

	public UILabel coinLabel;

	public UISlicedSprite scoreBG;

	private Transform _cachedScoreBGTransform;

	public UISlicedSprite multiplierBG;

	public UISlicedSprite coinBG;

	private Transform _cachedCoinBGTransform;

	public UIHeadStartHelper headstartHelper;

	public UILabel countdownStartingLabel;

	public UILabel countdownLabel;

	private float _countdownSeconds;

	private bool _countingDown;

	private Vector3 _cachedCountdownLabelScale = Vector3.zero;

	private int score = -1;

	private float mTimeStart;

	private float mTimeDelta;

	private float mActual;

	private bool mTimeStarted;

	public override void Init()
	{
		base.Init();
		PlayerInfo instance = PlayerInfo.Instance;
		instance.onScoreMultiplierChanged = (Action)Delegate.Combine(instance.onScoreMultiplierChanged, new Action(readMultiplier));
		GameStats instance2 = GameStats.Instance;
		instance2.OnCoinsChanged = (Action)Delegate.Combine(instance2.OnCoinsChanged, new Action(OnCoinsChanged));
		Game instance3 = Game.Instance;
		instance3.OnGameStarted = (Action)Delegate.Combine(instance3.OnGameStarted, new Action(OnGameStarted));
		readMultiplier();
		scoreLabel.text = string.Empty + GameStats.Instance.score;
		_cachedScoreBGTransform = scoreBG.cachedTransform;
		_cachedCoinBGTransform = coinBG.cachedTransform;
		countdownStartingLabel.text = string.Empty;
		countdownLabel.text = string.Empty;
	}

	public void OnDestroy()
	{
		PlayerInfo instance = PlayerInfo.Instance;
		instance.onScoreMultiplierChanged = (Action)Delegate.Remove(instance.onScoreMultiplierChanged, new Action(readMultiplier));
		GameStats instance2 = GameStats.Instance;
		instance2.OnCoinsChanged = (Action)Delegate.Remove(instance2.OnCoinsChanged, new Action(OnCoinsChanged));
		Game instance3 = Game.Instance;
		instance3.OnGameStarted = (Action)Delegate.Remove(instance3.OnGameStarted, new Action(OnGameStarted));
	}

	public override void Show()
	{
		base.Show();
		if (Game.Instance == null)
		{
			Debug.LogError("You must be running the wrong scene");
			return;
		}
		if (Game.Instance.isPaused)
		{
			_countdownSeconds = 3f;
			_countingDown = true;
		}
		if (!_countingDown)
		{
			Missions.Instance.inRun = true;
		}
	}

	public override void Hide()
	{
		base.Hide();
		readMultiplier();
		_countingDown = false;
		countdownStartingLabel.text = string.Empty;
		countdownLabel.text = string.Empty;
	}

	private void readMultiplier()
	{
		multiplierLabel.text = "x" + PlayerInfo.Instance.scoreMultiplier;
	}

	private void Update()
	{
		if (Game.Instance.isReadyForHeadStart && !Game.Instance.track.IsRunningOnTutorialTrack)
		{
			Game.Instance.isReadyForHeadStart = false;
			headstartHelper.ShowHeadStart();
		}
		GameStats.Instance.CalculateScore();
		if (score != GameStats.Instance.score)
		{
			SetScoreLabel();
		}
		if (!_countingDown)
		{
			return;
		}
		float num = UpdateRealTimeDelta();
		num *= 1.75f;
		_countdownSeconds -= num;
		countdownStartingLabel.text = "Starting in";
		countdownLabel.text = Mathf.CeilToInt(_countdownSeconds).ToString();
		if (!countdownLabel.enabled)
		{
			countdownStartingLabel.enabled = true;
			countdownLabel.enabled = true;
		}
		if (_cachedCountdownLabelScale == Vector3.zero)
		{
			_cachedCountdownLabelScale = countdownLabel.cachedTransform.localScale;
		}
		countdownLabel.cachedTransform.localScale = _cachedCountdownLabelScale * ((1f - _countdownSeconds % 1f) * 0.5f + 1f);
		if (_countdownSeconds < 0f)
		{
			_countingDown = false;
			countdownStartingLabel.text = string.Empty;
			countdownLabel.text = string.Empty;
			countdownStartingLabel.enabled = false;
			countdownLabel.enabled = false;
			if (Game.Instance != null)
			{
				Game.Instance.TriggerPause(false);
			}
		}
	}

	private void OnCoinsChanged()
	{
		coinLabel.text = string.Empty + GameStats.Instance.coins;
		ResizeCoinBox();
	}

	private void OnGameStarted()
	{
		if (!Game.Instance.isReadyForHeadStart)
		{
			headstartHelper.HideHeadStart(true);
		}
	}

	private void SetScoreLabel()
	{
		score = GameStats.Instance.score;
		string text;
		switch (Utility.NumberOfDigits(score))
		{
		case 1:
			text = "00000";
			break;
		case 2:
			text = "0000";
			break;
		case 3:
			text = "000";
			break;
		case 4:
			text = "00";
			break;
		case 5:
			text = "0";
			break;
		default:
			text = string.Empty;
			break;
		}
		scoreLabel.text = text + score;
		ResizeScoreBox();
	}

	private void ResizeScoreBox()
	{
		int length = scoreLabel.text.Length;
		float num = 99f;
		if (length > 6)
		{
			num += (float)(11 * (length - 6));
			multiplierBG.cachedTransform.parent.localPosition = new Vector3(-122 - 11 * (length - 6), -5f, 0f);
			scoreLabel.cachedTransform.localPosition = new Vector3(-79 - 11 * (length - 6), -4f, -1f);
		}
		if (_cachedScoreBGTransform.localScale.x != num)
		{
			_cachedScoreBGTransform.localScale = new Vector3(num, _cachedScoreBGTransform.localScale.y, _cachedScoreBGTransform.localScale.z);
		}
	}

	private void ResizeCoinBox()
	{
		int length = coinLabel.text.Length;
		float num = 64f;
		if (length > 1)
		{
			num += (float)(13 * (length - 1));
		}
		if (_cachedCoinBGTransform.localScale.x != num)
		{
			_cachedCoinBGTransform.localScale = new Vector3(num, _cachedCoinBGTransform.localScale.y, _cachedCoinBGTransform.localScale.z);
		}
	}

	private void ResizeMultiplierBox()
	{
		int length = multiplierLabel.text.Length;
		float num = 50f;
		if (length > 2)
		{
			num += (float)(10 * (length - 2));
		}
		if (multiplierBG.transform.localScale.x != num)
		{
			multiplierBG.transform.localScale = new Vector3(num, multiplierBG.transform.localScale.y, multiplierBG.transform.localScale.z);
		}
	}

	private void OnEnable()
	{
		mTimeStarted = true;
		mTimeDelta = 0f;
		mTimeStart = Time.realtimeSinceStartup;
	}

	private float UpdateRealTimeDelta()
	{
		if (mTimeStarted)
		{
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			float b = realtimeSinceStartup - mTimeStart;
			mActual += Mathf.Max(0f, b);
			mTimeDelta = 0.001f * Mathf.Round(mActual * 1000f);
			mActual -= mTimeDelta;
			mTimeStart = realtimeSinceStartup;
		}
		else
		{
			mTimeStarted = true;
			mTimeStart = Time.realtimeSinceStartup;
			mTimeDelta = 0f;
		}
		return mTimeDelta;
	}
}
