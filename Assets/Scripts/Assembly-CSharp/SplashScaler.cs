using UnityEngine;

[ExecuteInEditMode]
public class SplashScaler : MonoBehaviour
{
	private const float Y_SCALE = 1.775f;

	private bool passed;

	[SerializeField]
	private bool repositionNow;

	private void Update()
	{
		if (repositionNow)
		{
			passed = false;
			repositionNow = false;
		}
		if (!passed)
		{
			float y = base.transform.localScale.x * 1.775f;
			Vector3 localScale = new Vector3(base.transform.localScale.x, y, 1f);
			base.transform.localPosition = new Vector3(0f, 0f, 0f);
			base.transform.localScale = localScale;
			passed = true;
		}
	}
}
