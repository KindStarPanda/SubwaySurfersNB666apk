using UnityEngine;

public class CharacterPopup : UIScreen
{
	private CharacterModel characterModel;

	private UIModelController uimodelController;

	public override void Init()
	{
		base.Init();
		uimodelController = UIModelController.Instance;
	}

	protected void SetCharacter(Characters.CharacterType character)
	{
		uimodelController.ClearTutorialPopup();
		if (characterModel == null)
		{
			characterModel = NGUITools.AddChild(uimodelController.TutorialPopupAnchor, uimodelController.ModelPrefab).GetComponent<CharacterModel>();
			Utility.SetLayerRecursively(characterModel.transform, uimodelController.TutorialPopupAnchor.layer);
			Transform transform = characterModel.transform;
			transform.localPosition = new Vector3(35f, 23f, 50f);
			transform.localScale = Vector3.one * 19f;
			transform.localEulerAngles = new Vector3(50f, 200f, 0f);
			characterModel.HideBlobShadow();
		}
		characterModel.ChangeCharacterModel(character.ToString());
		characterModel.HideAllPowerups();
		characterModel.StartIdlePopupAnimations();
		CharacterScaler component = uimodelController.TutorialPopupAnchor.GetComponent<CharacterScaler>();
		if (component != null)
		{
			component.lookAtCamera = true;
		}
	}

	protected virtual void OnDisable()
	{
		if (!(UIScreenController.Instance == null) && !UIScreenController.Instance.stoppingFromEditor)
		{
			uimodelController.ClearTutorialPopup();
		}
	}
}
