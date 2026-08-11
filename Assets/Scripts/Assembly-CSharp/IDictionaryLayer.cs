public interface IDictionaryLayer<K, V>
{
	bool Active { get; set; }

	bool Lookup(K key, out V value);
}
