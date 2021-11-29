using System;
using System.Collections.Generic;
using xtEntityFramework.Attributes;

namespace Example.Blazor.Shared
{
    public class Tutor : Person
    {
        [Searchable]
        public List<Course> CoursesLeading { get; set; }
    }
}
