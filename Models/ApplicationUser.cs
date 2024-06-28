using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "İsim gereklidir.")]
        [StringLength(255, ErrorMessage = "İsim en fazla 255 karakter olabilir.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Soyisim gereklidir.")]
        [StringLength(255, ErrorMessage = "Soyisim en fazla 255 karakter olabilir.")]
        public string LastName { get; set; }

        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";

    }
}
