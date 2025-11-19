using System.ComponentModel.DataAnnotations.Schema;
using System; 
using System.Threading.Tasks;

namespace RestaurantApp.Models
{
    [Table("menu_items")]
    public class MenuItem
    {
        public int Id { get; set; }
        public string FoodName { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public int PreparationTime { get; set; }
        public bool IsAvailable { get; set; }
    }
}