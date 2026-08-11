using UnityEngine;

public class TutorialButton : MonoBehaviour
{
	public enum ButtonAction
	{
		Ok = 0,
		Cancel = 1
	}

	public enum TutorialPopupType
	{
		_notSet = 0,
		Missions1 = 1,
		Missions2 = 2,
		Facebook = 3,
		CollectFromFriends = 4,
		Hoverboards = 5,
		ChangeLog = 6,
		ChangeLogEndGame = 7
	}

	private const int NUMBER_OF_FREE_HOVERBOARDS = 3;

	[SerializeField]
	private ButtonAction buttonAction;

	public TutorialPopupType tutorialType;

	private void OnClick()
	{
		if (buttonAction == ButtonAction.Ok)
		{
			if (tutorialType == TutorialPopupType.Missions1)
			{
				UIScreenController.Instance.QueuePopup("Mission_popup");
			}
			else if (tutorialType == TutorialPopupType.Missions2)
			{
				UIScreenController.Instance.QueuePopup("Mission_popup");
			}
			else if (tutorialType == TutorialPopupType.Facebook)
			{
				UIScreenController.Instance.PushScreen(null, "FriendsUI");
			}
			else if (tutorialType == TutorialPopupType.CollectFromFriends)
			{
				PlayerInfo.Instance.dummyFriendShouldShow = true;
				UIScreenController.Instance.PushScreen(null, "FriendsUI");
			}
			else if (tutorialType == TutorialPopupType.Hoverboards)
			{
				UIScreenController.Instance.QueuePopup("HoverboardPopup");
				PlayerInfo.Instance.IncreaseUpgradeAmount(PowerupType.hoverboard, 3);
			}
			else if (tutorialType != TutorialPopupType.ChangeLog)
			{
				if (tutorialType == TutorialPopupType.ChangeLogEndGame)
				{
					UIScreenController.Instance.QueuePopup("Mission_popup");
				}
				else
				{
					Debug.LogError("tutorialType was not defined in " + base.gameObject.name, base.gameObject);
				}
			}
		}
		SendFlurryEvent();
		UIScreenController.Instance.ClosePopup();
	}

	private void SendFlurryEvent()
	{
	}
}
