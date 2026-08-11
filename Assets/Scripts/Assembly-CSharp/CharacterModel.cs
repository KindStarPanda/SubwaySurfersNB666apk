using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterModel : MonoBehaviour
{
	public SkinnedMeshRenderer model;

	public SkinnedMeshRenderer meshSuperSneaker;

	public MeshRenderer meshHoverboard;

	public MeshRenderer meshCoinMagnet;

	public MeshRenderer meshJetpack;

	public MeshRenderer meshSprayCan;

	public MeshRenderer meshBlobShadow;

	public Animation characterAnimation;

	public MeshRenderer shadow;

	public Transform spineTransform;

	public Transform shoulderTransform;

	public GameObject sprayCanModel;

	public Transform jetpackCloudPositionL;

	public Transform jetpackCloudPositionR;

	private SkinnedMeshRenderer[] models;

	private Dictionary<string, SkinnedMeshRenderer> modelLookupTable;

	private string[] modelNames;

	private Color overlayColor = Color.black;

	private bool blinking;

	public float blinkFrequency = 1.5f;

	private Animation _animationComponent;

	public string[] ModelNames
	{
		get
		{
			return modelNames;
		}
	}

	public Color OverlayColor
	{
		get
		{
			return overlayColor;
		}
		set
		{
			overlayColor = value;
			for (int i = 0; i < models.Length; i++)
			{
				SkinnedMeshRenderer skinnedMeshRenderer = models[i];
				skinnedMeshRenderer.sharedMaterial.SetColor("_OverlayColor", overlayColor);
			}
		}
	}

	public void Awake()
	{
		models = GetComponentsInChildren<SkinnedMeshRenderer>();
		modelNames = new string[models.Length];
		modelLookupTable = new Dictionary<string, SkinnedMeshRenderer>();
		for (int i = 0; i < models.Length; i++)
		{
			SkinnedMeshRenderer skinnedMeshRenderer = models[i];
			string text = skinnedMeshRenderer.gameObject.name;
			modelNames[i] = text;
			modelLookupTable.Add(text, skinnedMeshRenderer);
		}
		Characters.CharacterType currentCharacterType = (Characters.CharacterType)PlayerInfo.Instance.currentCharacter;
		if (!PlayerInfo.Instance.IsCollectionComplete(currentCharacterType))
		{
			Debug.LogWarning("CharacterModel: Resetting to jake/slick because of likely exploit", this);
			PlayerInfo.Instance.currentCharacter = 0;
			PlayerInfo.Instance.SaveIfDirty();
		}
		sprayCanModel.SetActiveRecursively(false);
		ChangeCharacterModel(currentCharacterType.ToString());
	}

	public void ChangeCharacterModel(string name)
	{
		StopIdleAnimations();
		SkinnedMeshRenderer value;
		if (modelLookupTable.TryGetValue(name, out value))
		{
			for (int i = 0; i < models.Length; i++)
			{
				SkinnedMeshRenderer skinnedMeshRenderer = models[i];
				skinnedMeshRenderer.enabled = skinnedMeshRenderer == value;
			}
		}
		else
		{
			Debug.LogWarning(string.Format("Could not find character by the name: {0}", name));
		}
		if (value != null)
		{
			model = value;
		}
	}

	public Animation GetAnimation()
	{
		if (_animationComponent == null)
		{
			_animationComponent = GetComponentInChildren<Animation>();
		}
		return _animationComponent;
	}

	public GameObject GetHoverboardGameObject()
	{
		if (meshHoverboard != null)
		{
			return meshHoverboard.gameObject;
		}
		return null;
	}

	public void SetNewHoverboard(GameObject newBoard)
	{
		meshHoverboard = newBoard.GetComponent<MeshRenderer>();
	}

	public void HideAllPowerups()
	{
		meshHoverboard.enabled = false;
		meshJetpack.enabled = false;
		meshSuperSneaker.enabled = false;
		meshCoinMagnet.enabled = false;
		meshSprayCan.enabled = model.name == "slick";
		sprayCanModel.SetActiveRecursively(false);
		ParticleSystem[] componentsInChildren = sprayCanModel.GetComponentsInChildren<ParticleSystem>(true);
		foreach (ParticleSystem particleSystem in componentsInChildren)
		{
			particleSystem.enableEmission = false;
		}
	}

	public void HideBlobShadow()
	{
		meshBlobShadow.enabled = false;
	}

	public void StartBlink()
	{
		blinking = true;
		StartCoroutine(Blink());
	}

	private IEnumerator Blink()
	{
		while (blinking)
		{
			OverlayColor = pMath.Square(Time.time * blinkFrequency) * Color.white;
			yield return null;
		}
		OverlayColor = Color.black;
	}

	public void StopBlink()
	{
		blinking = false;
	}

	public void ResetBlink()
	{
		OverlayColor = Color.black;
	}

	public void StartEyeAnimations()
	{
		if (!(model == null))
		{
			AvatarEyeAnimation component = model.GetComponent<AvatarEyeAnimation>();
			if (component != null)
			{
				component.AnimateEyes(false);
			}
		}
	}

	public void StopEyeAnimations()
	{
		if (!(model == null))
		{
			AvatarEyeAnimation component = model.GetComponent<AvatarEyeAnimation>();
			component.animate = false;
		}
	}

	public void StartIdleAnimations()
	{
		if (!(model == null))
		{
			AvatarAnimations component = model.GetComponent<AvatarAnimations>();
			if (component != null)
			{
				component.StartIdleAnimations();
			}
			AvatarEyeAnimation component2 = model.GetComponent<AvatarEyeAnimation>();
			if (component2 != null)
			{
				component2.AnimateEyes(false);
			}
		}
	}

	public void StartIdlePopupAnimations()
	{
		if (!(model == null))
		{
			AvatarAnimations component = model.GetComponent<AvatarAnimations>();
			if (component != null)
			{
				component.StartIdlePopupAnimations();
			}
			AvatarEyeAnimation component2 = model.GetComponent<AvatarEyeAnimation>();
			if (component2 != null)
			{
				component2.AnimateEyes(true);
			}
		}
	}

	public void StopIdleAnimations()
	{
		if (!(model == null))
		{
			AvatarAnimations component = model.GetComponent<AvatarAnimations>();
			if (component != null)
			{
				component.StopIdleAnimations();
			}
			AvatarEyeAnimation component2 = model.GetComponent<AvatarEyeAnimation>();
			if (component2 != null)
			{
				component2.StopAnimatingEyes();
			}
		}
	}

	public void PauseIdleAnimations()
	{
		if (!(model == null))
		{
			AvatarAnimations component = model.GetComponent<AvatarAnimations>();
			if (component != null)
			{
				component.PauseIdleAnimations();
			}
		}
	}

	public void ResumeIdleAnimations()
	{
		if (!(model == null))
		{
			AvatarAnimations component = model.GetComponent<AvatarAnimations>();
			if (component != null)
			{
				component.ResumeIdleAnimations();
			}
		}
	}
}
