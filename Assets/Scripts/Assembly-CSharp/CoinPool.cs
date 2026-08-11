using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPool : MonoBehaviour
{
	public GameObject coinPrefab;

	public int numDeletedPerCleanup = 4;

	public float cleanupIntervalInSeconds = 5f;

	private Vector3 spawnPoint = -1000f * Vector3.up;

	private Vector3 spawnSpacing = -20f * Vector3.right;

	private List<Transform> coins;

	private static CoinPool instance;

	private int numberOfActiveCoins;

	private int numberOfActiveCoins_high;

	private List<PickupRotate> activeRotatePickups = new List<PickupRotate>();

	public List<PickupRotate> ActiveRotatingPickups
	{
		get
		{
			return activeRotatePickups;
		}
	}

	public int NumberOfActiveCoins
	{
		get
		{
			return numberOfActiveCoins;
		}
	}

	public static CoinPool Instance
	{
		get
		{
			return instance ?? (instance = Object.FindObjectOfType(typeof(CoinPool)) as CoinPool);
		}
	}

	public void Awake()
	{
		coins = new List<Transform>();
		GetCoins();
	}

	private void Update()
	{
		for (int i = 0; i < activeRotatePickups.Count; i++)
		{
			if (activeRotatePickups[i].enabled)
			{
				activeRotatePickups[i].PhasedRotate();
			}
		}
	}

	private void OnEnable()
	{
		StartCoroutine(CleanUpCoins());
	}

	private IEnumerator CleanUpCoins()
	{
		while (base.enabled)
		{
			int numDeletes = ((coins.Count <= numDeletedPerCleanup) ? coins.Count : numDeletedPerCleanup);
			int lastIndex = Mathf.Max(0, coins.Count - (numDeletes + 1));
			for (int i = coins.Count - 1; i >= lastIndex; i--)
			{
				Object.Destroy(coins[i].gameObject);
				coins.RemoveAt(i);
			}
			yield return new WaitForSeconds(cleanupIntervalInSeconds);
		}
	}

	private void GetCoins()
	{
		foreach (Transform item in base.transform)
		{
			TrackObject component = item.GetComponent<TrackObject>();
			if (component != null)
			{
				coins.Add(item.transform);
			}
		}
	}

	private Transform MakeNewCoin(int coinIndex)
	{
		Vector3 position = spawnPoint + spawnSpacing * coinIndex;
		GameObject gameObject = Object.Instantiate(coinPrefab, position, Quaternion.identity) as GameObject;
		Transform transform = gameObject.transform;
		transform.parent = base.transform;
		coins.Add(transform);
		return transform;
	}

	public Transform GetCoin()
	{
		Transform transform = ((coins.Count <= 0) ? MakeNewCoin(coins.Count) : coins[0]);
		coins.Remove(transform);
		GameObject gameObject = transform.gameObject;
		if (!gameObject.active)
		{
			gameObject.SetActiveRecursively(true);
		}
		numberOfActiveCoins++;
		numberOfActiveCoins_high = Mathf.Max(numberOfActiveCoins_high, numberOfActiveCoins);
		return transform;
	}

	public void Put(Transform coin)
	{
		Put(new Transform[1] { coin });
	}

	public void Put(IEnumerable<Transform> coins)
	{
		foreach (Transform coin in coins)
		{
			coin.parent = base.transform;
			Vector3 position = coin.position;
			position.y = -1000f;
			coin.position = position;
			this.coins.Add(coin);
			numberOfActiveCoins--;
		}
	}
}
