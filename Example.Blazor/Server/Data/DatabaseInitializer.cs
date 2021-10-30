using System;
using System.Collections.Generic;
using System.Linq;
using Example.Blazor.Shared;

namespace Example.Blazor.Server.Data
{
    public interface IDatabaseInitializer
    {
        void Seed();
    }

    internal sealed class DatabaseInitializer : IDatabaseInitializer
    {
        private readonly ApplicationDbContext dbContext;

        public DatabaseInitializer(
            ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        Random rnd = new Random();

        public void Seed()
        {
            dbContext.Database.EnsureCreated();
            SeedStudents();
            SeedTutors();
            SeedCourses();
        }

        private void SeedStudents()
        {
            if (dbContext.Students.Any())
                return;

            var students = new List<Student>()
            {
                new Student()
                {
                    Firstname = "Tick",
                    Lastname = "Duck"
                },
                new Student()
                {
                    Firstname = "Trick",
                    Lastname = "Duck"
                },
                new Student()
                {
                    Firstname = "Track",
                    Lastname = "Duck"
                }
            };
            dbContext.Students.AddRange(students);

            dbContext.SaveChanges();
        }

        private void SeedTutors()
        {
            if (dbContext.Tutors.Any())
                return;

            var tutors = new List<Tutor>()
            {
                new Tutor()
                {
                    Firstname = "Donald",
                    Lastname = "Duck"
                },
                new Tutor()
                {
                    Firstname = "Gustav",
                    Lastname = "Gans"
                },
                new Tutor()
                {
                    Firstname = "Dagobert",
                    Lastname = "Duck"
                },
                new Tutor()
                {
                    Firstname = "Daniel",
                    Lastname = "Duesentrieb"
                }
            };
            dbContext.Tutors.AddRange(tutors);

            dbContext.SaveChanges();
        }

        private void SeedCourses()
        {
            if (dbContext.Courses.Any())
                return;

            var courses = new List<Course>()
            {
                new Course()
                {
                    Name = "How to have luck",
                    Tutor = RandomTutor(),
                    Students = RandomPeople(),
                    Type = CourseType.Mandatory
                },
                new Course()
                {
                    Name = "How to have bad luck",
                    Tutor = RandomTutor(),
                    Students = RandomPeople(),
                    Type= CourseType.Mandatory
                },
                new Course()
                {
                    Name = "How to be rich",
                    Tutor = RandomTutor(),
                    Students = RandomPeople(),
                    Type = CourseType.Optional
                },
                new Course()
                {
                    Name = "How to develop crazy stuff",
                    Tutor = RandomTutor(),
                    Students = RandomPeople(),
                    Type = CourseType.Optional
                }
            };

            dbContext.Courses.AddRange(courses);

            dbContext.SaveChanges();
        }

        private Tutor RandomTutor()
        {
            var index = rnd.Next(dbContext.Tutors.Count());
            return dbContext.Tutors.ToList()[index];
        }

        private List<Person> RandomPeople()
        {
            var count = rnd.Next(dbContext.People.Count());
            var people = new List<Person>();
            for(int i = 0; i < count; i++)
            {
                var index = rnd.Next(dbContext.People.Count());
                people.Add(dbContext.People.ToList()[index]);
            }
            return people.Distinct().ToList();
        }
    }
}
