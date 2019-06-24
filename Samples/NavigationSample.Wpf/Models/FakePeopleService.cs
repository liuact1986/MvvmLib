using MvvmLib.Mvvm;
using System.Collections.Generic;
using System.Linq;

namespace NavigationSample.Wpf.Models
{
    public class FakeData
    {
        public static List<Person> People = new List<Person>
        {
            new Person { Id=1, FirstName="First1",LastName="Last1" },
            new Person { Id=2, FirstName="First2",LastName="Last2" },
            new Person { Id=3, FirstName="First3",LastName="Last3"}
        };
    }

    public class FakePeopleService : IFakePeopleService
    {
        static int lastId = 1;
        static int GetNewId()
        {
            lastId += 1;
            return lastId;
        }

        Cloner cloner = new Cloner();

        static FakePeopleService()
        {
            lastId = FakeData.People.Last().Id;
        }

        public void Add(Person person)
        {
            person.Id = GetNewId();
            FakeData.People.Add(person);
        }

        public IList<Person> GetPeople()
        {
            var result = cloner.DeepClone(FakeData.People);
            return result;
        }

        public Person GetPersonById(int id)
        {
            foreach (var person in FakeData.People)
            {
                if (person.Id == id)
                {
                    var result = cloner.DeepClone(person);
                    return result;
                }
            }
            return null;
        }
    }

    public class Lookup
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
    }

    public interface IFakePeopleLookupService
    {
        IList<Lookup> GetPeople();
    }

    public class FakeLookupService : IFakePeopleLookupService
    {
        public IList<Lookup> GetPeople()
        {
            var people = FakeData.People;
            var lookups = new List<Lookup>();
            foreach (var person in people)
            {
                lookups.Add(new Lookup { Id = person.Id, DisplayName = person.FirstName });
            }
            return lookups;
        }
    }
}
