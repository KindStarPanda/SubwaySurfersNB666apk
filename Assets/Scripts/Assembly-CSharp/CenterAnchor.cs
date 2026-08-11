using UnityEngine;

public class CenterAnchor : MonoBehaviour
{
	private bool hasInited;

	[SerializeField]
	private bool useX;

	[SerializeField]
	private bool useY;

	private void Start()
	{
		float num = UIScreenController.Instance.root.manualHeight;
		if (!hasInited)
		{
			if (useY)
			{
				base.transform.localPosition = new Vector3(base.transform.localPosition.x, num / 2f, base.transform.localPosition.z);
			}
			hasInited = true;
		}
	}
}
