using UnityEngine;

public class ContentChange : MonoBehaviour
{
	private bool foldedOut;

	public GameObject button;

	public GameObject openButton;

	public GameObject descriptionButton;

	[SerializeField]
	private UILabel descriptionText;

	private GameObject _descriptionTextGameObject;

	[SerializeField]
	private BoxCollider buyButtonCollider;

	[SerializeField]
	private UISprite buyButtonSprite;

	[SerializeField]
	private UILabel buyButtonLabel;

	[SerializeField]
	private BoxCollider descButtonCollider;

	[SerializeField]
	private UISprite descButtonSprite;

	[SerializeField]
	private UILabel descButtonLabel;

	[SerializeField]
	private UISprite openButtonSprite;

	private UITable _table;

	private bool _hasInited;

	private void Start()
	{
		_descriptionTextGameObject = descriptionText.gameObject;
		foldedOut = false;
		_table = NGUITools.FindInParents<UITable>(base.gameObject);
		ContentActivation(false);
		_hasInited = true;
	}

	private void OnEnable()
	{
		if (_hasInited)
		{
			ContentActivation(foldedOut);
			if (_table != null)
			{
				_table.repositionNow = true;
			}
		}
	}

	public void TriggerContent()
	{
		if (foldedOut)
		{
			ContentActivation(foldedOut);
		}
	}

	public void FoldClicked()
	{
		if (!foldedOut)
		{
			foldedOut = true;
			return;
		}
		foldedOut = false;
		ContentActivation(false);
	}

	private void ContentActivation(bool foldedOut)
	{
		descriptionText.enabled = foldedOut;
		_descriptionTextGameObject.active = foldedOut;
		if (buyButtonSprite != null)
		{
			buyButtonCollider.center = new Vector3(buyButtonCollider.center.x, buyButtonCollider.center.y, -15f);
			buyButtonCollider.enabled = foldedOut;
			buyButtonSprite.enabled = foldedOut;
			buyButtonLabel.enabled = foldedOut;
		}
		if (button != null)
		{
			NGUITools.SetActive(button, foldedOut);
		}
		if (descButtonSprite != null)
		{
			descButtonCollider.center = new Vector3(descButtonCollider.center.x, descButtonCollider.center.y, -15f);
			descButtonCollider.enabled = foldedOut;
			descButtonSprite.enabled = foldedOut;
			descButtonLabel.enabled = foldedOut;
		}
		if (descriptionButton != null)
		{
			NGUITools.SetActive(descriptionButton, foldedOut);
		}
		openButtonSprite.enabled = !foldedOut;
		NGUITools.SetActive(openButton, !foldedOut);
		if (!foldedOut)
		{
			openButtonSprite.panel.SendMessage("LateUpdate");
			if (_table != null)
			{
				_table.Reposition();
			}
		}
	}
}
