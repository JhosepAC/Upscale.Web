using System.ComponentModel.DataAnnotations;

namespace Upscale.Web.Models.ViewModels
{
    /// <summary>
    /// View model for displaying and managing user profile information.
    /// Distinguishes between read-only identity data and editable contact information.
    /// </summary>
    public class ProfileViewModel
    {
        // --- READ-ONLY IDENTITY DATA ---

        /// <summary>
        /// Gets the user's full name in "LastNames, FirstName" format.
        /// </summary>
        public string FullName => $"{FirstLastName} {SecondLastName}, {FirstName}".Trim();

        public string FirstName { get; set; }
        public string FirstLastName { get; set; }
        public string SecondLastName { get; set; }

        /// <summary>
        /// Type of identity document
        /// </summary>
        public string DocumentType { get; set; }

        public string DocumentNumber { get; set; }

        public DateTime BirthDate { get; set; }

        public string BirthDateFormatted => BirthDate != DateTime.MinValue
            ? BirthDate.ToString("dd / MM / yyyy")
            : string.Empty;

        public string Nationality { get; set; }

        public string Gender { get; set; }

        // --- EDITABLE CONTACT DATA ---

        [Required(ErrorMessage = "El correo electrónico principal es requerido.")]
        [EmailAddress(ErrorMessage = "El correo electrónico principal no es válido.")]
        [Display(Name = "Correo Institucional")]
        public string Email { get; set; }

        [EmailAddress(ErrorMessage = "El correo electrónico secundario no es válido.")]
        [Display(Name = "Correo Secundario")]
        public string SecondaryEmail { get; set; }

        [Phone(ErrorMessage = "El teléfono móvil no es válido.")]
        [Display(Name = "Teléfono Móvil")]
        public string MobilePhone { get; set; }

        [Phone(ErrorMessage = "El teléfono secundario no es válido.")]
        [Display(Name = "Anexo / Teléfono Fijo")]
        public string SecondaryPhone { get; set; }

        // --- READ-ONLY EMPLOYMENT DATA ---

        public string JobTitle { get; set; }

        public string Organization { get; set; }

        public string ContractType { get; set; }

        public DateTime? HireDate { get; set; }

        public string HireDateFormatted => HireDate.HasValue
            ? HireDate.Value.ToString("dd / MM / yyyy")
            : string.Empty;

        // --- STATUS INDICATORS ---

        public bool IsActive { get; set; }

        /// <summary>
        /// Human-readable label for the user's account status.
        /// </summary>
        public string StatusLabel => IsActive ? "Activo" : "Inactivo";
    }
}