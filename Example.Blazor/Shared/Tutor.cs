using System;
using System.Collections.Generic;

namespace Example.Blazor.Shared
{
    public class Tutor : Person
    {
        public List<Course> CoursesLeading { get; set; }
    }
}
