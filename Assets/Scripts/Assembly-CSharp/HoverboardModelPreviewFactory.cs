using System;
using System.Collections.Generic;
using UnityEngine;

public class HoverboardModelPreviewFactory : MonoBehaviour
{
	[Serializable]
	public class HoverboardModelSetup
	{
		public string name;

		public Vector3 eulerAngles;

		public float menuYPosition;

		public AnimationClip clipHangtime;

		public AnimationClip clipRun;

		public GameObject hoverboardPrefab;
	}

	[SerializeField]
	private HoverboardModelSetup[] hoverboards;

	private Dictionary<string, HoverboardModelSetup> name2character = new Dictionary<string, HoverboardModelSetup>();

	private static HoverboardModelPreviewFactory instance;

	public static HoverboardModelPreviewFactory Instance
	{
		get
		{
			return instance ?? (instance = UnityEngine.Object.FindObjectOfType(typeof(HoverboardModelPreviewFactory)) as HoverboardModelPreviewFactory);
		}
	}

	public void Awake()
	{
		HoverboardModelSetup[] array = hoverboards;
		foreach (HoverboardModelSetup hoverboardModelSetup in array)
		{
			name2character.Add(hoverboardModelSetup.name, hoverboardModelSetup);
		}
	}

	public void SelectHoverboard(string name, ref GameObject hoverboardGO, Animation characterAnimation)
	{
		HoverboardModelSetup value;
		if (name2character.TryGetValue(name, out value))
		{
			characterAnimation.transform.parent.GetComponent<CharacterModel>().meshSuperSneaker.enabled = name == "Jumpboard";
			string text = hoverboardGO.name;
			Transform parent = hoverboardGO.transform.parent;
			UnityEngine.Object.Destroy(hoverboardGO);
			hoverboardGO = UnityEngine.Object.Instantiate(value.hoverboardPrefab) as GameObject;
			hoverboardGO.transform.parent = parent;
			hoverboardGO.transform.localPosition = Vector3.zero;
			hoverboardGO.transform.localRotation = Quaternion.identity;
			hoverboardGO.transform.localScale = Vector3.one;
			hoverboardGO.layer = Layers.Instance._3DGUI;
			hoverboardGO.name = text;
			if (characterAnimation[value.clipHangtime.name] == null)
			{
				characterAnimation.AddClip(value.clipHangtime, value.clipHangtime.name);
			}
			characterAnimation[value.clipHangtime.name].wrapMode = WrapMode.Once;
			characterAnimation.CrossFade(value.clipHangtime.name, 0.15f);
			if (characterAnimation[value.clipRun.name] == null)
			{
				characterAnimation.AddClip(value.clipRun, value.clipRun.name);
			}
			characterAnimation.CrossFadeQueued(value.clipRun.name, 0.5f);
		}
		else
		{
			Debug.LogError("could not find preview character model for '" + name + "'.");
		}
	}

	public Quaternion GetHoverboardDefaultRotation(string name)
	{
		HoverboardModelSetup value;
		if (name2character.TryGetValue(name, out value))
		{
			return Quaternion.Euler(value.eulerAngles);
		}
		return Quaternion.Euler(14f, 110.5f, 0f);
	}

	public GameObject GetHoverboardModelPreview(string name)
	{
		HoverboardModelSetup value;
		if (name2character.TryGetValue(name, out value))
		{
			GameObject gameObject = new GameObject("Hoverboard: " + name);
			GameObject gameObject2 = UnityEngine.Object.Instantiate(value.hoverboardPrefab) as GameObject;
			gameObject2.transform.parent = gameObject.transform;
			gameObject2.transform.localRotation = Quaternion.Euler(value.eulerAngles);
			Vector3 localPosition = gameObject2.transform.localPosition;
			gameObject2.transform.localPosition = localPosition + new Vector3(0f, value.menuYPosition, 0f);
			return gameObject;
		}
		Debug.LogError("could not find preview character model for '" + name + "'.");
		return null;
	}
}
