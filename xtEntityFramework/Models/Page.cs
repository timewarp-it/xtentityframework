using System;
using System.Collections.Generic;
using System.Linq;
using xtEntityFramework.Attributes;

namespace xtEntityFramework.Models
{
    public class Page<TEntity, TModel> where TModel : class where TEntity : class
    {
        public List<TModel> Data { get; set; }
        public FilterCollection<TEntity> Filters { get; set; }
        [QueryParameter]
        public string Search { get; set; }
        [QueryParameter]
        public string Sort { get; set; }
        [QueryParameter]
        public Order Order { get; set; }
        [QueryParameter]
        public int CurrentPage { get; set; }
        public int Pages { get; set; }
        [QueryParameter]
        public int? Size { get; set; }
        public int Rows { get; set; }
        public int From => (CurrentPage - 1) * Size ?? 1 + 1;
        public int To => Math.Min(CurrentPage * Size ?? 1, Rows);

        public Page()
        {
            Data = new List<TModel>();
            Filters = new FilterCollection<TEntity>();
            CurrentPage = 1;
            Order = Order.Ascending;
            Search = "";
            Sort = "";
            if (string.IsNullOrWhiteSpace(Sort))
            {
                var defaultOrderProps = PropertyCache.GetProperties<TEntity>().Where(pi => Attribute.IsDefined(pi, typeof(DefaultOrderAttribute)));
                if (defaultOrderProps.Count() > 0)
                {
                    if (defaultOrderProps.Count() > 1)
                    {
                        Sort = defaultOrderProps.Where(p => p.GetCustomAttributes(true).OfType<DefaultOrderAttribute>().Where(a => a.BaseClass == false).Any()).First().Name;
                        if ((defaultOrderProps.Where(p => p.GetCustomAttributes(true).OfType<DefaultOrderAttribute>().Where(a => a.BaseClass == false).Any()).First().GetCustomAttributes(true).First() as DefaultOrderAttribute)?.Order == Order.Descending)
                            Order = Order.Descending;
                    }
                    else
                    {
                        Sort = defaultOrderProps.First().Name;
                        if ((defaultOrderProps.First().GetCustomAttributes(true).OfType<DefaultOrderAttribute>().First() as DefaultOrderAttribute).Order == Order.Descending)
                            Order = Order.Descending;
                    }
                }
            }
        }

        public string ToQueryParameters(string Prefix, params string[]? Excludes)
        {
            var query = string.Join("&", GetType().GetProperties().Where(p => Attribute.IsDefined(p, typeof(QueryParameterAttribute)) && (Excludes == null || !Excludes!.Contains(p.Name))).Select(p => $"{ (!String.IsNullOrWhiteSpace(Prefix) ? Prefix + "." : "") }{ p.Name }={ p.GetValue(this) }"));
            for (int i = 0; i < Filters.Filters.Count; i++)
            {
                query = string.Join("&", query, $"{(!String.IsNullOrWhiteSpace(Prefix) ? Prefix + '.' : "")}filters.filters[{ i }].name={ Filters.Filters[i].Name }", $"{(!String.IsNullOrWhiteSpace(Prefix) ? Prefix + "." : "")}filters.filters[{ i }].comparison={ Filters.Filters[i].Comparison }", $"{(!String.IsNullOrWhiteSpace(Prefix) ? Prefix + "." : "")}filters.filters[{ i }].value={ Filters.Filters[i].Value }");
            }
            query = string.Join("&", query, $"{(!String.IsNullOrWhiteSpace(Prefix) ? Prefix + "." : "")}filters.logic={ Filters.Logic }");
            return query;
        }

        public Dictionary<string, string> ToRouteParameters(string Prefix, params string[]? Excludes)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            foreach(var p in GetType().GetProperties().Where(p => Attribute.IsDefined( p, typeof(QueryParameterAttribute)) && (Excludes == null || !Excludes!.Contains(p.Name))))
            {
                parameters.Add($"{ (!String.IsNullOrWhiteSpace(Prefix) ? Prefix + "." : "")}{ p.Name }", p.GetValue(this)?.ToString()!);
            }
            for (int i = 0; i < Filters.Filters.Count; i++)
            {
                parameters.Add($"{(!String.IsNullOrWhiteSpace(Prefix) ? Prefix + "." : "")}filters.filters[{ i }].name", Filters.Filters[i].Name!);
                parameters.Add($"{(!String.IsNullOrWhiteSpace(Prefix) ? Prefix + "." : "")}filters.filters[{ i }].comparison", Filters.Filters[i].Comparison.ToString());
                parameters.Add($"{(!String.IsNullOrWhiteSpace(Prefix) ? Prefix + "." : "")}filters.filters[{ i }].value", Filters.Filters[i].Value!);
            }
            parameters.Add($"{(!String.IsNullOrWhiteSpace(Prefix) ? Prefix + "." : "")}filters.logic", Filters.Logic.ToString());
            return parameters;
        }
    }
}
