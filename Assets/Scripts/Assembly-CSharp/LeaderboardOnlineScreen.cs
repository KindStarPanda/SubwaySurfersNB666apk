using System.Collections;
using UnityEngine;

public class LeaderboardOnlineScreen : UIScreen
{
	public GameObject FriendPrefab;

	public GameObject FacebookLoginPrefab;

	public GameObject FacebookLoginNoBonusPrefab;

	public GameObject GameCenterLoginPrefab;

	public GameObject GameCenterLoginNoBonusPrefab;

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
		base.transform.parent.GetComponent<UIPanel>().widgetsAreStatic = false;
		foreach (Transform item in _grid.transform)
		{
			NGUITools.SetActive(item.gameObject, false);
			Object.Destroy(item.gameObject);
		}
		Friend[] array = SocialManager.instance.FriendsSortedByScore();
		Transform transform2 = base.transform;
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		int num = 1;
		for (int i = 0; i < array.Length; i++)
		{
			GameObject gameObject = NGUITools.AddChild(_grid.gameObject, FriendPrefab);
			gameObject.name = string.Format("{0:000}", num);
			FriendHelperHighScore component = gameObject.GetComponent<FriendHelperHighScore>();
			if (!flag && PlayerInfo.Instance.highestScore >= array[i].score)
			{
				component.InitLocalUser(num, num % 2 == 1);
				num++;
				flag = true;
				if (i == 0)
				{
					transform2 = gameObject.transform;
				}
				gameObject = null;
				component = null;
				if (!flag2 && (bool)FacebookLoginPrefab && (bool)FacebookLoginNoBonusPrefab && !SocialManager.instance.facebookIsLoggedIn)
				{
					GameObject gameObject2 = ((!PlayerInfo.Instance.hasPayedOutFacebook) ? NGUITools.AddChild(_grid.gameObject, FacebookLoginPrefab) : NGUITools.AddChild(_grid.gameObject, FacebookLoginNoBonusPrefab));
					gameObject2.name = string.Format("{0:000}fb", num);
					flag2 = true;
				}
				gameObject = NGUITools.AddChild(_grid.gameObject, FriendPrefab);
				component = gameObject.GetComponent<FriendHelperHighScore>();
				gameObject.name = string.Format("{0:000}", num);
			}
			if (!flag)
			{
				transform2 = gameObject.transform;
			}
			component.InitFriend(array[i], num, num % 2 == 1);
			num++;
		}
		if (!flag)
		{
			GameObject gameObject3 = NGUITools.AddChild(_grid.gameObject, FriendPrefab);
			gameObject3.name = string.Format("{0:000}", num);
			FriendHelperHighScore component2 = gameObject3.GetComponent<FriendHelperHighScore>();
			transform2 = gameObject3.transform;
			component2.InitLocalUser(num, num % 2 == 1);
			num++;
			flag = true;
			if (!flag2 && (bool)FacebookLoginPrefab && (bool)FacebookLoginNoBonusPrefab && !SocialManager.instance.facebookIsLoggedIn)
			{
				GameObject gameObject4 = ((!PlayerInfo.Instance.hasPayedOutFacebook) ? NGUITools.AddChild(_grid.gameObject, FacebookLoginPrefab) : NGUITools.AddChild(_grid.gameObject, FacebookLoginNoBonusPrefab));
				gameObject4.name = string.Format("{0:000}fb", num);
				flag2 = true;
			}
		}
		UIPanel component3 = _grid.transform.parent.GetComponent<UIPanel>();
		Vector3 localPosition = component3.transform.localPosition;
		Vector4 clipRange = component3.clipRange;
		clipRange.y += localPosition.y;
		component3.clipRange = clipRange;
		Vector3 localPosition2 = localPosition;
		localPosition2.y = 0f;
		component3.transform.localPosition = localPosition2;
		_grid.sorted = false;
		_grid.SendMessage("Start", SendMessageOptions.DontRequireReceiver);
		component3.GetComponent<UIDraggablePanel>().RestrictWithinBounds(true);
		base.gameObject.BroadcastMessage("CreatePanel", SendMessageOptions.DontRequireReceiver);
		StartCoroutine(MoveScrollBarAtTop(2));
		StartCoroutine(SetStatic());
	}

	private IEnumerator SetStatic()
	{
		yield return null;
		_grid.transform.parent.GetComponent<UIPanel>().widgetsAreStatic = true;
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
