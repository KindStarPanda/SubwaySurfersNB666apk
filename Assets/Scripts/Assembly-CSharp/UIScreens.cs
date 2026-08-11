public class UIScreens
{
	public const string BRAG_POPUP = "BragPopup";

	public const string CHARACTER_SCREEN = "CharacterScreen";

	public const string COINS_SCREEN = "CoinsUI_shop";

	public const string COINS_POPUP = "CoinsUI_quick";

	public const string CHANGE_LOG_END_GAME_POPUP = "ChangeLogEndGamePopup";

	public const string CHANGE_LOG_POPUP = "ChangeLogPopup";

	public const string CHANGELOG_THEME_POPUP = "ChangelogThemePopup";

	public const string DAILY_CHALLENGE_POPUP = "DailyChallengePopup";

	public const string FACEBOOK_PAYOUT_POPUP = "FacebookPayoutPopup";

	public const string FRIENDS_SCREEN = "FriendsUI";

	public const string FRIENDS_SCREEN_OFFLINE = "FriendsUI_offline";

	public const string FRIENDS_SCREEN_ONLINE = "FriendsUI_online";

	public const string FRONT_SCREEN = "FrontUI";

	public const string GAMECENTER_PAYOUT_POPUP = "GameCenterPayoutPopup";

	public const string GAMEOVER_SCREEN = "GameoverUI";

	public const string HOVERBOARD_POPUP = "HoverboardPopup";

	public const string PRIVACY_POLICY_POPUP = "PrivacyPolicyPopup";

	public const string LEADERBOARD_SCREEN = "LeaderboardUI";

	public const string LEADERBOARD_SCREEN_OFFLINE = "LeaderboardUI_offline";

	public const string LEADERBOARD_SCREEN_ONLINE = "LeaderboardUI_online";

	public const string MISSION_POPUP = "Mission_popup";

	public const string MYSTERYBOX_POPUP = "MysteryBoxPopup";

	public const string PAUSE_SCREEN = "PauseUI";

	public const string PRE_BRAG_POPUP = "PreBragPopup";

	public const string PRIZE_MB_POPUP = "PrizeMBPopup";

	public const string PRIZE_SMB_POPUP = "PrizeSMBPopup";

	public const string SETTINGS_POPUP = "SettingsPopup";

	public const string INGAME_SCREEN = "IngameUI";

	public const string TROPHY_SCREEN = "TrophiesScreen";

	public const string TUTORIAL_COLLECT_FROM_FRIENDS = "TutorialCollectFromFriendsPopup";

	public const string TUTORIAL_DOUBLE_COINS = "TutorialDoubleCoinsPopup";

	public const string TUTORIAL_END_GAME_MISSIONS = "TutorialEndGameMissionsPopup";

	public const string TUTORIAL_FACEBOOK_POPUP = "TutorialFacebookPopup";

	public const string TUTORIAL_HOVERBOARDS = "TutorialHoverboardsPopup";

	public const string TUTORIAL_MISSION = "TutorialMissionPopup";

	public const string TUTORIAL_TOKEN = "TutorialTokenPopup";

	public const string UPDATE_APP_POPUP = "UpdateAppPopup";

	public const string UPGRADE_SCREEN = "UpgradesUI_shop";

	public const string UPGRADE_POPUP = "UpgradesUI_quick";

	public const string NOTEBOOK_BACKGROUND = "NotebookPanel2";

	public const string BOARD_SCREEN = "BoardScreen";

	public const string EARN_COINS_SCREEN = "EarnCoinsScreen";

	public static string friendsMenu_lastScreen = "FriendsUI";

	public static string meMenu_lastScreen = "CharacterScreen";

	public static string shopMenu_lastScreen = "UpgradesUI_shop";

	public static string GetFlurryPopUpWithOkAndCloseName(string goName)
	{
		string text = string.Empty;
		if (goName.Contains("ChangeLogEndGamePopup"))
		{
			text = "ChangeLogEndGamePopup";
		}
		else if (goName.Contains("ChangeLogPopup"))
		{
			text = "ChangeLogPopup";
		}
		else if (goName.Contains("ChangelogThemePopup"))
		{
			text = "SeasonPopup";
		}
		else if (goName.Contains("TutorialCollectFromFriendsPopup"))
		{
			text = "GuidelineCollectPopup";
		}
		else if (goName.Contains("TutorialEndGameMissionsPopup"))
		{
			text = "GuidelineEndGameMissionPopup";
		}
		else if (goName.Contains("TutorialFacebookPopup"))
		{
			text = "GuidelineFacebook";
		}
		else if (goName.Contains("TutorialMissionPopup"))
		{
			text = "GuidelineMissionPopup";
		}
		else if (goName.Contains("PrivacyPolicyPopup"))
		{
			text = "PrivacyPolicyPopup";
		}
		else if (goName.Contains("UpdateAppPopup"))
		{
			text = "Update App";
		}
		if (text != string.Empty)
		{
			text = "POPUP Screen " + text;
		}
		return text;
	}

	public static string GetFlurryScreenName(string goName)
	{
		string text = string.Empty;
		if (goName.Contains("BragPopup"))
		{
			text = "BragPopup";
		}
		else if (goName.Contains("CoinsUI_quick"))
		{
			text = "CoinsUI_quick";
		}
		else if (goName.Contains("CharacterScreen"))
		{
			text = "CharacterScreen";
		}
		else if (goName.Contains("FacebookPayoutPopup"))
		{
			text = "FacebookPayoutPopup";
		}
		else if (goName.Contains("DailyChallengePopup"))
		{
			text = "DailyChallengePopup";
		}
		else if (goName.Contains("GameCenterPayoutPopup"))
		{
			text = "GameCenterPayoutPopup";
		}
		else if (goName.Contains("PreBragPopup"))
		{
			text = "PreBragPopup";
		}
		else if (goName.Contains("HoverboardPopup"))
		{
			text = "HoverboardPopup";
		}
		else if (goName.Contains("Mission_popup"))
		{
			text = "Mission_popup";
		}
		else if (goName.Contains("MysteryBoxPopup"))
		{
			text = "MysteryBoxPopup";
		}
		else if (goName.Contains("UpgradesUI_quick"))
		{
			text = "UpgradesUI_quick";
		}
		else if (goName.Contains("TutorialDoubleCoinsPopup"))
		{
			text = "TutorialDoubleCoinsPopup";
		}
		else if (goName.Contains("TutorialTokenPopup"))
		{
			text = "TutorialTokenPopup";
		}
		else if (goName.Contains("PrizeMBPopup"))
		{
			text = "PrizeMBPopup";
		}
		else if (goName.Contains("PrizeSMBPopup"))
		{
			text = "PrizeSMBPopup";
		}
		else if (goName.Contains("SettingsPopup"))
		{
			text = "SettingsPopup";
		}
		else if (goName.Contains("CoinsUI_shop"))
		{
			text = "CoinsUI_shop";
		}
		else if (goName.Contains("FriendsUI"))
		{
			text = "FriendsUI";
		}
		else if (goName.Contains("FriendsUI_offline"))
		{
			text = "FriendsUI_offline";
		}
		else if (goName.Contains("FriendsUI_online"))
		{
			text = "FriendsUI_online";
		}
		else if (goName.Contains("FrontUI"))
		{
			text = "FrontUI";
		}
		else if (goName.Contains("GameoverUI"))
		{
			text = "GameoverUI";
		}
		else if (goName.Contains("LeaderboardUI"))
		{
			text = "LeaderboardUI";
		}
		else if (goName.Contains("LeaderboardUI_offline"))
		{
			text = "LeaderboardUI_offline";
		}
		else if (goName.Contains("LeaderboardUI_online"))
		{
			text = "LeaderboardUI_online";
		}
		else if (goName.Contains("PauseUI"))
		{
			text = "PauseUI";
		}
		else if (goName.Contains("IngameUI"))
		{
			text = "IngameUI";
		}
		else if (goName.Contains("TrophiesScreen"))
		{
			text = "TrophiesScreen";
		}
		else if (goName.Contains("UpgradesUI_shop"))
		{
			text = "UpgradesUI_shop";
		}
		else if (goName.Contains("BoardScreen"))
		{
			text = "BoardScreen";
		}
		else if (goName.Contains("EarnCoinsScreen"))
		{
			text = "EarnCoinsScreen";
		}
		if (text != string.Empty)
		{
			text = "UI Screen " + text;
		}
		return text;
	}
}
