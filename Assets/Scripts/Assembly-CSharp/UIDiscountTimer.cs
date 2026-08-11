using System;
using UnityEngine;

public class UIDiscountTimer : MonoBehaviour
{
	private UILabel _timerLabel;

	private double endTimeDouble;

	private DateTime endTime;

	private TimeSpan timeLeft;

	private void Awake()
	{
		_timerLabel = GetComponent<UILabel>();
	}

	private void OnEnable()
	{
		endTimeDouble = OnlineSettings.instance.GetValue("discount_end_time", 0);
		endTime = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(endTimeDouble);
	}

	private void Update()
	{
		timeLeft = endTime - DateTime.UtcNow;
		if (0 < timeLeft.Ticks)
		{
			_timerLabel.text = string.Format("{0:0} day{1} {2:00}:{3:00}:{4:00}", timeLeft.Days, (timeLeft.Days != 1) ? "s" : string.Empty, timeLeft.Hours, timeLeft.Minutes, timeLeft.Seconds);
		}
		else
		{
			SendMessageUpwards("ForceEndDiscount", SendMessageOptions.DontRequireReceiver);
		}
	}
}
