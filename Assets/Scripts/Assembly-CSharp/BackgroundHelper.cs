using UnityEngine;

public class BackgroundHelper : MonoBehaviour
{
	[SerializeField]
	private Material backgroundMat;

	[SerializeField]
	private Texture normalTex;

	[SerializeField]
	private Texture themeTex;

	private Theme cachedTheme = Theme.NORMAL;

	private bool _hasInited;

	private void OnEnable()
	{
		if (!_hasInited)
		{
			backgroundMat.mainTexture = normalTex;
			ThemeManager.Instance.OnChangeTheme += SetBackgroundTexture;
			if (DeviceInfo.formFactor == DeviceInfo.FormFactor.iPad)
			{
				base.gameObject.GetComponent<UIStretch>().relativeSize = Vector2.one;
			}
			else
			{
				base.gameObject.GetComponent<UIStretch>().relativeSize = new Vector2(1.0667f, 1.2f);
			}
			_hasInited = true;
		}
		SetBackgroundTexture(ThemeManager.Instance.Theme);
	}

	private void SetBackgroundTexture(Theme newTheme)
	{
		if (newTheme != cachedTheme)
		{
			if (newTheme == Globals.UPDATE_DEFAULT_THEME)
			{
				backgroundMat.mainTexture = themeTex;
			}
			else
			{
				backgroundMat.mainTexture = normalTex;
			}
			cachedTheme = newTheme;
		}
	}

	private void OnDestroy()
	{
		if (_hasInited)
		{
			ThemeManager.Instance.OnChangeTheme -= SetBackgroundTexture;
		}
	}
}
