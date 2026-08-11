using System.Collections;
using UnityEngine;

public class ScrollViewHeightResizer : MonoBehaviour
{
	[SerializeField]
	private GameObject _scrollBar;

	[SerializeField]
	private GameObject _scrollCollider;

	[SerializeField]
	private UIPanel _scrollPanel;

	[SerializeField]
	private GameObject _grid;

	[SerializeField]
	private float _staticObjectsHeight;

	[SerializeField]
	private float _centerOffset;

	[SerializeField]
	private float _topStaticObjectsHeight;

	[SerializeField]
	private bool _addTopCollider;

	[SerializeField]
	private bool _addBottomcollider;

	[SerializeField]
	private float _zDepth;

	private bool isCalculated;

	[SerializeField]
	private bool rearrange;

	public Vector4 clipping
	{
		get
		{
			if (!isCalculated)
			{
				RearrangeWidgets();
			}
			return _scrollPanel.clipRange;
		}
	}

	public Vector3 scrollPanelPosition
	{
		get
		{
			if (!isCalculated)
			{
				RearrangeWidgets();
			}
			return _scrollPanel.transform.localPosition;
		}
	}

	private void Update()
	{
		if (rearrange)
		{
			RearrangeWidgets();
			rearrange = false;
		}
	}

	private void Start()
	{
		rearrange = false;
		if (_scrollBar == null)
		{
			Debug.Log("ScrollBar not set in ScrollViewHeightResizer");
		}
		if (_scrollCollider == null)
		{
			Debug.Log("ScrollCollider not set in ScrollViewHeightResizer");
		}
		if (_scrollPanel == null)
		{
			Debug.Log("ScrollPanel not set in ScrollViewHeightResizer");
		}
		if (_grid == null)
		{
			Debug.Log("Grid not set in ScrollViewHeightResizer");
		}
		RearrangeWidgets();
	}

	public void RearrangeWidgets()
	{
		UIRoot uIRoot = null;
		if (UIScreenController.Instance != null)
		{
			uIRoot = UIScreenController.Instance.root;
		}
		if (uIRoot == null)
		{
			Debug.LogWarning("Root is not set in the UIScreenController");
		}
		UIScrollBar component = _scrollBar.GetComponent<UIScrollBar>();
		float y = (float)(uIRoot.manualHeight / 2) + _centerOffset;
		float w = (float)uIRoot.manualHeight - _staticObjectsHeight;
		Vector4 clipRange = _scrollPanel.clipRange;
		clipRange.y = y;
		clipRange.w = w;
		_scrollPanel.clipRange = clipRange;
		Vector3 localPosition = _scrollPanel.transform.localPosition;
		localPosition.y = 0f;
		_scrollPanel.transform.localPosition = localPosition;
		Vector3 localPosition2 = _grid.transform.localPosition;
		localPosition2.y = (float)uIRoot.manualHeight - _topStaticObjectsHeight - 2f;
		_grid.transform.localPosition = localPosition2;
		float y2 = (float)(uIRoot.manualHeight / 2) + _centerOffset;
		float y3 = (float)uIRoot.manualHeight - _staticObjectsHeight;
		Vector3 localPosition3 = _scrollCollider.transform.localPosition;
		localPosition3.y = y2;
		_scrollCollider.transform.localPosition = localPosition3;
		Vector3 localScale = _scrollCollider.transform.localScale;
		localScale.y = y3;
		_scrollCollider.transform.localScale = localScale;
		UISprite background = component.background;
		Vector3 localScale2 = background.transform.localScale;
		localScale2.y = (float)uIRoot.manualHeight - _staticObjectsHeight;
		background.transform.localScale = localScale2;
		Vector3 localPosition4 = _scrollBar.transform.localPosition;
		localPosition4.y = (float)uIRoot.manualHeight - _topStaticObjectsHeight;
		_scrollBar.transform.localPosition = localPosition4;
		StartCoroutine(RepositionDraggablePanel(2));
		AddColliders();
		isCalculated = true;
	}

	private IEnumerator RepositionDraggablePanel(int frames)
	{
		int index = 0;
		while (index < frames)
		{
			index++;
			yield return null;
		}
		UIDraggablePanel dragPn = _scrollPanel.GetComponent<UIDraggablePanel>();
		if (dragPn != null)
		{
			dragPn.repositionClipping = true;
		}
	}

	private void AddColliders()
	{
		Vector3 zero = Vector3.zero;
		zero.z = _zDepth;
		if (!isCalculated)
		{
			if (_addTopCollider)
			{
				GameObject gameObject = NGUITools.AddChild(base.gameObject.transform.gameObject);
				gameObject.AddComponent<BoxCollider>().center = zero;
				gameObject.name = "TopScrollBlocker";
				float num = (float)Screen.height - (_scrollCollider.transform.localPosition.y + _scrollCollider.transform.localScale.y / 2f);
				float y = (float)Screen.height - num / 2f;
				float x = Screen.width;
				float x2 = 0f;
				float z = base.gameObject.transform.localPosition.z;
				gameObject.transform.localPosition = new Vector3(x2, y, z);
				gameObject.transform.localScale = new Vector3(x, num, 1f);
			}
			if (_addBottomcollider)
			{
				GameObject gameObject2 = NGUITools.AddChild(base.gameObject.transform.gameObject);
				gameObject2.AddComponent<BoxCollider>().center = zero;
				gameObject2.name = "BottomScrollBlocker";
				float num2 = _scrollCollider.transform.localPosition.y - _scrollCollider.transform.localScale.y / 2f;
				float y2 = num2 / 2f;
				float x3 = Screen.width;
				float x4 = 0f;
				float z2 = base.gameObject.transform.localPosition.z;
				gameObject2.transform.localPosition = new Vector3(x4, y2, z2);
				gameObject2.transform.localScale = new Vector3(x3, num2, 1f);
			}
		}
	}
}
