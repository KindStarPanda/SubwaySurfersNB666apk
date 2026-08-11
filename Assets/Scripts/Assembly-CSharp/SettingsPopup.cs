using UnityEngine;

public class SettingsPopup : UIScreen
{
	private const int SCALEINCREASE = 62;

	[SerializeField]
	private Transform fill;

	[SerializeField]
	private Transform outline;

	[SerializeField]
	private Transform grid;

	[SerializeField]
	private Transform versionNr;

	[SerializeField]
	private GameObject seasonButton;

	[SerializeField]
	private GameObject gameCenterButton;

	private GameObject seasonButtonInstance;

	private int originalFillScale;

	private int originalOutlineScale;

	private int originalVersionNrHeight;

	public override void Init()
	{
		base.Init();
		originalFillScale = (int)fill.localScale.y;
		originalOutlineScale = (int)outline.localScale.y;
		originalVersionNrHeight = (int)versionNr.localPosition.y;
		Debug.Log("originalFillScale:  " + originalFillScale);
		Debug.Log("originalOutlineScale:  " + originalOutlineScale);
		Debug.Log("originalVersionNrHeight:  " + originalVersionNrHeight);
	}

	public override void Show()
	{
		base.Show();
		if (PlayerInfo.Instance.currentSeasonAvailable == PlayerInfo.Season.xmas)
		{
			if (seasonButtonInstance == null)
			{
				seasonButtonInstance = NGUITools.AddChild(grid.gameObject, seasonButton);
				seasonButtonInstance.name = "3 Season";
			}
			if (gameCenterButton != null)
			{
				fill.localScale = new Vector3(fill.localScale.x, originalFillScale, 1f);
				outline.localScale = new Vector3(outline.localScale.x, originalOutlineScale, 1f);
				versionNr.localPosition = new Vector3(versionNr.localPosition.x, originalVersionNrHeight, -1f);
				NGUITools.Destroy(gameCenterButton);
			}
		}
		else if (gameCenterButton != null)
		{
			fill.localScale = new Vector3(fill.localScale.x, originalFillScale - 62, 1f);
			outline.localScale = new Vector3(outline.localScale.x, originalOutlineScale - 62, 1f);
			versionNr.localPosition = new Vector3(versionNr.localPosition.x, originalVersionNrHeight + 62, -1f);
			NGUITools.Destroy(gameCenterButton);
		}
		grid.GetComponent<UIGrid>().repositionNow = true;
	}
}
