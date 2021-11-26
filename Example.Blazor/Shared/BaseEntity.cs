using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using xtEntityFramework.Attributes;

namespace Example.Blazor.Shared
{
    public class BaseEntity<TModel> where TModel : BaseEntity<TModel>, new()
    {
        [Filterable]
        [ListComparison]
        public int Id { get; set; }
        [Filterable]
        [Sortable]
        public static Expression<Func<TModel, NumberType>> NumberType
        {
            get
            { 
                return p => p.Id % 2 == 0
                    ? Shared.NumberType.Even
                    : Shared.NumberType.Odd;
            }
        }
    }

    public enum NumberType
    {
        Even,
        Odd
    }
}
