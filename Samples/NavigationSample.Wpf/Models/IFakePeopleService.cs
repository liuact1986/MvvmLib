using System.Collections.Generic;

namespace NavigationSample.Wpf.Models
{
    public interface IFakePeopleService
    {
        IList<Person> GetPeople();
        Person GetPersonById(int id);
    }
}
