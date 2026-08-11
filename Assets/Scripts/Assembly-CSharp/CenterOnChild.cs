using System;
using UnityEngine;

public class CenterOnChild : MonoBehaviour
{
	private const float TOUCH_WIDTH = 28f;

	private UIDraggablePanel mDrag;

	private GameObject mCenteredObject;

	[NonSerialized]
	public bool characterWasClicked;

	public GameObject centeredObject
	{
		get
		{
			return mCenteredObject;
		}
	}

	public void ClearCenterObject()
	{
		mCenteredObject = null;
	}

	private void OnEnable()
	{
		Recenter();
	}

	private void OnDragFinished()
	{
		if (base.enabled)
		{
			Recenter();
		}
	}

	public void Recenter()
	{
		if (mDrag == null)
		{
			mDrag = NGUITools.FindInParents<UIDraggablePanel>(base.gameObject);
			if (mDrag == null)
			{
				Debug.LogWarning(string.Concat(GetType(), " requires ", typeof(UIDraggablePanel), " on a parent object in order to work"), this);
				base.enabled = false;
				return;
			}
			mDrag.onDragFinished = OnDragFinished;
		}
		if (mDrag.panel == null)
		{
			return;
		}
		Vector4 clipRange = mDrag.panel.clipRange;
		Transform cachedTransform = mDrag.panel.cachedTransform;
		Vector3 localPosition = cachedTransform.localPosition;
		localPosition.x += clipRange.x;
		localPosition.y += clipRange.y;
		localPosition = cachedTransform.parent.TransformPoint(localPosition);
		Vector3 vector = localPosition - mDrag.currentMomentum * (mDrag.momentumAmount * 0.1f);
		mDrag.currentMomentum = Vector3.zero;
		float num = float.MaxValue;
		Transform transform = null;
		Transform transform2 = base.transform;
		int i = 0;
		for (int childCount = transform2.childCount; i < childCount; i++)
		{
			Transform child = transform2.GetChild(i);
			float num2 = Vector3.SqrMagnitude(child.position - vector);
			if (num2 < num)
			{
				num = num2;
				transform = child;
			}
		}
		if (transform != null)
		{
			mCenteredObject = transform.gameObject;
			Vector3 vector2 = cachedTransform.InverseTransformPoint(transform.position);
			Vector3 vector3 = cachedTransform.InverseTransformPoint(localPosition);
			Vector3 vector4 = vector2 - vector3;
			if (mDrag.scale.x == 0f)
			{
				vector4.x = 0f;
			}
			if (mDrag.scale.y == 0f)
			{
				vector4.y = 0f;
			}
			if (mDrag.scale.z == 0f)
			{
				vector4.z = 0f;
			}
			SpringPanel.Begin(mDrag.gameObject, cachedTransform.localPosition - vector4, 8f);
		}
		else
		{
			mCenteredObject = null;
		}
	}

