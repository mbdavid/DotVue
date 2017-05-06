using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotVue;

public static class DictionaryExtensions
{
    public static TValue GetValueOrDefault<TKey, TValue>
        (this IDictionary<TKey, TValue> dictionary,
         TKey key,
         TValue defaultValue = default(TValue))
    {
        if (dictionary == null) return defaultValue;

        TValue value;
        return dictionary.TryGetValue(key, out value) ? value : defaultValue;
    }
}