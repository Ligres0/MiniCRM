using System.ComponentModel.DataAnnotations;

namespace MiniCRM.Models
{
    public class Customers
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required")]
        [StringLength(20)]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Surname is required")]
        [StringLength(20)]
        public string Surname { get; set; } = string.Empty;
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string? Email { get; set; }
        [RegularExpression(
             @"^05\d{9}$",
             ErrorMessage = "The phone number must be in the format 05XXXXXXXXX.."
)]
        public string? Phone { get; set; } 
        public string? CompanyName { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
