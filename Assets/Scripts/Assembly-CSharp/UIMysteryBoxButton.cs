public class UIMysteryBoxButton : UIBasicButton
{
	protected override void Send()
	{
		MysteryBoxReward mysteryBoxReward = MysteryBox.Roll(MysteryBox.Type.Normal);
	}
}
