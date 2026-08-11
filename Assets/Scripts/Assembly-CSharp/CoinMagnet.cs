using System.Collections;
using UnityEngine;

public class CoinMagnet : CharacterModifier
{
	public float pullSpeed = 150f;

	private CharacterController characterController;

	private Animation characterAnimation;

	private Character character;

	private OnTriggerObject coinMagnetCollider;

	private CharacterRendering characterRendering;

	private CharacterModel characterModel;

	private Transform coinEFX;

	private Game game;

	public AudioStateLoop audioStateLoop;

	public AudioClipInfo powerDownSound;

	public ActivePowerup Powerup;

	private void Awake()
	{
		character = Character.Instance;
		characterController = character.characterController;
		coinEFX = character.CharacterPickupParticleSystem.CoinEFX.transform;
		characterRendering = CharacterRendering.Instance;
		characterModel = characterRendering.CharacterModel;
		characterAnimation = characterRendering.characterAnimation;
		coinMagnetCollider = character.coinMagnetCollider;
		characterAnimation["hold_magnet"].AddMixingTransform(characterRendering.CharacterModel.shoulderTransform);
		characterAnimation["hold_magnet"].layer = 3;
		characterAnimation["hold_magnet"].weight = 0.9f;
		characterAnimation["hold_magnet"].enabled = false;
		game = Game.Instance;
	}

	public override void Reset()
	{
		Paused = false;
	}

	public override IEnumerator Begin()
	{
		GameStats.Instance.pickedUpPowerups++;
		Paused = false;
		audioStateLoop.ChangeLoop(AudioState.Magnet);
		if (character.IsStumbling)
		{
			character.StopStumble();
		}
		characterModel.meshCoinMagnet.enabled = true;
		characterAnimation["hold_magnet"].enabled = true;
		characterAnimation.Play("hold_magnet");
		Powerup = GameStats.Instance.TriggerPowerup(PowerupType.coinmagnet);
		coinMagnetCollider.OnEnter = CoinHit;
		coinMagnetCollider.GetComponent<Collider>().enabled = true;
		base.enabled = true;
		stop = StopSignal.DONT_STOP;
		while (Powerup.timeLeft > 0f && stop == StopSignal.DONT_STOP)
		{
			coinEFX.position = characterModel.meshCoinMagnet.transform.position;
			yield return null;
		}
		coinMagnetCollider.GetComponent<Collider>().enabled = false;
		base.enabled = false;
		characterModel.meshCoinMagnet.enabled = false;
		coinEFX.localPosition = CharacterPickupParticles.coinEfxOffset;
		characterAnimation["hold_magnet"].enabled = false;
		audioStateLoop.ChangeLoop(AudioState.MagnetStop);
		if (Powerup.timeLeft <= 0f)
		{
			So.Instance.playSound(powerDownSound);
		}
	}

	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.B))
		{
			MonoBehaviour.print("STOP");
			stop = StopSignal.STOP;
		}
	}

	public void CoinHit(Collider collider)
	{
		Coin component = collider.GetComponent<Coin>();
		if (component != null)
		{
			component.GetComponent<Collider>().enabled = false;
			StartCoroutine(Pull(component));
		}
	}

	private IEnumerator Pull(Coin coin)
	{
		Transform pivot = coin.pivot.transform;
		Vector3 position = pivot.position;
		float distance = (position - characterController.transform.position).magnitude;
		yield return StartCoroutine(pTween.To(distance / (pullSpeed * game.NormalizedGameSpeed), delegate(float t)
		{
			pivot.position = Vector3.Lerp(position, characterModel.meshCoinMagnet.transform.position, t * t);
		}));
		Pickup pickup = coin.GetComponent<Pickup>();
		character.NotifyPickup(pickup);
		GameStats.Instance.coinsCoinMagnet++;
	}
}
