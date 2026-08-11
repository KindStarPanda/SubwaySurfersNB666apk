using System;
using System.Runtime.CompilerServices;

public class ThemeManager
{
	public delegate void OnChangeThemeDelegate(Theme theme);

	private Theme theme;

	private DateTime _themeExpirationDate;

	private static ThemeManager instance;

	public Theme Theme
	{
		get
		{
			return theme;
		}
		set
		{
			theme = value;
			if (this.OnChangeTheme != null)
			{
				this.OnChangeTheme(theme);
			}
		}
	}

	public DateTime themeExpirationDate
	{
		get
		{
			return _themeExpirationDate;
		}
		set
		{
			_themeExpirationDate = value;
		}
	}

	public static ThemeManager Instance
	{
		get
		{
			return instance ?? (instance = new ThemeManager());
		}
	}

	[method: MethodImpl(32)]
	public event OnChangeThemeDelegate OnChangeTheme;

	private ThemeManager()
	{
	}

	public void ForceRefresh()
	{
		Theme = theme;
	}

	public Theme themeForSeason(PlayerInfo.Season season)
	{
		Theme result = Theme.NORMAL;
		switch (season)
		{
		case PlayerInfo.Season.easter:
			result = Theme.EASTER;
			break;
		case PlayerInfo.Season.halloween:
			result = Theme.HALLOWEEN;
			break;
		case PlayerInfo.Season.xmas:
			result = Theme.XMAS;
			break;
		}
		return result;
	}
}
