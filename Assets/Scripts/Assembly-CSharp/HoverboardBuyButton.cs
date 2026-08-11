using UnityEngine;

public class HoverboardBuyButton : MonoBehaviour
{
	[SerializeField]
	private UILabel priceLabel;

	[SerializeField]
	private UILabel expirePriceLabel;

	[SerializeField]
	private UILabel expireTimeLabel;

	private BoxCollider col;

	private bool isEnabled = true;

	private bool _purchaseInProgress;

	private Hoverboards.BoardType _boardType;

	private bool _hasInited;

	private Characters.CharacterType _currentCharacter;

	public void SetupButton(Hoverboards.BoardType boardType)
	{
		if (!_hasInited)
		{
			col = GetComponent<BoxCollider>();
			isEnabled = true;
			_hasInited = true;
		}
		_boardType = boardType;
		Hoverboards.Board board = Hoverboards.boardData[_boardType];
		if (PlayerInfo.Instance.isHoverboardUnlocked(_boardType) || Hoverboards.boardData[_boardType].unlockType == Hoverboards.UnlockType.alwaysUnlocked)
		{
			hideAndDisable();
			return;
		}
		int price = board.price;
		priceLabel.text = price.ToString();
		expirePriceLabel.text = price.ToString();
		priceLabel.enabled = true;
		expirePriceLabel.enabled = false;
		expireTimeLabel.enabled = false;
		priceLabel.SendMessage("CreatePanel", SendMessageOptions.DontRequireReceiver);
		showAndEnable();
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
		expireTimeLabel.gameObject.active = false;
		expirePriceLabel.gameObject.active = false;
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
			if (priceLabel.enabled)
			{
				priceLabel.panel.Refresh();
			}
			else
			{
				expirePriceLabel.panel.Refresh();
			}
			col.enabled = true;
			isEnabled = true;
		}
	}

	private void OnClick()
	{
		if (!_purchaseInProgress)
		{
			_purchaseInProgress = true;
			PurchaseHandler.Instance.PurchaseHoverboardType(_boardType, this);
		}
	}

	public void PurchaseSuccessful()
	{
		_purchaseInProgress = false;
		NGUITools.FindInParents<BoardScreen>(base.gameObject).SelectCurrentlyShownBoard();
		Hoverboards.Board value;
		if (Hoverboards.boardData.TryGetValue(PlayerInfo.Instance.currentHoverboard, out value))
		{
			Flurry.LogEventWithAParameter("Hoverboard bought", "Id", char.ToUpper(value.name[0]) + value.name.Substring(1));
		}
		if (value.unlockType == Hoverboards.UnlockType.free)
		{
			Flurry.LogEventWithAParameter("Bread crumbs before purchasing the free hoverboard", "Total", PlayerInfo.Instance.numberOfBreadCrumbsShownOnFrontPage + string.Empty);
		}
	}

	public void PurchaseFailure()
	{
		_purchaseInProgress = false;
	}
}
