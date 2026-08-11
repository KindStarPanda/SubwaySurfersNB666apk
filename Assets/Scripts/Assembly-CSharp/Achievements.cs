public class Achievements
{
	public const int NUMBEROFACHIEVEMENTS = 41;

	private string[] _achievementIds = new string[41]
	{
		"Banker", "MagneticCenter", "Score_400000_points.", "OverwhelmingPower", "Simon_says_jump", "ABC", "Happy_Birthday", "Boxing_day", "Welcome_back", "One_legged",
		"Batter_Up", "Tonyhawk", "ItsABird", "Attractive", "pussinboots", "DoubleorNothing", "AllMissionsCompleted", "Elite", "FullThrottle", "TrickyTreasure",
		"MeepMeep", "UnlockKing", "GothGirl", "UnlockNinja", "UnlockFrank", "FoxyCleopatra", "BestFriends", "AllTrophiesCollected", "HaveChar11", "HaveChar12",
		"HaveChar13", "HaveChar14", "HaveChar15", "FacebookLoggedIn", "LandOnTrain", "ActivePowerUps", "MissionSetSingleRun", "ScoreWithoutPowerups", "HoverBoardMaster", "MysteryBoxGrandPrize",
		"HaveYutani"
	};

	private Mission[] _achievementArray = new Mission[41]
	{
		new Mission(Missions.MissionType.EarnCoin, 50000),
		new Mission(Missions.MissionType.CoinsWithMagnet, 2000),
		new Mission(Missions.MissionType.ScoreSingleRun, 400000),
		new Mission(Missions.MissionType.Powerups, 100),
		new Mission(Missions.MissionType.GuardJump, 100),
		new Mission(Missions.MissionType.Letters, 26),
		new Mission(Missions.MissionType.MysteryBoxes, 25),
		new Mission(Missions.MissionType.BuyMysterybox, 10),
		new Mission(Missions.MissionType.DailyQuests, 10),
		new Mission(Missions.MissionType.BumpBarrier, 50),
		new Mission(Missions.MissionType.HoverBoard, 40),
		new Mission(Missions.MissionType.HoverBoardExpire, 20),
		new Mission(Missions.MissionType.JetpackSingleRun, 5),
		new Mission(Missions.MissionType.MagnetsSingleRun, 6),
		new Mission(Missions.MissionType.SuperSneakersSingleRun, 6),
		new Mission(Missions.MissionType.DoubleMultiplierSingleRun, 6),
		new Mission(Missions.MissionType.ReachMissionSet, 29),
		new Mission(Missions.MissionType.HaveUpgrades, 20),
		new Mission(Missions.MissionType.HaveHeadStartLarge, 10),
		new Mission(Missions.MissionType.NoCoinsBeforeScore, 30000),
		new Mission(Missions.MissionType.Headstart, 15),
		new Mission(Missions.MissionType.HaveKing, 1),
		new Mission(Missions.MissionType.HaveLucy, 1),
		new Mission(Missions.MissionType.HaveNinja, 1),
		new Mission(Missions.MissionType.HaveFrank, 1),
		new Mission(Missions.MissionType.HaveFrizzy, 1),
		new Mission(Missions.MissionType.PokeFriend, 10),
		new Mission(Missions.MissionType.HaveTrophiesFirstAchievements, 8),
		new Mission(Missions.MissionType.HaveTag, 1),
		new Mission(Missions.MissionType.HaveTasha, 1),
		new Mission(Missions.MissionType.HaveZoe, 1),
		new Mission(Missions.MissionType.HaveBrody, 1),
		new Mission(Missions.MissionType.HavePrinceK, 1),
		new Mission(Missions.MissionType.FacebookLoggedIn, 1),
		new Mission(Missions.MissionType.LandOnTrainInRow, 15),
		new Mission(Missions.MissionType.ActivePowerups, 3),
		new Mission(Missions.MissionType.MissionSetSingleRun, 2),
		new Mission(Missions.MissionType.NoPowerUpsBeforeScore, 200000),
		new Mission(Missions.MissionType.HoverBoardExpireSingleRun, 6),
		new Mission(Missions.MissionType.MysteryBoxGrandPrize, 1),
		new Mission(Missions.MissionType.HaveYutani, Characters.characterData[Characters.CharacterType.yutani].Price)
	};

	private static Achievements _instance;

	public string[] achievementIds
	{
		get
		{
			return _achievementIds;
		}
	}

	public Mission[] achievementArray
	{
		get
		{
			return _achievementArray;
		}
	}

	public static Achievements Instance
	{
		get
		{
			return _instance ?? (_instance = new Achievements());
		}
	}

	public int GetNumberOfHoverboardsUsed()
	{
		for (int i = 0; i < _achievementArray.Length; i++)
		{
			if (_achievementArray[i].type == Missions.MissionType.HoverBoard)
			{
				return PlayerInfo.Instance.achievementProgress[i];
			}
		}
		return 0;
	}
}
