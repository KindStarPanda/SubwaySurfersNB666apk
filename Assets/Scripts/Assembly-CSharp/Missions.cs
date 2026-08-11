using System.Collections.Generic;
using UnityEngine;

public class Missions
{
	public enum MissionTarget
	{
		none = 0,
		EarnCoin = 1,
		SpendCoin = 2,
		Score = 3,
		JumpTrain = 4,
		Jump = 5,
		Roll = 6,
		RollLeft = 7,
		RollCenter = 8,
		RollRight = 9,
		RollUnderBarriers = 10,
		JumpBarriers = 11,
		DieToTrain = 12,
		Jetpack = 13,
		SuperSneakers = 14,
		Letters = 15,
		Magnets = 16,
		MysteryBoxes = 17,
		BeatFriends = 18,
		DailyQuests = 19,
		Tokens = 20,
		DodgeBarriers = 21,
		CrashBarriers = 22,
		CrashTrains = 23,
		Powerups = 24,
		Headstart = 25,
		CoinsWithMagnet = 26,
		BuyMysterybox = 27,
		CollectCoinPouch = 28,
		TimeDeath = 29,
		BumpTrain = 30,
		BumpBush = 31,
		BumpLightSignal = 32,
		BumpBarrier = 33,
		HoverBoard = 34,
		HoverBoardExpire = 35,
		NoCoinsBeforeScore = 36,
		HaveTrophiesFirstAchievements = 37,
		DoubleMultiplier = 38,
		ReachMissionSet = 39,
		GuardJump = 40,
		HaveUpgrades = 41,
		HaveHeadStartLarge = 42,
		HaveKing = 43,
		HaveLucy = 44,
		HaveNinja = 45,
		HaveFrank = 46,
		HaveFrizzy = 47,
		PokeFriend = 48,
		HaveTag = 49,
		HaveTasha = 50,
		HaveZoe = 51,
		HaveBrody = 52,
		HavePrinceK = 53,
		HaveYutani = 54,
		FacebookLoggedIn = 55,
		MysteryBoxGrandPrize = 56,
		MissionSet = 57,
		ActivePowerups = 58,
		LandOnTrainInRow = 59,
		StayInOneLane = 60,
		DailyQuestInRow = 61,
		NoJumpsBeforeScore = 62,
		NoRollsBeforeScore = 63,
		NoPowerUpsBeforeScore = 64,
		OneOfEachPowerup = 65
	}

	public enum MissionType
	{
		none = 0,
		EarnCoin = 1,
		EarnCoinSingleRun = 2,
		SpendCoin = 3,
		Score = 4,
		JumpTrain = 5,
		JumpTrainSingleRun = 6,
		Jump = 7,
		JumpSingleRun = 8,
		Roll = 9,
		RollSingleRun = 10,
		RollLeft = 11,
		RollCenter = 12,
		RollRight = 13,
		RollUnderBarriers = 14,
		JumpBarriers = 15,
		DieToTrain = 16,
		Jetpack = 17,
		JetpackSingleRun = 18,
		SuperSneakers = 19,
		SuperSneakersSingleRun = 20,
		Letters = 21,
		Magnets = 22,
		MagnetsSingleRun = 23,
		MysteryBoxes = 24,
		BeatFriends = 25,
		BeatFriendsSingleRun = 26,
		DailyQuests = 27,
		Tokens = 28,
		DodgeBarriers = 29,
		CrashBarriers = 30,
		CrashBarriersSingleRun = 31,
		CrashTrains = 32,
		BumpTrainSingleRun = 33,
		Powerups = 34,
		Headstart = 35,
		CoinsWithMagnet = 36,
		BuyMysterybox = 37,
		CollectCoinPouch = 38,
		TimeDeath = 39,
		BumpTrain = 40,
		BumpBush = 41,
		BumpLightSignal = 42,
		BumpBarrier = 43,
		ScoreSingleRun = 44,
		HoverBoard = 45,
		HoverBoardExpire = 46,
		NoCoinsBeforeScore = 47,
		DoubleMultiplierSingleRun = 48,
		ReachMissionSet = 49,
		GuardJump = 50,
		HaveUpgrades = 51,
		HaveHeadStartLarge = 52,
		HaveTrophiesFirstAchievements = 53,
		HaveKing = 54,
		HaveLucy = 55,
		HaveNinja = 56,
		HaveFrank = 57,
		HaveFrizzy = 58,
		PokeFriend = 59,
		HaveTag = 60,
		HaveTasha = 61,
		HaveZoe = 62,
		HaveBrody = 63,
		HavePrinceK = 64,
		HaveYutani = 65,
		DodgeBarriersSingleRun = 66,
		HoverBoardExpireSingleRun = 67,
		BumpBarrierSingleRun = 68,
		BumpLightSignalSingleRun = 69,
		FacebookLoggedIn = 70,
		MysteryBoxGrandPrize = 71,
		MissionSetSingleRun = 72,
		ActivePowerups = 73,
		LandOnTrainInRow = 74,
		StayInOneLane = 75,
		DailyQuestInRow = 76,
		NoJumpsBeforeScore = 77,
		NoRollsBeforeScore = 78,
		NoPowerUpsBeforeScore = 79,
		OneOfEachPowerup = 80
	}

	public delegate void MissionSetCompleteHandler();

	public delegate void MissionCompleteHandler(string msg);

	public const int NUMBER_OF_MISSIONS_IN_MISSION_SET = 3;

	private int _currentMissionTemplateSetLoaded = -1;

	private MissionTemplate[] _templates;

	private int[] _currentRunProgress;

	private bool syncAchievementsOnceDone;

	private int _currentMissionSetLoaded = -1;

	private Mission[] _combinedArray;

	public MissionSetCompleteHandler onMissionSetComplete;

	public MissionCompleteHandler onMissionComplete;

	private Mission[][] _missions;

