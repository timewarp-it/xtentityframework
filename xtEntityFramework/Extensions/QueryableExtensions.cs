using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using xtEntityFramework.Attributes;
using xtEntityFramework.Models;

namespace xtEntityFramework.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<TEntity> Filter<TEntity, TModel>(
            this IQueryable<TEntity> entities,
            Page<TEntity, TModel> page) where TEntity : class where TModel : class
        {
            if ((page.Filters.Filters ?? Enumerable.Empty<Filter<TEntity>>()).Any())
            {
                if (page.Filters.Filters!.Count > 1 && page.Filters.Logic == FilterLogic.Or)
                {
                    entities = entities.OrWhere(page.Filters.Filters);
                }
                else
                {
                    entities = entities.Where(page.Filters.Filters);
                }
            }
            return entities;
        }

        internal static IQueryable<TEntity> Where<TEntity>(
            this IQueryable<TEntity> source,
            List<Filter<TEntity>> filters)
        {
            return filters!.Aggregate(source, (current, filter) =>
               current.Where(PredicateBuilder.BuildPredicate<TEntity>(filter.Name!, filter.Comparison, filter.Value!)));
        }

        internal static IQueryable<TEntity> OrWhere<TEntity>(
            this IQueryable<TEntity> source,
            List<Filter<TEntity>> filters)
        {
            return source.Where(PredicateBuilder.BuildOrPredicate<TEntity>(filters));
        }

        public static IQueryable<TEntity> Search<TEntity, TModel>(
            this IQueryable<TEntity> entities,
            Page<TEntity, TModel> page) where TEntity : class where TModel : class
        {
            if (page.Search == null)
            {
                return entities;
            }

            var searchfilters = new FilterCollection<TEntity>()
            {
                Logic = FilterLogic.Or
            };

            foreach (var prop in PropertyCache.GetProperties<TEntity>().Where(pi => Attribute.IsDefined(pi, typeof(SearchableAttribute))))
            {
                var subprops = PropertyCache.GetProperties(prop.PropertyType).Where(pi => Attribute.IsDefined(pi, typeof(SearchableAttribute)));
                foreach (var subprop in subprops)
                {
                    searchfilters.Filters.Add(new Filter<TEntity>()
                    {
                        Comparison = Comparison.Contains,
                        Name = $"{ prop.Name }.{ subprop.Name }",
                        Value = page.Search
                    });
                }
                if (!subprops.Any())
                {   
                    searchfilters.Filters.Add(new Filter<TEntity>()
                    {
                        Comparison = Comparison.Contains,
                        Name = prop.Name,
                        Value = page.Search
                    });
                }
            }

            page.Filters = searchfilters;

            return entities.Filter(page);
        }

        public static Page<TEntity, TModel> Map<TEntity, TModel>(
            this IQueryable<TEntity> entities,
            IConfigurationProvider config,
            Page<TEntity, TModel> page) where TEntity : class where TModel : class
        {
            // invalid page will return page 1
            page.CurrentPage = (int)Math.Ceiling((double)entities.Count() / page.Size) >= page.CurrentPage
                ? page.CurrentPage
                : 1;
            return new Page<TEntity, TModel>
            {
                Rows = entities.Count(),
                Pages = (int)Math.Ceiling((double)entities.Count() / page.Size),
                Data = entities.Page(page.CurrentPage, page.Size).ProjectTo<TModel>(config).ToList(),
                Filters = page.Filters,
                Search = page.Search,
                CurrentPage = page.CurrentPage,
                Size = page.Size,
                Sort = page.Sort,
                Order = page.Order
            };
        }

        internal static IQueryable<TEntity> Page<TEntity>(
            this IQueryable<TEntity> entities,
            int page, int size) where TEntity : class
        {
            entities = entities.Skip((page - 1) * size).Take(size);

            return entities;
        }

        public static IQueryable<TEntity> Sort<TEntity, TModel>(
            this IQueryable<TEntity> entities,
            Page<TEntity, TModel> page) where TEntity : class where TModel : class
        {
            string orderMethod = "OrderBy";
            if (string.IsNullOrWhiteSpace(page.Sort))
            {
                var defaultOrderProps = PropertyCache.GetProperties<TEntity>().Where(pi => Attribute.IsDefined(pi, typeof(DefaultOrderAttribute)));
                if (defaultOrderProps.Count() > 0)
                {
                    if (defaultOrderProps.Count() > 1)
                    {
                        page.Sort = defaultOrderProps.Where(p => p.GetCustomAttributes(true).OfType<DefaultOrderAttribute>().Where(a => a.BaseClass == false).Any()).First().Name;
                        if ((defaultOrderProps.Where(p => p.GetCustomAttributes(true).OfType<DefaultOrderAttribute>().Where(a => a.BaseClass == false).Any()).First().GetCustomAttributes(true).First() as DefaultOrderAttribute)?.Order == Order.Descending)
                            orderMethod = "OrderByDescending";
                    }
                    else
                    {
                        page.Sort = defaultOrderProps.First().Name;
                        if ((defaultOrderProps.First().GetCustomAttributes(true).OfType<DefaultOrderAttribute>().First() as DefaultOrderAttribute).Order == Order.Descending)
                            orderMethod = "OrderByDescending";
                    }
                }
                else
                {
                    return entities;
                }
            }
            else
            {
                if (page.Order == Order.Descending)
                {
                    orderMethod = "OrderByDescending";
                }
            }

            var parameter = Expression.Parameter(typeof(TEntity), "p");
            LambdaExpression orderByExp;
            MethodCallExpression resultExp;

            // if property is aggregated with func
            if (typeof(TEntity).GetProperty(page.Sort)!.PropertyType.BaseType == typeof(LambdaExpression))
            {
                orderByExp = (typeof(TEntity).GetProperty(page.Sort)!.GetValue(null, null) as LambdaExpression)!;
                resultExp = Expression.Call(typeof(Queryable), orderMethod, new Type[] { typeof(TEntity), typeof(TEntity).GetProperty(page.Sort)!.PropertyType.GenericTypeArguments[0].GenericTypeArguments[1] }, entities.Expression, Expression.Quote(orderByExp));
            }
            else
            {
                // property is a subproperty with DefaultOrderAtttribute
                var defaultOrderProps = PropertyCache.GetProperties(PropertyCache.GetProperty(typeof(TEntity), page.Sort)!.PropertyType).Where(pi => Attribute.IsDefined(pi, typeof(DefaultOrderAttribute)));
                if (defaultOrderProps.Count() > 0)
                {
                    var subProp = "";
                    if (defaultOrderProps.Count() > 1)
                    {
                        subProp = defaultOrderProps.Where(p => p.GetCustomAttributes(true).OfType<DefaultOrderAttribute>().Where(a => a.BaseClass == false).Any()).First().Name;
                    }
                    else
                    {
                        subProp = defaultOrderProps.First().Name;
                    }
                    var exp = Expression.MakeMemberAccess(parameter, typeof(TEntity).GetProperty(page.Sort)!);
                    exp = Expression.MakeMemberAccess(exp, typeof(TEntity).GetProperty(page.Sort)!.PropertyType.GetProperty(subProp)!);

                    orderByExp = Expression.Lambda(
                        exp,
                        parameter);
                }
                // property is a list, order by count
                else if (typeof(System.Collections.IList).IsAssignableFrom(typeof(TEntity).GetProperty(page.Sort)!.PropertyType))
                {
                    var exp = Expression.MakeMemberAccess(parameter, typeof(TEntity).GetProperty(page.Sort)!);
                    exp = Expression.MakeMemberAccess(exp, typeof(TEntity).GetProperty(page.Sort)!.PropertyType.GetProperty("Count")!);

                    orderByExp = Expression.Lambda(
                        exp,
                        parameter);
                }
                else
                {
                    orderByExp = Expression.Lambda(Expression.MakeMemberAccess(parameter, typeof(TEntity).GetProperty(page.Sort)!), parameter);
                }
                resultExp = Expression.Call(
                    typeof(Queryable),
                    orderMethod,
                    new Type[] {
                        typeof(TEntity),
                        orderByExp.ReturnType
                    },
                    entities.Expression,
                    Expression.Quote(orderByExp));
            }



            return entities.Provider.CreateQuery<TEntity>(resultExp);

        }
    }
}
