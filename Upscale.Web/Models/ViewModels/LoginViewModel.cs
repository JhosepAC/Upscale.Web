using System.ComponentModel.DataAnnotations;

namespace Upscale.Web.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El número de documento es requerido")]
        [Display(Name = "Usuario")]
        public string DocumentNumber { get; set; }

        [Required(ErrorMessage = "La contraseña es requerida")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}