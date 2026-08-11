using System.Collections;
using UnityEngine;

public class FriendsOnlineScreen : UIScreen
{
	public GameObject FriendPrefab;

	public GameObject InvitePrefab;

	public GameObject FacebookLoginPrefab;

	public GameObject FacebookLoginNoBonusPrefab;

	public GameObject GameCenterLoginPrefab;

	public GameObject GameCenterLoginNoBonusPrefab;

	[SerializeField]
	private UILabel CrewHeader;

	[SerializeField]
	private UILabel NoFriends;

	[SerializeField]
	private UIGrid _grid;

	[SerializeField]
	private UIScrollBar _scrollBar;

	public override void Show()
	{
		base.Show();
		SocialManager.instance.AddFriendsConsolidatedHandler(ReloadFriends);
		ReloadFriends();
		SocialManager.instance.UpdateFriendScores(delegate
		{
			ReloadFriends();
		});
	}

	public override void Hide()
	{
		base.Hide();
		SocialManager.instance.RemoveFriendsConsolidatedHandler(ReloadFriends);
	}

	private void ReloadFriends()
	{
		foreach (Transform item in _grid.transform)
		{
			NGUITools.SetActive(item.gameObject, false);
			Object.Destroy(item.gameObject);
		}
		Friend[] array = SocialManager.instance.FriendsSortedByCash();
		Debug.Log("number of friends: " + array.Length);
		bool flag = false;
		bool flag2 = false;
		bool dummyFriendShouldShow = PlayerInfo.Instance.dummyFriendShouldShow;
		int num = -1;
		if (!flag && (bool)FacebookLoginPrefab && (bool)FacebookLoginNoBonusPrefab && !SocialManager.instance.facebookIsLoggedIn)
		{
			GameObject gameObject = ((!PlayerInfo.Instance.hasPayedOutFacebook) ? NGUITools.AddChild(_grid.gameObject, FacebookLoginPrefab) : NGUITools.AddChild(_grid.gameObject, FacebookLoginNoBonusPrefab));
			gameObject.name = string.Format("{0:000}fb", num);
			num++;
			flag = true;
		}
		if (num < 0)
		{
			num = 0;
		}
		if (dummyFriendShouldShow && !PlayerInfo.Instance.dummyFriendCollected)
		{
			GameObject gameObject2 = NGUITools.AddChild(_grid.gameObject, FriendPrefab);
			gameObject2.name = string.Format("{0:000000}", num);
			FriendHelperCrew component = gameObject2.GetComponent<FriendHelperCrew>();
			component.InitDummyFriend(true, num % 2 == 0);
			num++;
			dummyFriendShouldShow = false;
		}
		for (int i = 0; i < array.Length; i++)
		{
			GameObject gameObject3 = NGUITools.AddChild(_grid.gameObject, FriendPrefab);
			gameObject3.name = string.Format("{0:000000}", num);
			FriendHelperCrew component2 = gameObject3.GetComponent<FriendHelperCrew>();
			component2.InitFriend(array[i], num % 2 == 0);
			num++;
		}
		if (SocialManager.instance.facebookIsLoggedIn)
		{
			GameObject gameObject4 = NGUITools.AddChild(_grid.gameObject, InvitePrefab);
			gameObject4.name = "invite";
		}
		if (num == -1)
		{
			NoFriends.alpha = 1f;
			NoFriends.gameObject.active = true;
		}
		else
		{
			NoFriends.alpha = 0f;
			NoFriends.gameObject.active = false;
		}
		CrewHeader.text = "Friends (" + num + ")";
		_grid.sorted = false;
		_grid.SendMessage("Start", SendMessageOptions.DontRequireReceiver);
		base.gameObject.BroadcastMessage("CreatePanel", SendMessageOptions.DontRequireReceiver);
		StartCoroutine(MoveScrollBarAtTop(2));
	}

	public IEnumerator MoveScrollBarAtTop(int delayFrames)
	{
		int num = 0;
		while (num < delayFrames)
		{
			num++;
			yield return null;
		}
		if (_scrollBar != null)
		{
			_scrollBar.scrollValue = 0f;
		}
		else
		{
			Debug.Log("Scroll bar not set in FriendsUI_Online prefab");
		}
	}
}
