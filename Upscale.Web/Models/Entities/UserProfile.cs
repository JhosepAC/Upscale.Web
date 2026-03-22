using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Upscale.Web.Models.Entities
{
    /// <summary>
    /// Represents the extended profile details for a system user.
    /// This entity holds personal, contact, and professional information.
    /// </summary>
    public class UserProfile
    {
        [Key]
        public int ProfileId { get; set; }

        /// <summary>
        /// Foreign key reference to the parent User account.
        /// </summary>
        [ForeignKey("User")]
        public int UserId { get; set; }

        // --- PERSONAL IDENTIFICATION ---

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstLastName { get; set; }

        [Required]
        [MaxLength(100)]
        public string SecondLastName { get; set; }

        /// <summary>
        /// Type of identification document (e.g., DNI, CE, Passport).
        /// </summary>
        [Required]
        [MaxLength(10)]
        public string DocumentType { get; set; }

        public DateTime BirthDate { get; set; }

        [MaxLength(50)]
        public string Nationality { get; set; } = "Peruvian";

        [MaxLength(20)]
        public string Gender { get; set; }

        // --- CONTACT INFORMATION ---

        [MaxLength(150)]
        [EmailAddress]
        public string? SecondaryEmail { get; set; }

        [MaxLength(25)]
        public string? MobilePhone { get; set; }

        [MaxLength(25)]
        public string? SecondaryPhone { get; set; }

        // --- EMPLOYMENT DETAILS ---

        [MaxLength(50)]
        public string ContractType { get; set; }

        public DateTime? HireDate { get; set; }

        [MaxLength(150)]
        public string JobTitle { get; set; }

        [MaxLength(150)]
        public string Organization { get; set; }

        // --- NAVIGATION PROPERTIES ---

        /// <summary>
        /// Reference to the User entity associated with this profile.
        /// </summary>
        public virtual User User { get; set; }
    }
}