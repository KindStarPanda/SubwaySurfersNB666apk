using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarAnimations : MonoBehaviour
{
	public Animation Target;

	public bool PlayIdleAnimations;

	public int MinIdleTimes;

	public int MaxIdleTimes;

	public AnimationClip Breath;

	public List<AnimationClip> Idles;

	public List<AnimationClip> IdlesPopup;

	private bool usePopupIdles;

	public bool Paused;

	private IEnumerator animationRoutine;

	private float nextAnimationTime;

	private void Start()
	{
		Target = FindAnimationInParent(base.gameObject);
		if (Target == null)
		{
			Debug.LogError("AvatarAnimations: No animation component for avatar animations", this);
		}
		else if (PlayIdleAnimations)
		{
			if (usePopupIdles)
			{
				StartIdlePopupAnimations();
			}
			else
			{
				StartIdleAnimations();
			}
		}
	}

	private Animation FindAnimationInParent(GameObject current)
	{
		Animation component = current.GetComponent<Animation>();
		if (component != null)
		{
			return component;
		}
		if (current.transform.parent != null)
		{
			return FindAnimationInParent(current.transform.parent.gameObject);
		}
		return null;
	}

	private void Update()
	{
		if (PlayIdleAnimations && animationRoutine != null && !Paused)
		{
			animationRoutine.MoveNext();
		}
	}

	public void StartIdleAnimations()
	{
		PlayIdleAnimations = true;
		Paused = false;
		usePopupIdles = false;
		Target.AddClip(Breath, Breath.name);
		foreach (AnimationClip idle in Idles)
		{
			Target.AddClip(idle, idle.name);
		}
		animationRoutine = Play();
		animationRoutine.MoveNext();
	}

	public void StartIdlePopupAnimations()
	{
		PlayIdleAnimations = true;
		Paused = false;
		usePopupIdles = true;
		Target.AddClip(Breath, Breath.name);
		foreach (AnimationClip item in IdlesPopup)
		{
			Target.AddClip(item, item.name);
		}
		animationRoutine = Play();
		animationRoutine.MoveNext();
	}

	public void StopIdleAnimations()
	{
		PlayIdleAnimations = false;
		if (usePopupIdles)
		{
			Target.AddClip(Breath, Breath.name);
			foreach (AnimationClip item in IdlesPopup)
			{
				foreach (AnimationState item2 in Target)
				{
					if (item2.clip == item)
					{
						Target.RemoveClip(item);
					}
				}
			}
		}
		else
		{
			Target.AddClip(Breath, Breath.name);
			foreach (AnimationClip idle in Idles)
			{
				foreach (AnimationState item3 in Target)
				{
					if (item3.clip == idle)
					{
						Target.RemoveClip(idle);
					}
				}
			}
		}
		animationRoutine = null;
	}

	public void PauseIdleAnimations()
	{
		Paused = true;
		foreach (AnimationState item in Target.GetComponent<Animation>())
		{
			item.speed = 0f;
		}
	}

	public void ResumeIdleAnimations()
	{
		Paused = false;
		foreach (AnimationState item in Target.GetComponent<Animation>())
		{
			item.speed = 1f;
		}
	}

	private IEnumerator Play()
	{
		AnimationClip selectedClip = null;
		while (PlayIdleAnimations)
		{
			List<AnimationClip> possibleClips = ((!usePopupIdles) ? Idles : IdlesPopup).FindAll((AnimationClip a) => a != selectedClip);
			if (possibleClips.Count > 0)
			{
				selectedClip = possibleClips[Random.Range(0, possibleClips.Count)];
			}
			Target.Play(selectedClip.name);
			nextAnimationTime = selectedClip.length;
			while (nextAnimationTime > 0f)
			{
				nextAnimationTime -= Time.deltaTime;
				yield return null;
			}
			if (usePopupIdles)
			{
				continue;
			}
			int count = Random.Range(MinIdleTimes, MaxIdleTimes);
			for (int i = 0; i < count; i++)
			{
				Target.Play(Breath.name);
				nextAnimationTime = Breath.length;
				while (nextAnimationTime > 0f)
				{
					nextAnimationTime -= Time.deltaTime;
					yield return 0;
				}
			}
		}
		animationRoutine = null;
	}
}
