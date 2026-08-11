using System.Collections.Generic;
using UnityEngine;

public class ThemeShiftMeshes : MonoBehaviour
{
	private HashSet<MeshFilter> meshFilters = new HashSet<MeshFilter>();

	private void Awake()
	{
		MeshFilter[] componentsInChildren = base.transform.GetComponentsInChildren<MeshFilter>(true);
		MeshFilter[] array = componentsInChildren;
		foreach (MeshFilter item in array)
		{
			meshFilters.Add(item);
		}
	}

	private void Start()
	{
		ThemeManager.Instance.OnChangeTheme += OnChangeTheme;
	}

	private void OnDestroy()
	{
		ThemeManager.Instance.OnChangeTheme -= OnChangeTheme;
	}

	public void OnChangeTheme(Theme theme)
	{
		if (theme == null)
		{
			return;
		}
		string text = ThemeManager.Instance.Theme.Name;
		if (meshFilters != null)
		{
			foreach (MeshFilter meshFilter in meshFilters)
			{
				if (meshFilter != null && meshFilter.sharedMesh.name != string.Empty)
				{
					string text2 = meshFilter.sharedMesh.name;
					string text3 = text2.Substring(text2.IndexOf("_") + 1);
					string key = text + "_" + text3;
					Mesh value;
					if (ThemeAssets.Instance.environmentModelMeshes.TryGetValue(key, out value))
					{
						meshFilter.mesh = value;
					}
				}
			}
		}
		SkinnedMeshRenderer[] componentsInChildren = base.transform.GetComponentsInChildren<SkinnedMeshRenderer>(true);
		SkinnedMeshRenderer[] array = componentsInChildren;
		foreach (SkinnedMeshRenderer skinnedMeshRenderer in array)
		{
			string text4 = skinnedMeshRenderer.sharedMesh.name;
			string text5 = text4.Substring(text4.IndexOf("_") + 1);
			string key2 = text + "_" + text5;
			Mesh value2;
			if (ThemeAssets.Instance.characterModelMeshes.TryGetValue(key2, out value2))
			{
				skinnedMeshRenderer.sharedMesh = value2;
			}
		}
	}
}
