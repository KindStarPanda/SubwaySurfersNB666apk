using System;
using UnityEngine;

public class MysteryBoxPickup : MonoBehaviour
{
	private void Awake()
	{
		Pickup component = GetComponent<Pickup>();
		component.OnPickup = (Pickup.OnPickupDelegate)Delegate.Combine(component.OnPickup, new Pickup.OnPickupDelegate(OnPickup));
	}

	private void OnPickup(CharacterPickupParticles particles)
	{
		PlayerInfo.Instance.AddMysteryBoxToUnlock(MysteryBox.Type.Normal);
		GameStats.Instance.mysteryBoxPickups++;
		particles.PickedUpPowerUp();
		GameStats.Instance.AddScoreForPickup(PowerupType.mysterybox);
	}
}
