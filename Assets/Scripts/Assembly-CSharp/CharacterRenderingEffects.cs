using UnityEngine;

internal class CharacterRenderingEffects : MonoBehaviour
{
	[SerializeField]
	private GameObject jetpackParticles;

	public ParticleFollow jetpackParticleCloudL;

	public ParticleFollow jetpackParticleCloudR;

	public GameObject JetpackParticles
	{
		get
		{
			return jetpackParticles;
		}
	}

	public void Initialize(CharacterModel characterModel)
	{
		jetpackParticleCloudL.Target = characterModel.jetpackCloudPositionL;
		jetpackParticleCloudR.Target = characterModel.jetpackCloudPositionR;
	}
}
