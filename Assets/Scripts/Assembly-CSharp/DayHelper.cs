using UnityEngine;

public class DayHelper : MonoBehaviour
{
	public enum DayType
	{
		_notset = 0,
		regular = 1,
		lastDay = 2
	}

	public DayType dayType;

	private Color32 InactiveDayColor = new Color32(9, 42, 83, byte.MaxValue);

	private Color32 ActiveDayColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	private Color32 InactiveDayShadowColor = new Color32(0, 0, 0, 0);

	private Color32 ActiveDayShadowColor = new Color32(26, 116, 30, byte.MaxValue);

	private Color32 InactiveRewardColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	private Color32 ActiveRewardColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	private Color32 InactiveRewardShadowColor = new Color32(0, 36, 80, byte.MaxValue);

	private Color32 ActiveRewardShadowColor = new Color32(26, 116, 30, byte.MaxValue);

	private Color32 InactiveLastDayDayColor = new Color32(125, 27, 5, byte.MaxValue);

	private Color32 ActiveLastDayDayColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	private Color32 InactiveLastDayDayShadowColor = new Color32(0, 0, 0, 0);

	private Color32 ActiveLastDayDayShadowColor = new Color32(26, 116, 30, byte.MaxValue);

	private Color32 InactiveLastDayRewardShadowColor = new Color32(252, 233, 171, byte.MaxValue);

	private Color32 ActiveLastDayRewardShadowColor = new Color32(26, 116, 30, byte.MaxValue);

	private Color32 ActiveLastDayBackgroundColor = new Color32(157, 234, 5, byte.MaxValue);

	private Color32 InactiveLastDayBackgroundColor = new Color32(248, 225, 62, byte.MaxValue);

	private Color32 ActiveLastDayGradientColor = new Color32(102, 178, 0, byte.MaxValue);

	private Color32 InactiveLastDayGradientColor = new Color32(245, 169, 40, byte.MaxValue);

	private Color32 ActiveLastDayGlow = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 180);

	private Color32 InactiveLastDayGlow = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	[SerializeField]
	private UILabel dayLabel;

	[SerializeField]
	private UILabel rewardLabel;

	[SerializeField]
	private UILabel checkBackLabel;

	[SerializeField]
	private UISprite checkMark;

	[SerializeField]
	private UISprite divider;

	[SerializeField]
	private UISlicedSprite backgroundInactive;

	[SerializeField]
	private UISlicedSprite backgroundActive;

	[SerializeField]
	private UISlicedSprite checkbackBackground;

	[SerializeField]
	private UISlicedSprite[] gradientEffectSprites;

	[SerializeField]
	private UISprite glowEffect;

	[SerializeField]
	private bool ForceActive;

	[SerializeField]
	private bool UnlockColors;

	private int _dayIndex = -1;

	public void Init(int dayIndex)
	{
		_dayIndex = dayIndex;
		DailyWord.DailyWordReward rewardForDay = DailyWord.GetRewardForDay(_dayIndex);
		if (rewardForDay.type == DailyWord.DailyWordRewardType.Coins)
		{
			rewardLabel.text = rewardForDay.coins.ToString();
		}
		ForceActive = false;
		UnlockColors = false;
	}

	public void RefreshDay()
	{
		if (UnlockColors)
		{
			return;
		}
		bool mostRecentIsToday;
		int num = Mathf.Clamp(PlayerInfo.Instance.GetDailyWordDaysInARow(out mostRecentIsToday), 0, 5);
		int num2 = 0;
		num2 = ((!mostRecentIsToday) ? num : (num - 1));
		if (dayType == DayType.lastDay)
		{
			if (num2 >= 4 || ForceActive)
			{
				backgroundInactive.color = ActiveLastDayBackgroundColor;
				dayLabel.text = "Today!";
				dayLabel.color = ActiveLastDayDayColor;
				dayLabel.effectColor = ActiveLastDayDayShadowColor;
				rewardLabel.color = ActiveLastDayDayColor;
				rewardLabel.effectColor = ActiveLastDayRewardShadowColor;
				if (PlayerInfo.Instance.isDailyWordComplete())
				{
					checkMark.enabled = true;
					checkbackBackground.enabled = true;
					rewardLabel.enabled = false;
					checkBackLabel.enabled = true;
				}
				else
				{
					checkMark.enabled = false;
					checkbackBackground.enabled = false;
					rewardLabel.enabled = true;
					checkBackLabel.enabled = false;
				}
				for (int i = 0; i < gradientEffectSprites.Length; i++)
				{
					gradientEffectSprites[i].color = ActiveLastDayGradientColor;
				}
				glowEffect.color = ActiveLastDayGlow;
			}
			else
			{
				backgroundInactive.color = InactiveLastDayBackgroundColor;
				dayLabel.text = "Day 5+";
				dayLabel.color = InactiveLastDayDayColor;
				dayLabel.effectColor = InactiveLastDayDayShadowColor;
				rewardLabel.effectColor = InactiveLastDayRewardShadowColor;
				checkMark.enabled = false;
				checkbackBackground.enabled = false;
				rewardLabel.enabled = true;
				checkBackLabel.enabled = false;
				for (int j = 0; j < gradientEffectSprites.Length; j++)
				{
					gradientEffectSprites[j].color = InactiveLastDayGradientColor;
				}
				glowEffect.color = InactiveLastDayGlow;
			}
		}
		else if (dayType == DayType.regular)
		{
			if (num2 == _dayIndex)
			{
				divider.enabled = true;
				backgroundActive.enabled = true;
				backgroundInactive.enabled = false;
				dayLabel.text = "Today";
				dayLabel.color = ActiveDayColor;
				dayLabel.effectColor = ActiveDayShadowColor;
				rewardLabel.color = ActiveRewardColor;
				rewardLabel.effectColor = ActiveRewardShadowColor;
				checkMark.enabled = PlayerInfo.Instance.isDailyWordComplete();
			}
			else if (num2 < _dayIndex)
			{
				divider.enabled = false;
				backgroundActive.enabled = false;
				backgroundInactive.enabled = true;
				dayLabel.text = "Day " + (_dayIndex + 1);
				dayLabel.color = InactiveDayColor;
				dayLabel.effectColor = InactiveDayShadowColor;
				rewardLabel.color = InactiveRewardColor;
				rewardLabel.effectColor = InactiveRewardShadowColor;
				checkMark.enabled = false;
			}
			else
			{
				divider.enabled = false;
				backgroundActive.enabled = false;
				backgroundInactive.enabled = true;
				dayLabel.text = "Day " + (_dayIndex + 1);
				dayLabel.color = InactiveDayColor;
				dayLabel.effectColor = InactiveDayShadowColor;
				rewardLabel.color = InactiveRewardColor;
				rewardLabel.effectColor = InactiveRewardShadowColor;
				checkMark.enabled = true;
			}
		}
		else
		{
			Debug.LogError("Day was not set properly.", base.gameObject);
		}
	}
}
