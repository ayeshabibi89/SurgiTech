using System.ComponentModel.DataAnnotations;

namespace SurgiTech.Models
{
    public class AdminUser
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        // What shows up as "Welcome, ___" on the dashboard.
        public string? DisplayName { get; set; }
    }
}