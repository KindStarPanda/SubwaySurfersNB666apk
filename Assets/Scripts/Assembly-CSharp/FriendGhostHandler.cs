using System;
using UnityEngine;

public class FriendGhostHandler : MonoBehaviour
{
	public FriendGhostHelper helper;

	public Texture dummyImage;

	private Friend[] friendsDescending;

	private bool _localUserInserted;

	private bool _gameRunning;

	private int _currentThreshold;

	private int _currentFriend = -1;

	private Color localPlayerColor = new Color(1f, 73f / 85f, 0f, 1f);

	private Color friendColor = Color.white;

	private void Init()
	{
		Game instance = Game.Instance;
		instance.OnGameStarted = (Action)Delegate.Combine(instance.OnGameStarted, new Action(NewGame));
		Game instance2 = Game.Instance;
		instance2.OnGameEnded = (Action)Delegate.Combine(instance2.OnGameEnded, new Action(GameOver));
	}

	private void OnDestroy()
	{
		Game instance = Game.Instance;
		instance.OnGameStarted = (Action)Delegate.Remove(instance.OnGameStarted, new Action(NewGame));
		Game instance2 = Game.Instance;
		instance2.OnGameEnded = (Action)Delegate.Remove(instance2.OnGameEnded, new Action(GameOver));
	}

	private void Awake()
	{
		Init();
		NewGame();
	}

	public void NewGame()
	{
		Debug.Log("Friendghosthandler newgame. gameisRunning: " + _gameRunning);
		if (_gameRunning)
		{
		}
		_gameRunning = true;
		_localUserInserted = false;
		_currentThreshold = 0;
		GameStats.Instance.Reset();
		helper.GameOver();
		if (SocialManager.instance.consolidatedFriendsCompleted && (Social.localUser.authenticated || SocialManager.instance.facebookIsLoggedIn))
		{
			friendsDescending = SocialManager.instance.FriendsSortedByScore();
			if (friendsDescending != null)
			{
				_currentFriend = getNextFriendWithHigherScoreIndex(friendsDescending, GameStats.Instance.score);
			}
		}
		helper.NewGame();
		SetNewFriend();
		helper.AnimateIn();
	}

	private int getNextFriendWithHigherScoreIndex(Friend[] sortedFriendList, int score)
	{
		if (sortedFriendList == null)
		{
			return -1;
		}
		if (sortedFriendList.Length == 0)
		{
			return -1;
		}
		for (int num = sortedFriendList.Length - 1; num >= 0; num--)
		{
			if (sortedFriendList[num].score > score)
			{
				return num;
			}
		}
		return -1;
	}

	private void Update()
	{
		if (helper.currentAnimationState == FriendGhostHelper.AnimatingState.OnScreen && _gameRunning && !helper.noFriendsLeftToGhost && GameStats.Instance.score > _currentThreshold)
		{
			PassThreshold();
		}
	}

	private void PassThreshold()
	{
		helper.AnimateOut();
	}

	public void FinishedAnimatingOut()
	{
		if (_gameRunning)
		{
			SetNewFriend();
		}
	}

	public void SetNewFriend()
	{
		if (!_gameRunning)
		{
			return;
		}
		bool flag = false;
		if (friendsDescending != null || _currentFriend != -1)
		{
			int nextFriendWithHigherScoreIndex = getNextFriendWithHigherScoreIndex(friendsDescending, GameStats.Instance.score);
			_currentFriend = nextFriendWithHigherScoreIndex;
			if (_currentFriend == -1)
			{
				if (PlayerInfo.Instance.highestScore > GameStats.Instance.score && InsertLocalUser())
				{
					flag = true;
				}
			}
			else if (PlayerInfo.Instance.highestScore < friendsDescending[_currentFriend].score)
			{
				if (InsertLocalUser())
				{
					flag = true;
				}
				else if (InsertFriend())
				{
					flag = true;
				}
			}
			else if (InsertFriend())
			{
				flag = true;
			}
		}
		else if (!_localUserInserted && InsertLocalUser())
		{
			flag = true;
		}
		if (flag)
		{
			helper.AnimateIn();
		}
		else
		{
			helper.NoFriendsLeft();
		}
	}

	public void GameOver()
	{
		Debug.Log("Friendghost handler gameover");
		_gameRunning = false;
		helper.GameOver();
	}

	private bool InsertLocalUser()
	{
		if (PlayerInfo.Instance.highestScore > GameStats.Instance.score && !_localUserInserted)
		{
			if (SocialManager.instance.localUserImage != null)
			{
				helper.picture.material.mainTexture = SocialManager.instance.localUserImage;
			}
			else
			{
				helper.picture.material.mainTexture = dummyImage;
			}
			helper.points.text = PlayerInfo.Instance.highestScore.ToString();
			helper.points.color = localPlayerColor;
			_localUserInserted = true;
			_currentThreshold = PlayerInfo.Instance.highestScore;
			return true;
		}
		return false;
	}

	private bool InsertFriend()
	{
		int currentFriend = _currentFriend;
		if (currentFriend < 0 || currentFriend >= friendsDescending.Length)
		{
			Debug.LogError("Tried to insert a friend outside the array");
			return false;
		}
		Missions.Instance.PlayerDidThis(Missions.MissionTarget.BeatFriends);
		Friend friend = friendsDescending[currentFriend];
		if (friend.score > GameStats.Instance.score)
		{
			if (friend.image != null)
			{
				helper.picture.material.mainTexture = friend.image;
			}
			else
			{
				helper.picture.material.mainTexture = dummyImage;
			}
			helper.points.text = friend.score.ToString();
			helper.points.color = friendColor;
			_currentThreshold = friend.score;
			return true;
		}
		return false;
	}
}
