using System;
using System.Collections.Generic;
using UnityEngine;

public class FriendHandlerBrag : MonoBehaviour
{
	public enum LogInState
	{
		_notset = 0,
		Both = 1,
		Facebook = 2,
		GameCenter = 3
	}

	[SerializeField]
	private GameObject friendBragPrefab;

	[SerializeField]
	private GameObject friendNoBragPrefab;

	[SerializeField]
	private GameObject facebookLoginPrefab;

	[SerializeField]
	private GameObject facebookLoginNoBonusPrefab;

	[SerializeField]
	private GameObject gameCenterLoginPrefab;

	[SerializeField]
	private GameObject gameCenterLoginNoBonusPrefab;

	[SerializeField]
	private BragButtonHelper bragButton;

	[SerializeField]
	private GameObject facebookBonus;

	[SerializeField]
	private GameObject gameCenterBonus;

	[SerializeField]
	private GameObject OfflineParent;

	[SerializeField]
	private GameObject OnlineFacebookParent;

	[SerializeField]
	private GameObject OnlineGameCenterParent;

	[SerializeField]
	private GameObject OnlineBothBragParent;

	[SerializeField]
	private GameObject OnlineBothNoBragParent;

	[SerializeField]
	private UIGrid OnlineFacebookGrid;

	[SerializeField]
	private UIGrid OnlineGameCenterGrid;

	[SerializeField]
	private UIGrid OnlineBothBragGrid;

	[SerializeField]
	private UIGrid OnlineBothNoBragGrid;

	[SerializeField]
	private GameObject FacebookLoginButtonParent;

	[SerializeField]
	private GameObject GameCenterLoginButtonParent;

	[SerializeField]
	private UILabel gettingLabel;

	private UIGrid _currentGrid;

	private List<Friend> _bragList = new List<Friend>();

	[NonSerialized]
	public bool bragNotifyDone;

	[NonSerialized]
	public bool bragFacebookDone;

	[NonSerialized]
	public string preBragPopupString = string.Empty;

	private bool haveShownPreBragPopupThisRun;

	private static FriendHandlerBrag _instance;

	public List<Friend> bragList
	{
		get
		{
			if (_bragList != null)
			{
				return _bragList;
			}
			return null;
		}
	}

	public static FriendHandlerBrag instance
	{
		get
		{
			if (_instance == null)
			{
				Debug.LogError("FriendHandlerBrag _instance was null when you tried to access it!");
			}
			return _instance;
		}
	}

	private void ClearAllGrids()
	{
		foreach (Transform item in OnlineBothBragGrid.transform)
		{
			NGUITools.SetActive(item.gameObject, false);
			UnityEngine.Object.Destroy(item.gameObject);
		}
		foreach (Transform item2 in OnlineBothNoBragGrid.transform)
		{
			NGUITools.SetActive(item2.gameObject, false);
			UnityEngine.Object.Destroy(item2.gameObject);
		}
		foreach (Transform item3 in OnlineFacebookGrid.transform)
		{
			NGUITools.SetActive(item3.gameObject, false);
			UnityEngine.Object.Destroy(item3.gameObject);
		}
		foreach (Transform item4 in OnlineGameCenterGrid.transform)
		{
			NGUITools.SetActive(item4.gameObject, false);
			UnityEngine.Object.Destroy(item4.gameObject);
		}
	}

	public void ShowGettingReadyLabel()
	{
		ClearEverything();
		NGUITools.SetActive(gettingLabel.gameObject, true);
		haveShownPreBragPopupThisRun = false;
	}

	public void ClearEverything()
	{
		ClearAllGrids();
		NGUITools.SetActive(OnlineBothBragParent, false);
		NGUITools.SetActive(OnlineBothNoBragParent, false);
		NGUITools.SetActive(OnlineFacebookParent, false);
		NGUITools.SetActive(OnlineGameCenterParent, false);
		NGUITools.SetActive(OfflineParent, false);
		NGUITools.SetActive(gettingLabel.gameObject, false);
		foreach (Transform item in FacebookLoginButtonParent.transform)
		{
			UnityEngine.Object.Destroy(item.gameObject);
		}
		foreach (Transform item2 in GameCenterLoginButtonParent.transform)
		{
			UnityEngine.Object.Destroy(item2.gameObject);
		}
	}

