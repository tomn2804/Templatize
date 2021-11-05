﻿using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Schemata
{
    public partial class ImmutableOutline
    {
        public static implicit operator ImmutableOutline(Builder builder)
        {
            return builder.ToImmutable();
        }

        public Builder ToBuilder()
        {
            return new(this);
        }

        protected ImmutableDictionary<object, object?> Dictionary { get; }

        private ImmutableOutline(ImmutableDictionary<object, object?> dictionary)
        {
            Dictionary = dictionary;
        }

        private IImmutableDictionary<object, object?> IDictionary => Dictionary;
    }

    public partial class ImmutableOutline : IImmutableDictionary<object, object?>
    {
        public int Count => IDictionary.Count;
        public IEnumerable<object> Keys => IDictionary.Keys;
        public IEnumerable<object?> Values => IDictionary.Values;
        public object? this[object key] => IDictionary[key];

        public IImmutableDictionary<object, object?> Add(object key, object? value)
        {
            Builder builder = ToBuilder();
            builder.Add(key, value);
            return builder.ToImmutable();
        }

        public IImmutableDictionary<object, object?> AddRange(IEnumerable<KeyValuePair<object, object?>> pairs)
        {
            Builder builder = ToBuilder();
            foreach (KeyValuePair<object, object?> pair in pairs)
            {
                builder.Add(pair.Key, pair.Value);
            }
            return builder.ToImmutable();
        }

        public IImmutableDictionary<object, object?> Clear()
        {
            Builder builder = ToBuilder();
            builder.Clear();
            return builder.ToImmutable();
        }

        public bool Contains(KeyValuePair<object, object?> pair)
        {
            return IDictionary.Contains(pair);
        }

        public bool ContainsKey(object key)
        {
            return IDictionary.ContainsKey(key);
        }

        public IEnumerator<KeyValuePair<object, object?>> GetEnumerator()
        {
            return IDictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return IDictionary.GetEnumerator();
        }

        public IImmutableDictionary<object, object?> Remove(object key)
        {
            Builder builder = ToBuilder();
            builder.Remove(key);
            return builder.ToImmutable();
        }

        public IImmutableDictionary<object, object?> RemoveRange(IEnumerable<object> keys)
        {
            Builder builder = ToBuilder();
            foreach (object pair in keys)
            {
                builder.Remove(pair);
            }
            return builder.ToImmutable();
        }

        public IImmutableDictionary<object, object?> SetItem(object key, object? value)
        {
            Builder result = ToBuilder();
            result[key] = value;
            return result.ToImmutable();
        }

        public IImmutableDictionary<object, object?> SetItems(IEnumerable<KeyValuePair<object, object?>> items)
        {
            Builder result = ToBuilder();
            foreach (KeyValuePair<object, object?> item in items)
            {
                result[item.Key] = item.Value;
            }
            return result.ToImmutable();
        }

        public bool TryGetKey(object equalKey, out object actualKey)
        {
            return IDictionary.TryGetKey(equalKey, out actualKey);
        }

        public bool TryGetValue(object key, out object? value)
        {
            return IDictionary.TryGetValue(key, out value);
        }
    }
}