	private Mission[][] _repeatableMissions = new Mission[20][]
	{
		new Mission[3]
		{
			new Mission(MissionType.JumpTrainSingleRun, 4),
			new Mission(MissionType.JetpackSingleRun, 3),
			new Mission(MissionType.CoinsWithMagnet, 1200)
		},
		new Mission[3]
		{
			new Mission(MissionType.DodgeBarriersSingleRun, 30),
			new Mission(MissionType.MagnetsSingleRun, 5),
			new Mission(MissionType.Headstart, 10)
		},
		new Mission[3]
		{
			new Mission(MissionType.HoverBoardExpireSingleRun, 4),
			new Mission(MissionType.DoubleMultiplierSingleRun, 4),
			new Mission(MissionType.DailyQuestInRow, 3)
		},
		new Mission[3]
		{
			new Mission(MissionType.BumpBush, 5),
			new Mission(MissionType.MysteryBoxes, 10),
			new Mission(MissionType.Roll, 300)
		},
		new Mission[3]
		{
			new Mission(MissionType.JumpSingleRun, 70),
			new Mission(MissionType.SuperSneakersSingleRun, 5),
			new Mission(MissionType.NoPowerUpsBeforeScore, 120000)
		},
		new Mission[3]
		{
			new Mission(MissionType.StayInOneLane, 20),
			new Mission(MissionType.BumpTrainSingleRun, 12),
			new Mission(MissionType.SpendCoin, 9000)
		},
		new Mission[3]
		{
			new Mission(MissionType.ScoreSingleRun, 300000),
			new Mission(MissionType.NoJumpsBeforeScore, 25000),
			new Mission(MissionType.Score, 5000000)
		},
		new Mission[3]
		{
			new Mission(MissionType.BumpBarrier, 40),
			new Mission(MissionType.BumpBarrierSingleRun, 4),
			new Mission(MissionType.Jump, 300)
		},
		new Mission[3]
		{
			new Mission(MissionType.EarnCoinSingleRun, 800),
			new Mission(MissionType.OneOfEachPowerup, 1),
			new Mission(MissionType.Letters, 12)
		},
		new Mission[3]
		{
			new Mission(MissionType.RollSingleRun, 70),
			new Mission(MissionType.BuyMysterybox, 10),
			new Mission(MissionType.EarnCoin, 20000)
		},
		new Mission[3]
		{
			new Mission(MissionType.LandOnTrainInRow, 10),
			new Mission(MissionType.NoRollsBeforeScore, 25000),
			new Mission(MissionType.DodgeBarriers, 150)
		},
		new Mission[3]
		{
			new Mission(MissionType.HoverBoard, 20),
			new Mission(MissionType.HoverBoardExpire, 8),
			new Mission(MissionType.Powerups, 50)
		},
		new Mission[3]
		{
			new Mission(MissionType.BumpLightSignalSingleRun, 5),
			new Mission(MissionType.RollCenter, 200),
			new Mission(MissionType.BumpLightSignal, 40)
		},
		new Mission[3]
		{
			new Mission(MissionType.OneOfEachPowerup, 1),
			new Mission(MissionType.DoubleMultiplierSingleRun, 5),
			new Mission(MissionType.NoPowerUpsBeforeScore, 140000)
		},
		new Mission[3]
		{
			new Mission(MissionType.Jetpack, 12),
			new Mission(MissionType.JetpackSingleRun, 4),
			new Mission(MissionType.Headstart, 12)
		},
		new Mission[3]
		{
			new Mission(MissionType.EarnCoinSingleRun, 1000),
			new Mission(MissionType.CoinsWithMagnet, 1500),
			new Mission(MissionType.EarnCoin, 20000)
		},
		new Mission[3]
		{
			new Mission(MissionType.HoverBoardExpireSingleRun, 5),
			new Mission(MissionType.BuyMysterybox, 10),
			new Mission(MissionType.SpendCoin, 12000)
		},
		new Mission[3]
		{
			new Mission(MissionType.StayInOneLane, 30),
			new Mission(MissionType.MagnetsSingleRun, 5),
			new Mission(MissionType.Magnets, 20)
		},
		new Mission[3]
		{
			new Mission(MissionType.NoJumpsBeforeScore, 30000),
			new Mission(MissionType.NoRollsBeforeScore, 30000),
			new Mission(MissionType.NoPowerUpsBeforeScore, 160000)
		},
		new Mission[3]
		{
			new Mission(MissionType.LandOnTrainInRow, 12),
			new Mission(MissionType.HoverBoardExpire, 10),
			new Mission(MissionType.Powerups, 40)
		}
	};

	private Mission[][] _storylineMissions = new Mission[29][]
	{
		new Mission[3]
		{
			new Mission(MissionType.EarnCoin, 500),
			new Mission(MissionType.ScoreSingleRun, 1000),
			new Mission(MissionType.Powerups, 2)
		},
		new Mission[3]
		{
			new Mission(MissionType.EarnCoinSingleRun, 200),
			new Mission(MissionType.Jump, 20),
			new Mission(MissionType.SuperSneakers, 2)
		},
		new Mission[3]
		{
			new Mission(MissionType.Tokens, 2),
			new Mission(MissionType.Roll, 30),
			new Mission(MissionType.SpendCoin, 2000)
		},
		new Mission[3]
		{
			new Mission(MissionType.DailyQuests, 1),
			new Mission(MissionType.DodgeBarriers, 20),
			new Mission(MissionType.ScoreSingleRun, 6000)
		},
		new Mission[3]
		{
			new Mission(MissionType.EarnCoin, 2500),
			new Mission(MissionType.JumpSingleRun, 30),
			new Mission(MissionType.BuyMysterybox, 1)
		},
		new Mission[3]
		{
			new Mission(MissionType.HoverBoard, 1),
			new Mission(MissionType.Magnets, 5),
			new Mission(MissionType.BumpBarrier, 2)
		},
		new Mission[3]
		{
			new Mission(MissionType.Jetpack, 2),
			new Mission(MissionType.BeatFriends, 1),
			new Mission(MissionType.Headstart, 1)
		},
		new Mission[3]
		{
			new Mission(MissionType.BumpTrainSingleRun, 3),
			new Mission(MissionType.CoinsWithMagnet, 40),
			new Mission(MissionType.TimeDeath, 10)
		},
		new Mission[3]
		{
			new Mission(MissionType.HoverBoardExpire, 1),
			new Mission(MissionType.MysteryBoxes, 2),
			new Mission(MissionType.RollSingleRun, 30)
		},
		new Mission[3]
		{
			new Mission(MissionType.ScoreSingleRun, 20000),
			new Mission(MissionType.Powerups, 12),
			new Mission(MissionType.JumpTrain, 2)
		},
		new Mission[3]
		{
			new Mission(MissionType.NoCoinsBeforeScore, 4000),
			new Mission(MissionType.RollCenter, 50),
			new Mission(MissionType.EarnCoin, 5000)
		},
		new Mission[3]
		{
			new Mission(MissionType.DailyQuests, 2),
			new Mission(MissionType.DodgeBarriers, 40),
			new Mission(MissionType.SuperSneakers, 5)
		},
		new Mission[3]
		{
			new Mission(MissionType.BumpBush, 2),
			new Mission(MissionType.CoinsWithMagnet, 160),
			new Mission(MissionType.MagnetsSingleRun, 2)
		},
		new Mission[3]
		{
			new Mission(MissionType.MysteryBoxes, 4),
			new Mission(MissionType.RollSingleRun, 40),
			new Mission(MissionType.EarnCoinSingleRun, 400)
		},
		new Mission[3]
		{
			new Mission(MissionType.Score, 100000),
			new Mission(MissionType.Jetpack, 5),
			new Mission(MissionType.BumpLightSignal, 12)
		},
		new Mission[3]
		{
			new Mission(MissionType.DailyQuests, 3),
			new Mission(MissionType.SuperSneakersSingleRun, 3),
			new Mission(MissionType.JumpTrain, 4)
		},
		new Mission[3]
		{
			new Mission(MissionType.ScoreSingleRun, 50000),
			new Mission(MissionType.SpendCoin, 4000),
			new Mission(MissionType.Magnets, 15)
		},
		new Mission[3]
		{
			new Mission(MissionType.JetpackSingleRun, 2),
			new Mission(MissionType.BumpTrainSingleRun, 6),
			new Mission(MissionType.HoverBoardExpire, 3)
		},
		new Mission[3]
		{
			new Mission(MissionType.HoverBoard, 5),
			new Mission(MissionType.MagnetsSingleRun, 3),
			new Mission(MissionType.Tokens, 5)
		},
		new Mission[3]
		{
			new Mission(MissionType.Score, 250000),
			new Mission(MissionType.JumpSingleRun, 40),
			new Mission(MissionType.Powerups, 25)
		},
		new Mission[3]
		{
			new Mission(MissionType.BumpBarrier, 15),
			new Mission(MissionType.BuyMysterybox, 3),
			new Mission(MissionType.CoinsWithMagnet, 240)
		},
		new Mission[3]
		{
			new Mission(MissionType.DodgeBarriers, 80),
			new Mission(MissionType.SpendCoin, 8000),
			new Mission(MissionType.SuperSneakersSingleRun, 4)
		},
		new Mission[3]
		{
			new Mission(MissionType.Headstart, 8),
			new Mission(MissionType.JumpTrain, 10),
			new Mission(MissionType.Jetpack, 15)
		},
		new Mission[3]
		{
			new Mission(MissionType.MysteryBoxes, 8),
			new Mission(MissionType.RollCenter, 200),
			new Mission(MissionType.DailyQuests, 4)
		},
		new Mission[3]
		{
			new Mission(MissionType.EarnCoin, 15000),
			new Mission(MissionType.ScoreSingleRun, 120000),
			new Mission(MissionType.JumpTrainSingleRun, 3)
		},
		new Mission[3]
		{
			new Mission(MissionType.RollSingleRun, 50),
			new Mission(MissionType.Score, 500000),
			new Mission(MissionType.NoCoinsBeforeScore, 12000)
		},
		new Mission[3]
		{
			new Mission(MissionType.Tokens, 10),
			new Mission(MissionType.BuyMysterybox, 6),
			new Mission(MissionType.BumpLightSignal, 20)
		},
		new Mission[3]
		{
			new Mission(MissionType.JumpSingleRun, 50),
			new Mission(MissionType.HoverBoard, 12),
			new Mission(MissionType.HoverBoardExpire, 4)
		},
		new Mission[3]
		{
			new Mission(MissionType.EarnCoinSingleRun, 750),
			new Mission(MissionType.BumpBarrier, 25),
			new Mission(MissionType.ScoreSingleRun, 250000)
		}
	};

