using System;
using System.Collections.Generic;

namespace xtEntityFramework.Models
{
    public class FilterCollection<TEntity> where TEntity : class
    {
        public List<Filter<TEntity>> Filters { get; set; }
        public FilterLogic Logic { get; set; }

        public FilterCollection()
        {
            Filters = new List<Filter<TEntity>>();
        }
    }

    public enum FilterLogic
    {
        And = 0,
        Or = 1
    }
}
