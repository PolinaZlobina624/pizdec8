using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace RestaurantApp.Models
{
    [Table("financial_reports")]
    public class FinancialReport
    {
        public int Id { get; set; }
        public string ReportType { get; set; }
        public int? ShiftId { get; set; }
        public int? GeneratedBy { get; set; }
        public DateTime DataRangeStart { get; set; }
        public DateTime DataRangeEnd { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal CashRevenue { get; set; }
        public decimal CardRevenue { get; set; }
        public int TotalOrders { get; set; }
    }
}