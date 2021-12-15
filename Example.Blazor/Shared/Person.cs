using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using xtEntityFramework.Attributes;

namespace Example.Blazor.Shared
{
    public class Person : BaseEntity<Person>
    {
        [Searchable(CascadingSearchEnabled = true)]
        [Filterable]
        public string Firstname { get; set; }
        [DefaultOrder]
        [Searchable(CascadingSearchEnabled = true)]
        [Filterable]
        [FullTextSearchEnabled]
        public string Lastname { get; set; }
        public List<Course> Courses { get; set; }
        public List<Enrollment> Enrollments { get; set; }
        [Filterable]
        [Sortable]
        public static Expression<Func<Person, NumberType>> LocalNumberType
        {
            get
            {
                return p => ((p.Id % 2 == 0)
                    ? Shared.NumberType.Even
                    : Shared.NumberType.Odd);
            }
        }
    }
}
