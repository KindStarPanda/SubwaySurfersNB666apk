using UnityEngine;

public class UIScreen : MonoBehaviour
{
	public virtual void Init()
	{
		Component[] componentsInChildren = GetComponentsInChildren<UIAnchor>(true);
		Component[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			UIAnchor uIAnchor = (UIAnchor)array[i];
			uIAnchor.SendMessage("Start");
			uIAnchor.SendMessage("Update");
		}
		Component[] componentsInChildren2 = GetComponentsInChildren<UIStretch>(true);
		Component[] array2 = componentsInChildren2;
		for (int j = 0; j < array2.Length; j++)
		{
			UIStretch uIStretch = (UIStretch)array2[j];
			uIStretch.SendMessage("Start");
			uIStretch.SendMessage("Update");
		}
		AdjustToResolution();
	}

	public virtual void Show()
	{
		base.gameObject.SetActiveRecursively(true);
		string flurryScreenName = UIScreens.GetFlurryScreenName(base.gameObject.name);
		if (flurryScreenName != string.Empty)
		{
			Flurry.LogEvent(flurryScreenName);
		}
	}

	public virtual void Hide()
	{
		base.gameObject.SetActiveRecursively(false);
	}

	public virtual void AdjustToResolution()
	{
	}

	public static bool IsScreenHeightOutOfProportion()
	{
		return (float)Screen.height * 1f / ((float)Screen.width * 1f) > 1.5f;
	}
}
