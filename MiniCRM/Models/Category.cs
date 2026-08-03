using System.ComponentModel.DataAnnotations;

namespace MiniCRM.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Category field is required.") ]
        [StringLength(100, ErrorMessage = "The category name can be a maximum of 100 characters.")]

        public string Name { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
}
