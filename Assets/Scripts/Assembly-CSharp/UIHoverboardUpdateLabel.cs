using System;
using System.Collections;
using UnityEngine;

public class UIHoverboardUpdateLabel : MonoBehaviour
{
	private const float BLINK_SPEED = 4f;

	private const float DELAY_AFTER_BLINK = 0.1f;

	private const float SCROLL_TEXT_DURATION = 1f;

	private const float SCROLL_TEXT_FADEOUT_DURATION = 0.5f;

	private static readonly Vector3 SCROLL_TEXT_STARTOFFSET = new Vector3(0f, -20f, 0f);

	private static readonly Vector3 SCROLL_TEXT_ENDOFFSET = new Vector3(0f, 20f, 0f);

	[SerializeField]
	private UILabel amountLabel;

	[SerializeField]
	private UISprite blinkSprite;

	[SerializeField]
	private GameObject scrollingLabelPrefab;

	private int _lastAmountSet = -1;

	private bool _countUpCoroutineRunning;

	private void OnEnable()
	{
		PlayerInfo instance = PlayerInfo.Instance;
		instance.onPowerupAmountChanged = (Action)Delegate.Combine(instance.onPowerupAmountChanged, new Action(UpdateLabels));
		_lastAmountSet = -1;
		UpdateLabels();
		blinkSprite.enabled = false;
	}

	private void OnDisable()
	{
		PlayerInfo instance = PlayerInfo.Instance;
		instance.onPowerupAmountChanged = (Action)Delegate.Remove(instance.onPowerupAmountChanged, new Action(UpdateLabels));
		_countUpCoroutineRunning = false;
	}

	private void UpdateLabels()
	{
		int upgradeAmount = PlayerInfo.Instance.GetUpgradeAmount(PowerupType.hoverboard);
		if (_lastAmountSet == -1)
		{
			amountLabel.text = upgradeAmount.ToString();
			_lastAmountSet = upgradeAmount;
		}
		else if (_lastAmountSet < upgradeAmount)
		{
			if (!_countUpCoroutineRunning)
			{
				StartCoroutine(CountUpAndFlashCoroutine());
			}
		}
		else
		{
			amountLabel.text = upgradeAmount.ToString();
			_lastAmountSet = upgradeAmount;
		}
	}

	private IEnumerator CountUpAndFlashCoroutine()
	{
		if (_countUpCoroutineRunning)
		{
			yield break;
		}
		_countUpCoroutineRunning = true;
		int targetAmount;
		while (true)
		{
			targetAmount = PlayerInfo.Instance.GetUpgradeAmount(PowerupType.hoverboard);
			if (_lastAmountSet >= targetAmount)
			{
				break;
			}
			_lastAmountSet++;
			amountLabel.text = _lastAmountSet.ToString();
			SpawnScrollingLabel();
			blinkSprite.enabled = true;
			float aniFactor = 0f;
			while (aniFactor < 1f)
			{
				aniFactor = Mathf.Clamp01(aniFactor + 4f * Time.deltaTime);
				blinkSprite.color = new Color(1f, 1f, 1f, 1f - aniFactor);
				yield return null;
			}
			blinkSprite.enabled = false;
			yield return new WaitForSeconds(0.1f);
		}
		if (_lastAmountSet != targetAmount)
		{
			amountLabel.text = targetAmount.ToString();
			_lastAmountSet = targetAmount;
		}
		_countUpCoroutineRunning = false;
	}

	private void SpawnScrollingLabel()
	{
		GameObject gameObject = NGUITools.AddChild(base.gameObject, scrollingLabelPrefab);
		ScrollingTextLabel component = gameObject.GetComponent<ScrollingTextLabel>();
		if (component != null)
		{
			component.StartScrolling("+1", SCROLL_TEXT_STARTOFFSET, SCROLL_TEXT_ENDOFFSET, 1f, 0.5f, true);
		}
		else
		{
			Debug.LogError("No ScrollingTextLabel component on scrollingLabelPrefab");
		}
	}
}
