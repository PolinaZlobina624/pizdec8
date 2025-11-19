using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace RestaurantApp.Models
{
    [Table("employees")]
    public class Employee
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Position { get; set; }
        public DateTime HireDate { get; set; }
        public DateTime? LayoffDate { get; set; }
        public bool IsActive { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public byte[] PasswordHash { get; set; }
        public int RoleId { get; set; }
    }
}