using System.ComponentModel.DataAnnotations;

namespace Upscale.Web.Models.ViewModels
{
    public class ProfileViewModel
    {
        // ── Identidad (solo lectura) ──────────────────────────────
        public string FirstName { get; set; }
        public string FirstLastName { get; set; }
        public string SecondLastName { get; set; }
        public string FullName => $"{FirstLastName} {SecondLastName}, {FirstName}";

        // ── Documento (solo lectura) ──────────────────────────────
        public string DocumentType { get; set; }
        public string DocumentNumber { get; set; }

        // ── Datos personales (solo lectura) ───────────────────────
        public DateTime BirthDate { get; set; }
        public string BirthDateFormatted => BirthDate != DateTime.MinValue
            ? BirthDate.ToString("dd / MM / yyyy")
            : string.Empty;
        public string Nationality { get; set; }
        public string Gender { get; set; }

        // ── Contacto editable ─────────────────────────────────────
        [EmailAddress(ErrorMessage = "El correo electrónico principal no es válido.")]
        public string Email { get; set; }

        [EmailAddress(ErrorMessage = "El correo electrónico secundario no es válido.")]
        public string SecondaryEmail { get; set; }

        [Phone(ErrorMessage = "El teléfono móvil no es válido.")]
        public string MobilePhone { get; set; }

        [Phone(ErrorMessage = "El teléfono secundario no es válido.")]
        public string SecondaryPhone { get; set; }

        // ── Datos laborales (solo lectura) ────────────────────────
        public string JobTitle { get; set; }
        public string Organization { get; set; }
        public string ContractType { get; set; }
        public DateTime? HireDate { get; set; }
        public string HireDateFormatted => HireDate.HasValue
            ? HireDate.Value.ToString("dd / MM / yyyy")
            : string.Empty;

        // ── Estado ───────────────────────────────────────────────
        public bool IsActive { get; set; }
        public string StatusLabel => IsActive ? "Activo" : "Inactivo";
    }
}