using System;
using System.Collections.Generic;

public class DictionaryLayerCollection<K, V>
{
	private List<IDictionaryLayer<K, V>> layers = new List<IDictionaryLayer<K, V>>();

	public V this[K key]
	{
		get
		{
			foreach (IDictionaryLayer<K, V> layer in layers)
			{
				V value;
				if (layer.Active && layer.Lookup(key, out value))
				{
					return value;
				}
			}
			throw new Exception("could not find key = " + key.ToString());
		}
	}

	public void Add(IDictionaryLayer<K, V> layer)
	{
		layers.Insert(0, layer);
	}
}
