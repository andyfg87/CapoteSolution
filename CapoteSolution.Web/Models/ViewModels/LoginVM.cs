using System.ComponentModel.DataAnnotations;

namespace CapoteSolution.Web.Models.ViewModels
{
    public class LoginVM
    {
        [Required(ErrorMessage = "Usuario es requerido")]
        [EmailAddress(ErrorMessage = "Debe ser de tipo Correo")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Contraseña es requerida")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Recordarme")]
        public bool RememberMe { get; set; }
    }
}
