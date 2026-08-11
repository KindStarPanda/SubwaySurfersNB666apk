using UnityEngine;

public class WarmUpShader : MonoBehaviour
{
	private void Start()
	{
		Shader.WarmupAllShaders();
	}
}