	private static readonly Dictionary<MissionType, MissionTemplate> missionTemplates = new Dictionary<MissionType, MissionTemplate>
	{
		{
			MissionType.EarnCoin,
			new MissionTemplate
			{
				descriptionSingle = Strings.Get(StringID.MISSIONS_EARNCOIN_DESC_SINGLE),
				description = Strings.Get(StringID.MISSIONS_EARNCOIN_DESC),
				ultraShortDescriptionSingle = Strings.Get(StringID.MISSIONS_EARNCOIN_ULTRAS_DESC_SINGLE),
				ultraShortDescription = Strings.Get(StringID.MISSIONS_EARNCOIN_ULTRAS_DESC),
				missionTarget = MissionTarget.EarnCoin
			}
		},
		{
			MissionType.EarnCoinSingleRun,
			new MissionTemplate
			{
				descriptionSingle = "Collect {0} coin in one run. {1} left",
				description = "Collect {0} coins in one run. {1} left",
				ultraShortDescriptionSingle = "Collect {0} coin",
				ultraShortDescription = "Collect {0} coins",
				missionTarget = MissionTarget.EarnCoin,
				singleRun = true
			}
		},
		{
			MissionType.SpendCoin,
			new MissionTemplate
			{
				descriptionSingle = "Spend {0} coin. {1} left",
				description = "Spend {0} coins. {1} left",
				ultraShortDescriptionSingle = "Spend {0} coin",
				ultraShortDescription = "Spend {0} coins",
				missionTarget = MissionTarget.SpendCoin
			}
		},
		{
			MissionType.Score,
			new MissionTemplate
			{
				descriptionSingle = "Collect {0} point. {1} left",
				description = "Collect {0} points. {1} left",
				ultraShortDescriptionSingle = "Collect {0} point",
				ultraShortDescription = "Collect {0} points",
				missionTarget = MissionTarget.Score
			}
		},
		{
			MissionType.JumpTrain,
			new MissionTemplate
			{
				descriptionSingle = "Jump over {0} train. {1} left",
				description = "Jump over {0} trains. {1} left",
				ultraShortDescriptionSingle = "Jump {0} train",
				ultraShortDescription = "Jump {0} trains",
				missionTarget = MissionTarget.JumpTrain
			}
		},
		{
			MissionType.JumpTrainSingleRun,
			new MissionTemplate
			{
				descriptionSingle = "Jump over {0} train in one run. {1} left",
				description = "Jump over {0} trains in one run. {1} left",
				ultraShortDescriptionSingle = "Jump {0} train",
				ultraShortDescription = "Jump {0} trains",
				missionTarget = MissionTarget.JumpTrain,
				singleRun = true
			}
		},
		{
			MissionType.Jump,
			new MissionTemplate
			{
				descriptionSingle = "Jump {0} time. {1} left",
				description = "Jump {0} times. {1} left",
				ultraShortDescriptionSingle = "Jump {0} time",
				ultraShortDescription = "Jump {0} times",
				missionTarget = MissionTarget.Jump
			}
		},
		{
			MissionType.JumpSingleRun,
			new MissionTemplate
			{
				descriptionSingle = "Jump {0} time in one run. {1} left",
				description = "Jump {0} times in one run. {1} left",
				ultraShortDescriptionSingle = "Jump {0} time",
				ultraShortDescription = "Jump {0} times",
				missionTarget = MissionTarget.Jump,
				singleRun = true
			}
		},
		{
			MissionType.Roll,
			new MissionTemplate
			{
				descriptionSingle = "Roll {0} time. {1} left",
				description = "Roll {0} times in total. {1} left",
				ultraShortDescriptionSingle = "Roll {0} time",
				ultraShortDescription = "Roll {0} times",
				missionTarget = MissionTarget.Roll
			}
		},
		{
			MissionType.RollSingleRun,
			new MissionTemplate
			{
				descriptionSingle = "Roll {0} time in one run. {1} left",
				description = "Roll {0} times in one run. {1} left",
				ultraShortDescriptionSingle = "Roll {0} time",
				ultraShortDescription = "Roll {0} times",
				missionTarget = MissionTarget.Roll,
				singleRun = true
			}
		},
		{
			MissionType.RollLeft,
			new MissionTemplate
			{
				descriptionSingle = "Roll {0} time in left lane. {1} left",
				description = "Roll {0} times in left lane. {1} left",
				ultraShortDescriptionSingle = "Roll {0} time",
				ultraShortDescription = "Roll {0} times",
				missionTarget = MissionTarget.RollLeft
			}
		},
		{
			MissionType.RollCenter,
			new MissionTemplate
			{
				descriptionSingle = "Roll {0} time in center lane. {1} left",
				description = "Roll {0} times in center lane. {1} left",
				ultraShortDescriptionSingle = "Roll {0} time",
				ultraShortDescription = "Roll {0} times",
				missionTarget = MissionTarget.RollCenter
			}
		},
		{
			MissionType.RollRight,
			new MissionTemplate
			{
				descriptionSingle = "Roll {0} time in right lane. {1} left",
				description = "Roll {0} times in right lane. {1} left",
				ultraShortDescriptionSingle = "Roll {0} time",
				ultraShortDescription = "Roll {0} times",
				missionTarget = MissionTarget.RollRight
			}
		},
		{
			MissionType.RollUnderBarriers,
			new MissionTemplate
			{
				descriptionSingle = "Roll under {0} barrier. {1} left",
				description = "Roll under {0} barriers. {1} left",
				ultraShortDescriptionSingle = "Roll under {0} barrier",
				ultraShortDescription = "Roll under {0} barriers",
				missionTarget = MissionTarget.RollUnderBarriers
			}
		},
		{
			MissionType.JumpBarriers,
			new MissionTemplate
			{
				descriptionSingle = "Jump over {0} barrier. {1} left",
				description = "Jump over {0} barriers. {1} left",
				ultraShortDescriptionSingle = "Jump {0} barrier",
				ultraShortDescription = "Jump {0} barriers",
				missionTarget = MissionTarget.JumpBarriers
			}
		},
		{
			MissionType.DieToTrain,
			new MissionTemplate
			{
				descriptionSingle = "Get run over by {0} train. {1} left",
				description = "Get run over by {0} trains. {1} left",
				ultraShortDescriptionSingle = "Get run over {0} time",
				ultraShortDescription = "Get run over {0} times",
				missionTarget = MissionTarget.DieToTrain
			}
		},
		{
			MissionType.Jetpack,
			new MissionTemplate
			{
				descriptionSingle = "Pick up {0} Jetpack. {1} left",
				description = "Pick up {0} Jetpacks. {1} left",
				ultraShortDescriptionSingle = "Pick up {0} Jetpack",
				ultraShortDescription = "Pick up {0} Jetpacks",
				missionTarget = MissionTarget.Jetpack
			}
		},
		{
			MissionType.JetpackSingleRun,
			new MissionTemplate
			{
				descriptionSingle = "Pick up {0} Jetpack in one run. {1} left",
				description = "Pick up {0} Jetpacks in one run. {1} left",
				ultraShortDescriptionSingle = "Pick up {0} Jetpack",
				ultraShortDescription = "Pick up {0} Jetpacks",
				missionTarget = MissionTarget.Jetpack,
				singleRun = true
			}
		},
		{
			MissionType.SuperSneakers,
			new MissionTemplate
			{
				descriptionSingle = "Pick up {0} Super Sneaker. {1} left",
				description = "Pick up {0} Super Sneakers. {1} left",
				ultraShortDescriptionSingle = "Pick up {0} Sneaker",
				ultraShortDescription = "Pick up {0} Sneakers",
				missionTarget = MissionTarget.SuperSneakers
			}
		},
		{
			MissionType.SuperSneakersSingleRun,
			new MissionTemplate
			{
				descriptionSingle = "Pick up {0} Super Sneaker in one run. {1} left",
				description = "Pick up {0} Super Sneakers in one run. {1} left",
				ultraShortDescriptionSingle = "Pick up {0} Sneaker",
				ultraShortDescription = "Pick up {0} Sneakers",
				missionTarget = MissionTarget.SuperSneakers,
				singleRun = true
			}
		},
		{
			MissionType.Letters,
			new MissionTemplate
			{
				descriptionSingle = "Pick up {0} Daily Letter. {1} left",
				description = "Pick up {0} Daily Letters. {1} left",
				ultraShortDescriptionSingle = "Pick up {0} Letter",
				ultraShortDescription = "Pick up {0} Letters",
				missionTarget = MissionTarget.Letters
			}
		},
		{
			MissionType.Magnets,
			new MissionTemplate
			{
				descriptionSingle = "Pick up {0} Coin Magnet. {1} left",
				description = "Pick up {0} Coin Magnets. {1} left",
				ultraShortDescriptionSingle = "Pick up {0} Magnet",
				ultraShortDescription = "Pick up {0} Magnets",
				missionTarget = MissionTarget.Magnets
			}
		},
		{
			MissionType.MagnetsSingleRun,
			new MissionTemplate
			{
				descriptionSingle = "Pick up {0} Magnet in one run. {1} left",
				description = "Pick up {0} Magnets in one run. {1} left",
				ultraShortDescriptionSingle = "Pick up {0} Magnet",
				ultraShortDescription = "Pick up {0} Magnets",
				missionTarget = MissionTarget.Magnets,
				singleRun = true
			}
		},
		{
			MissionType.BeatFriends,
			new MissionTemplate
			{
				descriptionSingle = "Beat {0} friend. {1} left",
				description = "Beat {0} friends. {1} left",
				ultraShortDescriptionSingle = "Beat {0} friend",
				ultraShortDescription = "Beat {0} friends",
				missionTarget = MissionTarget.BeatFriends
			}
		},
		{
			MissionType.BeatFriendsSingleRun,
			new MissionTemplate
			{
				descriptionSingle = "Beat {0} friend in one run. {1} left",
				description = "Beat {0} friends in one run. {1} left",
				ultraShortDescriptionSingle = "Beat {0} friend",
				ultraShortDescription = "Beat {0} friends",
				missionTarget = MissionTarget.BeatFriends,
				singleRun = true
			}
		},
		{
			MissionType.DailyQuests,
			new MissionTemplate
			{
				descriptionSingle = "Complete {0} Daily Challenge. {1} left",
				description = "Complete {0} Daily Challenges. {1} left",
				ultraShortDescriptionSingle = "{0} Daily Challenge",
				ultraShortDescription = "{0} Daily Challenges",
				missionTarget = MissionTarget.DailyQuests
			}
		},
		{
			MissionType.Tokens,
			new MissionTemplate
			{
				descriptionSingle = "Get {0} character token from Mystery Box. {1} left",
				description = "Get {0} character tokens from Mystery Box. {1} left",
				ultraShortDescriptionSingle = "Get {0} token",
				ultraShortDescription = "Get {0} tokens",
				missionTarget = MissionTarget.Tokens
			}
		},
		{
			MissionType.DodgeBarriers,
			new MissionTemplate
			{
				descriptionSingle = "Dodge {0} barrier. {1} left",
				description = "Dodge {0} barriers. {1} left",
				ultraShortDescriptionSingle = "Dodge {0} barrier",
				ultraShortDescription = "Dodge {0} barriers",
				missionTarget = MissionTarget.DodgeBarriers
			}
		},
		{
			MissionType.CrashBarriers,
			new MissionTemplate
			{
				descriptionSingle = "Crash into {0} barrier. {1} left",
				description = "Crash into {0} barriers. {1} left",
				ultraShortDescriptionSingle = "Crash into {0} barrier",
				ultraShortDescription = "Crash into {0} barriers",
				missionTarget = MissionTarget.CrashBarriers
			}
		},
		{
			MissionType.CrashBarriersSingleRun,
			new MissionTemplate
			{
				descriptionSingle = "Crash into {0} barrier in one run. {1} left",
				description = "Crash into {0} barriers in one run. {1} left",
				ultraShortDescriptionSingle = "Crash into {0} barrier",
				ultraShortDescription = "Crash into {0} barriers",
				missionTarget = MissionTarget.CrashBarriers,
				singleRun = true
			}
		},
		{
			MissionType.CrashTrains,
			new MissionTemplate
			{
				descriptionSingle = "Crash into {0} train. {1} left",
				description = "Crash into {0} trains. {1} left",
				ultraShortDescriptionSingle = "Crash into {0} train",
				ultraShortDescription = "Crash into {0} trains",
				missionTarget = MissionTarget.CrashTrains
			}
		},
		{
			MissionType.BumpTrainSingleRun,
			new MissionTemplate
			{
				descriptionSingle = "Bump into {0} train in one run. {1} left",
				description = "Bump into {0} trains in one run. {1} left",
				ultraShortDescriptionSingle = "Bump into {0} train",
				ultraShortDescription = "Bump into {0} trains",
				missionTarget = MissionTarget.BumpTrain,
				singleRun = true
			}
		},
		{
			MissionType.Powerups,
			new MissionTemplate
			{
				descriptionSingle = "Pickup {0} Powerup. {1} left",
				description = "Pickup {0} Powerups. {1} left",
				ultraShortDescriptionSingle = "Pickup {0} Powerup",
				ultraShortDescription = "Pickup {0} Powerups",
				missionTarget = MissionTarget.Powerups
			}
		},
		{
			MissionType.Headstart,
			new MissionTemplate
			{
				descriptionSingle = "Use {0} Headstart. {1} left",
				description = "Use {0} Headstarts. {1} left",
				ultraShortDescriptionSingle = "Use {0} Headstart",
				ultraShortDescription = "Use {0} Headstarts",
				missionTarget = MissionTarget.Headstart
			}
		},
		{
			MissionType.CoinsWithMagnet,
			new MissionTemplate
			{
				descriptionSingle = "Pickup {0} coin with a Magnet. {1} left",
				description = "Pickup {0} coins with a Magnet. {1} left",
				ultraShortDescriptionSingle = "{0} coin with Magnet",
				ultraShortDescription = "{0} coins with Magnet",
				missionTarget = MissionTarget.CoinsWithMagnet
			}
		},
		{
			MissionType.BuyMysterybox,
			new MissionTemplate
			{
				descriptionSingle = "Buy {0} Mystery box. {1} left",
				description = "Buy {0} Mystery boxes. {1} left",
				ultraShortDescriptionSingle = "Buy {0} Mystery box",
				ultraShortDescription = "Buy {0} Mystery boxes",
				missionTarget = MissionTarget.BuyMysterybox
			}
		},
		{
			MissionType.CollectCoinPouch,
			new MissionTemplate
			{
				descriptionSingle = "Collect {0} coin pouch from friends menu. {1} left",
				description = "Collect {0} coin pouches from friends menu. {1} left",
				ultraShortDescriptionSingle = "{0} coin pouch",
				ultraShortDescription = "{0} coin pouches",
				missionTarget = MissionTarget.CollectCoinPouch
			}
		},
		{
			MissionType.TimeDeath,
			new MissionTemplate
			{
				descriptionSingle = "Get caught in first {0} second of run. Ran {1} sec",
				description = "Get caught in first {0} seconds of run. Ran {1} sec",
				ultraShortDescriptionSingle = "Caught in {0} sec",
				ultraShortDescription = "Caught in {0} sec",
				missionTarget = MissionTarget.TimeDeath,
				singleRun = true,
				completeIfLess = true
			}
		},
		{
			MissionType.MysteryBoxes,
			new MissionTemplate
			{
				descriptionSingle = "Pick up {0} Mystery Box. {1} left",
				description = "Pick up {0} Mystery Boxes. {1} left",
				ultraShortDescriptionSingle = "{0} Mystery Box",
				ultraShortDescription = "{0} Mystery Boxes",
				missionTarget = MissionTarget.MysteryBoxes
			}
		},
		{
			MissionType.BumpBarrier,
			new MissionTemplate
			{
				descriptionSingle = "Stumble into {0} barrier. {1} left",
				description = "Stumble into {0} barriers. {1} left",
				ultraShortDescriptionSingle = "Stumble {0} barrier",
				ultraShortDescription = "Stumble {0} barriers",
				missionTarget = MissionTarget.BumpBarrier
			}
		},
		{
			MissionType.BumpLightSignal,
			new MissionTemplate
			{
				descriptionSingle = "Bump into {0} light signal. {1} left",
				description = "Bump into {0} light signals. {1} left",
				ultraShortDescriptionSingle = "{0} light signal",
				ultraShortDescription = "{0} light signal",
				missionTarget = MissionTarget.BumpLightSignal
			}
		},
		{
			MissionType.BumpBush,
			new MissionTemplate
			{
				descriptionSingle = "Bump {0} bush. {1} left",
				description = "Bump {0} bushes. {1} left",
				ultraShortDescriptionSingle = "Bump {0} bush",
				ultraShortDescription = "Bump {0} bushes",
				missionTarget = MissionTarget.BumpBush
			}
		},
		{
			MissionType.BumpTrain,
			new MissionTemplate
			{
				descriptionSingle = "Bump {0} train. {1} left",
				description = "Bump {0} trains. {1} left",
				ultraShortDescriptionSingle = "Bump {0} train",
				ultraShortDescription = "Bump {0} trains",
				missionTarget = MissionTarget.BumpTrain
			}
		},
		{
			MissionType.HoverBoard,
			new MissionTemplate
			{
				descriptionSingle = "Use {0} Hoverboard. {1} left",
				description = "Use {0} Hoverboards. {1} left",
				ultraShortDescriptionSingle = "Use {0} Hoverboard",
				ultraShortDescription = "Use {0} Hoverboards",
				missionTarget = MissionTarget.HoverBoard
			}
		},
		{
			MissionType.HoverBoardExpire,
			new MissionTemplate
			{
				descriptionSingle = "Use {0} Hoverboard without crashing. {1} left",
				description = "Use {0} Hoverboards without crashing. {1} left",
				ultraShortDescriptionSingle = "{0} Hoverboard no crash",
				ultraShortDescription = "{0} Hoverboards no crash",
				missionTarget = MissionTarget.HoverBoardExpire
			}
		},
		{
			MissionType.ScoreSingleRun,
			new MissionTemplate
			{
				descriptionSingle = "Score {0} point in one run. {1} left",
				description = "Score {0} points in one run. {1} left",
				ultraShortDescriptionSingle = "{0} point in one run",
				ultraShortDescription = "{0} points in one run",
				missionTarget = MissionTarget.Score,
				singleRun = true
			}
		},
		{
			MissionType.NoCoinsBeforeScore,
			new MissionTemplate
			{
				descriptionSingle = "Score {0} point without collecting coins. {1} left",
				description = "Score {0} points without collecting coins. {1} left",
				ultraShortDescriptionSingle = "{0} point no coins",
				ultraShortDescription = "{0} points no coins",
				missionTarget = MissionTarget.NoCoinsBeforeScore,
				singleRun = true
			}
		},
		{
			MissionType.DoubleMultiplierSingleRun,
			new MissionTemplate
			{
				descriptionSingle = "Pick up {0} Double Multiplier. {1} left",
				description = "Pick up {0} Double Multipliers in one run. {1} left",
				ultraShortDescriptionSingle = "Pick up {0} Double Multiplier",
				ultraShortDescription = "Pick up {0} Double Multiplier",
				missionTarget = MissionTarget.DoubleMultiplier,
				singleRun = true
			}
		},
		{
			MissionType.ReachMissionSet,
			new MissionTemplate
			{
				missionTarget = MissionTarget.ReachMissionSet
			}
		},
		{
			MissionType.GuardJump,
			new MissionTemplate
			{
				missionTarget = MissionTarget.GuardJump
			}
		},
		{
			MissionType.HaveUpgrades,
			new MissionTemplate
			{
				missionTarget = MissionTarget.HaveUpgrades
			}
		},
		{
			MissionType.HaveHeadStartLarge,
			new MissionTemplate
			{
				missionTarget = MissionTarget.HaveHeadStartLarge
			}
		},
		{
			MissionType.HaveKing,
			new MissionTemplate
			{
				missionTarget = MissionTarget.HaveKing
			}
		},
		{
			MissionType.HaveLucy,
			new MissionTemplate
			{
				missionTarget = MissionTarget.HaveLucy
			}
		},
		{
			MissionType.HaveNinja,
			new MissionTemplate
			{
				missionTarget = MissionTarget.HaveNinja
			}
		},
		{
			MissionType.HaveFrank,
			new MissionTemplate
			{
				missionTarget = MissionTarget.HaveFrank
			}
		},
		{
			MissionType.HaveFrizzy,
			new MissionTemplate
			{
				missionTarget = MissionTarget.HaveFrizzy
			}
		},
		{
			MissionType.PokeFriend,
			new MissionTemplate
			{
				missionTarget = MissionTarget.PokeFriend
			}
		},
		{
			MissionType.HaveTag,
			new MissionTemplate
			{
				missionTarget = MissionTarget.HaveTag
			}
		},
		{
			MissionType.HaveTasha,
			new MissionTemplate
			{
				missionTarget = MissionTarget.HaveTasha
			}
		},
		{
			MissionType.HaveZoe,
			new MissionTemplate
			{
				missionTarget = MissionTarget.HaveZoe
			}
		},
		{
			MissionType.HaveBrody,
			new MissionTemplate
			{
				missionTarget = MissionTarget.HaveBrody
			}
		},
		{
			MissionType.HavePrinceK,
			new MissionTemplate
			{
				missionTarget = MissionTarget.HavePrinceK
			}
		},
		{
			MissionType.HaveYutani,
			new MissionTemplate
			{
				missionTarget = MissionTarget.HaveYutani
			}
		},
		{
			MissionType.DodgeBarriersSingleRun,
			new MissionTemplate
			{
				descriptionSingle = "Dodge {0} barrier in one run. {1} left",
				description = "Dodge {0} barriers in one run. {1} left",
				ultraShortDescriptionSingle = "Dodge {0} barrier",
				ultraShortDescription = "Dodge {0} barriers",
				missionTarget = MissionTarget.DodgeBarriers,
				singleRun = true
			}
		},
		{
			MissionType.HoverBoardExpireSingleRun,
			new MissionTemplate
			{
				descriptionSingle = "Use {0} Hoverboard without crashing. {1} left",
				description = "Use {0} Hoverboards in one run without crashing. {1} left",
				ultraShortDescriptionSingle = "{0} Hoverboard no crash",
				ultraShortDescription = "{0} Hoverboards no crash",
				missionTarget = MissionTarget.HoverBoardExpire,
				singleRun = true
			}
		},
		{
			MissionType.BumpBarrierSingleRun,
			new MissionTemplate
			{
				descriptionSingle = "Stumble upon {0} barrier. {1} left",
				description = "Stumble upon {0} barriers in one run. {1} left",
				ultraShortDescriptionSingle = "Stumble {0} barrier",
				ultraShortDescription = "Stumble {0} barriers",
				missionTarget = MissionTarget.BumpBarrier,
				singleRun = true
			}
		},
		{
			MissionType.BumpLightSignalSingleRun,
			new MissionTemplate
			{
				descriptionSingle = "Bump into {0} light signal. {1} left",
				description = "Bump into {0} light signals in one run. {1} left",
				ultraShortDescriptionSingle = "{0} light signal",
				ultraShortDescription = "{0} light signals",
				missionTarget = MissionTarget.BumpLightSignal,
				singleRun = true
			}
		},
		{
			MissionType.FacebookLoggedIn,
			new MissionTemplate
			{
				missionTarget = MissionTarget.FacebookLoggedIn
			}
		},
		{
			MissionType.MysteryBoxGrandPrize,
			new MissionTemplate
			{
				missionTarget = MissionTarget.MysteryBoxGrandPrize
			}
		},
		{
			MissionType.MissionSetSingleRun,
			new MissionTemplate
			{
				missionTarget = MissionTarget.MissionSet,
				singleRun = true
			}
		},
		{
			MissionType.ActivePowerups,
			new MissionTemplate
			{
				missionTarget = MissionTarget.ActivePowerups
			}
		},
		{
			MissionType.LandOnTrainInRow,
			new MissionTemplate
			{
				descriptionSingle = "Jump and land on a train. {1} left",
				description = "Jump and land on a train {0} times in a row. {1} left",
				ultraShortDescriptionSingle = "Land on a train",
				ultraShortDescription = "Land on {0} trains",
				missionTarget = MissionTarget.LandOnTrainInRow
			}
		},
		{
			MissionType.StayInOneLane,
			new MissionTemplate
			{
				descriptionSingle = "Stay in same lane for {0} second. {1} left",
				description = "Stay in same lane for {0} seconds. {1} left",
				ultraShortDescriptionSingle = "{0} second same lane",
				ultraShortDescription = "{0} seconds same lane",
				missionTarget = MissionTarget.StayInOneLane,
				singleRun = true
			}
		},
		{
			MissionType.DailyQuestInRow,
			new MissionTemplate
			{
				descriptionSingle = "Complete {0} Daily Challenge. {1} left",
				description = "Complete Daily Challenge {0} days in a row. {1} left",
				ultraShortDescriptionSingle = "{0} Daily Challenge",
				ultraShortDescription = "{0} Daily Challenges",
				missionTarget = MissionTarget.DailyQuestInRow
			}
		},
		{
			MissionType.NoJumpsBeforeScore,
			new MissionTemplate
			{
				descriptionSingle = "Score {0} point without jumping. {1} left",
				description = "Score {0} points without jumping. {1} left",
				ultraShortDescriptionSingle = "{0} point no jumps",
				ultraShortDescription = "{0} points no jumps",
				missionTarget = MissionTarget.NoJumpsBeforeScore,
				singleRun = true
			}
		},
		{
			MissionType.NoRollsBeforeScore,
			new MissionTemplate
			{
				descriptionSingle = "Score {0} point without rolling. {1} left",
				description = "Score {0} points without rolling. {1} left",
				ultraShortDescriptionSingle = "{0} point no rolls",
				ultraShortDescription = "{0} points no rolls",
				missionTarget = MissionTarget.NoRollsBeforeScore,
				singleRun = true
			}
		},
		{
			MissionType.NoPowerUpsBeforeScore,
			new MissionTemplate
			{
				descriptionSingle = "Score {0}, no Powerups. {1} left",
				description = "Score {0}, no Powerups. {1} left",
				ultraShortDescriptionSingle = "{0} without Powerups",
				ultraShortDescription = "{0} without Powerups",
				missionTarget = MissionTarget.NoPowerUpsBeforeScore,
				singleRun = true
			}
		},
		{
			MissionType.OneOfEachPowerup,
			new MissionTemplate
			{
				descriptionSingle = "Pick up at least 1 of each Powerup in one run. {1} left",
				description = "Pick up at least {0} of each Powerup in one run. {1} left",
				ultraShortDescriptionSingle = "All Powerup types",
				ultraShortDescription = "All Powerup types {0} times",
				missionTarget = MissionTarget.OneOfEachPowerup,
				singleRun = true
			}
		},
		{
			MissionType.HaveTrophiesFirstAchievements,
			new MissionTemplate
			{
				missionTarget = MissionTarget.HaveTrophiesFirstAchievements
			}
		},
		{
			MissionType.none,
			default(MissionTemplate)
		}
	};

