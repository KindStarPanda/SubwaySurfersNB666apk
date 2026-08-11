using System;

public class Theme
{
	public static Theme NORMAL = new Theme("normal");

	public static Theme HALLOWEEN = new Theme("haloween");

	public static Theme XMAS = new Theme("xmas");

	public static Theme EASTER = new Theme("easter");

	private string name;

	public string Name
	{
		get
		{
			return name;
		}
	}

	public TimeSpan TimeToExpire
	{
		get
		{
			DateTime dateTime = ThemeManager.Instance.themeExpirationDate;
			if (dateTime == DateTime.MinValue)
			{
				dateTime = DateTime.UtcNow;
			}
			return dateTime - DateTime.UtcNow;
		}
	}

	private Theme(string name)
	{
		this.name = name;
	}

	public static Theme FindByName(string name)
	{
		if (name == NORMAL.name)
		{
			return NORMAL;
		}
		if (name == HALLOWEEN.name)
		{
			return HALLOWEEN;
		}
		if (name == XMAS.name)
		{
			return XMAS;
		}
		if (name == EASTER.name)
		{
			return EASTER;
		}
		return null;
	}

	public override string ToString()
	{
		return name;
	}
}
