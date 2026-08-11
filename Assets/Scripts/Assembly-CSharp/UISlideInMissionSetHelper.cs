using UnityEngine;

public class UISlideInMissionSetHelper : UISlideIn
{
	[SerializeField]
	private UILabel line1;

	[SerializeField]
	private UILabel lineReward;

	[SerializeField]
	private UILabel lineRewardShadow;

	[SerializeField]
	private UISprite superMysteryBox;

	[SerializeField]
	private Transform glow;

	[SerializeField]
	private Transform background;

	[SerializeField]
	private Transform backgroundDark;

	private CoinLabelSizer coinLabelSizer;

	private void Awake()
	{
		coinLabelSizer = GetComponent<CoinLabelSizer>();
	}

	public void SetupSlideInMissionSet(int multiplier)
	{
		base.gameObject.SetActiveRecursively(true);
		if (PlayerInfo.Instance.missionCompletedSum > Missions.Instance.missionSetStoryCount + 1)
		{
			superMysteryBox.enabled = true;
			superMysteryBox.transform.localPosition = new Vector3(-61f, -31f, 0f);
			lineReward.enabled = false;
			lineRewardShadow.enabled = false;
		}
		else
		{
			lineReward.enabled = true;
			lineRewardShadow.enabled = true;
			lineReward.text = "x" + multiplier;
			superMysteryBox.enabled = false;
		}
		line1.text = "Mission Set\ncomplete";
		line1.transform.localPosition = new Vector3(-16f, -30f, -1f);
		coinLabelSizer.DisableCoinLabel();
		glow.localPosition = new Vector3(-64f, -30f, 0f);
		background.localScale = new Vector3(200f, background.transform.localScale.y, 1f);
		backgroundDark.localPosition = new Vector3(-100f, 0f, 0f);
		SlideIn();
	}

	public void SetupCoin()
	{
		base.gameObject.SetActiveRecursively(true);
		superMysteryBox.enabled = false;
		lineReward.enabled = false;
		lineRewardShadow.enabled = false;
		line1.text = "Daily Challenge \ncomplete";
		line1.transform.localPosition = new Vector3(-32f, -30f, -1f);
		glow.localPosition = new Vector3(-84f, -30f, 0f);
		background.localScale = new Vector3(240f, background.transform.localScale.y, 1f);
		backgroundDark.localPosition = new Vector3(-120f, 0f, 0f);
		SlideIn();
	}

	public void SetupMysteryBox()
	{
		base.gameObject.SetActiveRecursively(true);
		lineReward.enabled = false;
		lineRewardShadow.enabled = false;
		coinLabelSizer.DisableCoinLabel();
		superMysteryBox.enabled = true;
		superMysteryBox.transform.localPosition = new Vector3(-71f, -31f, 0f);
		line1.text = "Daily Challenge \ncomplete";
		line1.transform.localPosition = new Vector3(-31f, -30f, -1f);
		glow.localPosition = new Vector3(-74f, -30f, 0f);
		background.localScale = new Vector3(220f, background.transform.localScale.y, 1f);
		backgroundDark.localPosition = new Vector3(-110f, 0f, 0f);
		SlideIn();
	}
}
