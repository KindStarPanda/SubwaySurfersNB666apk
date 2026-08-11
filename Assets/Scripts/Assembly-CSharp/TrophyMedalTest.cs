using System.Collections;
using UnityEngine;

public class TrophyMedalTest : MonoBehaviour
{
	private TrophyMedal trophyMedal;

	public void Start()
	{
		StartCoroutine(Test());
	}

	public IEnumerator Test()
	{
		trophyMedal = GetComponent<TrophyMedal>();
		while (true)
		{
			MetalType[] array = new MetalType[3]
			{
				MetalType.BRONZE,
				MetalType.SILVER,
				MetalType.GOLD
			};
			foreach (MetalType medalMetalType in array)
			{
				for (int i = 1; i <= 5; i++)
				{
					trophyMedal.Setup(i, medalMetalType, medalMetalType);
					yield return new WaitForSeconds(0.3f);
				}
			}
		}
	}
}
