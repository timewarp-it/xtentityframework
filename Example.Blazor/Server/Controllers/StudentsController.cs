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
    public class StudentsController
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;

        public StudentsController(
            ApplicationDbContext dbContext,
            IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        [HttpGet]
        public Page<Student, ListViewModel> Get([FromQuery] Page<Student, ListViewModel> page)
        {
            page.Size = page.Size ?? 10;
            var students = dbContext.Students.AsQueryable();
            students = students.Search(page);
            students = students.Filter(page);
            students = students.Sort(page);
            return students.Map(
                mapper.ConfigurationProvider,
                page);
        }
    }
}