	public void CenterOnTransform(Transform target, bool instant = false)
	{
		if (mDrag == null)
		{
			mDrag = NGUITools.FindInParents<UIDraggablePanel>(base.gameObject);
			if (mDrag == null)
			{
				Debug.LogWarning(string.Concat(GetType(), " requires ", typeof(UIDraggablePanel), " on a parent object in order to work"), this);
				base.enabled = false;
				return;
			}
			mDrag.onDragFinished = OnDragFinished;
		}
		if (mDrag.panel == null)
		{
			return;
		}
		Transform transform = base.transform;
		bool flag = false;
		int i = 0;
		for (int childCount = transform.childCount; i < childCount; i++)
		{
			Transform child = transform.GetChild(i);
			if (child == target)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			return;
		}
		Vector4 clipRange = mDrag.panel.clipRange;
		Transform cachedTransform = mDrag.panel.cachedTransform;
		Vector3 localPosition = cachedTransform.localPosition;
		localPosition.x += clipRange.x;
		localPosition.y += clipRange.y;
		localPosition = cachedTransform.parent.TransformPoint(localPosition);
		mDrag.currentMomentum = Vector3.zero;
		if (target != null)
		{
			mCenteredObject = target.gameObject;
			Vector3 vector = cachedTransform.InverseTransformPoint(target.position);
			Vector3 vector2 = cachedTransform.InverseTransformPoint(localPosition);
			Vector3 vector3 = vector - vector2;
			if (mDrag.scale.x == 0f)
			{
				vector3.x = 0f;
			}
			if (mDrag.scale.y == 0f)
			{
				vector3.y = 0f;
			}
			if (mDrag.scale.z == 0f)
			{
				vector3.z = 0f;
			}
			if (instant)
			{
				Vector3 localPosition2 = cachedTransform.localPosition - vector3;
				Vector4 clipRange2 = mDrag.panel.clipRange;
				clipRange2.x += vector3.x;
				clipRange2.y += vector3.y;
				mDrag.panel.clipRange = clipRange2;
				cachedTransform.localPosition = localPosition2;
				if (mDrag != null)
				{
					mDrag.UpdateScrollbars(false);
				}
			}
			else
			{
				SpringPanel.Begin(mDrag.gameObject, cachedTransform.localPosition - vector3, 8f);
			}
		}
		else
		{
			mCenteredObject = null;
		}
	}

	public bool CenterOnClosestChildAtPosition(Vector2 clickPositionOnScreen)
	{
		if (mDrag == null)
		{
			mDrag = NGUITools.FindInParents<UIDraggablePanel>(base.gameObject);
			if (mDrag == null)
			{
				Debug.LogWarning(string.Concat(GetType(), " requires ", typeof(UIDraggablePanel), " on a parent object in order to work"), this);
				base.enabled = false;
				return false;
			}
			mDrag.onDragFinished = OnDragFinished;
		}
		if (mDrag.panel == null)
		{
			return false;
		}
		Vector4 clipRange = mDrag.panel.clipRange;
		Transform cachedTransform = mDrag.panel.cachedTransform;
		Vector3 localPosition = cachedTransform.localPosition;
		localPosition.x += clipRange.x;
		localPosition.y += clipRange.y;
		localPosition = cachedTransform.parent.TransformPoint(localPosition);
		Vector3 vector = clickPositionOnScreen;
		float num = (float)Screen.width / 320f;
		if (DeviceInfo.formFactor == DeviceInfo.FormFactor.iPad)
		{
			num = (float)Screen.width / 384f;
			vector.x -= 48f * num;
		}
		else
		{
			vector.x -= 16f * num;
		}
		vector /= num;
		vector.x += clipRange.x - clipRange.z / 2f;
		float num2 = float.MaxValue;
		Transform transform = null;
		Transform transform2 = base.transform;
		int i = 0;
		for (int childCount = transform2.childCount; i < childCount; i++)
		{
			Transform child = transform2.GetChild(i);
			float num3 = Mathf.Abs(child.localPosition.x - vector.x);
			if (num3 < num2)
			{
				num2 = num3;
				transform = child;
			}
		}
		if (num2 > 28f)
		{
			transform = null;
		}
		if (transform != null)
		{
			if (mCenteredObject == transform.gameObject)
			{
				return true;
			}
			mCenteredObject = transform.gameObject;
			characterWasClicked = true;
			Vector3 vector2 = cachedTransform.InverseTransformPoint(transform.position);
			Vector3 vector3 = cachedTransform.InverseTransformPoint(localPosition);
			Vector3 vector4 = vector2 - vector3;
			if (mDrag.scale.x == 0f)
			{
				vector4.x = 0f;
			}
			if (mDrag.scale.y == 0f)
			{
				vector4.y = 0f;
			}
			if (mDrag.scale.z == 0f)
			{
				vector4.z = 0f;
			}
			SpringPanel.Begin(mDrag.gameObject, cachedTransform.localPosition - vector4, 8f);
		}
		else
		{
			mCenteredObject = null;
		}
		return false;
	}

	public void CharacterFocusedFromClick()
	{
		characterWasClicked = false;
	}
}
