using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using xtEntityFramework.Attributes;

namespace Example.Blazor.Shared
{
    public class BaseEntity<TEntity> where TEntity : BaseEntity<TEntity>, new()
    {
        [Filterable]
        [ListComparison]
        public int Id { get; set; }
        [Filterable]
        [Sortable]
        public static Expression<Func<TEntity, NumberType>> NumberType
        {
            get
            {
                return p => ((p.Id % 2 == 0)
                    ? Shared.NumberType.Even 
                    : Shared.NumberType.Odd);
            }
        }
    }

    public enum NumberType
    {
        Even = 0,
        Odd = 1
    }
}
