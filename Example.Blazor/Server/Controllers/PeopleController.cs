using System;
using System.Collections.Generic;
using AutoMapper;
using Example.Blazor.Server.Data;
using Example.Blazor.Shared;
using Example.Blazor.Shared.ViewModels.Person;
using xtEntityFramework.Extensions;
using xtEntityFramework.Models;
using Microsoft.AspNetCore.Mvc;

namespace Example.Blazor.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PeopleController
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;

        public PeopleController(
            ApplicationDbContext dbContext,
            IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        [HttpGet]
        public Page<Person, ListViewModel> Get([FromQuery] Page<Person, ListViewModel> page)
        {
            page.Size = page.Size ?? 10;
            var people = dbContext.People.AsQueryable();
            people = people.Search(page);
            people = people.Filter(page);
            people = people.Sort(page);
            return people.Map(
                mapper.ConfigurationProvider,
                page);
        }
    }
}
