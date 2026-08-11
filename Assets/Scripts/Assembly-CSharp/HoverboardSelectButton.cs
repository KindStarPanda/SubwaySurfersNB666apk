using UnityEngine;

public class HoverboardSelectButton : MonoBehaviour
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

	private bool isEnabled;

	private bool _hasInited;

	private BoxCollider col;

	private Hoverboards.BoardType _boardType;

	public void SetupButton(Hoverboards.BoardType boardType)
	{
		if (!_hasInited)
		{
			col = GetComponent<BoxCollider>();
			_hasInited = true;
		}
		_boardType = boardType;
		if (PlayerInfo.Instance.currentHoverboard == _boardType)
		{
			showAndEnable();
			fillSprite.spriteName = fillSelected;
			label.text = textSelected;
			col.enabled = false;
		}
		else if (PlayerInfo.Instance.isHoverboardUnlocked(_boardType) || Hoverboards.boardData[_boardType].unlockType == Hoverboards.UnlockType.alwaysUnlocked)
		{
			showAndEnable();
			fillSprite.spriteName = fillSelect;
			label.text = textSelect;
			col.enabled = true;
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
			label.panel.Refresh();
			col.enabled = true;
			isEnabled = true;
		}
	}

	private void OnClick()
	{
		NGUITools.FindInParents<BoardScreen>(base.gameObject).SelectCurrentlyShownBoard();
	}
}
