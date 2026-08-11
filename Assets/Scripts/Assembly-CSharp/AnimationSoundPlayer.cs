using System.Collections.Generic;
using UnityEngine;

public class AnimationSoundPlayer : MonoBehaviour
{
	public Animation TargetAnimation;

	public List<KeyFrameAudio> AudioClips;

	private static List<string> nodesInitialized = new List<string>();

	private int nextAudioClipIndex;

	private List<KeyFrameAudio> AudioClipsInitialized = new List<KeyFrameAudio>();

	private void Start()
	{
		if (nodesInitialized.IndexOf(base.name) != -1)
		{
			return;
		}
		nodesInitialized.Add(base.name);
		foreach (KeyFrameAudio audioClip in AudioClips)
		{
			Add(audioClip);
		}
	}

	public void Add(KeyFrameAudio key)
	{
		AnimationEvent animationEvent = new AnimationEvent();
		animationEvent.messageOptions = SendMessageOptions.RequireReceiver;
		animationEvent.time = (float)key.KeyFrame / TargetAnimation[key.clip].clip.frameRate;
		animationEvent.intParameter = nextAudioClipIndex;
		animationEvent.functionName = "PlayKeyframeAnimation";
		if (Globals.TryAddAnimationEvent(TargetAnimation, key.clip, animationEvent))
		{
			nextAudioClipIndex++;
			AudioClipsInitialized.Add(key);
		}
	}

	public virtual void PlayKeyframeAnimation(int soundIndex)
	{
		if (soundIndex < AudioClipsInitialized.Count)
		{
			KeyFrameAudio keyFrameAudio = AudioClipsInitialized[soundIndex];
			if (keyFrameAudio.Callback != null)
			{
				keyFrameAudio.Callback(keyFrameAudio);
			}
			else
			{
				So.Instance.playSound(keyFrameAudio.Audio);
			}
		}
	}
}
