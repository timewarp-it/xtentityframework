using System;
using AutoMapper;
using Example.Blazor.Shared;
using Example.Blazor.Shared.ViewModels.Enrollment;

namespace Example.Blazor.Server.Maps
{
    public class EnrollmentProfile : Profile
    {
        public EnrollmentProfile()
        {
            CreateMap<Enrollment, ListViewModel>();
        }
    }
}
