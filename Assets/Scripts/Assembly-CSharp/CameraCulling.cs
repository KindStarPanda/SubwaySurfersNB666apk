using UnityEngine;

public class CameraCulling : MonoBehaviour
{
	private float[] distances = new float[32];

	public float TransparentFXCullingDistance
	{
		set
		{
			distances[LayerMask.NameToLayer("TransparentFX")] = value;
			base.GetComponent<Camera>().layerCullDistances = distances;
		}
	}
}
