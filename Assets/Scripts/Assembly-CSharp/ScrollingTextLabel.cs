using System.Collections;
using UnityEngine;

public class ScrollingTextLabel : MonoBehaviour
{
	[SerializeField]
	private UILabel label;

	private bool _isScrolling;

	private Transform _labelTransform;

	private Color _labelBaseColor;

	private bool _destroyOnDisable;

	private void OnEnable()
	{
		label.enabled = false;
		_labelTransform = label.transform;
		_labelBaseColor = label.color;
	}

	private void OnDisable()
	{
		_isScrolling = false;
		if (_destroyOnDisable)
		{
			NGUITools.Destroy(base.gameObject);
		}
	}

	public void StartScrolling(string text, Vector3 startLocalPos, Vector3 endLocalPos, float duration, float fadeOutDuration, bool destroyWhenDone)
	{
		if (duration <= 0f)
		{
			Debug.LogError("Duration must be > 0", this);
		}
		else if (!_isScrolling)
		{
			StartCoroutine(StartScrollingCoroutine(text, startLocalPos, endLocalPos, duration, fadeOutDuration, destroyWhenDone));
		}
	}

	private IEnumerator StartScrollingCoroutine(string text, Vector3 startLocalPos, Vector3 endLocalPos, float duration, float fadeOutDuration, bool destroyWhenDone)
	{
		if (_isScrolling)
		{
			yield break;
		}
		_isScrolling = true;
		_destroyOnDisable = destroyWhenDone;
		float fadeOutAniFactorStart = Mathf.Clamp01((duration - fadeOutDuration) / duration);
		label.enabled = true;
		label.text = text;
		label.color = _labelBaseColor;
		startLocalPos.z = -1f;
		endLocalPos.z = -1f;
		float aniFactor = 0f;
		while (aniFactor < 1f)
		{
			aniFactor = Mathf.Clamp01(aniFactor + Time.deltaTime / duration);
			_labelTransform.localPosition = Vector3.Lerp(startLocalPos, endLocalPos, aniFactor);
			if (aniFactor >= fadeOutAniFactorStart)
			{
				float fadeOutFactor = (aniFactor - fadeOutAniFactorStart) / (1f - fadeOutAniFactorStart);
				Color c = _labelBaseColor;
				c.a *= 1f - fadeOutFactor;
				label.color = c;
			}
			yield return null;
		}
		label.enabled = false;
		_isScrolling = false;
		if (destroyWhenDone)
		{
			NGUITools.Destroy(base.gameObject);
		}
	}
}
