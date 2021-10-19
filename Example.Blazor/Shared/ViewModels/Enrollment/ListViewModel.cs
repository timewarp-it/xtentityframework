using System;
namespace Example.Blazor.Shared.ViewModels.Enrollment
{
    public class ListViewModel
    {
        public Course.ListViewModel Course { get; set; }
        public Person.ListViewModel Student { get; set; }
        public int Grade { get; set; }
    }
}
