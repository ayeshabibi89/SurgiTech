using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SurgiTech.Models
{
    public class SurgicalInstrument
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Category { get; set; } = string.Empty; // e.g., Scalpels, Forceps, Scissors

        [StringLength(100)]
        public string MaterialGrade { get; set; } = string.Empty; // e.g., German Stainless Steel, Titanium

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        public int StockQuantity { get; set; }

        public string ImageUrl { get; set; } = "/images/default-instrument.jpg";
    }
}