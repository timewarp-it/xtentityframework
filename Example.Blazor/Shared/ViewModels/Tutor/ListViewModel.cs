using System;
using System.Collections.Generic;

namespace Example.Blazor.Shared.ViewModels.Tutor
{
    public class ListViewModel : Person.ListViewModel
    {
        public List<Course.ListViewModel> CoursesLeading { get; set; }
    }
}
