using UnityEngine;

public class LightSignalSafeSurface : MonoBehaviour
{
	private void Awake()
	{
		EnsureSafeCollider();
	}

	private void EnsureSafeCollider()
	{
		BoxCollider boxCollider = GetComponent<BoxCollider>();
		if (boxCollider == null)
		{
			boxCollider = gameObject.AddComponent<BoxCollider>();
		}

		Renderer renderer = GetComponentInChildren<Renderer>();
		if (renderer != null)
		{
			Bounds bounds = renderer.bounds;
			Vector3 localCenter = transform.InverseTransformPoint(bounds.center + Vector3.up * (bounds.extents.y * 0.05f));
			boxCollider.center = localCenter;
			boxCollider.size = new Vector3(Mathf.Max(bounds.size.x * 0.8f, 4f), Mathf.Max(bounds.size.y * 0.15f, 1.2f), Mathf.Max(bounds.size.z * 0.8f, 4f));
		}
		else
		{
			boxCollider.center = new Vector3(0f, 4f, 0f);
			boxCollider.size = new Vector3(6f, 1.2f, 6f);
		}

		boxCollider.isTrigger = false;
		boxCollider.enabled = true;
	}
}
