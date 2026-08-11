using System.Collections.Generic;
using UnityEngine;

public class TrophyMedal : MonoBehaviour
{
	[SerializeField]
	private MeshFilter medal;

	[SerializeField]
	private Mesh[] medalMeshes;

	[SerializeField]
	private Mesh[] starMeshes;

	[SerializeField]
	private GameObject[] starConstellationRoots;

	private List<MeshFilter[]> starConstellations;

	public void Awake()
	{
		starConstellations = new List<MeshFilter[]>();
		GameObject[] array = starConstellationRoots;
		foreach (GameObject gameObject in array)
		{
			starConstellations.Add(gameObject.GetComponentsInChildren<MeshFilter>());
		}
	}

	public void Setup(int numberOfStars, MetalType medalMetalType, MetalType starMetalType)
	{
		medal.mesh = medalMeshes[(int)medalMetalType];
		for (int i = 0; i < starConstellationRoots.Length; i++)
		{
			GameObject gameObject = starConstellationRoots[i];
			bool flag = i + 1 == numberOfStars;
			gameObject.SetActiveRecursively(flag);
			if (flag)
			{
				MeshFilter[] array = starConstellations[i];
				MeshFilter[] array2 = array;
				foreach (MeshFilter meshFilter in array2)
				{
					meshFilter.mesh = starMeshes[(int)starMetalType];
				}
			}
		}
	}
}
