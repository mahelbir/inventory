using System.ComponentModel.DataAnnotations;

namespace Inventory.ViewModels
{
    public class AccountBaseViewModel
    {
        [Required(ErrorMessage = "Kullanıcı adı gereklidir.")]
        [StringLength(50, ErrorMessage = "Kullanıcı adı en fazla 50 karakter olabilir.")]
        public string UserName { get; set; }

    }

    public class AccountFullViewModel : AccountBaseViewModel
    {

        [Required(ErrorMessage = "Email gereklidir.")]
        [EmailAddress(ErrorMessage = "Geçersiz email formatı.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "İsim gereklidir.")]
        [StringLength(255, ErrorMessage = "İsim en fazla 255 karakter olabilir.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Soyisim gereklidir.")]
        [StringLength(255, ErrorMessage = "Soyisim en fazla 255 karakter olabilir.")]
        public string LastName { get; set; }

    }


    public class AccountLoginViewModel : AccountBaseViewModel
    {
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Parola gereklidir.")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }

    }

    public class AccountRegisterViewModel : AccountFullViewModel
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
