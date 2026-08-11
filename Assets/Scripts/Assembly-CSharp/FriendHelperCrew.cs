using System;
using UnityEngine;

public class FriendHelperCrew : MonoBehaviour
{
	private const string PROGRESS_STRING = "{0} / {1} runs";

	private const int RUNS_TO_COLLECT = 50;

	private const int REWARD_MIN = 50;

	private const int REWARD_MAX = 350;

	private const string DUMMY_FRIEND_NAME = "Friend Bonus";

	public GameObject collectButtonPrefab;

	public GameObject progressPrefab;

	public UILabel friendName;

	public UITexture friendPicture;

	public UISlicedSprite friendBackground;

	public FriendCrewPokeHelper pokeHelper;

	private GameObject _collectionIndicator;

	public Texture2D dummyImage;

	private bool _imageSet;

	private Friend _friend;

	private bool _initialized;

	private bool _isDummyFriend;

	public Texture2D dummyFriendImage;

	public void InitDummyFriend(bool collectible, bool backgroundActive = false)
	{
		_isDummyFriend = true;
		if (!backgroundActive)
		{
			friendBackground.alpha = 0f;
		}
		else
		{
			friendBackground.alpha = 0.2f;
		}
		friendName.text = "Friend Bonus";
		friendPicture.material = new Material(Shader.Find("Unlit/Transparent Colored"));
		friendPicture.material.mainTexture = dummyFriendImage;
		_imageSet = true;
		GameObject gameObject;
		if (collectible)
		{
			gameObject = NGUITools.AddChild(base.gameObject, collectButtonPrefab);
			gameObject.GetComponent<UIButtonMessage>().target = base.gameObject;
			_collectionIndicator = gameObject;
			NGUITools.AddWidgetCollider(gameObject);
			pokeHelper.DeactivatePoke();
		}
		else
		{
			gameObject = NGUITools.AddChild(base.gameObject, progressPrefab);
			FriendProgressHelper component = gameObject.GetComponent<FriendProgressHelper>();
			component.label.text = string.Format("{0} / {1} runs", 0, 50);
			component.slider.sliderValue = 0f;
			pokeHelper.DeactivatePoke();
		}
		_collectionIndicator = gameObject;
		_initialized = true;
	}

	public void InitFriend(Friend friend, bool backgroundActive = false)
	{
		_friend = friend;
		if (!backgroundActive)
		{
			friendBackground.alpha = 0f;
		}
		else
		{
			friendBackground.alpha = 0.2f;
		}
		friendName.text = _friend.name;
		friendPicture.material = new Material(Shader.Find("Unlit/Transparent Colored"));
		if (_friend.image != null)
		{
			friendPicture.material.mainTexture = _friend.image;
			_imageSet = true;
		}
		else
		{
			friendPicture.material.mainTexture = dummyImage;
		}
		GameObject gameObject;
		if (_friend.gamesToCashIn >= 50)
		{
			gameObject = NGUITools.AddChild(base.gameObject, collectButtonPrefab);
			gameObject.GetComponent<UIButtonMessage>().target = base.gameObject;
			_collectionIndicator = gameObject;
			pokeHelper.DeactivatePoke();
		}
		else
		{
			gameObject = NGUITools.AddChild(base.gameObject, progressPrefab);
			FriendProgressHelper component = gameObject.GetComponent<FriendProgressHelper>();
			component.label.text = string.Format("{0} / {1} runs", Mathf.Clamp(_friend.gamesToCashIn, 0, 50), 50);
			component.slider.sliderValue = (float)_friend.gamesToCashIn / 50f;
			if ((DateTime.UtcNow - _friend.status.lastPokeTime).Days > 0)
			{
				if (_friend.status.lastPokeTime == DateTime.MinValue)
				{
					SocialManager.instance.SetPokeFirstTime(_friend);
					pokeHelper.DeactivatePoke();
				}
				else
				{
					pokeHelper.ActivatePoke(_friend);
				}
			}
			else
			{
				pokeHelper.DeactivatePoke();
			}
		}
		_collectionIndicator = gameObject;
		_initialized = true;
	}

	public void CollectReward()
	{
		if (_isDummyFriend)
		{
			Debug.Log("Collecting reward from dummy friend");
			int num = UnityEngine.Random.Range(50, 350);
			PlayerInfo.Instance.amountOfCoins += num;
			NGUITools.SetActive(_collectionIndicator, false);
			UnityEngine.Object.Destroy(_collectionIndicator);
			_collectionIndicator = NGUITools.AddChild(base.gameObject, progressPrefab);
			FriendProgressHelper component = _collectionIndicator.GetComponent<FriendProgressHelper>();
			component.label.text = string.Format("{0} / {1} runs", 0, 50);
			component.slider.sliderValue = 0f;
			UIScreenController.Instance.SpawnCollectText(component.GetCoinPouchGlobalPosition(), num.ToString());
			PlayerInfo.Instance.dummyFriendCollected = true;
		}
		else
		{
			Debug.Log("Collecting reward");
			Missions.Instance.PlayerDidThis(Missions.MissionTarget.CollectCoinPouch);
			SocialManager.instance.CollectFriendReward(_friend);
			int num2 = UnityEngine.Random.Range(50, 350);
			PlayerInfo.Instance.amountOfCoins += num2;
			NGUITools.SetActive(_collectionIndicator, false);
			UnityEngine.Object.Destroy(_collectionIndicator);
			_collectionIndicator = NGUITools.AddChild(base.gameObject, progressPrefab);
			FriendProgressHelper component2 = _collectionIndicator.GetComponent<FriendProgressHelper>();
			component2.label.text = string.Format("{0} / {1} runs", _friend.gamesToCashIn, 50);
			component2.slider.sliderValue = _friend.gamesToCashIn / 50;
			UIScreenController.Instance.SpawnCollectText(component2.GetCoinPouchGlobalPosition(), num2.ToString());
			SocialManager.instance.Save();
			Flurry.LogEvent("Friend reward collect");
		}
	}

	private void Update()
	{
		if (_initialized && !_imageSet && _friend.image != null)
		{
			friendPicture.material.mainTexture = _friend.image;
			_imageSet = true;
		}
	}
}
