using System.ComponentModel.DataAnnotations.Schema;
using System;
namespace RestaurantApp.Models
{
    [Table("orders")]
    public class Order
    {
        public int Id { get; set; }
        public int? TableId { get; set; }
        public int? WaiterId { get; set; }
        public int? ShiftId { get; set; }
        public string OrderStatus { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}