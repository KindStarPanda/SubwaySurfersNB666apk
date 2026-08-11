using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public class PlayerInfo
{
	public enum Season
	{
		none = 0,
		halloween = 1,
		xmas = 2,
		easter = 3
	}

	private enum Key
	{
		AmountOfCoins = 0,
		OldHighestScore = 1,
		HighestScore = 2,
		DailyWord = 3,
		DailyWordUnlockMask = 4,
		DailyWordExpireTime = 5,
		DailyWordPayedOutTime = 6,
		CurrentCharacter = 7,
		CurrentMissionSet = 8,
		CurrentMissionSetProgress = 9,
		CollectedCharacterTokens = 10,
		AmountOfMysteryBoxesOpened = 11,
		TutorialCompleted = 12,
		InAppPurchaseCount = 13,
		EarnCurrencyData = 14,
		PayBonusFacebook = 15,
		PayBonusGameCenter = 16,
		Count = 17,
		UnlockedTrophies = 18,
		AchievementProgress = 19,
		DoubleCoins = 20,
		DailyWordInRow = 21,
		MissionCompletedSum = 22,
		NumberOfRunsSinceLastGuideline = 23,
		HasShownHoverboardPopup = 24,
		ShouldShowHoverboardPopup = 25,
		HasShownFacebookPopup = 26,
		ShouldShowFacebookPopup = 27,
		HasShownMission1Popup = 28,
		ShouldShowMission1Popup = 29,
		HasShownCollectPopup = 30,
		ShouldShowCollectPopup = 31,
		HasShownMission2Popup = 32,
		ShouldShowMission2Popup = 33,
		DummyFriendCollected = 34,
		DummyFriendShouldShow = 35,
		DailyWordLastPayoutDayOfYear = 36,
		AmountOfSuperMysteryBoxesOpened = 37,
		SeasonPicked = 38,
		XmasSeasonEndDate = 39,
		IsNew_tasha = 40,
		IsNew_zoe = 41,
		IsNew_brody = 42,
		IsNew_princek = 43,
		HasHoverboardsBeenSeen = 44,
		UnlockedHoverboardTypes = 45,
		CurrentHoverboard = 46,
		BreadCrumbsShownOnFrontPage = 47,
		UnreportedGames = 48,
		UnreportedGamesTimestamp = 49,
		HistoryOfInapp = 50
	}

	private const string SECRET = "we12rtyuiklhgfdjerKJGHfvghyuhnjiokLJHl145rtyfghjvbn";

	private const int VERSION = 1;

	private static readonly CultureInfo DATETIME_CULTUREINFO = new CultureInfo("da-DK");

	private bool _dirty;

	public Action onCoinsChanged;

	private int _amountOfCoins;

	private int _highestScore;

	private int _oldHighestScore;

	public Action onHighScoreChanged;

	private int _highestMeters;

	public Season currentSeasonAvailable;

	private Season _currentSeasonPicked;

	public string _xmasSeasonExpirationDate;

	private int _amountOfMysteryBoxesOpened;

	private int _amountOfSuperMysteryBoxesOpened;

	private List<MysteryBox.Type> _mysteryBoxesToUnlock = new List<MysteryBox.Type>();

	private int _lastMissionCompleted = -1;

	private int _currentMissionSet = -1;

	private int[] _currentMissionProgress;

	private int _missionCompletedSum;

	public Action onScoreMultiplierChanged;

	private int _currentCharacter;

	public Action<Characters.CharacterType> OnTokenCollected;

	private int[] _collectedCharacterTokens = new int[Characters.characterData.Count];

	private bool _isNew_tasha = true;

	private bool _isNew_zoe = true;

	private bool _isNew_brody = true;

	private bool _isNew_princek = true;

	private Hoverboards.BoardType _currentHoverboard;

	private Dictionary<Hoverboards.BoardType, bool> _hoverboardUnlockStatus = new Dictionary<Hoverboards.BoardType, bool>();

	private Dictionary<Hoverboards.BoardType, bool> _hasHoverboardBeenSeen = new Dictionary<Hoverboards.BoardType, bool>();

	private int _unreportedGames;

	private DateTime _unreportedGamesTimestamp = new DateTime(0L);

	public Action<Trophies.Trophy> OnTrophyUnlocked;

	private bool[] _unlockedTrophies = new bool[Enum.GetValues(typeof(Trophies.Trophy)).Length];

	private int[] _achievementProgress = new int[41];

	private bool _hasPayedOutFacebook;

	private bool _hasPayedOutGameCenter;

	private string _dailyWord = string.Empty;

	private int _dailyWordInRow;

	private int _dailyWordLastPayoutDayOfYear;

	private IntMask _dailyWordUnlockedMask;

	private DateTime _dailyWordExpireTime;

	private DateTime _dailyWordPayedOutTime;

	public Action OnPickedUpLetter;

	private bool _tutorialCompleted;

	private int _numberOfBreadCrumbsShownOnFrontPage;

	private Hoverboards.BoardType[] _cachedFreeBoards;

	private bool _hasCachedFreeBoards;

	private int _numberOfRunsSinceLastGuideline;

	private bool _hasShownHoverboardPopup;

	private bool _shouldShowHoverboardPopup;

	private bool _hasShownFacebookPopup;

	private bool _shouldShowFacebookPopup;

	private bool _hasShownMission1Popup;

	private bool _shouldShowMission1Popup;

	private bool _hasShownCollectPopup;

	private bool _shouldShowCollectPopup;

	private bool _hasShownMission2Popup;

	private bool _shouldShowMission2Popup;

	private bool _dummyFriendCollected;

	private bool _dummyFriendShouldShow;

	private int _inAppPurchaseCount;

	private string _earnCurrenyData = string.Empty;

	public Action onPowerupAmountChanged;

	private Dictionary<PowerupType, int> _upgradeAmounts = new Dictionary<PowerupType, int>
	{
		{
			PowerupType.hoverboard,
			0
		},
		{
			PowerupType.headstart500,
			0
		},
		{
			PowerupType.headstart2000,
			0
		},
		{
			PowerupType.mysterybox,
			0
		}
	};

	private Dictionary<PowerupType, int> _upgradeTiers = new Dictionary<PowerupType, int>
	{
		{
			PowerupType.jetpack,
			0
		},
		{
			PowerupType.supersneakers,
			0
		},
		{
			PowerupType.coinmagnet,
			0
		},
		{
			PowerupType.letters,
			0
		},
		{
			PowerupType.doubleMultiplier,
			4
		}
	};

	private Dictionary<string, string> _inappHistory = new Dictionary<string, string>();

	private bool _hasDoubleCoins;

	private bool _doubleScore;

	private static PlayerInfo _instance;

	public bool dirty
	{
		get
		{
			return _dirty;
		}
	}

	public int amountOfCoins
	{
		get
		{
			return 123456789;
		}
		set
		{
			if (_amountOfCoins != value)
			{
				_amountOfCoins = value;
				_dirty = true;
				Action action = onCoinsChanged;
				if (action != null)
				{
					action();
				}
			}
		}
	}

	public int highestScore
	{
		get
		{
			return _highestScore;
		}
		set
		{
			if (value > _highestScore)
			{
				_oldHighestScore = _highestScore;
				_highestScore = value;
				_dirty = true;
				Action action = onHighScoreChanged;
				if (action != null)
				{
					action();
				}
			}
		}
	}

	public int oldHighestScore
	{
		get
		{
			return _oldHighestScore;
		}
	}

	public int highestMeters
	{
		get
		{
			return _highestMeters;
		}
		set
		{
			_highestMeters = value;
			_dirty = true;
		}
	}

	public Season currentSeasonPicked
	{
		get
		{
			return _currentSeasonPicked;
		}
		set
		{
			_currentSeasonPicked = value;
			_dirty = true;
		}
	}

	public string currentSeasonExpirationDate
	{
		get
		{
			return _xmasSeasonExpirationDate;
		}
		set
		{
			_xmasSeasonExpirationDate = value;
			_dirty = true;
		}
	}

	public int amountOfMysteryBoxesOpened
	{
		get
		{
			return _amountOfMysteryBoxesOpened;
		}
		set
		{
			if (_amountOfMysteryBoxesOpened != value)
			{
				_amountOfMysteryBoxesOpened = value;
				_dirty = true;
			}
		}
	}

	public int amountOfSuperMysteryBoxesOpened
	{
		get
		{
			return _amountOfSuperMysteryBoxesOpened;
		}
		set
		{
			if (_amountOfSuperMysteryBoxesOpened != value)
			{
				_amountOfSuperMysteryBoxesOpened = value;
				_dirty = true;
			}
		}
	}

	public int mysteryBoxesToUnlockCount
	{
		get
		{
			return _mysteryBoxesToUnlock.Count;
		}
	}

	public int currentMissionSet
	{
		get
		{
			return _currentMissionSet;
		}
	}

	public int lastMissionCompleted
	{
		get
		{
			return _lastMissionCompleted;
		}
	}

	public int missionCompletedSum
	{
		get
		{
			if (_missionCompletedSum == 0)
			{
				_missionCompletedSum = currentMissionSet + 1;
			}
			return _missionCompletedSum;
		}
		set
		{
			_missionCompletedSum = value;
		}
	}

	public int scoreMultiplier
	{
		get
		{
			int num = Mathf.Clamp(_currentMissionSet + 1, 0, 30);
			if (doubleScore)
			{
				num *= 2;
			}
			return num;
		}
	}

	public int rawMultiplier
	{
		get
		{
			return Mathf.Clamp(_currentMissionSet + 1, 0, 30);
		}
	}

	public int currentCharacter
	{
		get
		{
			return _currentCharacter;
		}
		set
		{
			if (value != _currentCharacter)
			{
				_currentCharacter = value;
				_dirty = true;
			}
		}
	}

	public Hoverboards.BoardType currentHoverboard
	{
		get
		{
			return _currentHoverboard;
		}
		set
		{
			if (value != _currentHoverboard)
			{
				_currentHoverboard = value;
				_dirty = true;
			}
		}
	}

	public int[] achievementProgress
	{
		get
		{
			return _achievementProgress;
		}
	}

	public bool hasPayedOutFacebook
	{
		get
		{
			return _hasPayedOutFacebook;
		}
		set
		{
			if (_hasPayedOutFacebook != value)
			{
				_hasPayedOutFacebook = value;
				_dirty = true;
			}
		}
	}

	public bool hasPayedOutGameCenter
	{
		get
		{
			return _hasPayedOutGameCenter;
		}
		set
		{
			if (_hasPayedOutGameCenter != value)
			{
				_hasPayedOutGameCenter = value;
				_dirty = true;
			}
		}
	}

	public string dailyWord
	{
		get
		{
			return _dailyWord;
		}
	}

	public int dailyWordInRow
	{
		get
		{
			return _dailyWordInRow;
		}
	}

	public int dailyWordLastPayoutDayOfYear
	{
		get
		{
			return _dailyWordLastPayoutDayOfYear;
		}
	}

	public IntMask dailyWordUnlockedMask
	{
		get
		{
			return _dailyWordUnlockedMask;
		}
	}

	public DateTime dailyWordExpireTime
	{
		get
		{
			return _dailyWordExpireTime;
		}
	}

	public DateTime dailyWordPayedOutTime
	{
		get
		{
			return _dailyWordPayedOutTime;
		}
	}

	public bool tutorialCompleted
	{
		get
		{
			return _tutorialCompleted;
		}
		set
		{
			if (_tutorialCompleted != value)
			{
				_tutorialCompleted = value;
				_dirty = true;
				SaveIfDirty();
			}
		}
	}

	public int numberOfBreadCrumbsShownOnFrontPage
	{
		get
		{
			return _numberOfBreadCrumbsShownOnFrontPage;
		}
	}

	public bool hasShownHoverboardPopup
	{
		get
		{
			return _hasShownHoverboardPopup;
		}
		set
		{
			if (_hasShownHoverboardPopup != value)
			{
				_hasShownHoverboardPopup = value;
				_dirty = true;
			}
		}
	}

	public bool shouldShowHoverboardPopup
	{
		get
		{
			return _shouldShowHoverboardPopup;
		}
		set
		{
			if (_shouldShowHoverboardPopup != value)
			{
				_shouldShowHoverboardPopup = value;
				_dirty = true;
			}
		}
	}

	public bool hasShownFacebookPopup
	{
		get
		{
			return _hasShownFacebookPopup;
		}
		set
		{
			_hasShownFacebookPopup = value;
			_dirty = true;
		}
	}

	public bool shouldShowFacebookPopup
	{
		get
		{
			return _shouldShowFacebookPopup;
		}
		set
		{
			_shouldShowFacebookPopup = value;
			_dirty = true;
		}
	}

	public bool hasShownMission1Popup
	{
		get
		{
			return _hasShownMission1Popup;
		}
		set
		{
			_hasShownMission1Popup = value;
			_dirty = true;
		}
	}

	public bool shouldShowMission1Popup
	{
		get
		{
			return _shouldShowMission1Popup;
		}
		set
		{
			_shouldShowMission1Popup = value;
			_dirty = true;
		}
	}

	public bool hasShownCollectPopup
	{
		get
		{
			return _hasShownCollectPopup;
		}
		set
		{
			_hasShownCollectPopup = value;
			_dirty = true;
		}
	}

	public bool shouldShowCollectPopup
	{
		get
		{
			return _shouldShowCollectPopup;
		}
		set
		{
			_shouldShowCollectPopup = value;
			_dirty = true;
		}
	}

	public bool hasShownMission2Popup
	{
		get
		{
			return _hasShownMission2Popup;
		}
		set
		{
			_hasShownMission2Popup = value;
			_dirty = true;
		}
	}

	public bool shouldShowMission2Popup
	{
		get
		{
			return _shouldShowMission2Popup;
		}
		set
		{
			_shouldShowMission2Popup = value;
			_dirty = true;
		}
	}

	public bool dummyFriendCollected
	{
		get
		{
			return _dummyFriendCollected;
		}
		set
		{
			_dummyFriendCollected = value;
			_dirty = true;
		}
	}

	public bool dummyFriendShouldShow
	{
		get
		{
			return _dummyFriendShouldShow;
		}
		set
		{
			_dummyFriendShouldShow = value;
			_dirty = true;
		}
	}

	public int inAppPurchaseCount
	{
		get
		{
			return _inAppPurchaseCount;
		}
		set
		{
			_inAppPurchaseCount = value;
			_dirty = true;
		}
	}

	public string earnCurrenyData
	{
		get
		{
			return _earnCurrenyData;
		}
		set
		{
			_earnCurrenyData = value;
			_dirty = true;
		}
	}

	public bool hasDoubleCoins
	{
		get
		{
			return _hasDoubleCoins;
		}
		set
		{
			if (_hasDoubleCoins != value)
			{
				_hasDoubleCoins = value;
				_dirty = true;
			}
		}
	}

	public float doubleScoreMultiplierDuration
	{
		get
		{
			return GetPowerupDuration(PowerupType.doubleMultiplier);
		}
	}

	public bool doubleScore
	{
		get
		{
			return _doubleScore;
		}
		set
		{
			if (value != _doubleScore)
			{
				_doubleScore = value;
				Action action = onScoreMultiplierChanged;
				if (action != null)
				{
					action();
				}
			}
		}
	}

	public static PlayerInfo Instance
	{
		get
		{
			return _instance ?? (_instance = new PlayerInfo());
		}
	}

	private PlayerInfo()
	{
		Load();
	}

	public void SetOldestHighestScore()
	{
		if (_oldHighestScore < _highestScore)
		{
			_oldHighestScore = _highestScore;
			_dirty = true;
		}
	}

	public void BragCompleted()
	{
		if (_oldHighestScore < _highestScore)
		{
			_oldHighestScore = _highestScore;
			_dirty = true;
		}
	}

	public void AddMysteryBoxToUnlock(MysteryBox.Type type)
	{
		_mysteryBoxesToUnlock.Add(type);
	}

	public MysteryBox.Type[] GetAndClearMysteryBoxesToUnlock()
	{
		MysteryBox.Type[] result = _mysteryBoxesToUnlock.ToArray();
		_mysteryBoxesToUnlock.Clear();
		return result;
	}

	public bool IsCurrentMissionSetInited()
	{
		return false;
	}

	public void InitCurrentMissionSet(int missionSet, int missionCount)
	{
		if (missionSet != _currentMissionSet)
		{
			_currentMissionSet = missionSet;
			_currentMissionProgress = new int[missionCount];
			for (int i = 0; i < missionCount; i++)
			{
				_currentMissionProgress[i] = 0;
			}
			_dirty = true;
			Action action = onScoreMultiplierChanged;
			if (action != null)
			{
				action();
			}
		}
	}

	public void ReInitCurrentMissionSet(int missionSet, int missionCount)
	{
		_currentMissionSet = missionSet;
		_currentMissionProgress = new int[missionCount];
		for (int i = 0; i < missionCount; i++)
		{
			_currentMissionProgress[i] = 0;
		}
		_dirty = true;
		Action action = onScoreMultiplierChanged;
		if (action != null)
		{
			action();
		}
	}

	public int GetCurrentMissionProgress(int mission)
	{
		if (mission >= 3)
		{
			return _achievementProgress[mission - 3];
		}
		if (_currentMissionProgress == null)
		{
			return 0;
		}
		if (mission < _currentMissionProgress.Length)
		{
			return _currentMissionProgress[mission];
		}
		return 0;
	}

	public void SetCurrentMissionProgress(int mission, int progress)
	{
		if (mission >= 3)
		{
			_achievementProgress[mission - 3] = progress;
		}
		else if (_currentMissionProgress[mission] != progress)
		{
			_currentMissionProgress[mission] = progress;
			_dirty = true;
		}
	}

	public bool IncrementCurrentMissionProgress(int mission, int target)
	{
		if (_currentMissionProgress[mission] < target)
		{
			_currentMissionProgress[mission]++;
			_dirty = true;
			return _currentMissionProgress[mission] == target;
		}
		return false;
	}

	public int GetCurrentRank()
	{
		return currentMissionSet / Missions.Instance.GetNumberOfBasicMission();
	}

	public void CollectToken(Characters.CharacterType characterType, int amount)
	{
		_collectedCharacterTokens[(int)characterType] += amount;
		_dirty = true;
		Action<Characters.CharacterType> onTokenCollected = OnTokenCollected;
		if (onTokenCollected != null)
		{
			onTokenCollected(characterType);
		}
	}

	public bool IsCollectionComplete(Characters.CharacterType characterType)
	{
		// All characters are unlocked by default for this modified build.
		return true;
	}

	public int GetCollectedTokens(Characters.CharacterType ModelType)
	{
		return _collectedCharacterTokens[(int)ModelType];
	}

	public bool IsTokenUseful(Characters.CharacterType characterType)
	{
		if (Instance.GetCollectedTokens(characterType) < Characters.characterData[characterType].Price)
		{
			return true;
		}
		return false;
	}

	public bool IsCharacterNew(Characters.CharacterType characterType)
	{
		if (Characters.characterData[characterType].isNewInThisUpdate)
		{
			switch (characterType)
			{
			case Characters.CharacterType.tasha:
				return _isNew_tasha;
			case Characters.CharacterType.zoe:
				return _isNew_zoe;
			case Characters.CharacterType.brody:
				return _isNew_brody;
			case Characters.CharacterType.princek:
				return _isNew_princek;
			}
		}
		return false;
	}

	public void MarkCharacterAsSeen(Characters.CharacterType characterType)
	{
		switch (characterType)
		{
		case Characters.CharacterType.tasha:
			_isNew_tasha = false;
			break;
		case Characters.CharacterType.zoe:
			_isNew_zoe = false;
			break;
		case Characters.CharacterType.brody:
			_isNew_brody = false;
			break;
		case Characters.CharacterType.princek:
			_isNew_princek = false;
			break;
		default:
			Debug.LogError(string.Concat("Attempted to mark ", characterType, " as seen, but no members has been made for it"));
			break;
		}
	}

	public void UnlockHoverboard(Hoverboards.BoardType boardType)
	{
		if (_hoverboardUnlockStatus.ContainsKey(boardType))
		{
			_hoverboardUnlockStatus[boardType] = true;
		}
		else
		{
			_hoverboardUnlockStatus.Add(boardType, true);
		}
		_dirty = true;
	}

	public bool isHoverboardUnlocked(Hoverboards.BoardType boardType)
	{
		if (Hoverboards.boardData[boardType].unlockType == Hoverboards.UnlockType.alwaysUnlocked)
		{
			return true;
		}
		if (_hoverboardUnlockStatus.ContainsKey(boardType))
		{
			return _hoverboardUnlockStatus[boardType];
		}
		return false;
	}

	public bool HasHoverboardBeenSeen(Hoverboards.BoardType boardType)
	{
		if (Hoverboards.boardData[boardType].season != 0)
		{
			return true;
		}
		if (!Hoverboards.boardData[boardType].isNewInThisUpdate)
		{
			return true;
		}
		if (_hasHoverboardBeenSeen.ContainsKey(boardType))
		{
			return _hasHoverboardBeenSeen[boardType];
		}
		return false;
	}

	public void MarkHoverboardAsSeen(Hoverboards.BoardType boardType)
	{
		if (_hasHoverboardBeenSeen.ContainsKey(boardType))
		{
			_hasHoverboardBeenSeen[boardType] = true;
		}
		else
		{
			_hasHoverboardBeenSeen.Add(boardType, true);
		}
		_dirty = true;
	}

	public float GetHoverBoardCoolDown()
	{
		return 5f;
	}

	public int IncrementUnreportedGames()
	{
		_unreportedGames++;
		if (_unreportedGames == 1)
		{
			_unreportedGamesTimestamp = DateTime.Now;
		}
		_dirty = true;
		return _unreportedGames;
	}

	public DateTime GetFirstUnreportedGameTimestamp()
	{
		if (_unreportedGames < 1)
		{
			Debug.LogWarning("PlayerInfo.GetFirstUnreportedGameTimestamp should not be called when number of unreported games is < 1");
		}
		return _unreportedGamesTimestamp;
	}

	public void ClearUnreportedGames()
	{
		_unreportedGames = 0;
		_dirty = true;
	}

	public bool[] GetUnlockedTrophies()
	{
		return _unlockedTrophies;
	}

	public int NumberOfUnlockedTrophyForFirstAchievement()
	{
		int num = 0;
		for (int i = 0; i < 8; i++)
		{
			if (_unlockedTrophies[i])
			{
				num++;
			}
		}
		return num;
	}

	public void UnlockTrophy(Trophies.Trophy trophy)
	{
		if (_unlockedTrophies[(int)trophy])
		{
			Debug.Log("Trophy: " + trophy.ToString() + " has already been unlocked");
			return;
		}
		_unlockedTrophies[(int)trophy] = true;
		_dirty = true;
		Action<Trophies.Trophy> onTrophyUnlocked = OnTrophyUnlocked;
		if (onTrophyUnlocked != null)
		{
			onTrophyUnlocked(trophy);
		}
	}

	public bool isTrophyUnlocked(Trophies.Trophy trophy)
	{
		return _unlockedTrophies[(int)trophy];
	}

	public int GetDailyWordDaysInARow(out bool mostRecentIsToday)
	{
		int dayOfYear = _dailyWordExpireTime.DayOfYear;
		if (_dailyWordLastPayoutDayOfYear == dayOfYear)
		{
			mostRecentIsToday = true;
			return _dailyWordInRow;
		}
		if (_dailyWordLastPayoutDayOfYear == dayOfYear - 1 || (dayOfYear == 1 && _dailyWordLastPayoutDayOfYear >= 365))
		{
			mostRecentIsToday = false;
			return _dailyWordInRow;
		}
		mostRecentIsToday = false;
		return 0;
	}

	public void CheckIfWeShouldRemoveProgressForDailyQuestInRow()
	{
		bool mostRecentIsToday = false;
		if (Instance.GetDailyWordDaysInARow(out mostRecentIsToday) == 0 && !mostRecentIsToday)
		{
			Missions.Instance.RemoveProgressForThis(Missions.MissionTarget.DailyQuestInRow);
		}
	}

	public void InitDailyWord(string word, DateTime expires)
	{
		if (!_dailyWord.Equals(word) || !_dailyWordExpireTime.Equals(expires))
		{
			_dailyWord = word;
			_dailyWordExpireTime = expires;
			_dailyWordPayedOutTime = DateTime.UtcNow;
			_dailyWordUnlockedMask = 0;
			_dirty = true;
		}
		DailyLetterPickupManager.Instance.UpdateLetter();
	}

	public void PickedupLetter(char letter)
	{
		Debug.Log("Picked up a letter: " + letter);
		for (int i = 0; i < _dailyWord.Length; i++)
		{
			if (_dailyWord[i] == letter && !_dailyWordUnlockedMask[i])
			{
				_dailyWordUnlockedMask[i] = true;
				Action onPickedUpLetter = OnPickedUpLetter;
				if (onPickedUpLetter != null)
				{
					onPickedUpLetter();
				}
				_dirty = true;
				break;
			}
		}
		if (!isDailyWordComplete() || !(_dailyWordPayedOutTime != _dailyWordExpireTime))
		{
			return;
		}
		bool mostRecentIsToday = false;
		_dailyWordInRow = GetDailyWordDaysInARow(out mostRecentIsToday) + 1;
		Debug.Log("PlayerInfo dailyWordInRow is now " + _dailyWordInRow);
		_dailyWordLastPayoutDayOfYear = _dailyWordExpireTime.DayOfYear;
		Missions.Instance.PlayerDidThis(Missions.MissionTarget.DailyQuests);
		Missions.Instance.PlayerDidThis(Missions.MissionTarget.DailyQuestInRow);
		DailyWord.DailyWordReward rewardForDay = DailyWord.GetRewardForDay(_dailyWordInRow - 1);
		if (rewardForDay.type == DailyWord.DailyWordRewardType.Coins)
		{
			if (rewardForDay.coins > 0)
			{
				_amountOfCoins += rewardForDay.coins;
				UIScreenController.Instance.coinReward.NewCoinAmount(rewardForDay.coins);
				UIScreenController.Instance.QueueSlideIn(UIScreenController.SlideInType.LettersCompleteCoins, string.Empty);
			}
		}
		else if (rewardForDay.type == DailyWord.DailyWordRewardType.MysteryBox)
		{
			AddMysteryBoxToUnlock(rewardForDay.mysteryBoxType);
			UIScreenController.Instance.QueueSlideIn(UIScreenController.SlideInType.LettersCompleteMysteryBox, string.Empty);
		}
		else
		{
			Debug.LogError("PlayerInfo Unhandled DailyWordRewardType: " + rewardForDay.type);
		}
		_dailyWordPayedOutTime = _dailyWordExpireTime;
		_dirty = true;
		Flurry.LogEventWithAParameter("Daily Challenge completed", "Id", _dailyWord);
	}

	public char GetNewDailyLetter()
	{
		for (int i = 0; i < _dailyWord.Length; i++)
		{
			if (!_dailyWordUnlockedMask[i])
			{
				return _dailyWord[i];
			}
		}
		return '\0';
	}

	public bool isDailyWordComplete()
	{
		if (string.IsNullOrEmpty(_dailyWord))
		{
			return false;
		}
		return (1 << _dailyWord.Length) - 1 == (int)_dailyWordUnlockedMask;
	}

	public bool ShouldShowBreadCrumb()
	{
		bool flag = false;
		if (_numberOfBreadCrumbsShownOnFrontPage < 30)
		{
			flag = true;
		}
		flag = ((_hasShownHoverboardPopup || _hasShownCollectPopup || _hasShownFacebookPopup || _hasShownMission1Popup) ? true : false);
		if (flag && UIScreenController.Instance.GetTopScreenName() == "BoardScreen")
		{
			flag = false;
		}
		if (flag)
		{
			bool flag2 = false;
			if (!_hasCachedFreeBoards)
			{
				List<Hoverboards.BoardType> list = new List<Hoverboards.BoardType>();
				foreach (Hoverboards.BoardType item in Hoverboards.boardOrder)
				{
					if (Hoverboards.boardData[item].unlockType == Hoverboards.UnlockType.free)
					{
						list.Add(item);
					}
				}
				_cachedFreeBoards = list.ToArray();
				_hasCachedFreeBoards = true;
			}
			for (int i = 0; i < _cachedFreeBoards.Length; i++)
			{
				if (!Instance.isHoverboardUnlocked(_cachedFreeBoards[i]))
				{
					flag2 = true;
					break;
				}
			}
			flag = flag2;
		}
		return flag;
	}

	public void BreadCrumbShownOnFrontPage()
	{
		_numberOfBreadCrumbsShownOnFrontPage++;
		_dirty = true;
	}

	public void RunCompleted()
	{
		_dirty = true;
		_numberOfRunsSinceLastGuideline++;
		if (_numberOfRunsSinceLastGuideline <= 4)
		{
			return;
		}
		_numberOfRunsSinceLastGuideline = 0;
		if (!_hasShownHoverboardPopup && (Achievements.Instance.GetNumberOfHoverboardsUsed() == 0 || _instance.GetUpgradeAmount(PowerupType.hoverboard) == 3 - Achievements.Instance.GetNumberOfHoverboardsUsed()))
		{
			_shouldShowHoverboardPopup = true;
			_shouldShowFacebookPopup = false;
			_shouldShowCollectPopup = false;
			_shouldShowMission1Popup = false;
			_shouldShowMission2Popup = false;
			_dirty = true;
			return;
		}
		_hasShownHoverboardPopup = true;
		_dirty = true;
		if (!_hasShownHoverboardPopup && _shouldShowHoverboardPopup)
		{
			return;
		}
		if (!_hasShownFacebookPopup && !SocialManager.instance.facebookIsLoggedIn && !_instance.hasPayedOutFacebook)
		{
			_shouldShowFacebookPopup = true;
			_dirty = true;
		}
		else if (!_hasShownMission1Popup)
		{
			if (rawMultiplier < 5)
			{
				_shouldShowMission1Popup = true;
				_dirty = true;
			}
		}
		else if (!_hasShownCollectPopup && (SocialManager.instance.facebookIsLoggedIn || Social.localUser.authenticated))
		{
			_shouldShowCollectPopup = true;
			_dirty = true;
		}
	}

	public bool AddTransactionToHistory(string orderId, string itemId)
	{
		if (!_inappHistory.ContainsKey(orderId))
		{
			_inappHistory.Add(orderId, itemId);
			_dirty = true;
			SaveIfDirty();
			return true;
		}
		Debug.Log(string.Format("InAppManager Product: {0} with orderId {1} has already been paid.", itemId, orderId));
		return false;
	}

	public bool RemoveTransactionFromHistory(string orderId)
	{
		if (_inappHistory.ContainsKey(orderId))
		{
			_inappHistory.Remove(orderId);
			return true;
		}
		return false;
	}

	public bool HasTransactionHistoryItemId(string itemId)
	{
		bool result = false;
		foreach (KeyValuePair<string, string> item in _inappHistory)
		{
			string key = item.Key;
			string value = item.Value;
			if (value == itemId)
			{
				Debug.Log("InAppManager Transaction history contains item: " + itemId + " orderId: " + key);
				result = true;
				break;
			}
		}
		return result;
	}

	public bool IsTransactionInHistory(string orderId)
	{
		return _inappHistory.ContainsKey(orderId);
	}

	public void PrintAllInappHistory()
	{
		string text = Globals.convertStringStringDictionaryToString(_inappHistory);
		Debug.Log("InAppManager history data: " + text);
	}

	public int GetUpgradeTierSum()
	{
		return GetCurrentTier(PowerupType.jetpack) + GetCurrentTier(PowerupType.doubleMultiplier) + GetCurrentTier(PowerupType.coinmagnet) + GetCurrentTier(PowerupType.supersneakers);
	}

	public int GetUpgradeAmount(PowerupType type)
	{
		return _upgradeAmounts[type];
	}

	public int GetCurrentTier(PowerupType type)
	{
		if (!_upgradeTiers.ContainsKey(type))
		{
			return 0;
		}
		return _upgradeTiers[type];
	}

	public float GetPowerupDuration(PowerupType type)
	{
		if (!Upgrades.upgrades.ContainsKey(type))
		{
			Debug.Log("Couldn't find any upgrades of the type: " + type.ToString() + ". Returning 0");
			return 0f;
		}
		return Upgrades.upgrades[type].durations[GetCurrentTier(type)];
	}

	public void IncreasePowerupTier(PowerupType type)
	{
		if (_upgradeTiers.ContainsKey(type))
		{
			_upgradeTiers[type] += 1;
			_dirty = true;
		}
		else
		{
			Debug.LogError("Trying to increase tier for a non-tiered upgrade");
		}
	}

	public void UseUpgrade(PowerupType type)
	{
		Debug.Log("Used powerup: " + type);
		if (_upgradeAmounts.ContainsKey(type))
		{
			Dictionary<PowerupType, int> upgradeAmounts;
			Dictionary<PowerupType, int> dictionary = (upgradeAmounts = _upgradeAmounts);
			PowerupType key;
			PowerupType key2 = (key = type);
			int num = upgradeAmounts[key];
			dictionary[key2] = num - 1;
			_dirty = true;
			Action action = onPowerupAmountChanged;
			if (action != null)
			{
				action();
			}
			if (type != PowerupType.hoverboard)
			{
			}
		}
	}

	public void IncreaseUpgradeAmount(PowerupType type, int amount = 1)
	{
		if (_upgradeAmounts.ContainsKey(type))
		{
			Dictionary<PowerupType, int> upgradeAmounts;
			Dictionary<PowerupType, int> dictionary = (upgradeAmounts = _upgradeAmounts);
			PowerupType key;
			PowerupType key2 = (key = type);
			int num = upgradeAmounts[key];
			dictionary[key2] = num + amount;
			_dirty = true;
			Action action = onPowerupAmountChanged;
			if (action != null)
			{
				action();
			}
		}
		else
		{
			Debug.LogError("Trying to increase upgrade amount for a non-consumable");
		}
	}

	public int GetNumberOfAffordableUpgrades()
	{
		int num = 0;
		bool flag = true;
		foreach (KeyValuePair<PowerupType, Upgrade> upgrade in Upgrades.upgrades)
		{
			Upgrade value = upgrade.Value;
			if (value.numberOfTiers > 0)
			{
				int num2 = GetCurrentTier(upgrade.Key) + 1;
				if (num2 < value.pricesRaw.Length)
				{
					int price = value.getPrice(num2);
					if (price <= amountOfCoins)
					{
						num++;
					}
				}
			}
			else if (value.pricesRaw != null && value.pricesRaw.Length > 0 && (upgrade.Key != PowerupType.skipmission1 || (flag && !Missions.Instance.GetMissionInfo(0).complete)) && (upgrade.Key != PowerupType.skipmission2 || (flag && !Missions.Instance.GetMissionInfo(1).complete)) && (upgrade.Key != PowerupType.skipmission3 || (flag && !Missions.Instance.GetMissionInfo(2).complete)))
			{
				int price2 = value.getPrice(0);
				if (price2 <= amountOfCoins)
				{
					num++;
				}
			}
		}
		return num;
	}

	private static string GetSaveDataPath()
	{
		return Application.persistentDataPath + "/playerdata";
	}

	public void SaveIfDirty()
	{
		if (_dirty)
		{
			Save();
		}
	}

	private void Save()
	{
		try
		{
			MemoryStream memoryStream = new MemoryStream(8192);
			BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
			binaryWriter.Write(1);
			Dictionary<Key, string> dictionary = new Dictionary<Key, string>();
			dictionary[Key.AmountOfCoins] = _amountOfCoins.ToString();
			dictionary[Key.HighestScore] = _highestScore.ToString();
			dictionary[Key.OldHighestScore] = _oldHighestScore.ToString();
			dictionary[Key.DailyWord] = _dailyWord;
			dictionary[Key.DailyWordUnlockMask] = ((int)_dailyWordUnlockedMask).ToString();
			dictionary[Key.DailyWordExpireTime] = _dailyWordExpireTime.ToString();
			dictionary[Key.DailyWordPayedOutTime] = _dailyWordPayedOutTime.ToString();
			dictionary[Key.CurrentCharacter] = _currentCharacter.ToString();
			dictionary[Key.CurrentMissionSet] = _currentMissionSet.ToString();
			dictionary[Key.AmountOfMysteryBoxesOpened] = _amountOfMysteryBoxesOpened.ToString();
			dictionary[Key.AmountOfSuperMysteryBoxesOpened] = _amountOfSuperMysteryBoxesOpened.ToString();
			dictionary[Key.TutorialCompleted] = _tutorialCompleted.ToString();
			dictionary[Key.InAppPurchaseCount] = _inAppPurchaseCount.ToString();
			dictionary[Key.EarnCurrencyData] = _earnCurrenyData;
			dictionary[Key.PayBonusFacebook] = _hasPayedOutFacebook.ToString();
			dictionary[Key.PayBonusGameCenter] = _hasPayedOutGameCenter.ToString();
			dictionary[Key.DoubleCoins] = _hasDoubleCoins.ToString();
			dictionary[Key.NumberOfRunsSinceLastGuideline] = _numberOfRunsSinceLastGuideline.ToString();
			dictionary[Key.HasShownHoverboardPopup] = _hasShownHoverboardPopup.ToString();
			dictionary[Key.ShouldShowHoverboardPopup] = _shouldShowHoverboardPopup.ToString();
			dictionary[Key.HasShownFacebookPopup] = _hasShownFacebookPopup.ToString();
			dictionary[Key.ShouldShowFacebookPopup] = _shouldShowHoverboardPopup.ToString();
			dictionary[Key.HasShownMission1Popup] = _hasShownMission1Popup.ToString();
			dictionary[Key.ShouldShowMission1Popup] = _shouldShowMission1Popup.ToString();
			dictionary[Key.HasShownCollectPopup] = _hasShownCollectPopup.ToString();
			dictionary[Key.ShouldShowCollectPopup] = _shouldShowCollectPopup.ToString();
			dictionary[Key.HasShownMission2Popup] = _hasShownMission2Popup.ToString();
			dictionary[Key.ShouldShowMission2Popup] = _shouldShowMission2Popup.ToString();
			dictionary[Key.DummyFriendCollected] = _dummyFriendCollected.ToString();
			dictionary[Key.DummyFriendShouldShow] = _dummyFriendShouldShow.ToString();
			if (_currentMissionSet >= 0)
			{
				dictionary[Key.CurrentMissionSetProgress] = string.Join(",", Array.ConvertAll(_currentMissionProgress, (int input) => input.ToString()));
			}
			dictionary[Key.CollectedCharacterTokens] = string.Join(",", Array.ConvertAll(_collectedCharacterTokens, (int input) => input.ToString()));
			dictionary[Key.UnlockedTrophies] = string.Join(",", Array.ConvertAll(_unlockedTrophies, (bool input) => input.ToString()));
			dictionary[Key.AchievementProgress] = string.Join(",", Array.ConvertAll(_achievementProgress, (int input) => input.ToString()));
			dictionary[Key.DailyWordInRow] = _dailyWordInRow.ToString();
			dictionary[Key.DailyWordLastPayoutDayOfYear] = _dailyWordLastPayoutDayOfYear.ToString();
			int num = (int)_currentSeasonPicked;
			dictionary[Key.SeasonPicked] = num.ToString();
			dictionary[Key.XmasSeasonEndDate] = _xmasSeasonExpirationDate;
			dictionary[Key.MissionCompletedSum] = _missionCompletedSum.ToString();
			dictionary[Key.IsNew_tasha] = _isNew_tasha.ToString();
			dictionary[Key.IsNew_zoe] = _isNew_zoe.ToString();
			dictionary[Key.IsNew_brody] = _isNew_brody.ToString();
			dictionary[Key.IsNew_princek] = _isNew_princek.ToString();
			dictionary[Key.HasHoverboardsBeenSeen] = Globals.convertEnumBoolDictionaryToString(_hasHoverboardBeenSeen);
			dictionary[Key.UnlockedHoverboardTypes] = Globals.convertEnumBoolDictionaryToString(_hoverboardUnlockStatus);
			dictionary[Key.CurrentHoverboard] = _currentHoverboard.ToString();
			dictionary[Key.BreadCrumbsShownOnFrontPage] = _numberOfBreadCrumbsShownOnFrontPage.ToString();
			dictionary[Key.UnreportedGames] = _unreportedGames.ToString();
			dictionary[Key.UnreportedGamesTimestamp] = _unreportedGamesTimestamp.Ticks.ToString();
			dictionary[Key.HistoryOfInapp] = Globals.convertStringStringDictionaryToString(_inappHistory);
			FileUtil.WriteEnumStringDictionary(binaryWriter, dictionary);
			FileUtil.WriteEnumIntDictionary(binaryWriter, _upgradeAmounts);
			FileUtil.WriteEnumIntDictionary(binaryWriter, _upgradeTiers);
			FileUtil.Save(GetSaveDataPath(), "we12rtyuiklhgfdjerKJGHfvghyuhnjiokLJHl145rtyfghjvbn", memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
			memoryStream.Close();
			_dirty = false;
		}
		catch (Exception ex)
		{
			Debug.LogError("Error saving player info: " + ex.ToString());
		}
	}

	public void Load()
	{
		try
		{
			byte[] buffer = FileUtil.Load(GetSaveDataPath(), "we12rtyuiklhgfdjerKJGHfvghyuhnjiokLJHl145rtyfghjvbn");
			MemoryStream memoryStream = new MemoryStream(buffer);
			BinaryReader binaryReader = new BinaryReader(memoryStream);
			int num = binaryReader.ReadInt32();
			Dictionary<Key, string> dictionary = FileUtil.ReadEnumStringDictionary<Key>(binaryReader);
			_amountOfCoins = (dictionary.ContainsKey(Key.AmountOfCoins) ? int.Parse(dictionary[Key.AmountOfCoins]) : 0);
			_highestScore = (dictionary.ContainsKey(Key.HighestScore) ? int.Parse(dictionary[Key.HighestScore]) : 0);
			_oldHighestScore = (dictionary.ContainsKey(Key.OldHighestScore) ? int.Parse(dictionary[Key.HighestScore]) : 0);
			_dailyWord = ((!dictionary.ContainsKey(Key.DailyWord)) ? string.Empty : dictionary[Key.DailyWord]);
			_dailyWordUnlockedMask = (dictionary.ContainsKey(Key.DailyWordUnlockMask) ? int.Parse(dictionary[Key.DailyWordUnlockMask]) : 0);
			_dailyWordExpireTime = ((!dictionary.ContainsKey(Key.DailyWordExpireTime)) ? DateTime.UtcNow : DateTime.Parse(dictionary[Key.DailyWordExpireTime]));
			_dailyWordPayedOutTime = ((!dictionary.ContainsKey(Key.DailyWordPayedOutTime)) ? DateTime.UtcNow : DateTime.Parse(dictionary[Key.DailyWordPayedOutTime]));
			_currentCharacter = (dictionary.ContainsKey(Key.CurrentCharacter) ? int.Parse(dictionary[Key.CurrentCharacter]) : 0);
			_currentMissionSet = ((!dictionary.ContainsKey(Key.CurrentMissionSet)) ? (-1) : int.Parse(dictionary[Key.CurrentMissionSet]));
			_amountOfMysteryBoxesOpened = (dictionary.ContainsKey(Key.AmountOfMysteryBoxesOpened) ? int.Parse(dictionary[Key.AmountOfMysteryBoxesOpened]) : 0);
			_amountOfSuperMysteryBoxesOpened = (dictionary.ContainsKey(Key.AmountOfSuperMysteryBoxesOpened) ? int.Parse(dictionary[Key.AmountOfSuperMysteryBoxesOpened]) : 0);
			_tutorialCompleted = dictionary.ContainsKey(Key.TutorialCompleted) && bool.Parse(dictionary[Key.TutorialCompleted]);
			_inAppPurchaseCount = (dictionary.ContainsKey(Key.InAppPurchaseCount) ? int.Parse(dictionary[Key.InAppPurchaseCount]) : 0);
			_earnCurrenyData = ((!dictionary.ContainsKey(Key.EarnCurrencyData)) ? string.Empty : dictionary[Key.EarnCurrencyData]);
			_hasPayedOutFacebook = dictionary.ContainsKey(Key.PayBonusFacebook) && bool.Parse(dictionary[Key.PayBonusFacebook]);
			_hasPayedOutGameCenter = dictionary.ContainsKey(Key.PayBonusGameCenter) && bool.Parse(dictionary[Key.PayBonusGameCenter]);
			_hasDoubleCoins = dictionary.ContainsKey(Key.DoubleCoins) && bool.Parse(dictionary[Key.DoubleCoins]);
			_numberOfRunsSinceLastGuideline = (dictionary.ContainsKey(Key.NumberOfRunsSinceLastGuideline) ? int.Parse(dictionary[Key.NumberOfRunsSinceLastGuideline]) : 0);
			_hasShownHoverboardPopup = dictionary.ContainsKey(Key.HasShownHoverboardPopup) && bool.Parse(dictionary[Key.HasShownHoverboardPopup]);
			_shouldShowHoverboardPopup = dictionary.ContainsKey(Key.ShouldShowHoverboardPopup) && bool.Parse(dictionary[Key.ShouldShowHoverboardPopup]);
			_hasShownFacebookPopup = dictionary.ContainsKey(Key.HasShownFacebookPopup) && bool.Parse(dictionary[Key.HasShownFacebookPopup]);
			_shouldShowFacebookPopup = dictionary.ContainsKey(Key.ShouldShowFacebookPopup) && bool.Parse(dictionary[Key.ShouldShowFacebookPopup]);
			_hasShownMission1Popup = dictionary.ContainsKey(Key.HasShownMission1Popup) && bool.Parse(dictionary[Key.HasShownMission1Popup]);
			_shouldShowMission1Popup = dictionary.ContainsKey(Key.ShouldShowMission1Popup) && bool.Parse(dictionary[Key.ShouldShowMission1Popup]);
			_hasShownCollectPopup = dictionary.ContainsKey(Key.HasShownCollectPopup) && bool.Parse(dictionary[Key.HasShownCollectPopup]);
			_shouldShowCollectPopup = dictionary.ContainsKey(Key.ShouldShowCollectPopup) && bool.Parse(dictionary[Key.ShouldShowCollectPopup]);
			_hasShownMission2Popup = dictionary.ContainsKey(Key.HasShownMission2Popup) && bool.Parse(dictionary[Key.HasShownMission2Popup]);
			_shouldShowMission2Popup = dictionary.ContainsKey(Key.ShouldShowMission2Popup) && bool.Parse(dictionary[Key.ShouldShowMission2Popup]);
			_dummyFriendCollected = dictionary.ContainsKey(Key.DummyFriendCollected) && bool.Parse(dictionary[Key.DummyFriendCollected]);
			_dummyFriendShouldShow = dictionary.ContainsKey(Key.DummyFriendShouldShow) && bool.Parse(dictionary[Key.DummyFriendShouldShow]);
			_currentMissionProgress = null;
			if (dictionary.ContainsKey(Key.CurrentMissionSetProgress))
			{
				string text = dictionary[Key.CurrentMissionSetProgress];
				if (!string.IsNullOrEmpty(text))
				{
					_currentMissionProgress = Array.ConvertAll(text.Split(','), (string input) => int.Parse(input));
				}
			}
			if (dictionary.ContainsKey(Key.CollectedCharacterTokens))
			{
				string text2 = dictionary[Key.CollectedCharacterTokens];
				if (!string.IsNullOrEmpty(text2))
				{
					int[] array = Array.ConvertAll(text2.Split(','), (string input) => int.Parse(input));
					int i = Mathf.Min(array.Length, _collectedCharacterTokens.Length);
					Array.Copy(array, _collectedCharacterTokens, i);
					for (; i < _collectedCharacterTokens.Length; i++)
					{
						_collectedCharacterTokens[i] = 0;
					}
				}
			}
			if (dictionary.ContainsKey(Key.UnlockedTrophies))
			{
				string text3 = dictionary[Key.UnlockedTrophies];
				if (!string.IsNullOrEmpty(text3))
				{
					bool[] array2 = Array.ConvertAll(text3.Split(','), (string input) => bool.Parse(input));
					int j = Mathf.Min(array2.Length, _unlockedTrophies.Length);
					Array.Copy(array2, _unlockedTrophies, j);
					for (; j < _unlockedTrophies.Length; j++)
					{
						_unlockedTrophies[j] = false;
					}
				}
			}
			if (dictionary.ContainsKey(Key.AchievementProgress))
			{
				string text4 = dictionary[Key.AchievementProgress];
				if (!string.IsNullOrEmpty(text4))
				{
					int[] array3 = Array.ConvertAll(text4.Split(','), (string input) => int.Parse(input));
					int k = Mathf.Min(array3.Length, _achievementProgress.Length);
					Array.Copy(array3, _achievementProgress, k);
					for (; k < _achievementProgress.Length; k++)
					{
						_achievementProgress[k] = 0;
					}
				}
			}
			_dailyWordInRow = (dictionary.ContainsKey(Key.DailyWordInRow) ? int.Parse(dictionary[Key.DailyWordInRow]) : 0);
			_dailyWordLastPayoutDayOfYear = (dictionary.ContainsKey(Key.DailyWordLastPayoutDayOfYear) ? int.Parse(dictionary[Key.DailyWordLastPayoutDayOfYear]) : 0);
			_currentSeasonPicked = (dictionary.ContainsKey(Key.SeasonPicked) ? ((Season)int.Parse(dictionary[Key.SeasonPicked])) : Season.none);
			_xmasSeasonExpirationDate = ((!dictionary.ContainsKey(Key.XmasSeasonEndDate)) ? null : dictionary[Key.XmasSeasonEndDate]);
			_missionCompletedSum = (dictionary.ContainsKey(Key.MissionCompletedSum) ? int.Parse(dictionary[Key.MissionCompletedSum]) : 0);
			_isNew_tasha = !dictionary.ContainsKey(Key.IsNew_tasha) || bool.Parse(dictionary[Key.IsNew_tasha]);
			_isNew_zoe = !dictionary.ContainsKey(Key.IsNew_zoe) || bool.Parse(dictionary[Key.IsNew_zoe]);
			_isNew_brody = !dictionary.ContainsKey(Key.IsNew_brody) || bool.Parse(dictionary[Key.IsNew_brody]);
			_isNew_princek = !dictionary.ContainsKey(Key.IsNew_princek) || bool.Parse(dictionary[Key.IsNew_princek]);
			_hasHoverboardBeenSeen = ((!dictionary.ContainsKey(Key.HasHoverboardsBeenSeen)) ? new Dictionary<Hoverboards.BoardType, bool>() : Globals.convertStringToEnumBoolDictionary<Hoverboards.BoardType>(dictionary[Key.HasHoverboardsBeenSeen]));
			_hoverboardUnlockStatus = ((!dictionary.ContainsKey(Key.UnlockedHoverboardTypes)) ? new Dictionary<Hoverboards.BoardType, bool>() : Globals.convertStringToEnumBoolDictionary<Hoverboards.BoardType>(dictionary[Key.UnlockedHoverboardTypes]));
			_inappHistory = ((!dictionary.ContainsKey(Key.HistoryOfInapp)) ? new Dictionary<string, string>() : Globals.convertStringToStringStringDictionary(dictionary[Key.HistoryOfInapp]));
			if (dictionary.ContainsKey(Key.CurrentHoverboard) && Enum.IsDefined(typeof(Hoverboards.BoardType), dictionary[Key.CurrentHoverboard]))
			{
				_currentHoverboard = (Hoverboards.BoardType)(int)Enum.Parse(typeof(Hoverboards.BoardType), dictionary[Key.CurrentHoverboard]);
			}
			_numberOfBreadCrumbsShownOnFrontPage = (dictionary.ContainsKey(Key.BreadCrumbsShownOnFrontPage) ? int.Parse(dictionary[Key.BreadCrumbsShownOnFrontPage]) : 0);
			_unreportedGames = (dictionary.ContainsKey(Key.UnreportedGames) ? int.Parse(dictionary[Key.UnreportedGames]) : 0);
			_unreportedGamesTimestamp = new DateTime((!dictionary.ContainsKey(Key.UnreportedGamesTimestamp)) ? 0 : long.Parse(dictionary[Key.UnreportedGamesTimestamp]));
			Dictionary<PowerupType, int> dictionary2 = FileUtil.ReadEnumIntDictionary<PowerupType>(binaryReader);
			foreach (KeyValuePair<PowerupType, int> item in dictionary2)
			{
				_upgradeAmounts[item.Key] = item.Value;
			}
			Dictionary<PowerupType, int> dictionary3 = FileUtil.ReadEnumIntDictionary<PowerupType>(binaryReader);
			foreach (KeyValuePair<PowerupType, int> item2 in dictionary3)
			{
				_upgradeTiers[item2.Key] = item2.Value;
			}
			memoryStream.Close();
			_dirty = false;
		}
		catch (Exception ex)
		{
			Debug.LogWarning("Error loading player info: " + ex.ToString());
			InitNew();
		}
	}

	public void InitNew()
	{
		_amountOfCoins = 0;
		_highestScore = 0;
		_dailyWord = string.Empty;
		_dailyWordUnlockedMask = 0;
		_dailyWordExpireTime = DateTime.UtcNow;
		_dailyWordPayedOutTime = DateTime.UtcNow;
		_dailyWordInRow = 0;
		_dailyWordLastPayoutDayOfYear = 0;
		_amountOfMysteryBoxesOpened = 0;
		_amountOfSuperMysteryBoxesOpened = 0;
		_currentCharacter = 0;
		_currentMissionSet = -1;
		_currentMissionProgress = null;
		_tutorialCompleted = false;
		_inAppPurchaseCount = 0;
		_earnCurrenyData = string.Empty;
		_hasPayedOutFacebook = false;
		_hasPayedOutGameCenter = false;
		_hasDoubleCoins = false;
		_numberOfRunsSinceLastGuideline = 0;
		_hasShownHoverboardPopup = false;
		_shouldShowHoverboardPopup = false;
		_hasShownFacebookPopup = false;
		_shouldShowFacebookPopup = false;
		_hasShownMission1Popup = false;
		_shouldShowMission1Popup = false;
		_hasShownCollectPopup = false;
		_shouldShowCollectPopup = false;
		_hasShownMission2Popup = false;
		_shouldShowMission2Popup = false;
		_dummyFriendCollected = false;
		_dummyFriendShouldShow = false;
		for (int i = 0; i < _collectedCharacterTokens.Length; i++)
		{
			_collectedCharacterTokens[i] = 0;
		}
		for (int j = 0; j < _achievementProgress.Length; j++)
		{
			_achievementProgress[j] = 0;
		}
		Dictionary<PowerupType, int> dictionary = new Dictionary<PowerupType, int>(_upgradeAmounts.Count);
		foreach (PowerupType key in _upgradeAmounts.Keys)
		{
			if (key == PowerupType.hoverboard)
			{
				dictionary[key] = 3;
			}
			else
			{
				dictionary[key] = 0;
			}
		}
		_upgradeAmounts = dictionary;
		dictionary = new Dictionary<PowerupType, int>(_upgradeTiers.Count);
		foreach (PowerupType key2 in _upgradeTiers.Keys)
		{
			dictionary[key2] = 0;
		}
		_upgradeTiers = dictionary;
		_isNew_tasha = true;
		_isNew_zoe = true;
		_isNew_brody = true;
		_isNew_princek = true;
		_hasHoverboardBeenSeen = new Dictionary<Hoverboards.BoardType, bool>();
		_hoverboardUnlockStatus = new Dictionary<Hoverboards.BoardType, bool>();
		_inappHistory = new Dictionary<string, string>();
		_currentHoverboard = Hoverboards.BoardType.normal;
		_numberOfBreadCrumbsShownOnFrontPage = 0;
		_unreportedGames = 0;
		_unreportedGamesTimestamp = new DateTime(0L);
	}
}
