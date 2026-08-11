using UnityEngine;

public class FlurryInit : MonoBehaviour
{
	private const string FLURRY_ALLOW_NEW_SESSION = "flurry_allow_new_ss";

	private const int FLURRY_MINUTES_DELAY = 2;

	private bool sessionEnded;

	private void Awake()
	{
		FlurryAndroid.onStartSession("YR898G65YFPWNMQ6X5H5");
		FlurryAndroid.setLogEnabled(true);
	}

	private void EndSession()
	{
		if (!sessionEnded)
		{
			Debug.Log("Flurry end session");
			FlurryAndroid.onEndSession();
			sessionEnded = true;
		}
	}

	private void OnDestroy()
	{
		Debug.Log("FlurryInit OnDestroy");
		EndSession();
	}

	private void OnApplicationQuit()
	{
		Debug.Log("FlurryInit OnApplicationQuit");
		EndSession();
	}

	private void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			EndSession();
			return;
		}
		FlurryAndroid.onStartSession("YR898G65YFPWNMQ6X5H5");
		sessionEnded = false;
		Debug.Log("Flurry start new session");
	}
}
