using UnityEngine;

public class BoardsNotificationHelper : MonoBehaviour
{
	[SerializeField]
	private UISprite notificationIcon;

	[SerializeField]
	private UILabel notificationLabel;

	private bool _hasGoneToZero;

	private void OnEnable()
	{
		RefreshNotification();
	}

	private void Update()
	{
		if (!_hasGoneToZero && UIScreenController.Instance.GetTopScreenName() == "BoardScreen")
		{
			RefreshNotification();
		}
	}

	private void RefreshNotification()
	{
		int num = 0;
		foreach (Hoverboards.BoardType item in Hoverboards.boardOrder)
		{
			Hoverboards.Board board = Hoverboards.boardData[item];
			if (board.isNewInThisUpdate && board.season == PlayerInfo.Season.none && !PlayerInfo.Instance.HasHoverboardBeenSeen(item))
			{
				num++;
			}
		}
		if (UIScreenController.Instance.GetTopScreenName() == "BoardScreen")
		{
			if (num == 0)
			{
				DeactivateNotification();
				_hasGoneToZero = true;
			}
			else
			{
				ActivateNotification(num);
			}
		}
		else if (PlayerInfo.Instance.ShouldShowBreadCrumb())
		{
			DeactivateNotification();
		}
		else if (num == 0)
		{
			DeactivateNotification();
			_hasGoneToZero = true;
		}
		else
		{
			ActivateNotification(num);
		}
	}

	private void ActivateNotification(int numberOfNewBoards)
	{
		notificationIcon.enabled = true;
		notificationLabel.enabled = true;
		notificationLabel.text = numberOfNewBoards.ToString();
	}

	private void DeactivateNotification()
	{
		notificationIcon.enabled = false;
		notificationLabel.enabled = false;
		notificationLabel.text = string.Empty;
	}
}
