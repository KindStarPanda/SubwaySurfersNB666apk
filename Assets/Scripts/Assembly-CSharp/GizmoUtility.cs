using System;
using UnityEngine;

public class GizmoUtility
{
	public static void DrawGizmoArrow(Vector3 fromPos, Vector3 toPos)
	{
		DrawGizmoArrow(fromPos, toPos, (fromPos - toPos).magnitude * 0.1f);
	}

	public static void DrawGizmoArrow(Vector3 fromPos, Vector3 toPos, float arrowHeadSize)
	{
		Vector3 normalized = (fromPos - toPos).normalized;
		Vector3 rhs = ((!Mathf.Approximately(normalized.x, normalized.z)) ? new Vector3(normalized.z, 0f, 0f - normalized.x).normalized : Vector3.right);
		Vector3 axis = Vector3.Cross(normalized, rhs);
		Gizmos.DrawLine(fromPos, toPos);
		Gizmos.DrawRay(toPos, Quaternion.AngleAxis(25f, axis) * normalized * arrowHeadSize);
		Gizmos.DrawRay(toPos, Quaternion.AngleAxis(-25f, axis) * normalized * arrowHeadSize);
	}

	public static void DrawGizmoCircle(Vector3 center, float radius)
	{
		Vector3 from = center + new Vector3(radius, 0f, 0f);
		int num = 16;
		for (int i = 1; i <= num; i++)
		{
			float f = (float)i / (float)num * (float)Math.PI * 2f;
			Vector3 vector = center + new Vector3(Mathf.Cos(f) * radius, 0f, Mathf.Sin(f) * radius);
			Gizmos.DrawLine(from, vector);
			from = vector;
		}
	}
}
