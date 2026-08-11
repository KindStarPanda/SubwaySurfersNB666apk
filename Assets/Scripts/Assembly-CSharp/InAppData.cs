using System.Collections.Generic;

public class InAppData
{
	public const string inAppTier1 = "com.kiloo.subways.coinstier1";

	public const string inAppTier2 = "com.kiloo.subways.coinstier2";

	public const string inAppTier3 = "com.kiloo.subways.coinstier3";

	public const string inAppTier4 = "com.kiloo.subways.coinstier4";

	public const string inAppTier5 = "com.kiloo.subways.coinstier5";

	public const string inAppDoubleCoins = "com.kiloo.subways.doublecoins";

	public const string inAppDoubleCoinsDiscount = "com.kiloo.subways.doublecoinsdiscount";

	public const string inAppTier1Discount = "com.kiloo.subways.coinstier1discount";

	public const string inAppTier2Discount = "com.kiloo.subways.coinstier2_discount";

	public const string inAppTier3Discount = "com.kiloo.subways.coinstier3discount";

	public const string inAppTier4Discount = "com.kiloo.subways.coinstier4discount";

	public const string inAppTier5Discount = "com.kiloo.subways.coinstier5discount";

	public static readonly string[] inAppTiersAndInAppTiersDiscount = new string[10] { "com.kiloo.subways.coinstier1", "com.kiloo.subways.coinstier2", "com.kiloo.subways.coinstier3", "com.kiloo.subways.coinstier4", "com.kiloo.subways.coinstier5", "com.kiloo.subways.coinstier1discount", "com.kiloo.subways.coinstier2_discount", "com.kiloo.subways.coinstier3discount", "com.kiloo.subways.coinstier4discount", "com.kiloo.subways.coinstier5discount" };

	public static readonly List<string> LIST_OF_MANAGED_INAPP_IDS = new List<string> { "com.kiloo.subways.doublecoins", "com.kiloo.subways.doublecoinsdiscount" };

	public static Dictionary<string, InAppProfile> inAppData = new Dictionary<string, InAppProfile>
	{
		{
			"com.kiloo.subways.coinstier1",
			new InAppProfile
			{
				amountOfCoins = 7500,
				title = "Stack of Coins",
				iconName = "icon_coinPack_1"
			}
		},
		{
			"com.kiloo.subways.coinstier2",
			new InAppProfile
			{
				amountOfCoins = 45000,
				title = "Pile of Coins",
				iconName = "icon_coinPack_2"
			}
		},
		{
			"com.kiloo.subways.coinstier3",
			new InAppProfile
			{
				amountOfCoins = 180000,
				title = "Bag of Coins",
				iconName = "icon_coinPack_3"
			}
		},
		{
			"com.kiloo.subways.coinstier4",
			new InAppProfile
			{
				amountOfCoins = 500000,
				title = "Chest of Coins",
				iconName = "icon_coinPack_4"
			}
		},
		{
			"com.kiloo.subways.coinstier5",
			new InAppProfile
			{
				amountOfCoins = 1200000,
				title = "Vault of Coins",
				iconName = "icon_coinPack_5"
			}
		},
		{
			"com.kiloo.subways.coinstier1discount",
			new InAppProfile
			{
				amountOfCoins = 7500,
				title = "Stack of Coins",
				iconName = "icon_coinPack_1"
			}
		},
		{
			"com.kiloo.subways.coinstier2_discount",
			new InAppProfile
			{
				amountOfCoins = 45000,
				title = "Pile of Coins",
				iconName = "icon_coinPack_2"
			}
		},
		{
			"com.kiloo.subways.coinstier3discount",
			new InAppProfile
			{
				amountOfCoins = 180000,
				title = "Bag of Coins",
				iconName = "icon_coinPack_3"
			}
		},
		{
			"com.kiloo.subways.coinstier4discount",
			new InAppProfile
			{
				amountOfCoins = 500000,
				title = "Chest of Coins",
				iconName = "icon_coinPack_4"
			}
		},
		{
			"com.kiloo.subways.coinstier5discount",
			new InAppProfile
			{
				amountOfCoins = 1200000,
				title = "Vault of Coins",
				iconName = "icon_coinPack_5"
			}
		},
		{
			"com.kiloo.subways.doublecoins",
			new InAppProfile
			{
				title = "Double Coins",
				description = "Doubles all coin pickups",
				iconName = "icon_doubleCoins"
			}
		},
		{
			"com.kiloo.subways.doublecoinsdiscount",
			new InAppProfile
			{
				title = "Double Coins",
				description = "Doubles all coin pickups",
				iconName = "icon_doubleCoins"
			}
		}
	};

	public int InAppPurchaseCount
	{
		get
		{
			return inAppData.Count;
		}
	}

	public string CommaSeparatedProductIds
	{
		get
		{
			string text = string.Empty;
			int num = 0;
			foreach (KeyValuePair<string, InAppProfile> inAppDatum in inAppData)
			{
				if (num > 0)
				{
					text += ",";
				}
				text += inAppDatum.Key;
				num++;
			}
			return text;
		}
	}
}
