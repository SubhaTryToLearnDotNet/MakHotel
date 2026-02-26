using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace MakQR.Models.Admin
{
    public class ChangePasswordModel
    {

        [Required(ErrorMessage = "Old password is required")]
        public string OldPassword { get; set; } = "";

        [Required(ErrorMessage = "New password is required")]
        public string NewPassword { get; set; } = "";

        [Required(ErrorMessage = "Confirm password is required")]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = "";

    }
}
