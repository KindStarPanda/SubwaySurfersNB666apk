using System.Collections.Generic;

public class Upgrades
{
	public const float SPAWNRATE_FOR_LETTERS = 1.5E+09f;

	public const float UPGRADE_FIRST_SPAWN_METERS = 250f;

	public const float UPGRADE_SPAWN_SPACING_METERS = 300f;

	public static readonly Dictionary<PowerupType, Upgrade> upgrades = new Dictionary<PowerupType, Upgrade>
	{
		{
			PowerupType.hoverboard,
			new Upgrade
			{
				name = "Hoverboard",
				description = "Protect yourself from crashing for 30 seconds. Activate by double tapping.",
				mysteryBoxDescription = "Double tap while running to activate crash protection",
				durations = new float[1] { 30f },
				pricesRaw = new int[1] { 300 },
				iconName = "icon_upgrades_hoverboard"
			}
		},
		{
			PowerupType.headstart500,
			new Upgrade
			{
				name = "Headstart",
				durations = new float[1] { 250f },
				description = "Skip ahead 250 meters in a run.",
				mysteryBoxDescription = "Activate at the beginning of a game to get a headstart",
				pricesRaw = new int[1] { 400 },
				iconName = "icon_upgrades_headstart500"
			}
		},
		{
			PowerupType.headstart2000,
			new Upgrade
			{
				name = "Mega Headstart",
				mysteryBoxDescription = "Activate at the beginning of a game to get a mega headstart",
				durations = new float[1] { 1000f },
				description = "Skip ahead 1000 meters in a run.",
				pricesRaw = new int[1] { 2000 },
				iconName = "icon_upgrades_headstart2000"
			}
		},
		{
			PowerupType.mysterybox,
			new Upgrade
			{
				name = "Mystery Box",
				description = string.Empty,
				spawnProbability = 20f,
				minimumMeters = 99999,
				pricesRaw = new int[1] { 500 },
				iconName = "icon_upgrades_mysteryBox"
			}
		},
		{
			PowerupType.jetpack,
			new Upgrade
			{
				name = "Jetpack",
				description = "Increases the duration of the Spray Can Jetpack powerup.",
				numberOfTiers = 6,
				durations = new float[6] { 8f, 9f, 10.5f, 12.5f, 15f, 19f },
				spawnProbability = 14f,
				minimumMeters = 1000,
				pricesRaw = new int[6] { 0, 500, 1500, 3000, 10000, 30000 },
				iconName = "icon_upgrades_jetpack"
			}
		},
		{
			PowerupType.supersneakers,
			new Upgrade
			{
				name = "Super Sneakers",
				description = "Increases the duration of the Super Sneakers powerup.",
				numberOfTiers = 6,
				spawnProbability = 22f,
				durations = new float[6] { 10f, 11.5f, 13.4f, 15.8f, 19f, 24f },
				pricesRaw = new int[6] { 0, 500, 1500, 3000, 10000, 30000 },
				iconName = "icon_upgrades_superSneakers"
			}
		},
		{
			PowerupType.coinmagnet,
			new Upgrade
			{
				name = "Coin Magnet",
				description = "Increases the duration of the Coin Magnet powerup.",
				numberOfTiers = 6,
				durations = new float[6] { 10f, 11.5f, 13.4f, 15.8f, 19f, 24f },
				spawnProbability = 32f,
				coinmagnetRange = 2,
				pricesRaw = new int[6] { 0, 500, 1500, 3000, 10000, 30000 },
				iconName = "icon_upgrades_coinMagnet"
			}
		},
		{
			PowerupType.doubleMultiplier,
			new Upgrade
			{
				name = "2x Multiplier",
				description = "Increases the duration of the Double Multiplier powerup.",
				numberOfTiers = 6,
				durations = new float[6] { 10f, 11.5f, 13.4f, 15.8f, 19f, 24f },
				spawnProbability = 32f,
				pricesRaw = new int[6] { 0, 500, 1500, 3000, 10000, 30000 },
				iconName = "icon_upgrades_doubleScore"
			}
		},
		{
			PowerupType.skipmission1,
			new Upgrade
			{
				name = "Skip Mission 1",
				description = "Skip your current mission 1",
				pricesRaw = new int[1] { 1500 },
				iconName = "icon_upgrades_skipMission1",
				levelPriceMultiplyer = 100
			}
		},
		{
			PowerupType.skipmission2,
			new Upgrade
			{
				name = "Skip Mission 2",
				description = "Skip your current mission 2",
				pricesRaw = new int[1] { 1500 },
				iconName = "icon_upgrades_skipMission2",
				levelPriceMultiplyer = 100
			}
		},
		{
			PowerupType.skipmission3,
			new Upgrade
			{
				name = "Skip Mission 3",
				description = "Skip your current mission 3",
				pricesRaw = new int[1] { 1500 },
				iconName = "icon_upgrades_skipMission3",
				levelPriceMultiplyer = 100
			}
		},
		{
			PowerupType.letters,
			new Upgrade
			{
				spawnProbability = 1.5E+09f
			}
		}
	};
}
