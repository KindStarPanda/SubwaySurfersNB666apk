using UnityEngine;

[ExecuteInEditMode]
public class UITextShadowHelper : MonoBehaviour
{
	public UILabel shadowLabel;

	public Color frontColor = Color.white;

	public Color shadowColor = Color.magenta;

	public bool updateDynamically;

	public bool fakeHD;

	private UILabel _frontLabel;

	public Vector3 shadowOffset;

	private Transform _frontTransform;

	private Transform _shadowTransform;

	private void Awake()
	{
		_frontLabel = base.gameObject.GetComponent<UILabel>();
		_frontTransform = base.transform;
		if (shadowLabel != null)
		{
			_shadowTransform = shadowLabel.cachedTransform;
			shadowLabel.depth = _frontLabel.depth - 1;
			shadowLabel.gameObject.name = base.name + "Shadow";
		}
	}

	private void Start()
	{
		UpdateText();
	}

	public void UpdateNow()
	{
		UpdateText();
	}

	private void UpdateText()
	{
		if (!(shadowLabel == null))
		{
			if (_frontLabel.color != frontColor)
			{
				_frontLabel.color = frontColor;
			}
			if (_frontLabel.effectColor != shadowColor)
			{
				_frontLabel.effectColor = shadowColor;
			}
			if (shadowLabel.text != _frontLabel.text)
			{
				shadowLabel.text = _frontLabel.text;
			}
			if (shadowLabel.color != shadowColor)
			{
				shadowLabel.color = shadowColor;
			}
			if (shadowLabel.depth != _frontLabel.depth - 1)
			{
				shadowLabel.depth = _frontLabel.depth - 1;
			}
			if (shadowLabel.gameObject.name != base.name + "Shadow")
			{
				shadowLabel.gameObject.name = base.name + "Shadow";
			}
			if (DeviceInfo.isHighres || fakeHD)
			{
				_shadowTransform.localPosition = _frontTransform.localPosition + shadowOffset;
			}
			else
			{
				_shadowTransform.localPosition = _frontTransform.localPosition + shadowOffset;
			}
		}
	}

	private void Update()
	{
		if (_frontLabel == shadowLabel)
		{
			Debug.LogError("front and shadow label are the same!", base.gameObject);
			shadowLabel = null;
			Debug.Break();
		}
		if (updateDynamically)
		{
			if (_shadowTransform == null)
			{
				_frontLabel = base.gameObject.GetComponent<UILabel>();
				_frontTransform = base.transform;
				_shadowTransform = shadowLabel.cachedTransform;
			}
			UpdateText();
		}
	}

	private void OnDisable()
	{
		if (shadowLabel != null)
		{
			shadowLabel.gameObject.active = false;
		}
	}

	private void OnEnable()
	{
		if (shadowLabel != null)
		{
			shadowLabel.gameObject.active = true;
		}
	}
}
