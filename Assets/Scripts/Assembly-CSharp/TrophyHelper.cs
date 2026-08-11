using System;
using UnityEngine;

public class TrophyHelper : MonoBehaviour
{
	private Trophies.Trophy _trophy;

	public UILabel title;

	public UILabel description;

	public UISprite image;

	private bool _hasInited;

	private void OnEnable()
	{
		PlayerInfo instance = PlayerInfo.Instance;
		instance.OnTrophyUnlocked = (Action<Trophies.Trophy>)Delegate.Combine(instance.OnTrophyUnlocked, new Action<Trophies.Trophy>(onTrophyUnlocked));
		updateDisplay();
	}

	private void OnDisable()
	{
		PlayerInfo instance = PlayerInfo.Instance;
		instance.OnTrophyUnlocked = (Action<Trophies.Trophy>)Delegate.Remove(instance.OnTrophyUnlocked, new Action<Trophies.Trophy>(onTrophyUnlocked));
	}

	private void onTrophyUnlocked(Trophies.Trophy trophy)
	{
		updateDisplay();
	}

	public void setTrophy(Trophies.Trophy trophy)
	{
		_trophy = trophy;
		_hasInited = true;
		updateDisplay();
	}

	public void updateDisplay()
	{
		if (!_hasInited)
		{
			return;
		}
		TrophyData trophyData = Trophies.trophyData[_trophy];
		title.text = trophyData.name;
		description.text = trophyData.description;
		if ((bool)image)
		{
			if (PlayerInfo.Instance.isTrophyUnlocked(_trophy))
			{
				image.spriteName = trophyData.spriteUnlocked;
				image.color = Color.white;
			}
			else
			{
				image.spriteName = trophyData.spriteLocked;
				image.color = Color.white;
			}
			image.MakePixelPerfect();
		}
	}
}
