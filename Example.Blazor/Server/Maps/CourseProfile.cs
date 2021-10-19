using System;
using AutoMapper;
using Example.Blazor.Shared;
using Example.Blazor.Shared.ViewModels.Course;

namespace Example.Blazor.Server.Maps
{
    public class CourseProfile : Profile
    {
        public CourseProfile()
        {
            CreateMap<Course, ListViewModel>();
        }
    }
}
