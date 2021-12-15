﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using xtEntityFramework.Attributes;
using xtEntityFramework.Models;

[assembly: InternalsVisibleTo("xtEntityFramework.Tests")]
namespace xtEntityFramework
{
    internal class ExpressionParameterReplacer : ExpressionVisitor
    {
        public ExpressionParameterReplacer(IList<ParameterExpression> fromParameters, IList<ParameterExpression> toParameters)
        {
            ParameterReplacements = new Dictionary<ParameterExpression, ParameterExpression>();
            for (int i = 0; i != fromParameters.Count && i != toParameters.Count; i++)
                ParameterReplacements.Add(fromParameters[i], toParameters[i]);
        }
        private IDictionary<ParameterExpression, ParameterExpression> ParameterReplacements
        {
            get;
            set;
        }
        protected override Expression VisitParameter(ParameterExpression node)
        {
            ParameterExpression replacement;
            if (ParameterReplacements.TryGetValue(node, out replacement!))
                node = replacement;
            return base.VisitParameter(node);
        }
    }

    internal static partial class PredicateBuilder
    {
        public static Expression<Func<T, bool>> BuildPredicate<T>(string Name, Comparison Comparison, string Value)
        {
            var parameter = Expression.Parameter(typeof(T), "c");
            Expression left;
            Expression body;
            if (PropertyCache.GetProperty<T>(Name) != null && PropertyCache.GetProperty<T>(Name)!.GetMethod!.IsStatic)
            {
                var source = (PropertyCache.GetProperty<T>(Name)!.GetValue(null, null) as LambdaExpression);
                left = source!.Body;
                parameter = source.Parameters[0];
                body = MapOperation(left, Comparison, Value);
            }
            else if(PropertyCache.GetProperty<T>(Name) != null && PropertyCache.GetProperty<T>(Name)!.PropertyType.GetInterfaces().Contains(typeof(IEnumerable)) && PropertyCache.GetProperty<T>(Name)!.PropertyType != typeof(string))
            {
                var ListType = PropertyCache.GetProperty<T>(Name)!.PropertyType.GenericTypeArguments[0];
                var InnerParameter = Expression.Parameter(ListType, "d");

                // create body "parameter.Any(cascadingsearchprop1.Contains(Value) || cascadingsearchprop2.Contains(Value) || ...)"
                var CascadingSearchEnabledProperties = PropertyCache.GetProperties(ListType).Where(pi => Attribute.IsDefined(pi, typeof(SearchableAttribute)) && pi.GetCustomAttribute<SearchableAttribute>()!.CascadingSearchEnabled);
                List<LambdaExpression> InnerExpressions = new List<LambdaExpression>();
                foreach (var prop in CascadingSearchEnabledProperties)
                {
                    MethodInfo innerMethod = typeof(PredicateBuilder).GetMethod(nameof(PredicateBuilder.BuildPredicate))!;
                    innerMethod = innerMethod.MakeGenericMethod(ListType);
                    InnerExpressions.Add((LambdaExpression)innerMethod.Invoke(null, new object[] { prop.Name, Comparison.Contains, Value })!);
                }

                var orExpression = InnerExpressions.FirstOrDefault();
                foreach (var expr in InnerExpressions.Skip(1))
                {
                    orExpression = Expression.Lambda(Expression.OrElse(orExpression!.Body, new ExpressionParameterReplacer(expr.Parameters, orExpression.Parameters).Visit(expr.Body)), orExpression.Parameters);
                }

                MethodInfo AnyMethod = typeof(Enumerable).GetMethods().Where(m => m.Name == "Any" && m.GetParameters().Length == 2).Single().MakeGenericMethod(ListType);

                left = Name.Split('.')
                    .Aggregate((Expression)parameter, Expression.Property);

                body = Expression.Call(AnyMethod, left, orExpression!);
            }
            else
            {
                left = Name.Split('.')
                    .Aggregate((Expression) parameter, Expression.Property);
                body = MapOperation(left, Comparison, Value);
            }

            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        public static Expression<Func<T, bool>> BuildOrPredicate<T>(List<Filter<T>> filters)
        {
            var result = BuildPredicate<T>(filters.First().Name!, filters.First().Comparison, filters.First().Value!);
            foreach (var filter in filters.Skip(1))
            {
                var newExpression = BuildPredicate<T>(filter.Name!, filter.Comparison, filter.Value!);
                result = Expression.Lambda<Func<T, bool>>(Expression.OrElse(result.Body, new ExpressionParameterReplacer(newExpression.Parameters, result.Parameters).Visit(newExpression.Body)), result.Parameters);
            }
            return result;
        }

        private static Expression MapOperation(Expression left, Comparison comparison, string value)
        {
            switch (comparison)
            {
                case Comparison.Eq:
                    return MakeBinary(ExpressionType.Equal, left, value);
                case Comparison.Neq:
                    return MakeBinary(ExpressionType.NotEqual, left, value);
                case Comparison.Gt:
                    return MakeBinary(ExpressionType.GreaterThan, left, value);
                case Comparison.Gte:
                    return MakeBinary(ExpressionType.GreaterThanOrEqual, left, value);
                case Comparison.Lt:
                    return MakeBinary(ExpressionType.LessThan, left, value);
                case Comparison.Lte:
                    return MakeBinary(ExpressionType.LessThanOrEqual, left, value);
                case Comparison.Contains:
                case Comparison.StartsWith:
                case Comparison.EndsWith:
                    if(left.Type.UnderlyingSystemType.GetInterface(typeof(IEnumerable).ToString()) != null && left.Type.UnderlyingSystemType.GenericTypeArguments.Any())
                    {
                        var innerType = left.Type.UnderlyingSystemType.GenericTypeArguments.First();
                        MethodInfo innerMethod = typeof(PredicateBuilder).GetMethod(nameof(PredicateBuilder.BuildPredicate))!;
                        innerMethod = innerMethod.MakeGenericMethod(innerType);

                        var innerProperty = PropertyCache.GetProperties(innerType).Where(pi => Attribute.IsDefined(pi, typeof(ListComparisonAttribute))).FirstOrDefault()?.Name;
                        var innerExp = (Expression)innerMethod.Invoke(null, new object[] { innerProperty!, Comparison.Eq, value })!;

                        MethodInfo method = typeof(Enumerable).GetMethods().Where(m => m.Name == "Any" && m.GetParameters().Length == 2).Single().MakeGenericMethod(innerType);

                        return Expression.Call(method, left, innerExp);
                    }
                    if ((left as MemberExpression).Member.GetCustomAttribute(typeof(FullTextSearchEnabledAttribute)) != null)
                    {
                        var sqlServerDbFunctionsExtensions = Type.GetType("Microsoft.EntityFrameworkCore.SqlServerDbFunctionsExtensions, Microsoft.EntityFrameworkCore.SqlServer");
                        var efFunctions = PropertyCache.GetProperty(Type.GetType("Microsoft.EntityFrameworkCore.EF, Microsoft.EntityFrameworkCore"), "Functions").GetValue(null);
                        var callExpression =  Expression.Call(
                            sqlServerDbFunctionsExtensions,
                            "Contains",
                            Type.EmptyTypes,
                            Expression.Constant(efFunctions),
                            left,
                            Expression.Constant($"\"*{value}*\"")
                        );
                        return callExpression;
                    }
                    return Expression.Call(MakeString(left), comparison.ToString(), Type.EmptyTypes, Expression.Constant(value, typeof(string)));
                default:
                    throw new NotSupportedException($"Invalid comparison operation '{comparison}'.");
            }
        }

        private static Expression MakeString(Expression source) => source.Type == typeof(string) || source.Type.IsEnum
            ? source
            : Expression.Call(source, "ToString", Type.EmptyTypes);

        private static Expression MakeBinary(ExpressionType type, Expression left, string value)
        {
            object typed = value;
            if (left.Type != typeof(string))
            {
                if (string.IsNullOrEmpty(value))
                {
                    typed = string.Empty;
                    if (Nullable.GetUnderlyingType(left.Type) == null)
                    {
                        left = Expression.Convert(left, typeof(Nullable<>).MakeGenericType(left.Type));
                    }
                }
                else
                {
                    var valueType = Nullable.GetUnderlyingType(left.Type) ?? left.Type;
                    typed = valueType.IsEnum switch
                    {
                        true => Enum.Parse(valueType, value),
                        _ => valueType == typeof(Guid)
                            ? Guid.Parse(value)
                                : Convert.ChangeType(value, valueType)
                    };
                }
            }
            var right = Expression.Constant(typed, left.Type);
            return Expression.MakeBinary(type, left, right);
        }
    }
}
