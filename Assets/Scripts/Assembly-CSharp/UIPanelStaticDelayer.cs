using System.Collections;
using UnityEngine;

public class UIPanelStaticDelayer : MonoBehaviour
{
	[SerializeField]
	private int framesToWait;

	private void Awake()
	{
		UIPanel component = GetComponent<UIPanel>();
		if (component == null)
		{
			Debug.LogWarning("UIPanelStaticDelayer is not set on a UIPanel");
			return;
		}
		if (framesToWait < 0)
		{
			Debug.LogWarning("UIPanelStaticDelayer.framesToWait can not be less than 0");
			return;
		}
		if (component.widgetsAreStatic)
		{
			component.widgetsAreStatic = false;
		}
		StartCoroutine(SetStaticDelayed(framesToWait, component));
	}

	public IEnumerator SetStaticDelayed(int delayFrames, UIPanel panel)
	{
		int num = 0;
		while (num < delayFrames)
		{
			num++;
			yield return null;
		}
		panel.widgetsAreStatic = true;
	}
}
