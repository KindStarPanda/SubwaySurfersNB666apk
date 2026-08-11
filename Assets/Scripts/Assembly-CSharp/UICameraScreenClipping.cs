using UnityEngine;

public class UICameraScreenClipping : MonoBehaviour
{
	private Camera _cam;

	private void Start()
	{
		_cam = base.gameObject.GetComponent<Camera>();
		if (_cam == null)
		{
			Debug.LogError("The UICameraScreenClipping script is not attached to a Camera");
		}
		else if (!UIScreen.IsScreenHeightOutOfProportion())
		{
			CalculateClipping();
		}
	}

	private void CalculateClipping()
	{
		UIRoot root = UIScreenController.Instance.root;
		if (root == null)
		{
			Debug.LogError("UIRoot not set in UIScreenController");
		}
		float num = Screen.width * 480 / Screen.height;
		float num2 = 300f / num;
		float x = 0.5f - num2 / 2f;
		Rect rect = _cam.rect;
		rect.x = x;
		rect.width = num2;
		_cam.rect = rect;
	}
}
