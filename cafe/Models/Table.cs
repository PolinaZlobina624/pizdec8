using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Threading.Tasks;

namespace RestaurantApp.Models
{
    [Table("tables")]
    public class Table
    {
        public int Id { get; set; }
        public string TableNumber { get; set; }
        public short Capacity { get; set; }
        public bool IsAvailable { get; set; }
    }
}