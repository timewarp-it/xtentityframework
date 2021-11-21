using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using xtEntityFramework.Attributes;
using xtEntityFramework.Models;

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
            var left = Name.Split('.')
                .Aggregate((Expression)parameter, Expression.Property);
            var body = MapOperation(left, Comparison, Value);
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
