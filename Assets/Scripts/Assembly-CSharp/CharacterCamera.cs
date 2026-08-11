using UnityEngine;

public class CharacterCamera : MonoBehaviour
{
	public Vector3 position;

	public Vector3 target;

	private Vector3 shake = Vector3.zero;

	private static CharacterCamera instance;

	public static CharacterCamera Instance
	{
		get
		{
			return instance ?? (instance = Object.FindObjectOfType(typeof(CharacterCamera)) as CharacterCamera);
		}
	}

	public void Shake()
	{
		Vector3 diff = Vector3.zero;
		float amplitude = 100f;
		StartCoroutine(pTween.To(0.3f, delegate(float t)
		{
			diff += Random.insideUnitSphere;
			shake = (1f - t) * diff * amplitude * Time.deltaTime;
		}));
	}

	public void LateUpdate()
	{
		Game game = Game.Instance;
		if (game != null && game.awakeDone && game.IsInGame.Value)
		{
			// 确定当前视角焦点：控制保安时看保安，否则看玩家角色
			Transform focus = null;
			if (game.controllingGuard)
			{
				FollowingGuard fg = FollowingGuard.Instance;
				if (fg != null)
				{
					focus = fg.transform;
				}
			}
			else if (game.firstPersonView)
			{
				focus = Character.Instance.transform;
			}
			// 第一人称视角：摄像头置于焦点头部并朝前看
			if (game.firstPersonView && focus != null)
			{
				Vector3 eye = focus.position + Vector3.up * game.firstPersonEyeHeight;
				base.transform.position = eye + shake;
				base.transform.LookAt(eye + Vector3.forward * 20f + shake);
				return;
			}
			// 第三人称看保安
			if (game.controllingGuard && focus != null)
			{
				Vector3 gp = focus.position;
				Vector3 offset = new Vector3(0f, 33f, -33f);
				base.transform.position = gp + offset + shake;
				base.transform.LookAt(gp + Vector3.up * 15f + shake);
				return;
			}
		}
		base.transform.position = position + shake;
		base.transform.LookAt(target + shake);
	}

	public void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawLine(position, target);
	}
}
