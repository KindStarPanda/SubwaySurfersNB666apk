using UnityEngine;

public class FriendGhostHelper : MonoBehaviour
{
	public enum AnimatingState
	{
		_notset = 0,
		OnScreen = 1,
		OffScreen = 2,
		AnimatingIn = 3,
		AnimatingOut = 4
	}

	public UISlicedSprite background;

	public UISlicedSprite frame;

	public UILabel points;

	public UITexture picture;

	private Transform _cachedTransform;

	private Vector3 _resetPosition = new Vector3(80f, -75f, 0f);

	private Vector3 _activePosition = new Vector3(0f, -75f, 0f);

	private Vector3 _moveOutPosition = new Vector3(0f, -215f, 0f);

	private float _backgroundAlphaDefault;

	private float _frameAlphaDefault;

	private float _pointsAlphaDefault;

	private float _pictureAlphaDefault;

	private bool inited;

	private bool _gameRunning;

	public bool noFriendsLeftToGhost;

	private FriendGhostHandler handler;

	private float _animationDuration = 0.5f;

	private float _currentAnimationDuration;

	public AnimatingState currentAnimationState;

	private void Update()
	{
		if (!inited)
		{
			Init();
		}
		switch (currentAnimationState)
		{
		case AnimatingState.OnScreen:
			break;
		case AnimatingState.OffScreen:
			break;
		case AnimatingState.AnimatingIn:
		{
			_currentAnimationDuration += Time.deltaTime;
			Vector3 localPosition2 = Vector3.Lerp(_resetPosition, _activePosition, _currentAnimationDuration / _animationDuration);
			_cachedTransform.localPosition = localPosition2;
			if (_currentAnimationDuration >= _animationDuration)
			{
				_currentAnimationDuration = 0f;
				currentAnimationState = AnimatingState.OnScreen;
			}
			break;
		}
		case AnimatingState.AnimatingOut:
		{
			_currentAnimationDuration += Time.deltaTime;
			float t = _currentAnimationDuration / _animationDuration;
			Vector3 localPosition = Vector3.Lerp(_activePosition, _moveOutPosition, _currentAnimationDuration / _animationDuration);
			background.alpha = Mathf.Lerp(_backgroundAlphaDefault, 0f, t);
			frame.alpha = Mathf.Lerp(_frameAlphaDefault, 0f, t);
			points.alpha = Mathf.Lerp(_pointsAlphaDefault, 0f, t);
			picture.alpha = Mathf.Lerp(_pictureAlphaDefault, 0f, t);
			if (_currentAnimationDuration >= _animationDuration)
			{
				_currentAnimationDuration = 0f;
				_cachedTransform.localPosition = _resetPosition;
				background.alpha = _backgroundAlphaDefault;
				frame.alpha = _frameAlphaDefault;
				points.alpha = _pointsAlphaDefault;
				picture.alpha = _pictureAlphaDefault;
				currentAnimationState = AnimatingState.OffScreen;
				handler.FinishedAnimatingOut();
			}
			else
			{
				_cachedTransform.localPosition = localPosition;
			}
			break;
		}
		}
	}

	private void Init()
	{
		_backgroundAlphaDefault = background.alpha;
		_frameAlphaDefault = frame.alpha;
		_pointsAlphaDefault = points.alpha;
		_pictureAlphaDefault = picture.alpha;
		_cachedTransform = base.transform;
		picture.material = new Material(Shader.Find("Unlit/Transparent Colored"));
		inited = true;
		handler = _cachedTransform.GetComponent<FriendGhostHandler>();
	}

	public void NewGame()
	{
		if (!inited)
		{
			Init();
		}
		_gameRunning = true;
		_cachedTransform.localPosition = _resetPosition;
		background.alpha = _backgroundAlphaDefault;
		frame.alpha = _frameAlphaDefault;
		points.alpha = _pointsAlphaDefault;
		picture.alpha = _pictureAlphaDefault;
		noFriendsLeftToGhost = false;
	}

	public void AnimateIn()
	{
		if (!noFriendsLeftToGhost)
		{
			currentAnimationState = AnimatingState.AnimatingIn;
		}
	}

	public void AnimateOut()
	{
		if (_gameRunning)
		{
			currentAnimationState = AnimatingState.AnimatingOut;
		}
	}

	public void NoFriendsLeft()
	{
		_cachedTransform.localPosition = _resetPosition;
		background.alpha = _backgroundAlphaDefault;
		frame.alpha = _frameAlphaDefault;
		points.alpha = _pointsAlphaDefault;
		picture.alpha = _pictureAlphaDefault;
		noFriendsLeftToGhost = true;
	}

	public void GameOver()
	{
		if (!inited)
		{
			Init();
		}
		_gameRunning = false;
		_cachedTransform.localPosition = _resetPosition;
		background.alpha = _backgroundAlphaDefault;
		frame.alpha = _frameAlphaDefault;
		points.alpha = _pointsAlphaDefault;
		picture.alpha = _pictureAlphaDefault;
		currentAnimationState = AnimatingState.OffScreen;
	}
}
