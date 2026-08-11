using UnityEngine;

public static class DeviceInfo
{
	public enum PerformanceLevel
	{
		Low = 0,
		Medium = 1,
		High = 2
	}

	public enum FormFactor
	{
		iPhone = 0,
		iPad = 1,
		small = 2,
		medium = 3,
		large = 4,
		iPhone5 = 5
	}

	public static PerformanceLevel performanceLevel;

	public static string deviceModel;

	public static readonly float dpi;

	public static readonly FormFactor formFactor;

	public static readonly bool isHighres;

	static DeviceInfo()
	{
		performanceLevel = PerformanceLevel.High;
		deviceModel = SystemInfo.deviceModel;
		isHighres = (float)Screen.height > 480f;
		if (Screen.height < 500)
		{
			formFactor = FormFactor.small;
			Debug.Log("Form factor small");
		}
		else if (Screen.height < 900)
		{
			formFactor = FormFactor.medium;
			Debug.Log("Form factor medium");
		}
		else
		{
			formFactor = FormFactor.large;
			Debug.Log("Form factor large");
		}
		if (isTablet())
		{
			formFactor = FormFactor.iPad;
			Debug.Log("Form factor tablet");
		}
		if (Screen.height >= 500 && Screen.width > 320)
		{
			isHighres = true;
		}
		else
		{
			isHighres = false;
		}
		dpi = Screen.dpi;
		if (dpi <= 0f)
		{
			dpi = 300f;
		}
		Debug.Log("Dpi: " + dpi);
		Debug.Log("High res set to: " + isHighres);
		if (isDeviceLowPerformance())
		{
			Debug.Log("DeviceInfo: Change to performance level: Low");
			performanceLevel = PerformanceLevel.Low;
		}
	}

	private static bool isTablet()
	{
		float f = ((!(Screen.dpi > 0f)) ? ((float)Screen.width) : ((float)Screen.width / Screen.dpi));
		float f2 = ((!(Screen.dpi > 0f)) ? ((float)Screen.height) : ((float)Screen.height / Screen.dpi));
		double num = Mathf.Sqrt(Mathf.Pow(f, 2f) + Mathf.Pow(f2, 2f));
		Debug.Log("size of inches: " + num);
		return num >= 6.0;
	}

	private static bool isDeviceLowPerformance()
	{
		int processorCount = SystemInfo.processorCount;
		string processorType = SystemInfo.processorType;
		int systemMemorySize = SystemInfo.systemMemorySize;
		int graphicsMemorySize = SystemInfo.graphicsMemorySize;
		if (processorCount >= 4)
		{
			return false;
		}
		if (processorType.Contains("rev"))
		{
			int num = processorType.IndexOf("rev");
			string text = processorType.Substring(num + 3).Trim();
			if (text.Contains(" "))
			{
				int length = text.IndexOf(" ");
				string s = text.Substring(0, length).Trim();
				int result;
				if (int.TryParse(s, out result))
				{
					bool flag = processorCount >= 2;
					bool flag2 = result >= 6;
					bool flag3 = systemMemorySize >= 512;
					bool flag4 = graphicsMemorySize >= 250;
					if (flag && flag2 && flag3 && flag4)
					{
						return false;
					}
				}
			}
		}
		return true;
	}
}
