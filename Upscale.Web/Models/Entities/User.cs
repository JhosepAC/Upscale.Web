using System.ComponentModel.DataAnnotations;

namespace Upscale.Web.Models.Entities
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        public string DocumentNumber { get; set; }

        [Required]
        public string Email { get; set; }

        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

        public int FailedAttempts { get; set; } = 0;
        public bool IsLocked { get; set; } = false;
        public DateTime? LockoutEnd { get; set; }

        public bool IsActive { get; set; } = true;

        public virtual UserProfile Profile { get; set; }
    }
}