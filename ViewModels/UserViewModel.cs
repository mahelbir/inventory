using Inventory.Models;
using System.ComponentModel.DataAnnotations;

namespace Inventory.ViewModels
{

    public class UserListItemViewModel
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string FullName { get; set; }

        public bool IsActive { get; set; }
    }

    public class UserEditViewModel
    {

        public ApplicationUser UserDetails { get; set; }

        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Parola en az {2} ve en fazla {1} karakter olmalıdır.", MinimumLength = 6)]
        public string? UserPassword { get; set; }

        public IList<string>? UserRoles { get; set; }

        public IList<string>? AllRoles { get; set; }

    }

}
