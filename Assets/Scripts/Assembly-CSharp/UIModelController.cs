using System;
using UnityEngine;

public class UIModelController : MonoBehaviour
{
	public enum ModelScreen
	{
		Character = 0,
		GameOver = 1,
		TutorialPopup = 2,
		Boards = 3
	}

	public GameObject CharacterAnchor;

	public GameObject GameOverAnchor;

	public GameObject MysteryBoxAnchor;

	public GameObject TutorialPopupAnchor;

	public GameObject ModelPrefab;

	private CharacterModel _cachedActiveModel;

	private Action _onChangedCurrentlyShown;

	private Characters.CharacterType _currentlyShownModel;

	private static UIModelController _instance;

	public Characters.CharacterType currentlyShownModel
	{
		get
		{
			return _currentlyShownModel;
		}
	}

	public static UIModelController Instance
	{
		get
		{
			return _instance ?? (_instance = UnityEngine.Object.FindObjectOfType(typeof(UIModelController)) as UIModelController);
		}
	}

	public void ActivateGameOverModel()
	{
		Debug.Log("Activate Game Over Model");
		_ActivateModel((Characters.CharacterType)PlayerInfo.Instance.currentCharacter, ModelScreen.GameOver);
	}

	public void AddOnChangedCurrentlyShownHandler(Action handler)
	{
		_onChangedCurrentlyShown = (Action)Delegate.Combine(_onChangedCurrentlyShown, handler);
	}

	public void RemoveOnChangedCurrentlyShownHandler(Action handler)
	{
		_onChangedCurrentlyShown = (Action)Delegate.Remove(_onChangedCurrentlyShown, handler);
	}

	public void SelectCurrentModel()
	{
		PlayerInfo.Instance.currentCharacter = (int)_currentlyShownModel;
		if (Game.Instance != null)
		{
			Game.Instance.Character.characterModel.ChangeCharacterModel(currentlyShownModel.ToString());
		}
		Action onChangedCurrentlyShown = _onChangedCurrentlyShown;
		if (onChangedCurrentlyShown != null)
		{
			onChangedCurrentlyShown();
		}
	}

	public void ActivateCharacterModel()
	{
		Debug.Log("ActivateCharacterModel");
		_currentlyShownModel = (Characters.CharacterType)PlayerInfo.Instance.currentCharacter;
		_ActivateModel(_currentlyShownModel, ModelScreen.Character);
		Action onChangedCurrentlyShown = _onChangedCurrentlyShown;
		if (onChangedCurrentlyShown != null)
		{
			onChangedCurrentlyShown();
		}
	}

	public void ShowMenuModel(Characters.CharacterType charType)
	{
		Debug.Log("Show menu model");
		_currentlyShownModel = charType;
		_SwitchModel(charType);
		Action onChangedCurrentlyShown = _onChangedCurrentlyShown;
		if (onChangedCurrentlyShown != null)
		{
			onChangedCurrentlyShown();
		}
	}

	public void ActivateHoverboardModel()
	{
		_currentlyShownModel = (Characters.CharacterType)PlayerInfo.Instance.currentCharacter;
		_ActivateModel(_currentlyShownModel, ModelScreen.Boards);
	}

	public CharacterModel GetActiveCharacterModel()
	{
		return _cachedActiveModel;
	}

	private void _ActivateModel(Characters.CharacterType characterName, ModelScreen screen)
	{
		if (_cachedActiveModel != null)
		{
			ClearModels();
		}
		Debug.Log("_ActivateModel called. Modelname: " + characterName.ToString() + " screen: " + screen);
		switch (screen)
		{
		case ModelScreen.Character:
		{
			GameObject gameObject3 = UnityEngine.Object.Instantiate(ModelPrefab) as GameObject;
			gameObject3.transform.parent = CharacterAnchor.transform;
			gameObject3.transform.localPosition = new Vector3(0f, 0f, 0f);
			Utility.SetLayerRecursively(gameObject3.transform, CharacterAnchor.layer);
			gameObject3.transform.localScale = new Vector3(21f, 21f, 21f);
			gameObject3.transform.localRotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
			CharacterModel component3 = gameObject3.GetComponent<CharacterModel>();
			component3.ChangeCharacterModel(characterName.ToString());
			component3.HideAllPowerups();
			component3.StartIdleAnimations();
			_cachedActiveModel = component3;
			break;
		}
		case ModelScreen.GameOver:
		{
			GameObject gameObject2 = UnityEngine.Object.Instantiate(ModelPrefab) as GameObject;
			gameObject2.transform.parent = GameOverAnchor.transform;
			gameObject2.transform.localPosition = new Vector3(0f, 0f, 0f);
			Utility.SetLayerRecursively(gameObject2.transform, GameOverAnchor.layer);
			gameObject2.transform.localScale = new Vector3(18f, 18f, 18f);
			gameObject2.transform.localRotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
			CharacterModel component2 = gameObject2.GetComponent<CharacterModel>();
			component2.ChangeCharacterModel(characterName.ToString());
			component2.HideAllPowerups();
			component2.StartIdleAnimations();
			_cachedActiveModel = component2;
			break;
		}
		case ModelScreen.Boards:
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(ModelPrefab) as GameObject;
			gameObject.transform.parent = CharacterAnchor.transform;
			gameObject.transform.localPosition = new Vector3(-20f, 0f, 100f);
			Utility.SetLayerRecursively(gameObject.transform, CharacterAnchor.layer);
			gameObject.transform.localScale = new Vector3(21f, 21f, 21f);
			gameObject.transform.localRotation = Quaternion.Euler(new Vector3(20f, 154.5f, 358f));
			CharacterModel component = gameObject.GetComponent<CharacterModel>();
			component.ChangeCharacterModel(characterName.ToString());
			component.HideAllPowerups();
			component.StartEyeAnimations();
			_cachedActiveModel = component;
			break;
		}
		}
	}

	private void _SwitchModel(Characters.CharacterType characterName)
	{
		if (_cachedActiveModel != null)
		{
			_cachedActiveModel.ChangeCharacterModel(characterName.ToString());
			_cachedActiveModel.HideAllPowerups();
			_cachedActiveModel.StartIdleAnimations();
		}
		else
		{
			_ActivateModel(characterName, ModelScreen.Character);
		}
	}

	public void ClearModels()
	{
		Debug.Log("ClearModels");
		if (_cachedActiveModel != null)
		{
			_cachedActiveModel = null;
		}
		foreach (Transform item in CharacterAnchor.transform)
		{
			UnityEngine.Object.Destroy(item.gameObject);
		}
		foreach (Transform item2 in GameOverAnchor.transform)
		{
			UnityEngine.Object.Destroy(item2.gameObject);
		}
		foreach (Transform item3 in TutorialPopupAnchor.transform)
		{
			UnityEngine.Object.Destroy(item3.gameObject);
		}
	}

	public void ClearTutorialPopup()
	{
		foreach (Transform item in TutorialPopupAnchor.transform)
		{
			UnityEngine.Object.Destroy(item.gameObject);
		}
	}
}
