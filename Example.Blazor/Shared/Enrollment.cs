using System;
using xtEntityFramework.Attributes;

namespace Example.Blazor.Shared
{
    public class Enrollment
    {
        public Course Course { get; set; }
        public Person Student { get; set; }
        [Filterable]
        public int Grade { get; set; }

        public Enrollment()
        {
            Grade = new Random().Next(1, 5);
        }
    }
}