	private static Missions _instance;

	private PlayerInfo playerinfo;

	private MissionTemplate[] templates
	{
		get
		{
			if (_currentMissionTemplateSetLoaded != playerinfo.currentMissionSet || _templates == null)
			{
				if (_templates == null)
				{
					_templates = new MissionTemplate[44];
				}
				for (int i = 0; i < 3; i++)
				{
					_templates[i] = missionTemplates[missions[playerinfo.currentMissionSet][i].type];
				}
				if (_currentMissionTemplateSetLoaded == -1)
				{
					for (int j = 0; j < 41; j++)
					{
						_templates[j + 3] = missionTemplates[Achievements.Instance.achievementArray[j].type];
					}
				}
			}
			_currentMissionTemplateSetLoaded = playerinfo.currentMissionSet;
			return _templates;
		}
	}

	public bool inRun
	{
		get
		{
			return _currentRunProgress != null;
		}
		set
		{
			if (value)
			{
				_currentRunProgress = new int[combinedArray.Length];
			}
			else
			{
				if (_currentRunProgress == null)
				{
					return;
				}
				if (UIScreenController.Instance.GetTopScreenName() != "PauseUI")
				{
					for (int i = 0; i < combinedArray.Length; i++)
					{
						if (templates[i].singleRun && templates[i].completeIfLess && _currentRunProgress[i] < combinedArray[i].goal)
						{
							playerinfo.SetCurrentMissionProgress(i, combinedArray[i].goal);
							Complete(i);
						}
					}
				}
				PlayerDidThis(MissionTarget.StayInOneLane, (int)(Time.time - Character.Instance.sameLaneTimeStamp));
				RemoveProgressForThis(MissionTarget.StayInOneLane);
				CheckAllCompleteAndIncrement();
				if (!syncAchievementsOnceDone)
				{
					SyncAchievements();
				}
				if (GameStats.Instance.coins == 50)
				{
					SocialManager.instance.CompleteThisAchievement("CollectExactly50Coins", 100f);
				}
				_currentRunProgress = null;
			}
		}
	}

