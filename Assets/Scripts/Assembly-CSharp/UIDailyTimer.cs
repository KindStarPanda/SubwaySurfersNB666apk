using System;
using UnityEngine;

public class UIDailyTimer : MonoBehaviour
{
	[SerializeField]
	private UILabel _timerLabel;

	[SerializeField]
	private string prefix = string.Empty;

	private bool doARefresh;

	private TimeSpan timeLeft;

	private int _totalSeconds = -1;

	private void Update()
	{
		timeLeft = PlayerInfo.Instance.dailyWordExpireTime - DateTime.UtcNow;
		if (_totalSeconds == (int)timeLeft.TotalSeconds)
		{
			return;
		}
		_totalSeconds = (int)timeLeft.TotalSeconds;
		if (timeLeft.Ticks > 0)
		{
			_timerLabel.text = string.Format("{0} {1:00}:{2:00}:{3:00}", prefix, timeLeft.Hours, timeLeft.Minutes, timeLeft.Seconds);
			return;
		}
		_timerLabel.text = string.Format("{0} {1:00}:{2:00}:{3:00}", prefix, 0, 0, 0);
		if (doARefresh)
		{
			Debug.Log("refresh timer bug fix");
			SendMessageUpwards("RefreshDailyChallenge", SendMessageOptions.DontRequireReceiver);
			doARefresh = false;
		}
	}

	private void OnApplicationPause(bool paused)
	{
		if (!paused)
		{
			doARefresh = true;
		}
	}
}
