using System.Collections.Generic;
using UnityEngine;

public class ResolutionSetup : MonoBehaviour
{
	private const string _atlasPath = "Atlases/";

	[SerializeField]
	private UIAtlas[] usedAtlasses;

	[SerializeField]
	private string[] lowResAtlasses;

	[SerializeField]
	private string[] highResAtlasses;

	[SerializeField]
	private UIFont[] usedFonts;

	[SerializeField]
	private UIFont[] lowResFonts;

	[SerializeField]
	private UIFont[] highResFonts;

	private UIAtlas[] _editorAtlases;

	private List<UILabel> allLabels;

	private List<UIFont> allModifiedLabelsOldFonts;

	private void Awake()
	{
		_editorAtlases = new UIAtlas[usedAtlasses.Length];
		for (int i = 0; i < usedAtlasses.Length; i++)
		{
			_editorAtlases[i] = usedAtlasses[i].replacement;
		}
		if (lowResAtlasses.Length != highResAtlasses.Length)
		{
			Debug.LogError("Low res and high res atlasses do not fit!");
		}
		else if (lowResFonts.Length != highResFonts.Length)
		{
			Debug.LogError("Low res and high res fonts do not fit!");
		}
		else if (DeviceInfo.isHighres)
		{
			for (int j = 0; j < usedFonts.Length; j++)
			{
				usedFonts[j].replacement = highResFonts[j];
			}
			for (int k = 0; k < usedAtlasses.Length; k++)
			{
				string path = "Atlases/" + highResAtlasses[k];
				GameObject gameObject = Resources.Load(path, typeof(GameObject)) as GameObject;
				UIAtlas component = gameObject.GetComponent<UIAtlas>();
				usedAtlasses[k].replacement = component;
			}
		}
		else
		{
			for (int l = 0; l < usedFonts.Length; l++)
			{
				usedFonts[l].replacement = lowResFonts[l];
			}
			for (int m = 0; m < usedAtlasses.Length; m++)
			{
				string path2 = "Atlases/" + lowResAtlasses[m];
				GameObject gameObject2 = Resources.Load(path2, typeof(GameObject)) as GameObject;
				UIAtlas component2 = gameObject2.GetComponent<UIAtlas>();
				usedAtlasses[m].replacement = component2;
			}
		}
	}

	private void OnDisable()
	{
		for (int i = 0; i < usedFonts.Length; i++)
		{
			usedFonts[i].replacement = lowResFonts[i];
		}
		for (int j = 0; j < highResAtlasses.Length; j++)
		{
			usedAtlasses[j].replacement = _editorAtlases[j];
		}
	}

	public void SwitchFontResolution()
	{
		UILabel[] array = Resources.FindObjectsOfTypeAll(typeof(UILabel)) as UILabel[];
		allLabels = new List<UILabel>();
		allModifiedLabelsOldFonts = new List<UIFont>();
		UILabel[] array2 = array;
		foreach (UILabel uILabel in array2)
		{
			for (int j = 0; j < lowResFonts.Length; j++)
			{
				if (uILabel.font == lowResFonts[j])
				{
					Debug.Log("Switching to high res font now!");
					allLabels.Add(uILabel);
					allModifiedLabelsOldFonts.Add(lowResFonts[j]);
					uILabel.font = highResFonts[j];
					break;
				}
			}
		}
	}

	public void ResetFontResolution()
	{
		Debug.Log("Resetting fonts");
		for (int i = 0; i < allLabels.Count; i++)
		{
			allLabels[i].font = allModifiedLabelsOldFonts[i];
		}
	}
}
