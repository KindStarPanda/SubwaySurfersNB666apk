using UnityEngine;

public class HoverboardOverlayHelper : MonoBehaviour
{
	private Hoverboards.BoardType _cachedType;

	private Hoverboards.Board _cachedBoard;

	private int _placementIndex;

	private Transform _trackerTransform;

	private Camera _3DClipCam;

	private Camera _2DOverlayCam;

	private bool _hasInited;

	private Transform _cachedTransform;

	[SerializeField]
	private Transform topOverlay;

	[SerializeField]
	private Transform bottomOverlay;

	[SerializeField]
	private float yOffset = 1f;

	[SerializeField]
	private float xOffset = 1f;

	[SerializeField]
	private UISprite newSprite;

	[SerializeField]
	private UISprite limitedSprite;

	[SerializeField]
	private UISprite ownedSprite;

	[SerializeField]
	private UISprite selectedSprite;

	[SerializeField]
	private UISprite giftSprite;

	private float _topOverlayMinX = 100f;

	private float _topOverlayMaxX = 110f;

	private float _currentDistFromCenter;

	private bool _dirtyDist = true;

	public float currentDistFromCenter
	{
		set
		{
			if (_currentDistFromCenter != value)
			{
				_currentDistFromCenter = value;
				_dirtyDist = true;
			}
		}
	}

	public void Init(int index, Hoverboards.BoardType type, Transform transformToTrack)
	{
		_placementIndex = index;
		_cachedType = type;
		_cachedBoard = Hoverboards.boardData[_cachedType];
		_3DClipCam = NGUITools.FindCameraForLayer(29);
		_2DOverlayCam = NGUITools.FindCameraForLayer(28);
		_cachedTransform = base.transform;
		_hasInited = true;
		_trackerTransform = transformToTrack;
		giftSprite.enabled = _cachedBoard.unlockType == Hoverboards.UnlockType.free && !PlayerInfo.Instance.isHoverboardUnlocked(_cachedType);
		limitedSprite.enabled = false;
		newSprite.enabled = !limitedSprite.enabled && !giftSprite.enabled && !PlayerInfo.Instance.HasHoverboardBeenSeen(_cachedType);
		UpdateSelected();
	}

	public int GetIndex()
	{
		return _placementIndex;
	}

	public void SelectedInMenu()
	{
		if (newSprite.enabled || !PlayerInfo.Instance.HasHoverboardBeenSeen(_cachedType))
		{
			newSprite.enabled = false;
			PlayerInfo.Instance.MarkHoverboardAsSeen(_cachedType);
		}
	}

	public Hoverboards.BoardType GetBoardType()
	{
		return _cachedType;
	}

	private void Update()
	{
		UpdateSelected();
		if (limitedSprite.enabled)
		{
			UpdateLimited();
		}
		if (giftSprite.enabled)
		{
			UpdateGift();
		}
	}

	private void UpdateLimited()
	{
		if (PlayerInfo.Instance.isHoverboardUnlocked(_cachedType))
		{
			limitedSprite.enabled = false;
		}
	}

	private void UpdateGift()
	{
		giftSprite.enabled = _cachedBoard.unlockType == Hoverboards.UnlockType.free && !PlayerInfo.Instance.isHoverboardUnlocked(_cachedType);
	}

	private void UpdateSelected()
	{
		selectedSprite.enabled = PlayerInfo.Instance.currentHoverboard == _cachedType;
		ownedSprite.enabled = !selectedSprite.enabled && PlayerInfo.Instance.isHoverboardUnlocked(_cachedType);
	}

	private void LateUpdate()
	{
		if (_hasInited && _dirtyDist)
		{
			_dirtyDist = false;
			Vector3 vector = _cachedTransform.parent.InverseTransformPoint(_2DOverlayCam.ScreenToWorldPoint(_3DClipCam.WorldToScreenPoint(_trackerTransform.position)));
			float y = Mathf.SmoothStep(_topOverlayMaxX, _topOverlayMinX, _currentDistFromCenter);
			topOverlay.localPosition = new Vector3(vector.x, y, _cachedTransform.localPosition.z);
			bottomOverlay.localPosition = new Vector3(vector.x, vector.y, _cachedTransform.localPosition.z);
		}
	}
}
