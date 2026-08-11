using UnityEngine;

public class Layers
{
	public readonly int Default = FindLayer("Default");

	public readonly int HitBounceOnly = FindLayer("HitBounceOnly");

	public readonly int KeepOnHoverboard = FindLayer("KeepOnHoverboard");

	public readonly int _3DGUI = FindLayer("3DGUI");

	private static Layers instance;

	public static Layers Instance
	{
		get
		{
			return instance ?? (instance = new Layers());
		}
	}

	private Layers()
	{
	}

	private static int FindLayer(string name)
	{
		int num = LayerMask.NameToLayer(name);
		if (num == -1)
		{
			Debug.LogError("Could not find layer '" + name + "'.");
		}
		return num;
	}
}
