using System;
using System.Collections.Generic;
using System.Linq;
using xtEntityFramework.Attributes;

namespace xtEntityFramework.Models
{
    public class Page<TEntity, TModel> where TModel : class where TEntity : class
    {
        public List<TModel> Data { get; set; }
        [QueryParameter]
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
        public int Size { get; set; }
        public int Rows { get; set; }
        public int From => (CurrentPage - 1) * Size + 1;
        public int To => Math.Min(CurrentPage * Size, Rows);

        public Page()
        {
            Data = new List<TModel>();
            Filters = new FilterCollection<TEntity>();
            CurrentPage = 1;
            Size = 10;
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

        public string ToQueryParameters()
        {
            return string.Join("&", GetType().GetProperties().Where(p => Attribute.IsDefined(p, typeof(QueryParameterAttribute))).Select(p => $"{ p.Name }={ p.GetValue(this) }"));
        }
    }
}