	private Mission[] combinedArray
	{
		get
		{
			if (_currentMissionSetLoaded != playerinfo.currentMissionSet || _combinedArray == null)
			{
				if (_combinedArray == null)
				{
					_combinedArray = new Mission[44];
				}
				for (int i = 0; i < 3; i++)
				{
					_combinedArray[i] = missions[playerinfo.currentMissionSet][i];
				}
				if (_currentMissionSetLoaded == -1)
				{
					for (int j = 0; j < 41; j++)
					{
						_combinedArray[j + 3] = Achievements.Instance.achievementArray[j];
					}
				}
			}
			_currentMissionSetLoaded = currentMissionSet;
			return _combinedArray;
		}
	}

	public int currentMissionSet
	{
		get
		{
			return playerinfo.currentMissionSet;
		}
		set
		{
			if (value != playerinfo.currentMissionSet)
			{
				int num = Mathf.Clamp(value, 0, missionSetCount);
				playerinfo.InitCurrentMissionSet(num, missions[num].Length);
			}
		}
	}

	private Mission[][] missions
	{
		get
		{
			if (_missions == null)
			{
				_missions = new Mission[GetNumberOfBasicMission() + _repeatableMissions.Length][];
				for (int i = 0; i < GetNumberOfBasicMission(); i++)
				{
					_missions[i] = _storylineMissions[i];
				}
				for (int j = 0; j < _repeatableMissions.Length; j++)
				{
					_missions[GetNumberOfBasicMission() + j] = _repeatableMissions[j];
				}
			}
			return _missions;
		}
	}

