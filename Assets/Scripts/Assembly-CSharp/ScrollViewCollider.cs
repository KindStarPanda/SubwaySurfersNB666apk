using UnityEngine;

public class ScrollViewCollider : MonoBehaviour
{
	[SerializeField]
	private GameObject _scrollCollider;

	[SerializeField]
	private bool _addTopCollider;

	[SerializeField]
	private bool _addBottomCollider;

	[SerializeField]
	private float _zDepth;

	private bool _added;

	private void Start()
	{
		AddTopAndBottomColliders();
	}

	public void AddTopAndBottomColliders()
	{
		if (_scrollCollider != null)
		{
			Vector3 zero = Vector3.zero;
			zero.z = _zDepth;
			if (_addTopCollider && !_added)
			{
				GameObject gameObject = NGUITools.AddChild(base.gameObject.transform.parent.gameObject);
				gameObject.AddComponent<BoxCollider>().center = zero;
				gameObject.name = "TopScrollBlocker";
				float num = (float)(Screen.height / 2) - (_scrollCollider.transform.localScale.y / 2f + _scrollCollider.transform.localPosition.y);
				float y = (float)(Screen.height / 2) - num / 2f;
				float x = Screen.width;
				float x2 = 0f;
				float z = base.gameObject.transform.localPosition.z;
				gameObject.transform.localPosition = new Vector3(x2, y, z);
				gameObject.transform.localScale = new Vector3(x, num, 1f);
			}
			if (_addBottomCollider && !_added)
			{
				GameObject gameObject2 = NGUITools.AddChild(base.gameObject.transform.parent.gameObject);
				gameObject2.AddComponent<BoxCollider>().center = zero;
				gameObject2.name = "BottomScrollBlocker";
				float num2 = (float)(Screen.height / 2) - (_scrollCollider.transform.localScale.y / 2f - _scrollCollider.transform.localPosition.y);
				float y2 = (float)(-Screen.height / 2) + num2 / 2f;
				float x3 = Screen.width;
				float x4 = 0f;
				float z2 = base.gameObject.transform.localPosition.z;
				gameObject2.transform.localPosition = new Vector3(x4, y2, z2);
				gameObject2.transform.localScale = new Vector3(x3, num2, 1f);
			}
			_added = true;
		}
	}
}
