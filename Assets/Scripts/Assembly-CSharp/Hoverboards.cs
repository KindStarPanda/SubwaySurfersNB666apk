using System.Collections.Generic;

public class Hoverboards
{
	public enum BoardType
	{
		normal = 0,
		bouncer = 1,
		lowrider = 2,
		snowboard = 3,
		surfboard = 4,
		theoriginal = 5,
		starboard = 6
	}

	public enum UnlockType
	{
		alwaysUnlocked = 0,
		free = 1,
		coins = 2
	}

	public struct Board
	{
		public string name;

		public string boardModelName;

		public int price;

		public UnlockType unlockType;

		public string description;

		public string seasonLimitedDescription;

		public bool isNewInThisUpdate;

		public PlayerInfo.Season season;
	}

	public static readonly Dictionary<BoardType, Board> boardData = new Dictionary<BoardType, Board>
	{
		{
			BoardType.normal,
			new Board
			{
				name = "Hoverboard",
				boardModelName = "Hoverboard",
				description = string.Empty,
				seasonLimitedDescription = string.Empty
			}
		},
		{
			BoardType.bouncer,
			new Board
			{
				name = "Bouncer",
				boardModelName = "Jumpboard",
				price = 280000,
				unlockType = UnlockType.coins,
				description = "Super Jump",
				seasonLimitedDescription = string.Empty,
				isNewInThisUpdate = true
			}
		},
		{
			BoardType.lowrider,
			new Board
			{
				name = "Lowrider",
				boardModelName = "Lowrider",
				price = 320000,
				unlockType = UnlockType.coins,
				description = "Stays Low",
				seasonLimitedDescription = string.Empty,
				isNewInThisUpdate = true
			}
		},
		{
			BoardType.snowboard,
			new Board
			{
				name = "Freestyler",
				boardModelName = "Snowboard",
				price = 45000,
				unlockType = UnlockType.coins,
				description = string.Empty,
				seasonLimitedDescription = string.Empty,
				isNewInThisUpdate = true
			}
		},
		{
			BoardType.surfboard,
			new Board
			{
				name = "Big Kahuna",
				boardModelName = "Surfboard",
				price = 65000,
				unlockType = UnlockType.coins,
				description = string.Empty,
				seasonLimitedDescription = string.Empty,
				isNewInThisUpdate = true
			}
		},
		{
			BoardType.theoriginal,
			new Board
			{
				name = "Superhero",
				boardModelName = "Bamboard",
				price = 8000,
				unlockType = UnlockType.coins,
				description = string.Empty,
				seasonLimitedDescription = string.Empty,
				isNewInThisUpdate = true
			}
		},
		{
			BoardType.starboard,
			new Board
			{
				name = "Starboard",
				boardModelName = "Starboard",
				unlockType = UnlockType.free,
				description = string.Empty,
				seasonLimitedDescription = string.Empty,
				isNewInThisUpdate = true
			}
		}
	};

	public static List<BoardType> boardOrder = new List<BoardType>
	{
		BoardType.snowboard,
		BoardType.bouncer,
		BoardType.normal,
		BoardType.lowrider,
		BoardType.starboard,
		BoardType.surfboard,
		BoardType.theoriginal
	};
}
