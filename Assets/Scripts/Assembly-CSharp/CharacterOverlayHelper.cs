using UnityEngine;

public class CharacterOverlayHelper : MonoBehaviour
{
	private Characters.CharacterType _cachedType;

	private int _placementIndex;

	private Camera _3DClipCam;

	private Camera _2DOverlayCam;

	[SerializeField]
	private Transform headTrack;

	[SerializeField]
	private Transform feetTrack;

	private bool _hasInited;

	private Transform _cachedTransform;

	private Transform _headTransform;

	private Transform _chestTransform;

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

	public void Init(int index, Characters.CharacterType type, Transform transformToTrack)
	{
		_placementIndex = index;
		_cachedType = type;
		_3DClipCam = NGUITools.FindCameraForLayer(29);
		_2DOverlayCam = NGUITools.FindCameraForLayer(28);
		_cachedTransform = base.transform;
		_hasInited = true;
		_headTransform = TransformUtility.FindChild(transformToTrack, "jAfro_3");
		_chestTransform = TransformUtility.FindChild(transformToTrack, "jLowerSpine");
		limitedSprite.enabled = Characters.characterData[type].characterSeason != PlayerInfo.Season.none;
		newSprite.enabled = !limitedSprite.enabled && PlayerInfo.Instance.IsCharacterNew(type);
		UpdateSelected();
	}

	public int GetIndex()
	{
		return _placementIndex;
	}

	public void SelectedInMenu()
	{
		if (newSprite.enabled || PlayerInfo.Instance.IsCharacterNew(_cachedType))
		{
			newSprite.enabled = false;
			PlayerInfo.Instance.MarkCharacterAsSeen(_cachedType);
		}
	}

	public Characters.CharacterType GetCharacterType()
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
	}

	private void UpdateLimited()
	{
		if (PlayerInfo.Instance.IsCollectionComplete(_cachedType))
		{
			limitedSprite.enabled = false;
		}
	}

	private void UpdateSelected()
	{
		selectedSprite.enabled = PlayerInfo.Instance.currentCharacter == (int)_cachedType;
		ownedSprite.enabled = !selectedSprite.enabled && PlayerInfo.Instance.IsCollectionComplete(_cachedType);
	}

	private void LateUpdate()
	{
		if (_hasInited)
		{
			Vector3 position = new Vector3(_chestTransform.position.x, _headTransform.position.y * yOffset, _chestTransform.position.z);
			Vector3 position2 = _3DClipCam.WorldToScreenPoint(position);
			Vector3 position3 = _2DOverlayCam.ScreenToWorldPoint(position2);
			position3 = _cachedTransform.parent.InverseTransformPoint(position3);
			headTrack.localPosition = new Vector3(position3.x, position3.y, _cachedTransform.localPosition.z);
			Vector3 position4 = new Vector3(_chestTransform.position.x * xOffset, _chestTransform.position.y, _chestTransform.position.z);
			Vector3 position5 = _3DClipCam.WorldToScreenPoint(position4);
			Vector3 position6 = _2DOverlayCam.ScreenToWorldPoint(position5);
			position6 = _cachedTransform.parent.InverseTransformPoint(position6);
			position6 = new Vector3(position6.x + 20f, _cachedTransform.localPosition.y + 100f, _cachedTransform.localPosition.z);
			feetTrack.localPosition = position6;
		}
	}
}
