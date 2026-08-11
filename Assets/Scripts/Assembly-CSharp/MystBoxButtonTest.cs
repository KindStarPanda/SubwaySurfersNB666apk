public class MystBoxButtonTest : UIBasicButton
{
	public bool doubleBox;

	protected override void Send()
	{
		PlayerInfo.Instance.AddMysteryBoxToUnlock(MysteryBox.Type.Normal);
		if (doubleBox)
		{
			PlayerInfo.Instance.AddMysteryBoxToUnlock(MysteryBox.Type.Normal);
		}
	}
}
