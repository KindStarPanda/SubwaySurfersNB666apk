using UnityEngine;

public class DailyChallengeScreen : UIScreen
{
	private bool _hasInited;

	[SerializeField]
	private DayHelper[] dayHelpers = new DayHelper[0];

	[SerializeField]
	private GameObject[] rewardIcons = new GameObject[0];

	[SerializeField]
	private UILabel coinRewardLabel;

	[SerializeField]
	private UISprite coinRewardSprite;

	[SerializeField]
	private UIWidget[] challengeActiveWidgets = new UIWidget[0];

	[SerializeField]
	private UIWidget[] challengeCompletedWidgets = new UIWidget[0];

	public override void Init()
	{
		base.Init();
		_hasInited = true;
		for (int i = 0; i < dayHelpers.Length; i++)
		{
			dayHelpers[i].Init(i);
		}
	}

	public override void Show()
	{
		base.Show();
		RefreshDailyChallengeScreen();
	}

	private void RefreshDailyChallengeScreen()
	{
		for (int i = 0; i < dayHelpers.Length; i++)
		{
			dayHelpers[i].RefreshDay();
		}
		bool mostRecentIsToday;
		int num = Mathf.Clamp(PlayerInfo.Instance.GetDailyWordDaysInARow(out mostRecentIsToday), 0, 5);
		int num2 = 0;
		num2 = num;
		if (num2 > 4)
		{
			num2 = 4;
		}
		for (int j = 0; j < rewardIcons.Length; j++)
		{
			UISprite[] componentsInChildren = rewardIcons[j].GetComponentsInChildren<UISprite>();
			foreach (UISprite uISprite in componentsInChildren)
			{
				uISprite.enabled = j == num2;
			}
		}
		if (num2 >= 4)
		{
			UISprite[] componentsInChildren2 = rewardIcons[rewardIcons.Length - 1].GetComponentsInChildren<UISprite>();
			foreach (UISprite uISprite2 in componentsInChildren2)
			{
				uISprite2.enabled = true;
			}
			coinRewardLabel.enabled = false;
			coinRewardSprite.enabled = false;
		}
		else
		{
			coinRewardLabel.enabled = true;
			coinRewardSprite.enabled = true;
			coinRewardLabel.text = DailyWord.GetRewardForDay(num2).coins.ToString();
		}
		for (int m = 0; m < challengeActiveWidgets.Length; m++)
		{
			challengeActiveWidgets[m].enabled = !mostRecentIsToday;
		}
		for (int n = 0; n < challengeCompletedWidgets.Length; n++)
		{
			challengeCompletedWidgets[n].enabled = mostRecentIsToday;
		}
	}
}
