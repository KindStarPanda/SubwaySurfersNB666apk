using System.Collections.Generic;
using UnityEngine;

public class RandomizerHold : Selector
{
	private static int startIndex = 0;

	private static int[] randomIndices = new int[21]
	{
		0, 1, 2, 3, 0, 4, 5, 1, 0, 2,
		4, 1, 3, 2, 0, 5, 1, 0, 3, 1,
		3
	};

	private static float holdDistance = 3000f;

	[SerializeField]
	private GameObject[] children;

	public static void Initialize()
	{
		startIndex = Random.Range(0, randomIndices.Length);
	}

	public override void PerformSelection(List<GameObject> objectsToVisit)
	{
		int num = Mathf.FloorToInt(base.transform.position.z / holdDistance) + startIndex;
		int num2 = randomIndices[(startIndex + num) % randomIndices.Length];
		for (int i = 0; i < children.Length; i++)
		{
			GameObject gameObject = children[i];
			if (i == num2)
			{
				objectsToVisit.Add(gameObject);
			}
			else
			{
				gameObject.SetActiveRecursively(false);
			}
		}
	}
}
