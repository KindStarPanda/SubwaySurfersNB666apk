using UnityEngine;

public class HackFix : MonoBehaviour
{
	private int originalMask;

	private void Awake()
	{
		originalMask = base.GetComponent<Camera>().cullingMask;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.C))
		{
			base.GetComponent<Camera>().cullingMask = 2048;
		}
		if (Input.GetKeyDown(KeyCode.V))
		{
			base.GetComponent<Camera>().cullingMask = originalMask;
		}
	}
}
