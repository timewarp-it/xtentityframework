using System;
using System.Collections.Generic;
using xtEntityFramework.Attributes;

namespace Example.Blazor.Shared
{
    public class Course : BaseEntity<Course>
    {
        [DefaultOrder]
        [Searchable(true)]
        [Filterable]
        public string Name { get; set; }
        [Searchable(true)]
        public string XXX { get; set; }
        public List<Person> Students { get; set; }
        public List<Enrollment> Enrollments { get; set; }
        [Searchable]
        public Tutor Tutor { get; set; }
        [Searchable]
        public CourseType Type { get; set; }
    }

    public enum CourseType
    {
        Mandatory = 0,
        Optional = 1
    }
}