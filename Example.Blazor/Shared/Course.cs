using System;
using System.Collections.Generic;
using xtEntityFramework.Attributes;

namespace Example.Blazor.Shared
{
    public class Course : BaseEntity<Course>
    {
        [DefaultOrder]
        [Searchable]
        [Filterable]
        public string Name { get; set; }
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