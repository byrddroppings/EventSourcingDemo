using System;
using System.ComponentModel.DataAnnotations;

namespace EventSourcingDemo.Models
{
    public class Person
    {
        public Guid PersonId { get; set; }
        public string Status { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
    }
}