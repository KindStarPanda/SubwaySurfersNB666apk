using System.Collections.Generic;
using UnityEngine;

public class BoardScreen : UIScreen
{
	private class BoardState
	{
		public Quaternion defaultRotation;

		public GameObject boardRoot;

		public Transform boardTransform;
	}

	private const float DEFAULT_SCALE_FACTOR = 7f;

	private const float FOCUSED_SCALE_FACTOR = 10f;

	[SerializeField]
	private GameObject scrollAnchor;

	[SerializeField]
	private UIGrid scrollGrid;

	[SerializeField]
	private GameObject overlayParent;

	[SerializeField]
	private UIPanel scrollPanel;

	[SerializeField]
	private GameObject scroll2dElements;

	[SerializeField]
	private GameObject dummyObject;

	[SerializeField]
	private UILabel nameLabel;

	[SerializeField]
	private UILabel staticPowerupLabel;

	[SerializeField]
	private UILabel descriptionLabel;

	[SerializeField]
	private UISprite powerIcon;

	[SerializeField]
	private UISprite backgroundSticker;

	[SerializeField]
	private UILabel stickerTopLabel;

	[SerializeField]
	private UILabel stickerBottomLabel;

	[SerializeField]
	private AudioClip selectSound;

	[SerializeField]
	private HoverboardBuyButton buyButton;

	[SerializeField]
	private HoverboardSelectButton selectButton;

	private bool _hasInited;

	private CenterOnChild _centerer;

	private float _cellWidth;

	private bool _hasSetupScroll;

	private Hoverboards.BoardType _currentlyShownBoard;

	private List<BoardState> scrollBoards = new List<BoardState>();

	private List<HoverboardOverlayHelper> boardHelpers = new List<HoverboardOverlayHelper>();

	private List<OverlayIndex> boardIndices = new List<OverlayIndex>();

	private bool _inappOverlayActive;

	private bool _popupActive;

	private bool _modelsEnabled;

	public override void Init()
	{
		base.Init();
		_cellWidth = scrollGrid.cellWidth;
		_centerer = scrollGrid.GetComponent<CenterOnChild>();
		InitBoards();
		_hasInited = true;
	}

	public override void Show()
	{
		base.Show();
		RefreshBoards();
		UIModelController.Instance.ActivateHoverboardModel();
		ShowHoverboardInMenu();
	}

	public override void Hide()
	{
		base.Hide();
		UIModelController.Instance.ClearModels();
	}

	private void ShowHoverboardInMenu()
	{
		GameObject hoverboardGO = UIModelController.Instance.GetActiveCharacterModel().GetHoverboardGameObject();
		HoverboardModelPreviewFactory.Instance.SelectHoverboard(Hoverboards.boardData[_currentlyShownBoard].boardModelName, ref hoverboardGO, UIModelController.Instance.GetActiveCharacterModel().GetAnimation());
		UIModelController.Instance.GetActiveCharacterModel().SetNewHoverboard(hoverboardGO);
		UpdateNamesAndButtons();
	}

	private void UpdateNamesAndButtons()
	{
		bool flag = PlayerInfo.Instance.isHoverboardUnlocked(_currentlyShownBoard);
		nameLabel.text = Hoverboards.boardData[_currentlyShownBoard].name;
		descriptionLabel.text = Hoverboards.boardData[_currentlyShownBoard].description;
		if (string.IsNullOrEmpty(descriptionLabel.text))
		{
			powerIcon.enabled = false;
			staticPowerupLabel.text = string.Empty;
		}
		else
		{
			powerIcon.enabled = true;
			staticPowerupLabel.text = "SPECIAL POWER";
		}
		buyButton.SetupButton(_currentlyShownBoard);
		selectButton.SetupButton(_currentlyShownBoard);
		if (flag)
		{
			stickerTopLabel.text = string.Empty;
			stickerBottomLabel.text = string.Empty;
			stickerTopLabel.enabled = false;
			stickerBottomLabel.enabled = false;
			backgroundSticker.enabled = false;
		}
		else if (Hoverboards.boardData[_currentlyShownBoard].unlockType == Hoverboards.UnlockType.free)
		{
			stickerTopLabel.text = "FREE";
			stickerBottomLabel.text = "GIFT";
			stickerTopLabel.enabled = true;
			stickerBottomLabel.enabled = true;
			backgroundSticker.enabled = true;
		}
		else
		{
			stickerTopLabel.text = string.Empty;
			stickerBottomLabel.text = string.Empty;
			stickerTopLabel.enabled = false;
			stickerBottomLabel.enabled = false;
			backgroundSticker.enabled = false;
		}
	}

