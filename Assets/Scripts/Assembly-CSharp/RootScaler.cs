using UnityEngine;

[RequireComponent(typeof(UIRoot))]
public class RootScaler : MonoBehaviour
{
	private UIRoot myUIRoot;

	private void Awake()
	{
		myUIRoot = base.gameObject.GetComponent<UIRoot>();
		if (UIScreen.IsScreenHeightOutOfProportion())
		{
			float num = Screen.height;
			if (num > 480f)
			{
				float num2 = (float)Screen.width / 320f;
				num2 = (float)Screen.height / num2;
				myUIRoot.manualHeight = (int)num2;
			}
		}
		else
		{
			myUIRoot.manualHeight = 480;
		}
	}
}
