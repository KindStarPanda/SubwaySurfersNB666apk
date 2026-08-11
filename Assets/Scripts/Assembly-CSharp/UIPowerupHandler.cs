using System.Collections.Generic;
using UnityEngine;

public class UIPowerupHandler : MonoBehaviour
{
	public GameObject PowerupPrefab;

	private Vector3 _offScreenPosition = new Vector3(0f, -1000f, 0f);

	private Vector3[] slotPositions = new Vector3[4]
	{
		new Vector3(-135f, 10f, 0f),
		new Vector3(20f, 10f, 0f),
		new Vector3(-135f, 50f, 0f),
		new Vector3(20f, 50f, 0f)
	};

	private UIPowerupHelper[] _powerupSlots = new UIPowerupHelper[4];

	private void Start()
	{
		for (int i = 0; i < 4; i++)
		{
			GameObject gameObject = NGUITools.AddChild(base.gameObject, PowerupPrefab);
			gameObject.transform.localPosition = _offScreenPosition;
			_powerupSlots[i] = gameObject.GetComponent<UIPowerupHelper>();
		}
	}

	private void Update()
	{
		List<ActivePowerup> activePowerups = GameStats.Instance.GetActivePowerups();
		int i = 0;
		for (int num = activePowerups.Count - 1; num >= 0; num--)
		{
			_powerupSlots[i].SetPowerupSlot(activePowerups[num]);
			_powerupSlots[i].gameObject.active = true;
			if (_powerupSlots[i].transform.localPosition != slotPositions[num])
			{
				_powerupSlots[i].transform.localPosition = slotPositions[num];
			}
			i++;
		}
		for (; i < 4; i++)
		{
			if (_powerupSlots[i].gameObject.active)
			{
				_powerupSlots[i].HidePowerupSlot();
				_powerupSlots[i].transform.localPosition = _offScreenPosition;
				_powerupSlots[i].gameObject.active = false;
			}
		}
	}
}
