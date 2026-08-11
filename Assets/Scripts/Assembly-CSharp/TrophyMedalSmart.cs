using System.Collections.Generic;
using UnityEngine;

public class TrophyMedalSmart : MonoBehaviour
{
	private const int MAX_NUMBERS_OF_STARS = 6;

	[SerializeField]
	private Mesh[] medalMeshes;

	[SerializeField]
	private Mesh[] starMeshes;

	[SerializeField]
	private GameObject starPrefab;

	private List<GameObject> stars = new List<GameObject>();

	private int starCount;

	private MetalType metal;

	private readonly Vector3 Star1 = new Vector3(0f, 1.137436f, -0.310811f);

	private readonly Vector3[] Star2 = new Vector3[2]
	{
		new Vector3(0f, 2.227913f, -0.3108109f),
		new Vector3(0f, 0.1491928f, -0.310811f)
	};

	private readonly Vector3[] Star3 = new Vector3[3]
	{
		new Vector3(0f, 2.464571f, -0.3108109f),
		new Vector3(1.103336f, 0.5071144f, -0.310811f),
		new Vector3(-1.103336f, 0.5071144f, -0.310811f)
	};

	private readonly Vector3[] Star4 = new Vector3[4]
	{
		new Vector3(0f, 2.6f, -0.3108109f),
		new Vector3(0f, -0.1717745f, -0.310811f),
		new Vector3(-1.4f, 1.246856f, -0.310811f),
		new Vector3(1.4f, 1.246856f, -0.310811f)
	};

	private readonly Vector3[] Star5 = new Vector3[5]
	{
		new Vector3(0.85f, 0.05f, -0.310811f),
		new Vector3(1.35f, 1.7f, -0.310811f),
		new Vector3(0f, 2.7f, -0.310811f),
		new Vector3(-1.35f, 1.7f, -0.310811f),
		new Vector3(-0.85f, 0.05f, -0.310811f)
	};

	private readonly Vector3[] Star6 = new Vector3[6]
	{
		new Vector3(0.85f, 0.05f, -0.310811f),
		new Vector3(1.35f, 1.7f, -0.310811f),
		new Vector3(0f, 2.7f, -0.310811f),
		new Vector3(-1.35f, 1.7f, -0.310811f),
		new Vector3(-0.85f, 0.05f, -0.310811f),
		new Vector3(0f, 1.137436f, -0.310811f)
	};

	private void OnEnable()
	{
		int num = Mathf.Clamp(-1, -1, 17);
		if (num < 0)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		starCount = 1 + num % 6;
		metal = (MetalType)(num / 6);
		for (int i = 0; i < starCount; i++)
		{
			stars.Add(NGUITools.AddChild(base.gameObject, starPrefab));
		}
		if (starCount == 1)
		{
			stars[0].transform.localPosition = Star1;
		}
		else if (starCount == 2)
		{
			for (int j = 0; j < starCount; j++)
			{
				stars[j].transform.localPosition = Star2[j];
			}
		}
		else if (starCount == 3)
		{
			for (int k = 0; k < starCount; k++)
			{
				stars[k].transform.localPosition = Star3[k];
			}
		}
		else if (starCount == 4)
		{
			for (int l = 0; l < starCount; l++)
			{
				stars[l].transform.localPosition = Star4[l];
			}
		}
		else if (starCount == 5)
		{
			for (int m = 0; m < starCount; m++)
			{
				stars[m].transform.localPosition = Star5[m];
			}
		}
		else if (starCount == 6)
		{
			for (int n = 0; n < starCount; n++)
			{
				stars[n].transform.localPosition = Star6[n];
			}
		}
		for (int num2 = 0; num2 < starCount; num2++)
		{
			stars[num2].transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
			stars[num2].GetComponent<MeshFilter>().mesh = starMeshes[(int)metal];
		}
		base.gameObject.GetComponent<MeshFilter>().mesh = medalMeshes[(int)metal];
	}

	private void OnDisable()
	{
		foreach (GameObject star in stars)
		{
			if (star != null)
			{
				Object.Destroy(star);
			}
		}
		stars.Clear();
	}
}
