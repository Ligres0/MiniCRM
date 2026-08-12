using System.ComponentModel.DataAnnotations;

namespace MiniCRM.ViewModels
{
    public class UserCreateViewModel
    {
        [Required]
        [StringLength(50)]
        public string UserName { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;
        [Required]
        [Compare(
            nameof(Password),
            ErrorMessage =
                "Passwords do not match.")]
        public string ConfirmPassword { get; set; }= string.Empty;
    }
}
