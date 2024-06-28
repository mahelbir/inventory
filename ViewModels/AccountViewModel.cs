using Inventory.Models;
using System.ComponentModel.DataAnnotations;

namespace Inventory.ViewModels
{

    public class AccountLoginViewModel
    {
        [Required(ErrorMessage = "Kullanıcı adı gereklidir.")]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Parola gereklidir.")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }

    }

    public class AccountRegisterViewModel : ApplicationUser
    {
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Parola gereklidir.")]
        [StringLength(100, ErrorMessage = "Parola en az {2} ve en fazla {1} karakter olmalıdır.", MinimumLength = 6)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Parola tekrarı gereklidir.")]
        [Compare("Password", ErrorMessage = "Parolalar eşleşmiyor.")]
        public string ConfirmPassword { get; set; }

    }

}
