using System.ComponentModel.DataAnnotations;

namespace Upscale.Web.Models.ViewModels
{
    /// <summary>
    /// View model used to capture and validate user credentials during the login process.
    /// </summary>
    public class LoginViewModel
    {
        /// <summary>
        /// Gets or sets the document number (DNI, CE, etc.) used as the unique username.
        /// </summary>
        [Required(ErrorMessage = "El número de documento es requerido")]
        [StringLength(10, ErrorMessage = "El número de documento no puede exceder los 10 caracteres")]
        [Display(Name = "Usuario")]
        public string DocumentNumber { get; set; }

        /// <summary>
        /// Gets or sets the plain-text password provided by the user.
        /// </summary>
        [Required(ErrorMessage = "La contraseña es requerida")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener entre 6 y 100 caracteres")]
        public string Password { get; set; }

        /// <summary>
        /// Indicates whether the authentication cookie should persist across browser sessions.
        /// </summary>
        [Display(Name = "Recordarme")]
        public bool RememberMe { get; set; }
    }
}