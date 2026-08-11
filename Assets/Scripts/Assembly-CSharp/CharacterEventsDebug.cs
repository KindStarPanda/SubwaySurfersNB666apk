using UnityEngine;

public class CharacterEventsDebug : MonoBehaviour
{
	private Character character;

	public void Awake()
	{
		character = GetComponent<Character>();
		character.OnStumble += delegate(Character.StumbleType stumbleType, Character.StumbleHorizontalHit horizontalHit, Character.StumbleVerticalHit verticalHit, string colliderName)
		{
			EventDebug(string.Concat("OnStumble (stumbleType=", stumbleType, ", horizontalHit=", horizontalHit, ", verticalHit=", verticalHit, ")"));
		};
		character.OnCriticalHit += delegate(Character.CriticalHitType type)
		{
			EventDebug(string.Concat("OnCriticalHit (type=", type, ")"));
		};
		character.OnJump += delegate
		{
			EventDebug("OnJump");
		};
		character.OnLanding += delegate
		{
			EventDebug("OnLanding");
		};
		character.OnHangtime += delegate
		{
			EventDebug("OnHangtime");
		};
		character.OnRoll += delegate
		{
			EventDebug("OnRoll");
		};
		character.OnHitByTrain += delegate
		{
			EventDebug("OnHitByTrain");
		};
		character.OnChangeTrack += delegate
		{
			EventDebug("OnChangeTrack");
		};
		character.OnTutorialMoveBackToCheckPoint += delegate
		{
			EventDebug("OnTutorialMoveBackToCheckPoint");
		};
		character.OnTutorialStartFromCheckPoint += delegate
		{
			EventDebug("OnTutorialStartFromCheckPoint");
		};
		character.OnJumpOverTrain += delegate
		{
			EventDebug("OnJumpOverTrain");
		};
	}

	private void EventDebug(string eventName)
	{
		Debug.Log("Character." + eventName);
	}
}
