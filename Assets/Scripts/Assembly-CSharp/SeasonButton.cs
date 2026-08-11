using UnityEngine;

public class SeasonButton : MonoBehaviour
{
	[SerializeField]
	private UISprite iconOff;

	[SerializeField]
	private UISprite icon;

	[SerializeField]
	private UILabel desc;

	[SerializeField]
	private UILabel title;

	private PlayerInfo playerInfoInstance;

	private void IconAndLabelUpdate()
	{
		if (playerInfoInstance.currentSeasonPicked == PlayerInfo.Season.xmas)
		{
			icon.spriteName = "icon_xmas";
			title.text = string.Format(Strings.Get(StringID.SEASON_BUTTON_TITLE), Strings.Get(StringID.ON));
			desc.text = Strings.Get(StringID.SEASON_BUTTON_DESCRIPTION);
			iconOff.GetComponent<UISprite>().enabled = false;
		}
		else if (playerInfoInstance.currentSeasonAvailable == PlayerInfo.Season.xmas)
		{
			icon.spriteName = "icon_xmas";
			title.text = string.Format(Strings.Get(StringID.SEASON_BUTTON_TITLE), Strings.Get(StringID.OFF));
			desc.text = Strings.Get(StringID.SEASON_BUTTON_DESCRIPTION);
			iconOff.GetComponent<UISprite>().enabled = true;
		}
		icon.MakePixelPerfect();
	}

	private void OnEnable()
	{
		playerInfoInstance = PlayerInfo.Instance;
		IconAndLabelUpdate();
	}

	private void Click()
	{
		if (playerInfoInstance.currentSeasonPicked == PlayerInfo.Season.none)
		{
			playerInfoInstance.currentSeasonPicked = playerInfoInstance.currentSeasonAvailable;
			Settings.optionSeason = true;
		}
		else
		{
			playerInfoInstance.currentSeasonPicked = PlayerInfo.Season.none;
			Settings.optionSeason = false;
		}
		IconAndLabelUpdate();
		if (playerInfoInstance.currentSeasonPicked == PlayerInfo.Season.none && playerInfoInstance.currentSeasonAvailable == PlayerInfo.Season.none)
		{
			Object.Destroy(base.gameObject);
			UIScreenController.Instance.ClosePopup(base.gameObject);
			UIScreenController.Instance.QueuePopup("SettingsPopup");
		}
		PlayerInfo.Instance.SaveIfDirty();
	}
}
