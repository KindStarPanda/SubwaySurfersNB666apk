using System.Collections.Generic;
using UnityEngine;

public class RandomDictionaryLayer<K, V> : IDictionaryLayer<K, V>
{
	private bool active;

	private Dictionary<K, V[]> dictionary;

	public bool Active
	{
		get
		{
			return active;
		}
		set
		{
			active = value;
		}
	}

	public RandomDictionaryLayer(Dictionary<K, V[]> dictionary)
	{
		this.dictionary = dictionary;
	}

	public bool Lookup(K key, out V value)
	{
		V[] value2;
		if (dictionary.TryGetValue(key, out value2) && value2.Length > 0)
		{
			value = value2[Random.Range(0, value2.Length)];
			return true;
		}
		value = default(V);
		return false;
	}
}
