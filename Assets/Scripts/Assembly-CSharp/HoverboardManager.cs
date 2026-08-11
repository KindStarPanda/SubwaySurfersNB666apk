using System.Runtime.CompilerServices;

internal class HoverboardManager
{
	public delegate void OnHoverboardChangeDelegate(Hoverboards.BoardType hoverboard);

	private Hoverboards.BoardType hoverboard = PlayerInfo.Instance.currentHoverboard;

	private static HoverboardManager instance;

	public Hoverboards.BoardType Hoverboard
	{
		get
		{
			return hoverboard;
		}
		set
		{
			hoverboard = value;
		}
	}

	public static HoverboardManager Instance
	{
		get
		{
			return instance ?? (instance = new HoverboardManager());
		}
	}

	[method: MethodImpl(32)]
	public event OnHoverboardChangeDelegate OnHoverboardChange;
}