	public void GoOffline(bool enableButtons)
	{
		ClearEverything();
		NGUITools.SetActive(OfflineParent, true);
		OfflineParent.BroadcastMessage("CreatePanel", SendMessageOptions.DontRequireReceiver);
		if (enableButtons)
		{
			OfflineParent.GetComponent<GameOverOfflineHelper>().EnableButtons();
		}
		else
		{
			OfflineParent.GetComponent<GameOverOfflineHelper>().DisableButtons();
		}
		if (PlayerInfo.Instance.hasPayedOutFacebook)
		{
			NGUITools.SetActive(facebookBonus, false);
		}
		if (PlayerInfo.Instance.hasPayedOutGameCenter)
		{
			NGUITools.SetActive(gameCenterBonus, false);
		}
	}

	private LogInState GetLoginState()
	{
		LogInState logInState = LogInState._notset;
		if (Social.localUser.authenticated)
		{
			logInState = LogInState.GameCenter;
		}
		if (SocialManager.instance.facebookIsLoggedIn)
		{
			logInState = ((logInState == LogInState.GameCenter) ? LogInState.Both : LogInState.Facebook);
		}
		return logInState;
	}

	private void SetCurrentGridWhenLoginStateBoth(Friend[] friends)
	{
		bool flag = false;
		for (int i = 0; i < friends.Length; i++)
		{
			if (friends[i].score <= PlayerInfo.Instance.highestScore && friends[i].score > PlayerInfo.Instance.oldHighestScore)
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			_currentGrid = OnlineBothBragGrid;
			NGUITools.SetActive(OnlineBothBragParent, true);
		}
		else
		{
			_currentGrid = OnlineBothNoBragGrid;
			NGUITools.SetActive(OnlineBothNoBragParent, true);
		}
	}

	private void SetCurrentGridWhenLoginStateGameCenter(Friend[] friends)
	{
	}

	private void SetCurrentGridWhenLoginStateFacebook(Friend[] friends)
	{
		_currentGrid = OnlineFacebookGrid;
		NGUITools.SetActive(OnlineFacebookParent, true);
	}

	private void AddFriendsIntoGrid(Friend[] friends, Transform cursorTransform)
	{
		bool flag = false;
		GameObject prefab = friendNoBragPrefab;
		if (_currentGrid == OnlineBothBragGrid)
		{
			prefab = friendBragPrefab;
		}
		int num = 1;
		for (int i = 0; i < friends.Length; i++)
		{
			GameObject gameObject = NGUITools.AddChild(_currentGrid.gameObject, prefab);
			gameObject.name = string.Format("{0:000}", num);
			FriendHelperBrag component = gameObject.GetComponent<FriendHelperBrag>();
			if (!flag && PlayerInfo.Instance.highestScore >= friends[i].score)
			{
				if (i == 0)
				{
					cursorTransform = gameObject.transform;
				}
				component.InitLocalUser(num, num % 2 == 1);
				num++;
				flag = true;
				gameObject = NGUITools.AddChild(_currentGrid.gameObject, prefab);
				gameObject.name = string.Format("{0:000}", num);
				component = gameObject.GetComponent<FriendHelperBrag>();
			}
			bool flag2 = _currentGrid == OnlineBothBragGrid && friends[i].score <= PlayerInfo.Instance.highestScore && friends[i].score > PlayerInfo.Instance.oldHighestScore && (bool)_currentGrid;
			component.InitFriend(friends[i], num, flag2, num % 2 == 1);
			if (!flag)
			{
				cursorTransform = gameObject.transform;
			}
			if (flag2 && !_bragList.Contains(friends[i]))
			{
				AddBragFriend(friends[i]);
			}
			num++;
		}
		if (!flag)
		{
			GameObject gameObject2 = NGUITools.AddChild(_currentGrid.gameObject, prefab);
			gameObject2.name = string.Format("{0:000}", num);
			FriendHelperBrag component2 = gameObject2.GetComponent<FriendHelperBrag>();
			cursorTransform = gameObject2.transform;
			component2.InitLocalUser(num, num % 2 == 1);
			num++;
			flag = true;
		}
	}

	private void SetPropertiesWhenCurrentGridIsOnlineBothNoBragGrid(Transform cursorTransform)
	{
		bragNotifyDone = false;
		bragFacebookDone = false;
		Debug.Log("Should show 4 players now.");
		_currentGrid.sorted = false;
		_currentGrid.gameObject.SendMessage("Start");
		_currentGrid.Reposition();
		_currentGrid.gameObject.SendMessage("Update");
		UIPanel component = _currentGrid.transform.parent.GetComponent<UIPanel>();
		Vector3 localPosition = component.transform.localPosition;
		Vector3 localPosition2 = localPosition;
		localPosition2.y = 0f;
		component.transform.localPosition = localPosition2;
		Vector4 clipRange = component.clipRange;
		clipRange.y += localPosition.y;
		component.clipRange = clipRange;
		component.GetComponent<UIDraggablePanel>().RestrictWithinBounds(true);
		_currentGrid.gameObject.BroadcastMessage("CreatePanel", SendMessageOptions.DontRequireReceiver);
	}

