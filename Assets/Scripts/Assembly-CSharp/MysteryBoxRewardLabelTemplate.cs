using UnityEngine;

public class MysteryBoxRewardLabelTemplate : MonoBehaviour
{
	public UILabel bigLabel;

	[SerializeField]
	private UILabel subLabel;

	public float Alpha
	{
		get
		{
			return bigLabel.alpha;
		}
		set
		{
			bigLabel.alpha = Mathf.Clamp01(value);
		}
	}

	public void SetupPowerup(PowerupType powerup, int amount)
	{
		Alpha = 0f;
		bigLabel.text = _GetPowerupLabel(powerup, amount);
		subLabel.text = Upgrades.upgrades[powerup].mysteryBoxDescription;
	}

	public void SetupCoins(int amount)
	{
		Alpha = 0f;
		bigLabel.text = _GetCoinsLabel(amount);
		subLabel.text = string.Empty;
	}

	public void UpdateCoins(int amount)
	{
		bigLabel.text = _GetCoinsLabel(amount);
		subLabel.text = string.Empty;
	}

	public void SetupToken(Characters.CharacterType characterType, int amount)
	{
		Alpha = 0f;
		bigLabel.text = amount + "x " + Characters.characterData[characterType].tokenName;
		Characters.Model model = Characters.characterData[characterType];
		int num = model.Price - PlayerInfo.Instance.GetCollectedTokens(characterType) - amount;
		if (num > 0)
		{
			subLabel.text = string.Format("Collect {0} more to unlock {1}", num, model.modelName);
		}
		else
		{
			subLabel.text = "You Unlocked " + model.modelName;
		}
	}

	public void SetupTrophy(Trophies.Trophy trophy)
	{
		Alpha = 0f;
		bigLabel.text = _GetTrophyLabel(trophy);
		subLabel.text = "See your collection in the trophy menu";
	}

	public void SetupMedal()
	{
		Alpha = 0f;
		bigLabel.text = "Medal";
		subLabel.text = string.Empty;
	}

	private string _GetPowerupLabel(PowerupType type, int amount)
	{
		string empty = string.Empty;
		empty = empty + amount + "x ";
		return empty + Upgrades.upgrades[type].name;
	}

	private string _GetTrophyLabel(Trophies.Trophy trophy)
	{
		return Trophies.trophyData[trophy].name;
	}

	private string _GetCoinsLabel(int amount)
	{
		return amount + " Coins";
	}
}
