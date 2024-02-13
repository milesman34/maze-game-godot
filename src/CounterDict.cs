using System.Collections.Generic;
using System.Linq;

/// <summary>
/// CounterDict maintains a dictionary mapping values to a count.
/// </summary>
/// <typeparam name="T">Type of the key</typeparam>
public class CounterDict<T> {
    // Internal dictionary
    private Dictionary<T, int> dictionary;

    /// <summary>
    /// Constructs a CounterDict.
    /// </summary>
    public CounterDict() {
        dictionary = new Dictionary<T, int>();
    }

    /// <summary>
    /// Returns how many instances of an item appear, returning zero if there are none.
    /// </summary>
    /// <param name="key">Key to get the count of</param>
    /// <returns></returns>
    public int this[T key] {
        get {
            return dictionary.GetValueOrDefault(key, 0);
        }

        set {
            dictionary[key] = value;
        }
    }

    /// <summary>
    /// Does the CounterDict contain the key?
    /// </summary>
    /// <param name="key">Key to search for</param>
    /// <returns></returns>
    public bool Contains(T key) {
        return this[key] > 0;
    }

    /// <summary>
    /// Adds an object to the counter dict.
    /// </summary>
    /// <param name="value">The CounterDict to add to</param>
    /// <param name="key">The key to add</param>
    /// <returns></returns>
    public static CounterDict<T> operator+(CounterDict<T> value, T key) {
        value[key]++;
        return value;
    }

    /// <summary>
    /// Removes an object from the counter dict.
    /// </summary>
    /// <param name="value">The CounterDict to remove from</param>
    /// <param name="key">The key to remove</param>
    /// <returns></returns>
    public static CounterDict<T> operator-(CounterDict<T> value, T key) {
        if (value.Contains(key)) {
            value[key]--;

            if (value[key] == 0) {
                value.dictionary.Remove(key);
            }
        }

        return value;
    }

    /// <summary>
    /// Returns the maximum valued key (only designed to work for comparable keys).
    /// </summary>
    /// <returns></returns>
    public T GetMaxKey() {
        var keys = dictionary.Keys.ToList();

        return keys.Max();
    }

    /// <summary>
    /// Returns the number of keys in the dictionary.
    /// </summary>
    /// <returns></returns>
    public int NumKeys() {
        return dictionary.Count();
    }
}