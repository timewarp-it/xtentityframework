using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace xtEntityFramework
{
    public static class PropertyCache
    {
        private static readonly ConcurrentDictionary<Type, Dictionary<string, PropertyInfo>> Cache = new();

        public static IEnumerable<PropertyInfo> GetProperties<T>()
        {
            return GetProperties(typeof(T));
        }

        public static IEnumerable<PropertyInfo> GetProperties(Type type)
        {
            return GetPropertiesInternal(type).Values;
        }

        public static PropertyInfo? GetProperty<T>(string? propertyName) =>
            propertyName != null
                ? GetProperty(typeof(T), propertyName)
                : null;

        public static PropertyInfo? GetProperty(Type type, string? propertyName) =>
            GetPropertiesInternal(type).TryGetValue(propertyName!, out var propertyInfo)
                ? propertyInfo
                : null;

        private static IReadOnlyDictionary<string, PropertyInfo> GetPropertiesInternal(Type type)
        {
            return Cache.GetOrAdd(type, t => t.GetProperties(BindingFlags.Static |
                          BindingFlags.FlattenHierarchy |
                          BindingFlags.Instance |
                          BindingFlags.Public).ToDictionary(property => property.Name, property => property));
        }
    }
}
