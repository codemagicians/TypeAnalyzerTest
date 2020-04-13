using System;
using ClassAnalyzer;

namespace Azrielit_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var p = new Person
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
            };
            var analyzer = new Analyzer();
            var s = analyzer.GetStringObjRepresentation(p);
            Console.WriteLine(s);
        }
    }
}
