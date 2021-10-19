using System;
using Example.Blazor.Shared;
using Microsoft.EntityFrameworkCore;

namespace Example.Blazor.Server.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Course> Courses { get; set; }

        public DbSet<Person> People { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Tutor> Tutors { get; set; }

        public DbSet<Enrollment> Enrollments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Course>(builder =>
            {
                builder.HasOne(c => c.Tutor)
                    .WithMany(t => t.CoursesLeading);
                builder.HasMany(c => c.Students)
                    .WithMany(s => s.Courses)
                    .UsingEntity<Enrollment>(
                        j => j.HasOne(e => e.Student)
                            .WithMany(p => p.Enrollments),
                        j => j.HasOne(e => e.Course)
                            .WithMany(c => c.Enrollments));
            });
        }
    }
}