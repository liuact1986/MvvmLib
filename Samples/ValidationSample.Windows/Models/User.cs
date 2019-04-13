using MvvmLib.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ValidationSample.Windows.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(5)] // for demo
        public string FirstName { get; set; }

        [StringLength(2)] // for demo
        public string LastName { get; set; }

        // object, list , etc.
    }

    public class UserWrapper : ModelWrapper<User>
    {
        public UserWrapper(User model) : base(model)
        {
        }

        public int Id { get { return Model.Id; } }

        public string FirstName
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string LastName
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        // etc.

        // custom validations
        protected override IEnumerable<string> DoCustomValidations(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(FirstName):
                    if (string.Equals(FirstName, "Marie", StringComparison.OrdinalIgnoreCase))
                    {
                        yield return "Marie is not allowed";
                    }
                    break;
            }
        }
    }
}
