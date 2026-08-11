using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Facebook
{
	public string accessToken;

	public string appAccessToken;

	private static Facebook _instance;

	public static Facebook instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new Facebook();
			}
			return _instance;
		}
	}

	public Facebook()
	{
	}

	public void graphRequest(string path, Action<string, object> completionHandler)
	{
	}
}
