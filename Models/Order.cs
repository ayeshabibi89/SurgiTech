using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SurgiTech.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string HospitalName { get; set; } = string.Empty;

        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Required]
        public string Status { get; set; } = "Processing"; // Shipped, Processing, Delivered
    }
}