	public int missionSetCount
	{
		get
		{
			return missions.Length;
		}
	}

	public int missionSetStoryCount
	{
		get
		{
			return missions.Length - _repeatableMissions.Length;
		}
	}

	public static Missions Instance
	{
		get
		{
			return _instance ?? (_instance = new Missions());
		}
	}

	private Missions()
	{
		playerinfo = PlayerInfo.Instance;
		if (playerinfo.currentMissionSet == -1)
		{
			playerinfo.InitCurrentMissionSet(0, missions[0].Length);
		}
	}

	public void SkipMission(int missionNumber)
	{
		playerinfo.SetCurrentMissionProgress(missionNumber, missions[playerinfo.currentMissionSet][missionNumber].goal);
		Complete(missionNumber);
	}

	private void Complete(int mission, float completedFactor = 1f, bool sendEvenIfInRun = false)
	{
		if (mission < 3)
		{
			if (completedFactor != 1f)
			{
				return;
			}
			MissionCompleteHandler missionCompleteHandler = onMissionComplete;
			if (missionCompleteHandler != null)
			{
				string format = ((combinedArray[mission].goal != 1) ? templates[mission].ultraShortDescription : templates[mission].ultraShortDescriptionSingle);
				missionCompleteHandler(string.Format(format, combinedArray[mission].goal));
			}
			bool flag = true;
			for (int i = 0; i < 3; i++)
			{
				int currentMissionProgress = playerinfo.GetCurrentMissionProgress(i);
				if (currentMissionProgress < combinedArray[i].goal)
				{
					flag = false;
					break;
				}
			}
			if (!flag)
			{
				return;
			}
			Flurry.LogEventWithAParameter("Mission Set completed", "Mission Set", (PlayerInfo.Instance.missionCompletedSum + 1).ToString());
			PlayerDidThis(MissionTarget.MissionSet);
			if (_currentRunProgress != null)
			{
				for (int j = 0; j < 3; j++)
				{
					if (_currentRunProgress[j] != 0)
					{
						_currentRunProgress[j] = 0;
					}
				}
			}
			if (currentMissionSet + 1 == missionSetStoryCount)
			{
				playerinfo.shouldShowMission2Popup = true;
			}
			if (currentMissionSet + 1 > missionSetStoryCount)
			{
				playerinfo.AddMysteryBoxToUnlock(MysteryBox.Type.Super);
				if (!inRun)
				{
					UIScreenController.Instance.QueueMysteryBox();
				}
			}
			int num = ((currentMissionSet < missionSetCount - 1) ? (playerinfo.currentMissionSet + 1) : (playerinfo.currentMissionSet - _repeatableMissions.Length + 1));
			playerinfo.missionCompletedSum++;
			int missionCount = 0;
			if (num < missionSetCount)
			{
				missionCount = missions[num].Length;
			}
			playerinfo.InitCurrentMissionSet(num, missionCount);
			MissionSetCompleteHandler missionSetCompleteHandler = onMissionSetComplete;
			if (missionSetCompleteHandler != null)
			{
				missionSetCompleteHandler();
			}
			PlayerDidThis(MissionTarget.ReachMissionSet);
		}
		else if (!inRun || sendEvenIfInRun)
		{
			SocialManager.instance.ProgressThisAchievement(mission - 3, completedFactor * 100f);
		}
	}

