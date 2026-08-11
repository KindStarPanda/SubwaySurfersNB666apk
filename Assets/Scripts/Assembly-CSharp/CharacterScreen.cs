using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterScreen : UIScreen
{
	private const float DEFAULT_SCALE_FACTOR = 7f;

	private const float FOCUSED_SCALE_FACTOR = 10f;

	[SerializeField]
	private GameObject characterScrollAnchor;

	[SerializeField]
	private UIGrid characterGrid;

	[SerializeField]
	private GameObject overlayParent;

	[SerializeField]
	private UIPanel scrollPanel;

	[SerializeField]
	private GameObject char2dElements;

	[SerializeField]
	private GameObject dummyObject;

	[SerializeField]
	private UILabel nameLabel;

	[SerializeField]
	private UILabel themeLimitedLabel;

	[SerializeField]
	private UISprite backgroundSticker;

	[SerializeField]
	private UILabel stickerDayLabel;

	[SerializeField]
	private UILabel stickerLeftLabel;

	[SerializeField]
	private AudioClip selectSound;

	private bool _hasInited;

	private CenterOnChild _centerer;

	private float _cellWidth;

	private bool _hasShownModel;

	private Characters.CharacterType _cachedModelType;

	private List<KeyValuePair<Characters.CharacterType, Characters.Model>> _characterList = new List<KeyValuePair<Characters.CharacterType, Characters.Model>>();

	private List<GameObject> characters = new List<GameObject>();

	private List<CharacterOverlayHelper> characterHelpers = new List<CharacterOverlayHelper>();

	private List<OverlayIndex> characterIndices = new List<OverlayIndex>();

	private bool _inappOverlayActive;

	private bool _popupActive;

	private bool _charactersEnabled;

	public override void Init()
	{
		base.Init();
		_cellWidth = characterGrid.cellWidth;
		_centerer = characterGrid.GetComponent<CenterOnChild>();
		InitCharacters();
		_hasInited = true;
	}

	public override void Show()
	{
		base.Show();
		RefreshCharacters();
		UIModelController.Instance.AddOnChangedCurrentlyShownHandler(_OnChangedCurrentlyShownModel);
		_OnChangedCurrentlyShownModel();
		if (_hasShownModel)
		{
			ShowModel(_cachedModelType);
		}
		else
		{
			Update();
		}
	}

	public override void Hide()
	{
		base.Hide();
		UIModelController.Instance.RemoveOnChangedCurrentlyShownHandler(_OnChangedCurrentlyShownModel);
		UIModelController.Instance.ClearModels();
		Debug.Log("Character hide");
	}

	private void _OnChangedCurrentlyShownModel()
	{
		Characters.CharacterType currentlyShownModel = UIModelController.Instance.currentlyShownModel;
		Characters.Model model = Characters.characterData[currentlyShownModel];
		bool flag = PlayerInfo.Instance.IsCollectionComplete(currentlyShownModel);
		nameLabel.text = model.modelName;
		if (flag)
		{
			if (model.characterSeason != 0)
			{
				themeLimitedLabel.text = model.characterSeasonLimitedDescription;
				themeLimitedLabel.enabled = true;
			}
			else
			{
				themeLimitedLabel.text = string.Empty;
				themeLimitedLabel.enabled = false;
			}
			stickerDayLabel.text = string.Empty;
			stickerLeftLabel.text = string.Empty;
			stickerDayLabel.enabled = false;
			stickerLeftLabel.enabled = false;
			backgroundSticker.enabled = false;
		}
		else if (model.characterSeason != 0)
		{
			themeLimitedLabel.enabled = true;
			stickerDayLabel.enabled = true;
			stickerLeftLabel.enabled = true;
			backgroundSticker.enabled = true;
			themeLimitedLabel.text = model.characterSeasonLimitedDescription;
			TimeSpan timeToExpire = ThemeManager.Instance.themeForSeason(model.characterSeason).TimeToExpire;
			if (timeToExpire.Days < 1)
			{
				stickerDayLabel.text = "LAST";
				stickerLeftLabel.text = "DAY";
			}
			else
			{
				stickerDayLabel.text = string.Format("{0:0} DAY{1}", timeToExpire.Days, (timeToExpire.Days == 1) ? string.Empty : "S");
				stickerLeftLabel.text = "LEFT";
			}
		}
		else
		{
			themeLimitedLabel.text = string.Empty;
			stickerDayLabel.text = string.Empty;
			stickerLeftLabel.text = string.Empty;
			stickerDayLabel.enabled = false;
			themeLimitedLabel.enabled = false;
			stickerLeftLabel.enabled = false;
			backgroundSticker.enabled = false;
		}
	}

	private void InitCharacters()
	{
		PlayerInfo.Season currentSeasonAvailable = PlayerInfo.Instance.currentSeasonAvailable;
		_characterList = new List<KeyValuePair<Characters.CharacterType, Characters.Model>>();
		foreach (Characters.CharacterType item in Characters.characterOrder)
		{
			Characters.Model value = Characters.characterData[item];
			if (PlayerInfo.Instance.IsCollectionComplete(item) || value.characterSeason == currentSeasonAvailable || value.characterSeason == PlayerInfo.Season.none)
			{
				_characterList.Add(new KeyValuePair<Characters.CharacterType, Characters.Model>(item, value));
			}
		}
		int num = 0;
		foreach (KeyValuePair<Characters.CharacterType, Characters.Model> character in _characterList)
		{
			GameObject characterModelPreview = CharacterModelPreviewFactory.Instance.GetCharacterModelPreview(character.Key.ToString());
			if (characterModelPreview == null)
			{
				characterModelPreview = CharacterModelPreviewFactory.Instance.GetCharacterModelPreview(Characters.CharacterType.zombiejake.ToString());
			}
			characterModelPreview.name = string.Format("{0:000}{1}", num, character.Key.ToString());
			characters.Add(characterModelPreview);
			Transform transform = characterModelPreview.transform;
			transform.parent = characterScrollAnchor.transform;
			transform.localPosition = new Vector3((float)num * _cellWidth, 0f, 50f);
			transform.localScale = Vector3.one * 7f;
			transform.localEulerAngles = new Vector3(53f, 183f, 360f);
			GameObject gameObject = NGUITools.AddChild(overlayParent, char2dElements);
			gameObject.name = string.Format("{0:000}{1}", num, character.Key.ToString());
			characterHelpers.Add(gameObject.GetComponent<CharacterOverlayHelper>());
			characterHelpers[num].Init(num, character.Key, transform);
			GameObject gameObject2 = NGUITools.AddChild(characterGrid.gameObject, dummyObject);
			characterIndices.Add(gameObject2.AddComponent<OverlayIndex>());
			characterIndices[num].index = num;
			gameObject2.name = string.Format("{0:000}{1}", num, character.Key.ToString());
			num++;
		}
		Utility.SetLayerRecursively(characterScrollAnchor.transform, 29);
		Utility.SetLayerRecursively(overlayParent.transform, 28);
		characterGrid.SendMessage("Start");
		characterGrid.Reposition();
		int num2 = 0;
		Characters.CharacterType currentCharacter = (Characters.CharacterType)PlayerInfo.Instance.currentCharacter;
		using (List<KeyValuePair<Characters.CharacterType, Characters.Model>>.Enumerator enumerator3 = _characterList.GetEnumerator())
		{
			while (enumerator3.MoveNext() && currentCharacter != enumerator3.Current.Key)
			{
				num2++;
			}
		}
		_centerer.CenterOnTransform(characterIndices[num2].transform, true);
		ScaleScrollModels();
	}

	private void RefreshCharacters()
	{
		PlayerInfo.Season currentSeasonAvailable = PlayerInfo.Instance.currentSeasonAvailable;
		_characterList = new List<KeyValuePair<Characters.CharacterType, Characters.Model>>();
		foreach (Characters.CharacterType item in Characters.characterOrder)
		{
			Characters.Model value = Characters.characterData[item];
			if (PlayerInfo.Instance.IsCollectionComplete(item) || value.characterSeason == currentSeasonAvailable || value.characterSeason == PlayerInfo.Season.none)
			{
				_characterList.Add(new KeyValuePair<Characters.CharacterType, Characters.Model>(item, value));
			}
		}
		bool flag = false;
		for (int i = 0; i < characterHelpers.Count; i++)
		{
			if (_characterList[i].Key != characterHelpers[i].GetCharacterType())
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			return;
		}
		Debug.Log("Refreshing character list");
		foreach (GameObject character in characters)
		{
			UnityEngine.Object.Destroy(character);
		}
		characters.Clear();
		foreach (CharacterOverlayHelper characterHelper in characterHelpers)
		{
			UnityEngine.Object.Destroy(characterHelper.gameObject);
		}
		characterHelpers.Clear();
		foreach (OverlayIndex characterIndex in characterIndices)
		{
			UnityEngine.Object.Destroy(characterIndex.gameObject);
		}
		characterIndices.Clear();
		foreach (Transform item2 in characterGrid.transform)
		{
			item2.gameObject.active = false;
			UnityEngine.Object.Destroy(item2);
		}
		overlayParent.transform.localPosition = Vector3.zero;
		characterScrollAnchor.transform.localPosition = new Vector3(0f, characterScrollAnchor.transform.localPosition.y, characterScrollAnchor.transform.localPosition.z);
		scrollPanel.cachedTransform.localPosition = Vector3.zero;
		Vector4 clipRange = scrollPanel.clipRange;
		scrollPanel.clipRange = new Vector4(0f, clipRange.y, clipRange.z, clipRange.w);
		_centerer.ClearCenterObject();
		UnityEngine.Object.Destroy(scrollPanel.GetComponent<SpringPanel>());
		InitCharacters();
	}

	public void ScrollClicked(Vector2 pos)
	{
		bool flag = _centerer.CenterOnClosestChildAtPosition(pos);
		Debug.Log("was focused " + flag);
		if (flag && PlayerInfo.Instance.IsCollectionComplete(UIModelController.Instance.currentlyShownModel))
		{
			UIModelController.Instance.SelectCurrentModel();
			NGUITools.PlaySound(selectSound);
		}
	}

	private void Update()
	{
		if (!_hasInited)
		{
			return;
		}
		if (_centerer.centeredObject != null)
		{
			int index = _centerer.centeredObject.GetComponent<OverlayIndex>().index;
			Characters.CharacterType characterType = characterHelpers[index].GetCharacterType();
			if (!_hasShownModel)
			{
				Debug.Log("center char: " + characterType);
				Debug.Log("Cached character type: " + _cachedModelType);
				_hasShownModel = true;
				_cachedModelType = characterType;
				ShowModel(_cachedModelType);
				characterHelpers[index].SelectedInMenu();
			}
			if (_cachedModelType != characterType)
			{
				Debug.Log("Selected in Update");
				_cachedModelType = characterType;
				ShowModel(_cachedModelType);
				characterHelpers[index].SelectedInMenu();
			}
		}
		ScaleScrollModels();
		if (UIScreenController.Instance.isShowingPopup)
		{
			if (!_popupActive || !_inappOverlayActive)
			{
				_popupActive = true;
			}
		}
		else if (_popupActive && !_inappOverlayActive)
		{
			_popupActive = false;
		}
		if (UIScreenController.Instance.inAppPurchaseOverlay.active)
		{
			if (!_inappOverlayActive)
			{
				_inappOverlayActive = true;
			}
		}
		else if (_inappOverlayActive)
		{
			_inappOverlayActive = false;
		}
		if (_inappOverlayActive || _popupActive)
		{
			if (_charactersEnabled)
			{
				characterScrollAnchor.SetActiveRecursively(false);
				overlayParent.SetActiveRecursively(false);
				_charactersEnabled = false;
			}
		}
		else if (!_charactersEnabled)
		{
			characterScrollAnchor.SetActiveRecursively(true);
			overlayParent.SetActiveRecursively(true);
			_charactersEnabled = true;
		}
	}

	private void ShowModel(Characters.CharacterType charType)
	{
		UIModelController.Instance.ShowMenuModel(charType);
	}

	private void ScaleScrollModels()
	{
		float num = Mathf.Abs(characterScrollAnchor.transform.localPosition.x);
		for (int i = 0; i < characters.Count; i++)
		{
			float num2 = Mathf.Abs(num - (float)i * _cellWidth);
			float num3 = 1.5f * _cellWidth;
			float num4 = Mathf.SmoothStep(10f, 7f, num2 / num3);
			characters[i].transform.localScale = Vector3.one * num4;
		}
	}
}
