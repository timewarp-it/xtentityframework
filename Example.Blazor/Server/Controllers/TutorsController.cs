using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Example.Blazor.Server.Data;
using Example.Blazor.Shared;
using Example.Blazor.Shared.ViewModels.Tutor;
using xtEntityFramework.Extensions;
using xtEntityFramework.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Example.Blazor.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TutorsController
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;

        public TutorsController(
            ApplicationDbContext dbContext,
            IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        [HttpGet]
        public Page<Tutor, ListViewModel> Get([FromQuery] Page<Tutor, ListViewModel> page)
        {
            page.Size = page.Size ?? 10;
            var tutors = dbContext.Tutors.Include(t => t.CoursesLeading).Include(t => t.Enrollments).AsQueryable();
            tutors = tutors.Search(page);
            tutors = tutors.Filter(page);
            tutors = tutors.Sort(page);
            return tutors.Map(
                mapper.ConfigurationProvider,
                page);
        }
    }
}
