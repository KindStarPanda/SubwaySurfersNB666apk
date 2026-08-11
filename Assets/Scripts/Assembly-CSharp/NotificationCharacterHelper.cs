using System.Collections.Generic;
using UnityEngine;

public class NotificationCharacterHelper : MonoBehaviour
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
		if (!_hasGoneToZero && UIScreenController.Instance.GetTopScreenName() == "CharacterScreen")
		{
			RefreshNotification();
		}
	}

	private void RefreshNotification()
	{
		int num = 0;
		PlayerInfo.Season currentSeasonAvailable = PlayerInfo.Instance.currentSeasonAvailable;
		List<Characters.CharacterType> list = new List<Characters.CharacterType>();
		foreach (Characters.CharacterType item in Characters.characterOrder)
		{
			Characters.Model model = Characters.characterData[item];
			if (PlayerInfo.Instance.IsCollectionComplete(item) || model.characterSeason == currentSeasonAvailable || model.characterSeason == PlayerInfo.Season.none)
			{
				list.Add(item);
			}
		}
		foreach (Characters.CharacterType item2 in list)
		{
			if (PlayerInfo.Instance.IsCharacterNew(item2))
			{
				num++;
			}
		}
		if (num > 0)
		{
			notificationIcon.enabled = true;
			notificationLabel.enabled = true;
			notificationLabel.text = num.ToString();
		}
		else
		{
			notificationIcon.enabled = false;
			notificationLabel.enabled = false;
			_hasGoneToZero = true;
		}
	}
}
