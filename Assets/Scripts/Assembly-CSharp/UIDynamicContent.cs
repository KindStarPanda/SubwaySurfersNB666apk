using UnityEngine;

public class UIDynamicContent : MonoBehaviour
{
	public GameObject[] PanelElements;

	private void Start()
	{
		InitElements();
	}

	public void InitElements()
	{
		for (int i = 0; i < PanelElements.Length; i++)
		{
			NGUITools.AddChild(base.gameObject, PanelElements[i]);
		}
		base.gameObject.BroadcastMessage("CreatePanel", SendMessageOptions.DontRequireReceiver);
	}
}
