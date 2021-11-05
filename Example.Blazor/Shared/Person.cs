using System;
using System.Collections.Generic;
using xtEntityFramework.Attributes;

namespace Example.Blazor.Shared
{
    public class Person
    {
        [Filterable]
        [ListComparison]
        public int Id { get; set; }
        [Searchable(CascadingSearchEnabled = true)]
        [Filterable]
        public string Firstname { get; set; }
        [DefaultOrder]
        [Searchable(CascadingSearchEnabled = true)]
        [Filterable]
        public string Lastname { get; set; }
        public List<Course> Courses { get; set; }
        public List<Enrollment> Enrollments { get; set; }
    }
}
