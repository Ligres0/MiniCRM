using System.ComponentModel.DataAnnotations;

namespace MiniCRM.ViewModels
{
    public class RoleCreateViewModel
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }  = string.Empty;
    }
}