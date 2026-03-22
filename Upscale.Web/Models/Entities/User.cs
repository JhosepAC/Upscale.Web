using System.ComponentModel.DataAnnotations;

namespace Upscale.Web.Models.Entities
{
    /// <summary>
    /// Represents a system user account and its authentication-related security data.
    /// </summary>
    public class User
    {
        [Key]
        public int UserId { get; set; }

        /// <summary>
        /// Unique identification document number used as a login identifier.
        /// </summary>
        [Required]
        public string DocumentNumber { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        // --- SECURITY DATA ---

        /// <summary>
        /// Salted password hash generated with HMACSHA512.
        /// </summary>
        public byte[] PasswordHash { get; set; }

        /// <summary>
        /// Unique salt used to generate the password hash.
        /// </summary>
        public byte[] PasswordSalt { get; set; }

        // --- ACCOUNT LOCKOUT LOGIC ---

        public int FailedAttempts { get; set; } = 0;

        public bool IsLocked { get; set; } = false;

        /// <summary>
        /// The timestamp when the account lockout period expires.
        /// </summary>
        public DateTime? LockoutEnd { get; set; }

        /// <summary>
        /// Indicates if the account is logically active in the system.
        /// </summary>
        public bool IsActive { get; set; } = true;

        // --- NAVIGATION PROPERTIES ---

        /// <summary>
        /// Navigation property for the user's detailed profile information.
        /// </summary>
        public virtual UserProfile Profile { get; set; }
    }
}