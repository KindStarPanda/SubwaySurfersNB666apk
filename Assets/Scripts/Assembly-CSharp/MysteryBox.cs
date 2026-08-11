using System;
using System.Collections.Generic;
using UnityEngine;

public static class MysteryBox
{
	public enum Type
	{
		Normal = 0,
		Super = 1
	}

	private class LootTableEntry
	{
		public MysteryBoxRewardType itemType;

		public float weight;

		public int min = 1;

		public int max = 1;

		public PowerupType powerupType;
	}

	public static readonly Characters.CharacterType[] tokenRewardTypes;

	private static readonly LootTableEntry[][] _lootTables;

	private static readonly MysteryBoxReward TOKEN_CONSOLATION_REWARD;

	private static readonly MysteryBoxReward TROPHY_CONSOLATION_REWARD;

	static MysteryBox()
	{
		tokenRewardTypes = new Characters.CharacterType[4]
		{
			Characters.CharacterType.fresh,
			Characters.CharacterType.spike,
			Characters.CharacterType.tricky,
			Characters.CharacterType.yutani
		};
		TOKEN_CONSOLATION_REWARD = new MysteryBoxReward
		{
			type = MysteryBoxRewardType.coins,
			amount = 500
		};
		TROPHY_CONSOLATION_REWARD = new MysteryBoxReward
		{
			type = MysteryBoxRewardType.coins,
			amount = 500
		};
		Type[] array = (Type[])Enum.GetValues(typeof(Type));
		_lootTables = new LootTableEntry[array.Length][];
		_lootTables[0] = new LootTableEntry[15]
		{
			new LootTableEntry
			{
				itemType = MysteryBoxRewardType.token,
				weight = 40f,
				min = 1,
				max = 1
			},
			new LootTableEntry
			{
				itemType = MysteryBoxRewardType.trophy,
				weight = 2f,
				min = 1,
				max = 1
			},
			new LootTableEntry
			{
				itemType = MysteryBoxRewardType.coins,
				weight = 18f,
				min = 200,
				max = 300
			},
			new LootTableEntry
			{
				itemType = MysteryBoxRewardType.powerup,
				weight = 10.15f,
				min = 1,
				max = 1,
				powerupType = PowerupType.headstart500
			},
			new LootTableEntry
			{
				itemType = MysteryBoxRewardType.coins,
				weight = 11f,
				min = 500,
				max = 500
			},
			new LootTableEntry
			{
				itemType = MysteryBoxRewardType.powerup,
				weight = 7f,
				min = 2,
				max = 2,
				powerupType = PowerupType.hoverboard
			},
			new LootTableEntry
			{
				itemType = MysteryBoxRewardType.coins,
				weight = 3f,
				min = 1000,
				max = 1000
			},
			new LootTableEntry
			{
				itemType = MysteryBoxRewardType.coins,
				weight = 2f,
				min = 1500,
				max = 1500
			},
			new LootTableEntry
			{
				itemType = MysteryBoxRewardType.powerup,
				weight = 1f,
				min = 3,
				max = 9,
				powerupType = PowerupType.hoverboard
			},
			new LootTableEntry
			{
				itemType = MysteryBoxRewardType.powerup,
				weight = 3f,
				min = 1,
				max = 1,
				powerupType = PowerupType.headstart2000
			},
			new LootTableEntry
			{
				itemType = MysteryBoxRewardType.coins,
				weight = 1f,
				min = 2500,
				max = 2500
			},
			new LootTableEntry
			{
				itemType = MysteryBoxRewardType.powerup,
				weight = 0.8f,
				min = 10,
				max = 10,
				powerupType = PowerupType.hoverboard
			},
			new LootTableEntry
			{
				itemType = MysteryBoxRewardType.coins,
				weight = 0.5f,
				min = 5000,
				max = 5000
			},
			new LootTableEntry
			{
				itemType = MysteryBoxRewardType.powerup,
				weight = 0.5f,
				min = 3,
				max = 3,
				powerupType = PowerupType.headstart2000
			},
			new LootTableEntry
			{
				itemType = MysteryBoxRewardType.coins,
				weight = 0.05f,
				min = 100000,
				max = 100000
			}
		};
		_lootTables[1] = new LootTableEntry[15]
		{
			new LootTableEntry
			{
				itemType = MysteryBoxRewardType.token,
				weight = 25f,
				min = 3,
				max = 3
			},
			new LootTableEntry
			{
				itemType = MysteryBoxRewardType.powerup,
				weight = 12f,
				min = 3,
				max = 3,
				powerupType = PowerupType.headstart500
			},
			new LootTableEntry
			{
				itemType = MysteryBoxRewardType.coins,
				weight = 25f,
				min = 1500,
				max = 1500
			},
			new LootTableEntry
			{
				itemType = MysteryBoxRewardType.powerup,
				weight = 10f,
				min = 4,
				max = 8,
				powerupType = PowerupType.hoverboard
			},
			new LootTableEntry
			{
				itemType = MysteryBoxRewardType.coins,
				weight = 10f,
				min = 3000,
				max = 3000
			},
			new LootTableEntry
			{
				itemType = MysteryBoxRewardType.powerup,
				weight = 3f,
				min = 2,
				max = 2,
				powerupType = PowerupType.headstart2000
			},
			new LootTableEntry
			{
				itemType = MysteryBoxRewardType.coins,
				weight = 8f,
				min = 4000,
				max = 4000
			},
			new LootTableEntry
			{
				itemType = MysteryBoxRewardType.powerup,
				weight = 1f,
				min = 9,
				max = 19,
				powerupType = PowerupType.hoverboard
			},
			new LootTableEntry
			{
				itemType = MysteryBoxRewardType.powerup,
				weight = 2f,
				min = 3,
				max = 3,
				powerupType = PowerupType.headstart2000
			},
			new LootTableEntry
			{
				itemType = MysteryBoxRewardType.powerup,
				weight = 1f,
				min = 20,
				max = 20,
				powerupType = PowerupType.hoverboard
			},
			new LootTableEntry
			{
				itemType = MysteryBoxRewardType.coins,
				weight = 1f,
				min = 8000,
				max = 8000
			},
			new LootTableEntry
			{
				itemType = MysteryBoxRewardType.powerup,
				weight = 1f,
				min = 5,
				max = 5,
				powerupType = PowerupType.headstart2000
			},
			new LootTableEntry
			{
				itemType = MysteryBoxRewardType.powerup,
				weight = 0.5f,
				min = 40,
				max = 40,
				powerupType = PowerupType.hoverboard
			},
			new LootTableEntry
			{
				itemType = MysteryBoxRewardType.coins,
				weight = 0.4f,
				min = 50000,
				max = 50000
			},
			new LootTableEntry
			{
				itemType = MysteryBoxRewardType.coins,
				weight = 0.1f,
				min = 300000,
				max = 300000
			}
		};
	}

