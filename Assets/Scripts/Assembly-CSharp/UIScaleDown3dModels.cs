using UnityEngine;

public class UIScaleDown3dModels : MonoBehaviour
{
	public enum ScaleAnchorType
	{
		CharacterAnchor = 0,
		GameOverAnchor = 1,
		TutorialPupupAnchor = 2
	}

	public ScaleAnchorType anchorType;

	private void OnEnable()
	{
		if (DeviceInfo.formFactor == DeviceInfo.FormFactor.iPad)
		{
			base.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
		}
		else if (DeviceInfo.formFactor == DeviceInfo.FormFactor.iPhone5)
		{
			base.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
		}
		else if (DeviceInfo.formFactor == DeviceInfo.FormFactor.small)
		{
			if (anchorType == ScaleAnchorType.CharacterAnchor)
			{
				base.transform.localPosition = new Vector3(-80f, -39f, 20f);
			}
			else if (anchorType == ScaleAnchorType.TutorialPupupAnchor)
			{
				base.transform.localPosition = new Vector3(75f, 10f, 30f);
			}
			else if (anchorType == ScaleAnchorType.GameOverAnchor)
			{
				base.transform.localPosition = new Vector3(-75f, 10f, 30f);
			}
			base.transform.localScale = new Vector3(0.85f, 0.85f, 0.85f);
		}
		else if (DeviceInfo.formFactor == DeviceInfo.FormFactor.medium)
		{
			if (anchorType == ScaleAnchorType.CharacterAnchor)
			{
				base.transform.localPosition = new Vector3(-80f, -39f, 20f);
			}
			else if (anchorType == ScaleAnchorType.TutorialPupupAnchor)
			{
				base.transform.localPosition = new Vector3(75f, 10f, 30f);
			}
			else if (anchorType == ScaleAnchorType.GameOverAnchor)
			{
				base.transform.localPosition = new Vector3(-75f, 10f, 30f);
			}
			base.transform.localScale = new Vector3(0.85f, 0.85f, 0.85f);
		}
		else if (DeviceInfo.formFactor == DeviceInfo.FormFactor.large)
		{
			if (anchorType == ScaleAnchorType.CharacterAnchor)
			{
				base.transform.localPosition = new Vector3(-80f, -39f, 20f);
			}
			else if (anchorType == ScaleAnchorType.TutorialPupupAnchor)
			{
				base.transform.localPosition = new Vector3(75f, 10f, 30f);
			}
			else if (anchorType == ScaleAnchorType.GameOverAnchor)
			{
				base.transform.localPosition = new Vector3(-75f, 10f, 30f);
			}
			base.transform.localScale = new Vector3(0.85f, 0.85f, 0.85f);
		}
	}
}
