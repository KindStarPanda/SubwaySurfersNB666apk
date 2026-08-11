using System;
using UnityEngine;

public class TrophiesScreen : UIScreen
{
	[SerializeField]
	private GameObject gridGo;

	[SerializeField]
	private GameObject trophyPrefab;

	public override void Init()
	{
		base.Init();
		loadTrophies();
	}

	private void loadTrophies()
	{
		foreach (Transform item in gridGo.transform)
		{
			NGUITools.SetActive(item.gameObject, false);
			UnityEngine.Object.Destroy(item.gameObject);
		}
		Trophies.Trophy[] array = Enum.GetValues(typeof(Trophies.Trophy)) as Trophies.Trophy[];
		for (int i = 0; i < array.Length; i++)
		{
			GameObject gameObject = NGUITools.AddChild(gridGo, trophyPrefab);
			gameObject.name = string.Format("{0:000}trophy", i);
			TrophyHelper component = gameObject.GetComponent<TrophyHelper>();
			component.setTrophy(array[i]);
		}
	}
}
