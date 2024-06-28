using System.ComponentModel.DataAnnotations;

namespace Inventory.ViewModels
{
    public class UserViewModel : AccountFullViewModel
    {
        public string Id { get; set; }

    }

    public class UserEditViewModel : UserViewModel
    {
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Parola en az {2} ve en fazla {1} karakter olmalıdır.", MinimumLength = 6)]
        public string? Password { get; set; }

        public IList<string> Roles { get; set; }

    }

}