	public void RemoveProgressForThis(MissionTarget myTask)
	{
		if (templates == null)
		{
			Debug.LogError("currentTemplates == null");
		}
		for (int i = 0; i < combinedArray.Length; i++)
		{
			if (templates[i].missionTarget == myTask && !GetMissionInfo(i).complete)
			{
				playerinfo.SetCurrentMissionProgress(i, 0);
				if (_currentRunProgress != null)
				{
					_currentRunProgress[i] = 0;
				}
			}
		}
	}

	private void CheckAllCompleteAndIncrement()
	{
		for (int i = 3; i < combinedArray.Length; i++)
		{
			if (templates[i].singleRun && templates[i].completeIfLess)
			{
				if (_currentRunProgress[i] < combinedArray[i].goal)
				{
					playerinfo.SetCurrentMissionProgress(i, combinedArray[i].goal);
					Complete(i, 1f, true);
				}
			}
			else if (templates[i].singleRun && _currentRunProgress != null)
			{
				Complete(i, (float)_currentRunProgress[i] / (float)combinedArray[i].goal, true);
			}
			else
			{
				Complete(i, (float)playerinfo.GetCurrentMissionProgress(i) / (float)combinedArray[i].goal, true);
			}
		}
	}

	private void OverWriteAchievementsProgress(MissionTarget myTask, int overWrite)
	{
		for (int i = 3; i < combinedArray.Length; i++)
		{
			if (templates[i].missionTarget == myTask)
			{
				playerinfo.SetCurrentMissionProgress(i, overWrite);
			}
		}
	}

