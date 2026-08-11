using UnityEngine;

public class ThemeShiftTextures : MonoBehaviour
{
	private Renderer[] renderes;

	private void Start()
	{
		RelinkMaterialWithThemeAssets();
	}

	private void RelinkMaterialWithThemeAssets()
	{
		renderes = base.gameObject.GetComponentsInChildren<Renderer>(true);
		Renderer[] array = renderes;
		foreach (Renderer renderer in array)
		{
			Material[] sharedMaterials = renderer.sharedMaterials;
			for (int j = 0; j < sharedMaterials.Length; j++)
			{
				Material value;
				if (ThemeAssets.Instance != null && ThemeAssets.Instance.original2CloneMaterials.TryGetValue(sharedMaterials[j], out value))
				{
					sharedMaterials[j] = ThemeAssets.Instance.ReplaceMaterial(sharedMaterials[j]);
				}
			}
			renderer.materials = sharedMaterials;
		}
	}
}
