using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SurgiTech.Models
{
    // Single-row table (like AdminUser) holding shop-wide settings.
    public class ShopSettings
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string WhatsAppNumber { get; set; } = string.Empty; // digits only, no +, e.g. 923001234567

        [Required]
        public string ContactEmail { get; set; } = "orders@surgitech.example";

        [Column(TypeName = "decimal(18,2)")]
        public decimal ShippingFee { get; set; } = 25.00m;
    }
}