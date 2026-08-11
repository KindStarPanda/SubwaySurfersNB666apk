using System.Collections.Generic;
using UnityEngine;

public class MysteryBoxDropTester : MonoBehaviour
{
	public MysteryBox.Type type;

	private Dictionary<MysteryBoxRewardType, int> rewardTypeCounts = new Dictionary<MysteryBoxRewardType, int>();

	private Dictionary<PowerupType, int> powerupTypeCounts = new Dictionary<PowerupType, int>();

	private int rollCount;

	private int coinTotal;

	private int coinMin = int.MaxValue;

	private int coinMax = int.MinValue;
}
