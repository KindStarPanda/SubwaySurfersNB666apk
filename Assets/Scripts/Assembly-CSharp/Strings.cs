using System;
using UnityEngine;

public class Strings
{
	private static string language;

	private static string[] values;

	public static string Language
	{
		get
		{
			return language;
		}
		set
		{
			Load(value);
		}
	}

	public static string Get(StringID key)
	{
		if (language == null)
		{
			Debug.LogWarning("Strings not loaded. Loading default language");
			Language = "english";
		}
		return values[(int)key];
	}

	public static string Get(string keyString)
	{
		return (!string.IsNullOrEmpty(keyString)) ? Get((StringID)(int)Enum.Parse(typeof(StringID), keyString, true)) : null;
	}

	public static bool Exists(string keyString)
	{
		try
		{
			Get(keyString);
		}
		catch (ArgumentException)
		{
			return false;
		}
		return true;
	}

	private static void Load(string language)
	{
		if (!(Strings.language != language))
		{
			return;
		}
		Debug.Log("Loading strings: " + language);
		Strings.language = language;
		if (values == null)
		{
			int[] array = (int[])Enum.GetValues(typeof(StringID));
			values = new string[array[array.Length - 1] + 1];
		}
		else
		{
			for (int i = 0; i < values.Length; i++)
			{
				values[i] = null;
			}
		}
		TextAsset textAsset = (TextAsset)Resources.Load("text/" + language, typeof(TextAsset));
		string text = textAsset.text;
		int num = 0;
		string key;
		string value;
		while ((num = StringUtility.GetNextKeyValuePair(text, num, out key, out value)) >= 0)
		{
			int num2 = (int)Enum.Parse(typeof(StringID), key, true);
			values[num2] = value;
			if (num == text.Length)
			{
				break;
			}
		}
		if (values[0] != null)
		{
			throw new Exception("Strings.Load: String set for " + Enum.GetName(typeof(StringID), 0));
		}
		for (int j = 1; j < values.Length; j++)
		{
			if (values[j] == null && Enum.IsDefined(typeof(StringID), j))
			{
				throw new Exception("Strings.Load: String not set for " + Enum.GetName(typeof(StringID), j));
			}
		}
	}
}
