using System.Collections.Generic;

public class DictionaryLayer<K, V> : IDictionaryLayer<K, V>
{
	private bool active;

	private Dictionary<K, V> dictionary;

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

	public DictionaryLayer(Dictionary<K, V> dictionary)
	{
		this.dictionary = dictionary;
	}

	public bool Lookup(K key, out V value)
	{
		return dictionary.TryGetValue(key, out value);
	}
}
