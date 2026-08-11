using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterModelPreviewFactory : MonoBehaviour
{
	[Serializable]
	private class CharacterModelSetup
	{
		public string name;

		public Material material;

		public Mesh mesh;

		public AnimationClip animationClip;

		public AnimationClip headRotationAnimationClip;
	}

	[SerializeField]
	private GameObject characterModelPreviewPrefab;

	[SerializeField]
	private CharacterModelSetup[] characters;

	private Dictionary<string, CharacterModelSetup> name2character = new Dictionary<string, CharacterModelSetup>();

	private static CharacterModelPreviewFactory instance;

	public static CharacterModelPreviewFactory Instance
	{
		get
		{
			return instance ?? (instance = UnityEngine.Object.FindObjectOfType(typeof(CharacterModelPreviewFactory)) as CharacterModelPreviewFactory);
		}
	}

	public void Awake()
	{
		CharacterModelSetup[] array = characters;
		foreach (CharacterModelSetup characterModelSetup in array)
		{
			name2character.Add(characterModelSetup.name, characterModelSetup);
		}
	}

	public GameObject GetCharacterModelPreview(string name)
	{
		CharacterModelSetup value;
		if (name2character.TryGetValue(name, out value))
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(characterModelPreviewPrefab) as GameObject;
			SkinnedMeshRenderer componentInChildren = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
			componentInChildren.material = value.material;
			componentInChildren.sharedMesh = value.mesh;
			Animation component = gameObject.GetComponent<Animation>();
			component.AddClip(value.animationClip, "clip");
			component.Play("clip");
			component["clip"].speed = 0f;
			component.AddClip(value.headRotationAnimationClip, "headClip");
			component["headClip"].blendMode = AnimationBlendMode.Blend;
			component["headClip"].weight = 1f;
			component["headClip"].layer = 10;
			component["headClip"].enabled = true;
			component["headClip"].speed = 0f;
			component["headClip"].normalizedTime = 0f;
			return gameObject;
		}
		Debug.LogError("could not find preview character model for '" + name + "'.");
		return null;
	}

	public void Start()
	{
	}
}
