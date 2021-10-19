using System;
using System.Collections.Generic;

namespace Example.Blazor.Shared.ViewModels.Person
{
    public class ListViewModel
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public List<Enrollment.ListViewModel> Enrollments { get; set; }

        public override string ToString()
        {
            return $"{ Firstname } { Lastname }";
        }
    }
}