	private void SetPropertiesWhenCurrentGridIsOnlineBothBragGrid()
	{
		if (PlayerInfo.Instance.highestScore < PlayerInfo.Instance.oldHighestScore)
		{
			return;
		}
		if (Settings.optionAutoMessage)
		{
			SocialManager.instance.BragNotify(PlayerInfo.Instance.highestScore, bragList);
			bragNotifyDone = true;
		}
		preBragPopupString = "You beat";
		string text;
		if (_bragList.Count > 0)
		{
			if (_bragList.Count == 1)
			{
				preBragPopupString = preBragPopupString + " " + _bragList[0].name;
			}
			else if (_bragList.Count == 2)
			{
				text = preBragPopupString;
				preBragPopupString = text + " " + _bragList[0].name + " and " + _bragList[1].name;
			}
			else
			{
				int num = bragList.Count - 1;
				text = preBragPopupString;
				preBragPopupString = text + " " + _bragList[0].name + " and " + num + " other friend" + ((num <= 1) ? string.Empty : "s");
			}
		}
		text = preBragPopupString;
		preBragPopupString = text + " with a score of " + PlayerInfo.Instance.highestScore + "!";
		if (!haveShownPreBragPopupThisRun)
		{
			UIScreenController.Instance.QueuePopup("PreBragPopup");
			haveShownPreBragPopupThisRun = true;
		}
	}

	public void ShowFriendList()
	{
		Transform parent = base.transform.parent;
		Friend[] array = SocialManager.instance.FriendsSortedByScore();
		LogInState loginState = GetLoginState();
		if (loginState == LogInState.Both || loginState == LogInState.Facebook)
		{
			SetCurrentGridWhenLoginStateBoth(array);
			AddFriendsIntoGrid(array, parent);
			if (bragList.Count == 0)
			{
				bragButton.DisableButton();
			}
			else
			{
				bragButton.EnableButton();
			}
			if (_currentGrid == OnlineBothNoBragGrid)
			{
				SetPropertiesWhenCurrentGridIsOnlineBothNoBragGrid(parent);
			}
			else
			{
				Debug.Log("430");
				_currentGrid.sorted = false;
				_currentGrid.gameObject.SendMessage("Start");
				_currentGrid.Reposition();
				_currentGrid.gameObject.SendMessage("Update");
				UIPanel component = _currentGrid.transform.parent.GetComponent<UIPanel>();
				Vector3 localPosition = component.transform.localPosition;
				Vector3 localPosition2 = localPosition;
				localPosition2.y = 0f;
				component.transform.localPosition = localPosition2;
				Vector4 clipRange = component.clipRange;
				clipRange.y += localPosition.y;
				component.clipRange = clipRange;
				component.GetComponent<UIDraggablePanel>().RestrictWithinBounds(true);
				_currentGrid.gameObject.BroadcastMessage("CreatePanel", SendMessageOptions.DontRequireReceiver);
			}
			if (_currentGrid == OnlineBothBragGrid)
			{
				SetPropertiesWhenCurrentGridIsOnlineBothBragGrid();
			}
		}
		else
		{
			Debug.LogError("Tried to show friends while offline.", this);
		}
	}

	public void AddBragFriend(Friend friend)
	{
		if (!_bragList.Contains(friend))
		{
			_bragList.Add(friend);
			if (!bragButton.buttonEnabled)
			{
				bragButton.EnableButton();
			}
		}
	}

	public void RemoveBragFriend(Friend friend)
	{
		if (_bragList.Contains(friend))
		{
			_bragList.Remove(friend);
			if (_bragList.Count == 0)
			{
				bragButton.DisableButton();
			}
		}
	}

	public void CompletedBrag()
	{
		bragButton.DisableButton();
		_currentGrid.gameObject.BroadcastMessage("CompletedBragging", SendMessageOptions.DontRequireReceiver);
		_bragList.Clear();
		PlayerInfo.Instance.BragCompleted();
		ClearEverything();
		ShowFriendList();
	}

	private void MovedAwayFromGameOverScreenClicked()
	{
		PlayerInfo.Instance.BragCompleted();
		_bragList.Clear();
	}

	private void OnDisable()
	{
		MovedAwayFromGameOverScreenClicked();
	}

	private void Awake()
	{
		_instance = this;
	}
}
