using UnityEngine;

public class UICharacterSelectButton : MonoBehaviour
{
	[SerializeField]
	private UISlicedSprite fillSprite;

	[SerializeField]
	private UILabel label;

	private string fillSelect = "background_character_buy";

	private string fillSelected = "button_fill_selected";

	private string fillNotAvailable = "button_fill_info";

	private string textSelect = "SELECT";

	private string textSelected = "SELECTED";

	private string textProgress = "{0} / {1}";

	private bool isShowingNotAvailableGraphics = true;

	[SerializeField]
	private GameObject notAvailableGraphics;

	[SerializeField]
	private UISprite tokenSprite;

	[SerializeField]
	private UILabel progressLabel;

	private bool isEnabled;

	private bool _hasInited;

	private BoxCollider col;

	private void OnEnable()
	{
		if (_hasInited)
		{
			isShowingNotAvailableGraphics = true;
			UIModelController.Instance.AddOnChangedCurrentlyShownHandler(OnChangedCurrentlyShownModel);
			OnChangedCurrentlyShownModel();
		}
	}

	private void Start()
	{
		if (!_hasInited)
		{
			col = GetComponent<BoxCollider>();
			isShowingNotAvailableGraphics = true;
			UIModelController.Instance.AddOnChangedCurrentlyShownHandler(OnChangedCurrentlyShownModel);
			OnChangedCurrentlyShownModel();
			_hasInited = true;
		}
	}

	private void OnDisable()
	{
		UIModelController.Instance.RemoveOnChangedCurrentlyShownHandler(OnChangedCurrentlyShownModel);
	}

	private void OnChangedCurrentlyShownModel()
	{
		Characters.CharacterType currentlyShownModel = UIModelController.Instance.currentlyShownModel;
		Characters.Model model = Characters.characterData[currentlyShownModel];
		if (PlayerInfo.Instance.currentCharacter == (int)UIModelController.Instance.currentlyShownModel)
		{
			showAndEnable();
			setNotAvalibleVisibility(false);
			fillSprite.spriteName = fillSelected;
			label.text = textSelected;
			col.enabled = false;
		}
		else if (PlayerInfo.Instance.IsCollectionComplete(currentlyShownModel) ? true : false)
		{
			showAndEnable();
			setNotAvalibleVisibility(false);
			fillSprite.spriteName = fillSelect;
			label.text = textSelect;
			col.enabled = true;
		}
		else if (model.unlockType == Characters.UnlockType.tokens)
		{
			showAndEnable();
			fillSprite.spriteName = fillNotAvailable;
			label.text = string.Empty;
			progressLabel.text = string.Format(textProgress, PlayerInfo.Instance.GetCollectedTokens(currentlyShownModel), model.Price);
			tokenSprite.spriteName = Characters.characterData[currentlyShownModel].tokenSprite2dName;
			setNotAvalibleVisibility(true);
			col.enabled = false;
		}
		else
		{
			hideAndDisable();
		}
	}

	private void hideAndDisable()
	{
		for (int i = 0; i < base.transform.childCount; i++)
		{
			Transform child = base.transform.GetChild(i);
			child.gameObject.active = false;
		}
		setNotAvalibleVisibility(false);
		col.enabled = false;
		isEnabled = false;
	}

	private void showAndEnable()
	{
		if (!isEnabled)
		{
			for (int i = 0; i < base.transform.childCount; i++)
			{
				Transform child = base.transform.GetChild(i);
				child.gameObject.active = true;
			}
			setNotAvalibleVisibility(false);
			label.panel.Refresh();
			col.enabled = true;
			isEnabled = true;
		}
	}

	private void setNotAvalibleVisibility(bool val)
	{
		if (val != isShowingNotAvailableGraphics)
		{
			for (int i = 0; i < notAvailableGraphics.transform.childCount; i++)
			{
				Transform child = notAvailableGraphics.transform.GetChild(i);
				child.gameObject.active = val;
			}
			isShowingNotAvailableGraphics = val;
		}
	}
}