	private void InitBoards()
	{
		List<KeyValuePair<Hoverboards.BoardType, Hoverboards.Board>> list = new List<KeyValuePair<Hoverboards.BoardType, Hoverboards.Board>>();
		foreach (Hoverboards.BoardType item in Hoverboards.boardOrder)
		{
			Hoverboards.Board value = Hoverboards.boardData[item];
			list.Add(new KeyValuePair<Hoverboards.BoardType, Hoverboards.Board>(item, value));
		}
		int num = 0;
		foreach (KeyValuePair<Hoverboards.BoardType, Hoverboards.Board> item2 in list)
		{
			BoardState boardState = new BoardState();
			boardState.boardRoot = HoverboardModelPreviewFactory.Instance.GetHoverboardModelPreview(item2.Value.boardModelName);
			if (boardState.boardRoot == null)
			{
				Debug.LogError("Board: '" + item2.Value.boardModelName + "' not found.");
			}
			boardState.boardRoot.name = string.Format("{0:000}{1}", num, item2.Value.name);
			boardState.boardTransform = boardState.boardRoot.transform.GetChild(0);
			boardState.defaultRotation = HoverboardModelPreviewFactory.Instance.GetHoverboardDefaultRotation(item2.Value.boardModelName);
			scrollBoards.Add(boardState);
			Transform transform = boardState.boardRoot.transform;
			transform.parent = scrollAnchor.transform;
			transform.localPosition = new Vector3((float)num * _cellWidth, 0f, 50f);
			transform.localScale = Vector3.one * 7f;
			transform.localEulerAngles = new Vector3(53f, 183f, 360f);
			GameObject gameObject = NGUITools.AddChild(overlayParent, scroll2dElements);
			gameObject.name = string.Format("{0:000}{1}", num, item2.Value.name);
			boardHelpers.Add(gameObject.GetComponent<HoverboardOverlayHelper>());
			boardHelpers[num].Init(num, item2.Key, transform);
			GameObject gameObject2 = NGUITools.AddChild(scrollGrid.gameObject, dummyObject);
			boardIndices.Add(gameObject2.AddComponent<OverlayIndex>());
			boardIndices[num].index = num;
			gameObject2.name = string.Format("{0:000}{1}", num, item2.Key.ToString());
			num++;
		}
		Utility.SetLayerRecursively(scrollAnchor.transform, 29);
		Utility.SetLayerRecursively(overlayParent.transform, 28);
		scrollGrid.SendMessage("Start");
		scrollGrid.Reposition();
		int num2 = 0;
		Hoverboards.BoardType currentHoverboard = PlayerInfo.Instance.currentHoverboard;
		using (List<KeyValuePair<Hoverboards.BoardType, Hoverboards.Board>>.Enumerator enumerator3 = list.GetEnumerator())
		{
			while (enumerator3.MoveNext() && currentHoverboard != enumerator3.Current.Key)
			{
				num2++;
			}
		}
		_centerer.CenterOnTransform(boardIndices[num2].transform, true);
		UIModelController.Instance.ActivateHoverboardModel();
		_currentlyShownBoard = currentHoverboard;
		ShowHoverboardInMenu();
		float num3 = Mathf.Abs(scrollAnchor.transform.localPosition.x);
		for (int i = 0; i < scrollBoards.Count; i++)
		{
			float num4 = Mathf.Abs(num3 - (float)i * _cellWidth);
			float num5 = 1.5f * _cellWidth;
			float num6 = Mathf.SmoothStep(10f, 7f, num4 / num5);
			boardHelpers[i].currentDistFromCenter = num4 / num5;
			scrollBoards[i].boardRoot.transform.localScale = Vector3.one * num6;
		}
	}

