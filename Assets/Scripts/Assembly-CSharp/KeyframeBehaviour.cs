using System.Collections.Generic;
using UnityEngine;

public class KeyframeBehaviour : MonoBehaviour
{
	public Animation TargetAnimation;

	public List<ParticleSystem> TargetObjects;

	public List<KeyFrameAction> Actions;

	private void Start()
	{
		int num = 0;
		foreach (KeyFrameAction action in Actions)
		{
			if (TargetAnimation[action.clip] != null)
			{
				AnimationEvent animationEvent = new AnimationEvent();
				animationEvent.messageOptions = SendMessageOptions.RequireReceiver;
				if (TargetAnimation[action.clip] != null)
				{
					animationEvent.time = (float)action.KeyFrame / TargetAnimation[action.clip].clip.frameRate;
				}
				animationEvent.intParameter = num;
				animationEvent.functionName = "DoKeyframeAnimation";
				Globals.TryAddAnimationEvent(TargetAnimation, action.clip, animationEvent);
			}
			num++;
		}
	}

	public void DoKeyframeAnimation(int soundIndex)
	{
		KeyFrameAction info = Actions[soundIndex];
		TargetObjects.ForEach(delegate(ParticleSystem t)
		{
			t.enableEmission = info.state;
		});
	}
}
