using UnityEngine;

public struct Upgrade
{
	public string name;

	public string description;

	public string mysteryBoxDescription;

	public int numberOfTiers;

	public float[] durations;

	public float spawnProbability;

	public int minimumMeters;

	public int coinmagnetRange;

	public int[] pricesRaw;

	public int levelPriceMultiplyer;

	public string iconName;

	public int getPrice(int tier)
	{
		if (pricesRaw == null)
		{
			Debug.LogWarning("Prices is not initialized");
			return -1;
		}
		return pricesRaw[tier] + levelPriceMultiplyer * Mathf.Clamp(Missions.Instance.currentMissionSet, 0, Missions.Instance.missionSetStoryCount);
	}
}
