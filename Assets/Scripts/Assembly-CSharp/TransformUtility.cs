using UnityEngine;

public class TransformUtility
{
	public static Transform FindChild(Transform transform, string pattern)
	{
		return FindChild(transform, pattern, StringUtility.MatchType.Is, true);
	}

	public static Transform FindChild(Transform transform, string pattern, StringUtility.MatchType matchType)
	{
		return FindChild(transform, pattern, matchType, true);
	}

	public static Transform FindChild(Transform transform, string pattern, StringUtility.MatchType matchType, bool ignoreCase)
	{
		if (StringUtility.Match(transform.name, pattern, matchType, ignoreCase))
		{
			return transform;
		}
		foreach (Transform item in transform)
		{
			Transform transform3 = FindChild(item, pattern, matchType, ignoreCase);
			if (transform3 != null)
			{
				return transform3;
			}
		}
		return null;
	}

	public static void AddAndResetChild(Transform transform, Transform child)
	{
		child.parent = transform;
		child.transform.localRotation = Quaternion.identity;
		child.transform.localPosition = Vector3.zero;
		child.transform.localScale = Vector3.one;
	}

	public static int CountAllChildren(Transform parent)
	{
		int num = 0;
		foreach (Transform item in parent)
		{
			num += CountAllChildren(item);
			num++;
		}
		return num;
	}

	public static void SetChildrenActiveRecursively(Transform parent, bool active)
	{
		foreach (Transform item in parent)
		{
			item.gameObject.SetActiveRecursively(active);
		}
	}

	public static T GetComponentInParents<T>(Transform child) where T : Component
	{
		Transform parent = child.parent;
		if (parent != null)
		{
			T component = parent.GetComponent<T>();
			if (component != null)
			{
				return component;
			}
			return GetComponentInParents<T>(parent);
		}
		return (T)null;
	}

	public static void SetLayerRecursively(Transform t, int layer)
	{
		t.gameObject.layer = layer;
		foreach (Transform item in t)
		{
			SetLayerRecursively(item, layer);
		}
	}
}
