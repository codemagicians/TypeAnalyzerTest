using System;
using System.Collections.Generic;
using ClassAnalyzer;
using ClassAnalyzer.Domain;

namespace Azrielit_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var list = new List<object>
            {
                new Person
                {
                    Age = 30,
                    Name = new Name
                    {
                        FirstName = "Alex",
                        LastName = "Green",
                        Address = new Address
                        {
                            City = "Tel Aviv",
                            Street = "Ha Shalom",
                            Country = "Israel",
                            PostCode = null
                        }
                    }
                },
                5,
                "FooBar",
                new Employee
                {
                    ExperienceYears = 10,
                    Age = 30,
                    Name = new Name
                    {
                        FirstName = "Tom",
                        LastName = "Riddle",
                        Address = new Address
                        {
                            City = "London",
                            Street = "Cannon street",
                            Country = "United Kingdom",
                            PostCode = "NE150XQ"
                        }
                    }
                }
            };
            var analyzer = new Analyzer();
            var s = analyzer.GetStringObjRepresentations(list);
            Console.WriteLine(s);
        }
    }
}
