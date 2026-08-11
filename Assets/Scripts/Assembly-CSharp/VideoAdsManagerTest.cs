using UnityEngine;

public class VideoAdsManagerTest : MonoBehaviour
{
	private void OnGUI()
	{
		if (GUILayout.Button("Update OnlineSettings", GUILayout.MinHeight(Screen.height / 8)))
		{
			OnlineSettings.instance.DownloadNow();
		}
		if (GUILayout.Button("Init Flurry", GUILayout.MinHeight(Screen.height / 8)))
		{
			FlurryClips.InitAndEnableVideoAds();
			Flurry.StartSession("GDGAT7PY98MAEQQIWC68");
		}
		if (GUILayout.Button("Init manager", GUILayout.MinHeight(Screen.height / 8)))
		{
			VideoAdsManager instance = VideoAdsManager.instance;
			instance.Init();
			instance.AddVideoAvailableHandler(_onVideoAvailable);
			instance.AddVideoUnavailableHandler(_onVideoUnavailable);
			instance.AddTakeoverBeganHandler(_onTakeoverBegan);
			instance.AddTakeoverEndedHandler(_onTakeoverEnded);
			instance.AddVideoWatchedHandler(_onVideoWatched);
		}
		if (GUILayout.Button("Play video if available", GUILayout.MinHeight(Screen.height / 8)))
		{
			Debug.Log("VideoAdsManagerTest: VideoAdsManager.PlayVideoIfAvailable(100) = " + VideoAdsManager.instance.PlayVideoIfAvailable(100), this);
		}
	}

	private void _onVideoAvailable(VideoAdProvider provider)
	{
		Debug.Log("VideoAdsManagerTest: _onVideoAvailable", this);
	}

	private void _onVideoUnavailable(VideoAdProvider provider)
	{
		Debug.Log("VideoAdsManagerTest: _onVideoUnavailable", this);
	}

	private void _onTakeoverBegan(VideoAdProvider provider)
	{
		Debug.Log("VideoAdsManagerTest: _onTakeoverBegan", this);
	}

	private void _onTakeoverEnded(VideoAdProvider provider)
	{
		Debug.Log("VideoAdsManagerTest: _onTakeoverEnded", this);
	}

	private void _onVideoWatched(VideoAdProvider provider, int reward)
	{
		Debug.Log("VideoAdsManagerTest: _onVideoWatched  reward = " + reward, this);
	}
}