	private void SyncAchievements()
	{
		OverWriteAchievementsProgress(MissionTarget.ReachMissionSet, playerinfo.currentMissionSet);
		OverWriteAchievementsProgress(MissionTarget.HaveUpgrades, playerinfo.GetUpgradeTierSum());
		OverWriteAchievementsProgress(MissionTarget.HaveHeadStartLarge, playerinfo.GetUpgradeAmount(PowerupType.headstart2000));
		OverWriteAchievementsProgress(MissionTarget.HaveTrophiesFirstAchievements, playerinfo.NumberOfUnlockedTrophyForFirstAchievement());
		OverWriteAchievementsProgress(MissionTarget.FacebookLoggedIn, SocialManager.instance.facebookIsLoggedIn.CompareTo(false));
		foreach (KeyValuePair<Characters.CharacterType, Characters.Model> characterDatum in Characters.characterData)
		{
			if (characterDatum.Value.missionTargetKey != 0)
			{
				OverWriteAchievementsProgress(characterDatum.Value.missionTargetKey, playerinfo.GetCollectedTokens(characterDatum.Key) / characterDatum.Value.Price);
			}
		}
	}

	public void PlayerDidThis(MissionTarget myTask, int magnitude = 1)
	{
		if (templates == null)
		{
			Debug.LogError("currentTemplates == null");
		}
		for (int i = 0; i < combinedArray.Length; i++)
		{
			if ((templates[i].singleRun && !inRun) || templates[i].missionTarget != myTask)
			{
				continue;
			}
			int num = playerinfo.GetCurrentMissionProgress(i);
			if (templates[i].singleRun && inRun && num < combinedArray[i].goal && _currentRunProgress != null)
			{
				num = _currentRunProgress[i];
			}
			int num2 = num + magnitude;
			if (templates[i].completeIfLess)
			{
				if (num2 > combinedArray[i].goal)
				{
					if (templates[i].singleRun && inRun)
					{
						if (_currentRunProgress != null)
						{
							_currentRunProgress[i] = num2;
						}
					}
					else
					{
						playerinfo.SetCurrentMissionProgress(i, num2);
						Complete(i, (float)num2 / (float)combinedArray[i].goal);
					}
					continue;
				}
				if (templates[i].singleRun && _currentRunProgress != null)
				{
					_currentRunProgress[i] = num2;
				}
				playerinfo.SetCurrentMissionProgress(i, combinedArray[i].goal);
				if (num > combinedArray[i].goal)
				{
					Complete(i);
				}
			}
			else if (num2 < combinedArray[i].goal)
			{
				if (templates[i].singleRun)
				{
					if (_currentRunProgress != null)
					{
						_currentRunProgress[i] = num2;
					}
				}
				else
				{
					playerinfo.SetCurrentMissionProgress(i, num2);
					Complete(i, (float)num2 / (float)combinedArray[i].goal);
				}
			}
			else
			{
				playerinfo.SetCurrentMissionProgress(i, combinedArray[i].goal);
				if (num < combinedArray[i].goal)
				{
					Complete(i);
				}
			}
		}
	}

	public MissionInfo[] GetMissionInfo()
	{
		MissionInfo[] array = new MissionInfo[combinedArray.Length];
		for (int i = 0; i < combinedArray.Length; i++)
		{
			GetMissionInfo(i, ref array[i]);
		}
		return array;
	}

	public MissionInfo GetMissionInfo(int missionNumber)
	{
		MissionInfo info = default(MissionInfo);
		GetMissionInfo(missionNumber, ref info);
		return info;
	}

	public void GetMissionInfo(int missionNumber, ref MissionInfo info)
	{
		info.mission = combinedArray[missionNumber];
		info.template = templates[missionNumber];
		info.progress = playerinfo.GetCurrentMissionProgress(missionNumber);
		info.complete = info.progress >= info.mission.goal;
		if (!info.complete && info.template.singleRun && inRun)
		{
			info.progress = _currentRunProgress[missionNumber];
		}
	}

	public int GetCurrentRank()
	{
		return playerinfo.GetCurrentRank();
	}

	public int GetNumberOfBasicMission()
	{
		return _storylineMissions.Length;
	}

	public Dictionary<MissionType, MissionTemplate> GetMissionTemplates()
	{
		return missionTemplates;
	}
}
