using System;
using AutoMapper;
using Example.Blazor.Shared;
using Example.Blazor.Shared.ViewModels.Person;

namespace Example.Blazor.Server.Maps
{
    public class PersonProfile : Profile
    {
        public PersonProfile()
        {
            CreateMap<Tutor, ListViewModel>();
            CreateMap<Student, ListViewModel>();
            CreateMap<Person, ListViewModel>();
        }
    }
}
