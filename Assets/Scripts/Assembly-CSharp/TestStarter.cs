using UnityEngine;

public class TestStarter : MonoBehaviour
{
	public string screenToShowAtStart;

	private void Update()
	{
		if (!string.IsNullOrEmpty(screenToShowAtStart))
		{
			UIScreenController.Instance.PushScreen(null, screenToShowAtStart);
		}
		base.enabled = false;
	}
}
