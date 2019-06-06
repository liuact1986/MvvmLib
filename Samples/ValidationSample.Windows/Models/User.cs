using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace ValidationSample.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(5)]
        public string FirstName { get; set; }

        [StringLength(2)]
        public string LastName { get; set; }


        [MaxItems(3, ErrorMessage = "The user cannot have more than 2 pets.")]
        public ICollection<string> Pets { get; set; }

        public User()
        {
            this.Pets = new Collection<string>();
        }
    }

    public class MaxItemsAttribute : ValidationAttribute
    {
        private readonly int maxItems;

        public MaxItemsAttribute(int maxItems)
        {
            this.maxItems = maxItems;
        }

        public override bool IsValid(object value)
        {
            if (value is IList)
            {
                var isValid = ((IList)value).Count < maxItems;
                return isValid;
            }
            else
                throw new NotSupportedException();
        }
    }
}
