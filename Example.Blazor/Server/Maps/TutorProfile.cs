using System;
using AutoMapper;
using Example.Blazor.Shared;
using Example.Blazor.Shared.ViewModels.Tutor;

namespace Example.Blazor.Server.Maps
{
    public class TutorProfile : Profile
    {
        public TutorProfile()
        {
            CreateMap<Tutor, ListViewModel>();
        }
    }
}
