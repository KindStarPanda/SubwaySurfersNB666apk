using UnityEngine;

public class NotebookScreen : UIScreen
{
	private const int _pixelsOutsideTheBackground = 84;

	private const int _pixelsOutsideTheDecal = 130;

	[SerializeField]
	private UITexture blurredBackground;

	[SerializeField]
	private UISprite background;

	[SerializeField]
	private UISprite decal;

	[SerializeField]
	private UISprite spiral;

	[SerializeField]
	private UIAnchor _topDecalAnchor;

	[SerializeField]
	private UIAnchor _centerBackrenderAnchor;

	private bool isOnLoadScreen;

	public override void Show()
	{
		base.Show();
		ShowBackground();
	}

	public override void Hide()
	{
		base.Hide();
		HideBackground();
	}

	public override void AdjustToResolution()
	{
		base.AdjustToResolution();
		UIRoot uIRoot = null;
		if (UIScreenController.Instance != null)
		{
			uIRoot = UIScreenController.Instance.root;
		}
		else
		{
			isOnLoadScreen = true;
		}
		if (uIRoot == null)
		{
			uIRoot = NGUITools.FindInParents<UIRoot>(base.gameObject);
		}
		if (!isOnLoadScreen && UIScreen.IsScreenHeightOutOfProportion())
		{
			if (background != null && decal != null)
			{
				Vector3 localScale = background.transform.localScale;
				localScale.y = uIRoot.manualHeight - 84;
				background.transform.localScale = localScale;
				Vector3 localScale2 = decal.transform.localScale;
				localScale2.y = uIRoot.manualHeight - 130;
				decal.transform.localScale = localScale2;
			}
			else
			{
				Debug.Log("Set background and decal in NotebookScreen. They are null!");
			}
		}
		if (isOnLoadScreen)
		{
			Debug.Log("IS on LoadScene");
			ScaleForLoadScene();
		}
	}

	private void ScaleForLoadScene()
	{
		_topDecalAnchor.depthOffset = 1f;
		_centerBackrenderAnchor.depthOffset = 1f;
		Camera uiCamera = NGUITools.FindInParents<Camera>(base.gameObject);
		_topDecalAnchor.uiCamera = uiCamera;
		_centerBackrenderAnchor.uiCamera = uiCamera;
	}

	public void ShowBackground()
	{
		blurredBackground.enabled = true;
	}

	public void HideBackground()
	{
		blurredBackground.enabled = false;
	}
}
