using MvvmLib.Mvvm;

namespace NavigationSample.Wpf.Models
{
    public class Person : BindableBase
    {
        private string firstName;
        public string FirstName
        {
            get { return firstName; }
            set { SetProperty(ref firstName, value); }
        }

        private string lastName;
        public string LastName
        {
            get { return lastName; }
            set { SetProperty(ref lastName, value); }
        }

        private string emailAddress;
        public string EmailAddress
        {
            get { return emailAddress; }
            set { SetProperty(ref emailAddress, value); }
        }

        public int Id { get; internal set; }

        public override string ToString()
        {
            return $"{firstName} {lastName}";
        }
    }
}