	public static void TestRolls(int numberOfRolls, Type mysteryBoxType = Type.Normal)
	{
		for (int i = 0; i < numberOfRolls; i++)
		{
			MysteryBoxReward mysteryBoxReward = Roll(mysteryBoxType);
			Debug.Log("Type: " + mysteryBoxReward.type.ToString() + " Amount: " + mysteryBoxReward.amount);
		}
	}

	public static MysteryBoxReward Roll(Type mysteryBoxType)
	{
		MysteryBoxReward mysteryBoxReward = new MysteryBoxReward();
		if (mysteryBoxType == Type.Normal)
		{
			PlayerInfo.Instance.amountOfMysteryBoxesOpened++;
			if (PlayerInfo.Instance.amountOfMysteryBoxesOpened == 1)
			{
				mysteryBoxReward.type = MysteryBoxRewardType.coins;
				mysteryBoxReward.amount = 1000;
				return mysteryBoxReward;
			}
			if (PlayerInfo.Instance.amountOfMysteryBoxesOpened == 2)
			{
				mysteryBoxReward.type = MysteryBoxRewardType.token;
				mysteryBoxReward.amount = 1;
				mysteryBoxReward.characterType = Characters.CharacterType.tricky;
				return mysteryBoxReward;
			}
			if (PlayerInfo.Instance.amountOfMysteryBoxesOpened == 3)
			{
				mysteryBoxReward.type = MysteryBoxRewardType.powerup;
				mysteryBoxReward.amount = 3;
				mysteryBoxReward.powerupType = PowerupType.hoverboard;
				return mysteryBoxReward;
			}
		}
		else
		{
			PlayerInfo.Instance.amountOfSuperMysteryBoxesOpened++;
		}
		LootTableEntry lootTableEntry = MakeRoll(mysteryBoxType);
		if (lootTableEntry == null)
		{
			Debug.LogError("A reward was empty, how did this happen?");
		}
		if (lootTableEntry.itemType == MysteryBoxRewardType.coins)
		{
			mysteryBoxReward.type = MysteryBoxRewardType.coins;
			mysteryBoxReward.amount = UnityEngine.Random.Range(lootTableEntry.min, lootTableEntry.max);
		}
		else if (lootTableEntry.itemType == MysteryBoxRewardType.powerup)
		{
			mysteryBoxReward.type = MysteryBoxRewardType.powerup;
			mysteryBoxReward.powerupType = lootTableEntry.powerupType;
			mysteryBoxReward.amount = UnityEngine.Random.Range(lootTableEntry.min, lootTableEntry.max);
		}
		else if (lootTableEntry.itemType == MysteryBoxRewardType.token)
		{
			Characters.CharacterType[] array = new Characters.CharacterType[tokenRewardTypes.Length];
			int num = 0;
			Characters.CharacterType[] array2 = tokenRewardTypes;
			foreach (Characters.CharacterType characterType in array2)
			{
				if (PlayerInfo.Instance.IsTokenUseful(characterType))
				{
					array[num] = characterType;
					num++;
				}
			}
			if (num > 0)
			{
				mysteryBoxReward.type = MysteryBoxRewardType.token;
				mysteryBoxReward.amount = UnityEngine.Random.Range(lootTableEntry.min, lootTableEntry.max);
				mysteryBoxReward.characterType = array[UnityEngine.Random.Range(0, num)];
			}
			else
			{
				mysteryBoxReward = TOKEN_CONSOLATION_REWARD;
			}
		}
		else if (lootTableEntry.itemType == MysteryBoxRewardType.trophy)
		{
			Trophies.Trophy[] array3 = Enum.GetValues(typeof(Trophies.Trophy)) as Trophies.Trophy[];
			List<Trophies.Trophy> list = new List<Trophies.Trophy>();
			Trophies.Trophy[] array4 = array3;
			foreach (Trophies.Trophy trophy in array4)
			{
				TrophyData value;
				if (!PlayerInfo.Instance.isTrophyUnlocked(trophy) && Trophies.trophyData.TryGetValue(trophy, out value) && value.obtainSource == Trophies.TrophyObtainSource.Mysterybox)
				{
					list.Add(trophy);
				}
			}
			if (list.Count > 0)
			{
				mysteryBoxReward.type = MysteryBoxRewardType.trophy;
				mysteryBoxReward.amount = UnityEngine.Random.Range(lootTableEntry.min, lootTableEntry.max);
				mysteryBoxReward.trophyType = list[UnityEngine.Random.Range(0, list.Count)];
			}
			else
			{
				mysteryBoxReward = TROPHY_CONSOLATION_REWARD;
			}
		}
		else if (lootTableEntry.itemType == MysteryBoxRewardType.medal)
		{
			mysteryBoxReward.type = MysteryBoxRewardType.medal;
			mysteryBoxReward.amount = 1;
		}
		return mysteryBoxReward;
	}

	private static LootTableEntry MakeRoll(Type mysteryBoxType)
	{
		float num = 0f;
		for (int i = 0; i < _lootTables[(int)mysteryBoxType].Length; i++)
		{
			LootTableEntry lootTableEntry = _lootTables[(int)mysteryBoxType][i];
			num += lootTableEntry.weight;
		}
		float num2 = UnityEngine.Random.Range(0f, 1f) * num;
		float num3 = 0f;
		LootTableEntry result = null;
		for (int j = 0; j < _lootTables[(int)mysteryBoxType].Length; j++)
		{
			LootTableEntry lootTableEntry2 = _lootTables[(int)mysteryBoxType][j];
			num3 += lootTableEntry2.weight;
			if (num2 <= num3)
			{
				result = lootTableEntry2;
				break;
			}
		}
		return result;
	}
}
