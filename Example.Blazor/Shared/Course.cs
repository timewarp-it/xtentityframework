using System;
using System.Collections.Generic;
using xtEntityFramework.Attributes;

namespace Example.Blazor.Shared
{
    public class Course
    {
        [Filterable]
        public int Id { get; set; }
        [DefaultOrder]
        [Searchable]
        [Filterable]
        public string Name { get; set; }
        public List<Person> Students { get; set; }
        public List<Enrollment> Enrollments { get; set; }
        [Searchable]
        public Tutor Tutor { get; set; }
    }
}