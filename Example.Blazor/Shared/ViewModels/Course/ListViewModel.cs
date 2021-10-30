using System;
using System.Collections.Generic;

namespace Example.Blazor.Shared.ViewModels.Course
{
    public class ListViewModel
    {
        public string Name { get; set; }
        public Person.ListViewModel Tutor { get; set; }
        public List<Enrollment.ListViewModel> Enrollments { get; set; }
        public CourseType Type { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
