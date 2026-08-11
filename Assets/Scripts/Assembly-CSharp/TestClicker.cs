using UnityEngine;

public class TestClicker : MonoBehaviour
{
	private void OnEnable()
	{
	}

	private void OnDisable()
	{
	}

	private void OnClick()
	{
		PlayerInfo.Instance.InitCurrentMissionSet(28, 3);
		PlayerInfo.Instance.SaveIfDirty();
	}
}
