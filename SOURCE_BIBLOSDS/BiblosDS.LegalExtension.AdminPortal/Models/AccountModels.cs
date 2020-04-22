using System.ComponentModel.DataAnnotations;

namespace BiblosDS.LegalExtension.AdminPortal.Models
{

    public class ChangePasswordModel
    {
        [Required(ErrorMessage = "Il campo Password corrente è obbligatorio")]
        [DataType(DataType.Password)]
        [Display(Name = "Password corrente")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Il campo Nuova password è obbligatorio")]
        [DataType(DataType.Password)]
        [Display(Name = "Nuova password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Conferma nuova password")]
        [Compare("NewPassword", ErrorMessage = "La nuova password e la relativa conferma non corrispondono.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginModel
    {
        [Required(ErrorMessage = "Il campo Username è obbligatorio")]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Il campo Password è obbligatorio")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Ricorda credenziali?")]
        public bool RememberMe { get; set; }
    }
}
