using MvvmLib.Mvvm;
using System.Collections.Generic;

namespace NavigationSample.Wpf.Models
{
    public class FakePeopleService: IFakePeopleService
    {
        static List<Person> people = new List<Person>
        {
            new Person { Id=1, FirstName="First1",LastName="Last1" },
            new Person { Id=2, FirstName="First2",LastName="Last2" },
            new Person { Id=3, FirstName="First3",LastName="Last3"}
        };

      
        Cloner cloner = new Cloner();

        public IList<Person> GetPeople()
        {
            var result = cloner.DeepClone(people);
            return result;
        }

        public Person GetPersonById(int id)
        {
            foreach (var person in people)
            {
                if(person.Id == id)
                {
                    var result = cloner.DeepClone(person);
                    return result;
                }
            }
            return null;
        }
    }
}
