public class UISlideInUnlock : UISlideIn
{
	public UILabel UnlockName;

	public void SetupSlideInUnlock(string message)
	{
		base.gameObject.SetActiveRecursively(true);
		UnlockName.text = message;
		SlideIn();
	}
}
