using UnityEngine;

public class CoinLabelSizer : MonoBehaviour
{
	[SerializeField]
	private Transform coinIcon;

	[SerializeField]
	private UILabel coinLabel;

	private Vector3 originalLabelPos;

	private float coinDist;

	private float cachedWidth;

	private bool _hasInited;

	private void Awake()
	{
		if (!_hasInited)
		{
			Init();
		}
	}

	private void Init()
	{
		originalLabelPos = coinLabel.transform.localPosition;
		coinDist = coinIcon.localScale.x * 0.5f + coinLabel.cachedTransform.localScale.x * 0.08f;
		_hasInited = true;
	}

	public void DisableCoinLabel()
	{
		coinIcon.gameObject.active = false;
		coinLabel.gameObject.active = false;
	}

	public void NewCoinAmount(int coinAmount)
	{
		NewCoinAmount(coinAmount.ToString());
	}

	public void NewCoinAmount(string coinAmount)
	{
		if (!_hasInited)
		{
			Init();
		}
		coinLabel.text = coinAmount;
		float num = coinLabel.relativeSize.x * coinLabel.cachedTransform.localScale.x;
		if (num < 1f)
		{
			num = 1f;
		}
		if (num != cachedWidth)
		{
			coinLabel.transform.localPosition = originalLabelPos + new Vector3(coinDist, 0f, 0f);
			coinIcon.localPosition = coinLabel.transform.localPosition - new Vector3(num * 0.5f + coinDist, 0f, 0f);
			cachedWidth = num;
		}
		coinLabel.MakePositionPerfect();
		coinIcon.GetComponent<UISprite>().MakePixelPerfect();
		if (coinLabel.panel != null)
		{
			coinLabel.panel.widgetsAreStatic = false;
			coinLabel.panel.SendMessage("LateUpdate");
			coinLabel.panel.widgetsAreStatic = true;
		}
	}

	public UILabel GetCoinLabel()
	{
		return coinLabel;
	}
}
