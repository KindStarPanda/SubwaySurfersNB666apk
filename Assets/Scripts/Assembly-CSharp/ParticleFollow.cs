using UnityEngine;

public class ParticleFollow : MonoBehaviour
{
	public Transform Target;

	public float TweenTime;

	public float SineOffset;

	public float SineSpeed;

	public float RotationTweenTime;

	private float tweenVelocity;

	private Vector3 baseRotation;

	private float tweenRotVelocity;

	private void Awake()
	{
		baseRotation = base.transform.localEulerAngles;
		base.gameObject.SetActiveRecursively(false);
	}

	private void OnEnable()
	{
		tweenVelocity = 0f;
		tweenRotVelocity = 0f;
	}

	private void LateUpdate()
	{
		Vector3 position = Target.position;
		position.x = base.transform.position.x;
		float num = Mathf.SmoothDamp(position.x, Target.position.x, ref tweenVelocity, TweenTime);
		if (!float.IsNaN(num))
		{
			position.x = num;
		}
		else
		{
			tweenVelocity = 0f;
			tweenRotVelocity = 0f;
		}
		base.transform.position = position;
		Vector3 localEulerAngles = baseRotation;
		localEulerAngles.y = Mathf.SmoothDampAngle(localEulerAngles.y, Target.localEulerAngles.y, ref tweenRotVelocity, RotationTweenTime);
		float num2 = Mathf.Sin(SineOffset + Time.time * SineSpeed) * 5.5f;
		localEulerAngles.y += num2;
		base.transform.localEulerAngles = localEulerAngles;
	}
}
