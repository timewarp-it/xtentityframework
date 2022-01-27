using System;
using System.Collections.Generic;
using AutoMapper;
using Example.Blazor.Server.Data;
using Example.Blazor.Shared;
using Example.Blazor.Shared.ViewModels.Enrollment;
using xtEntityFramework.Extensions;
using xtEntityFramework.Models;
using Microsoft.AspNetCore.Mvc;

namespace Example.Blazor.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EnrollmentsController
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;

        public EnrollmentsController(
            ApplicationDbContext dbContext,
            IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        [HttpGet]
        public Page<Enrollment, ListViewModel> Get([FromQuery] Page<Enrollment, ListViewModel> page)
        {
            var enrollments = dbContext.Enrollments.AsQueryable();
            enrollments = enrollments.Search(page);
            enrollments = enrollments.Filter(page);
            enrollments = enrollments.Sort(page);
            return enrollments.Map(
                mapper.ConfigurationProvider,
                page);
        }
    }
}
