using System;
using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class Expense
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string Category { get; set; } = "General";

        [Required]
        public decimal Amount { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow;

        public int UserId { get; set; }
        public User? User { get; set; }
    }
}