	private void RefreshBoards()
	{
		List<KeyValuePair<Hoverboards.BoardType, Hoverboards.Board>> list = new List<KeyValuePair<Hoverboards.BoardType, Hoverboards.Board>>();
		foreach (Hoverboards.BoardType item in Hoverboards.boardOrder)
		{
			Hoverboards.Board value = Hoverboards.boardData[item];
			list.Add(new KeyValuePair<Hoverboards.BoardType, Hoverboards.Board>(item, value));
		}
		bool flag = false;
		for (int i = 0; i < boardHelpers.Count; i++)
		{
			if (list[i].Key != boardHelpers[i].GetBoardType())
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
		foreach (BoardState scrollBoard in scrollBoards)
		{
			Object.Destroy(scrollBoard.boardRoot);
		}
		scrollBoards.Clear();
		foreach (HoverboardOverlayHelper boardHelper in boardHelpers)
		{
			Object.Destroy(boardHelper.gameObject);
		}
		boardHelpers.Clear();
		foreach (OverlayIndex boardIndex in boardIndices)
		{
			Object.Destroy(boardIndex.gameObject);
		}
		boardIndices.Clear();
		foreach (Transform item2 in scrollGrid.transform)
		{
			item2.gameObject.active = false;
			Object.Destroy(item2);
		}
		overlayParent.transform.localPosition = Vector3.zero;
		scrollAnchor.transform.localPosition = new Vector3(0f, scrollAnchor.transform.localPosition.y, scrollAnchor.transform.localPosition.z);
		scrollPanel.cachedTransform.localPosition = Vector3.zero;
		Vector4 clipRange = scrollPanel.clipRange;
		scrollPanel.clipRange = new Vector4(0f, clipRange.y, clipRange.z, clipRange.w);
		_centerer.ClearCenterObject();
		Object.Destroy(scrollPanel.GetComponent<SpringPanel>());
		InitBoards();
	}

	public void ScrollClicked(Vector2 pos)
	{
		if (_centerer.CenterOnClosestChildAtPosition(pos) && PlayerInfo.Instance.isHoverboardUnlocked(_currentlyShownBoard))
		{
			SelectCurrentlyShownBoard();
			NGUITools.PlaySound(selectSound);
		}
	}

	public void SelectCurrentlyShownBoard()
	{
		PlayerInfo.Instance.currentHoverboard = _currentlyShownBoard;
		HoverboardManager.Instance.Hoverboard = _currentlyShownBoard;
		UpdateNamesAndButtons();
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
			Transform boardTransform = scrollBoards[index].boardTransform;
			boardTransform.RotateAround(boardTransform.right, 0f - Time.deltaTime);
			Hoverboards.BoardType boardType = boardHelpers[index].GetBoardType();
			if (!_hasSetupScroll)
			{
				int num = 0;
				foreach (KeyValuePair<Hoverboards.BoardType, Hoverboards.Board> boardDatum in Hoverboards.boardData)
				{
					if (_currentlyShownBoard == boardDatum.Key)
					{
						break;
					}
					num++;
				}
				_centerer.CenterOnTransform(boardHelpers[num].transform, true);
				_currentlyShownBoard = boardType;
				_hasSetupScroll = true;
			}
			else if (_currentlyShownBoard != boardType)
			{
				_currentlyShownBoard = boardType;
				ShowHoverboardInMenu();
				boardHelpers[index].SelectedInMenu();
			}
			index = ((!(_centerer.centeredObject != null)) ? (-1) : _centerer.centeredObject.GetComponent<OverlayIndex>().index);
			for (int i = 0; i < scrollBoards.Count; i++)
			{
				if (i != index)
				{
					BoardState boardState = scrollBoards[i];
					Quaternion to = boardState.boardTransform.parent.rotation * boardState.defaultRotation;
					boardState.boardTransform.rotation = Quaternion.Lerp(boardState.boardTransform.rotation, to, Time.deltaTime);
				}
			}
		}
		float num2 = -1f * scrollAnchor.transform.localPosition.x;
		for (int j = 0; j < scrollBoards.Count; j++)
		{
			float num3 = Mathf.Abs(num2 - (float)j * _cellWidth);
			float num4 = 1.5f * _cellWidth;
			float num5 = Mathf.SmoothStep(10f, 7f, num3 / num4);
			boardHelpers[j].currentDistFromCenter = num3 / num4;
			scrollBoards[j].boardRoot.transform.localScale = Vector3.one * num5;
		}
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
			if (_modelsEnabled)
			{
				scrollAnchor.SetActiveRecursively(false);
				overlayParent.SetActiveRecursively(false);
				_modelsEnabled = false;
			}
		}
		else if (!_modelsEnabled)
		{
			scrollAnchor.SetActiveRecursively(true);
			overlayParent.SetActiveRecursively(true);
			_modelsEnabled = true;
		}
	}
}
