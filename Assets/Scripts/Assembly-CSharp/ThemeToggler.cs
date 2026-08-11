using UnityEngine;

public class ThemeToggler : MonoBehaviour
{
	private Theme theme;

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.T))
		{
			if (theme == Theme.NORMAL)
			{
				theme = Theme.HALLOWEEN;
				ThemeManager.Instance.Theme = theme;
			}
			else
			{
				theme = Theme.NORMAL;
				ThemeManager.Instance.Theme = theme;
			}
		}
	}
}
