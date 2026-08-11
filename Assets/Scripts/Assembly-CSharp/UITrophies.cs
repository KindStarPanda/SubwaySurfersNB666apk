using UnityEngine;

public class UITrophies : MonoBehaviour
{
	[SerializeField]
	private GameObject trophyMedalSmartPrefab;

	private GameObject trophyMedalSmart;

	private void OnEnable()
	{
		if (!(trophyMedalSmartPrefab == null))
		{
			if (trophyMedalSmart == null)
			{
				trophyMedalSmart = NGUITools.AddChild(UIScreenController.Instance.MenuElements3D, trophyMedalSmartPrefab);
				trophyMedalSmart.transform.localScale = new Vector3(10f, 10f, 1f);
				trophyMedalSmart.transform.localPosition = new Vector3(144f, 144f, 0f);
			}
			if (!trophyMedalSmart.active)
			{
				trophyMedalSmart.SetActiveRecursively(true);
			}
		}
	}

	private void OnDisable()
	{
		if (trophyMedalSmart != null)
		{
			Object.Destroy(trophyMedalSmart);
		}
	}
}
