using System;
using UnityEngine;

public class SprayDetectionAwaker : MonoBehaviour
{
	private RunnerAnimPlayer runnerAnimPlayer;

	private void Start()
	{
		runnerAnimPlayer = Utils.FindObject<RunnerAnimPlayer>();
		if (UIScreenController.isInstanced)
		{
			UIScreenController instance = UIScreenController.Instance;
			instance.OnChangedScreen = (Action<string>)Delegate.Combine(instance.OnChangedScreen, new Action<string>(CheckScreen));
		}
	}

	private void CheckScreen(string screenName)
	{
		runnerAnimPlayer.PlayOrMutePaintSound(screenName == "FrontUI");
	}
}
