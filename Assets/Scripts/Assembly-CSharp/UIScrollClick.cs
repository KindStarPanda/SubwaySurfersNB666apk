using UnityEngine;

public class UIScrollClick : MonoBehaviour
{
	[SerializeField]
	private GameObject target;

	private void OnClick()
	{
		Vector2 pos = UICamera.currentTouch.pos;
		target.SendMessage("ScrollClicked", pos, SendMessageOptions.DontRequireReceiver);
	}
}
