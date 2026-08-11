using UnityEngine;

public class InAppHelper : MonoBehaviour
{
	[SerializeField]
	private AudioClip purchaseSuccessSound;

	private void Start()
	{
		InAppManager.Instance.purchaseSuccessSound = purchaseSuccessSound;
	}
}
