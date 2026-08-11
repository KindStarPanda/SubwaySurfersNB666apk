using UnityEngine;

public class Glow : MonoBehaviour
{
	private MeshRenderer meshRenderer;

	public void Awake()
	{
		Transform transform = base.transform;
		Transform parent = transform.parent;
		Transform transform2 = ((!(parent == null)) ? parent.parent : null);
		meshRenderer = GetComponentInChildren<MeshRenderer>();
		if (DeviceInfo.performanceLevel != 0 || (!parent.gameObject.name.Contains("coin") && (!(transform2 != null) || !transform2.gameObject.name.Contains("coin"))))
		{
			return;
		}
		foreach (Transform item in base.transform)
		{
			Object.Destroy(item.gameObject);
		}
		meshRenderer = GetComponentInChildren<MeshRenderer>();
		if (meshRenderer != null)
		{
			meshRenderer.enabled = false;
			base.enabled = false;
			Object.Destroy(meshRenderer);
			meshRenderer = null;
		}
	}

	public void SetVisible(bool visible)
	{
		if (meshRenderer != null)
		{
			meshRenderer.enabled = visible;
			base.enabled = visible;
		}
	}
}
