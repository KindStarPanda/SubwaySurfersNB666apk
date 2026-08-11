using UnityEngine;

public class TutorialDoubleCoinsPopup : CharacterPopup
{
	[SerializeField]
	private UILabel topTitle;

	[SerializeField]
	private UILabel centerTitle;

	[SerializeField]
	private UILabel bottomTitle;

	[SerializeField]
	private UILabel line1Big;

	[SerializeField]
	private UILabel line1Small;

	[SerializeField]
	private UILabel line2Big;

	[SerializeField]
	private UILabel line2Small;

	[SerializeField]
	private UILabel line3Big;

	[SerializeField]
	private UILabel line3Small;

	[SerializeField]
	private GameObject doubleCoinsPrefab;

	private GameObject doubleCoins;

	public override void Show()
	{
		base.Show();
		SetCharacter(Characters.CharacterType.frizzy);
		topTitle.text = "Double";
		centerTitle.text = "Your";
		bottomTitle.text = "Coins";
		line1Big.text = "Get the Double Coin BOOSTER!";
		line1Small.text = "Every Coin = 2 COINS";
		line2Big.text = "Buy once, keep FOREVER!";
		line2Small.text = "Get Upgrades and New Stuff FASTER";
		line3Big.text = "BOOST your game!";
		line3Small.text = "Collected Coins will be worth DOUBLE";
		if (doubleCoins == null)
		{
			doubleCoins = NGUITools.AddChild(base.gameObject, doubleCoinsPrefab);
			doubleCoins.GetComponent<DoubleCoinUpgradeHelper>().Init();
			float y = UIScreenController.Instance.root.manualHeight / 2 - 180;
			doubleCoins.transform.localPosition = new Vector3(2f, y, 0f);
			NGUITools.AddWidgetCollider(doubleCoins);
		}
	}

	private void CloseClicked()
	{
		UIScreenController.Instance.ClosePopup();
	}
}
