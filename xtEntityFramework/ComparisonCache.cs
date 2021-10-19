using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using xtEntityFramework.Models;

namespace xtEntityFramework
{
    public class ComparisonCache
    {
        private static readonly ConcurrentDictionary<Type, List<Comparison>> Cache = new();

        private static readonly Dictionary<Type, List<Comparison>> StandardTypes = new Dictionary<Type, List<Comparison>>()
            {
                {
                    typeof(string),
                    new List<Comparison>()
                    {
                        Comparison.Contains,
                        Comparison.EndsWith,
                        Comparison.Eq,
                        Comparison.Neq,
                        Comparison.StartsWith
                    }
                },
                {
                    typeof(int),
                    new List<Comparison>()
                    {
                        Comparison.Contains,
                        Comparison.EndsWith,
                        Comparison.Eq,
                        Comparison.Gt,
                        Comparison.Gte,
                        Comparison.Lt,
                        Comparison.Lte,
                        Comparison.Neq,
                        Comparison.StartsWith
                    }
                },
                {
                    typeof(DateTime),
                    new List<Comparison>()
                    {
                        Comparison.Contains,
                        Comparison.Eq,
                        Comparison.Gt,
                        Comparison.Gte,
                        Comparison.Lt,
                        Comparison.Lte,
                        Comparison.Neq
                    }
                }
            };

        public static IEnumerable<Comparison> GetComparisons<T>()
        {
            return GetComparisons(typeof(T));
        }

        public static IEnumerable<Comparison> GetComparisons(Type type)
        {
            return GetComparisonsInternal(type);
        }

        private static List<Comparison> GetComparisonsInternal(Type type)
        {
            return Cache.GetOrAdd(type, t => StandardTypes.ContainsKey(t) ? StandardTypes[t] : new List<Comparison>() { Comparison.Neq });
        }
    }
}
