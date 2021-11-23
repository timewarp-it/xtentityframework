using System;
using System.Collections.Generic;
using AutoMapper;
using Example.Blazor.Server.Data;
using Example.Blazor.Shared;
using Example.Blazor.Shared.ViewModels.Course;
using xtEntityFramework.Extensions;
using xtEntityFramework.Models;
using Microsoft.AspNetCore.Mvc;
using xtEntityFramework;

namespace Example.Blazor.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CoursesController
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;

        public CoursesController(
            ApplicationDbContext dbContext,
            IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        [HttpGet]
        public Page<Course, ListViewModel> Get([FromQuery] Page<Course, ListViewModel> page)
        {
            page.Size = page.Size ?? 10;
            var courses = dbContext.Courses.AsQueryable();
            courses = courses.Search(page);
            courses = courses.Filter(page);
            courses = courses.Sort(page);
            return courses.Map(
                mapper.ConfigurationProvider,
                page);
        }
    }
}
