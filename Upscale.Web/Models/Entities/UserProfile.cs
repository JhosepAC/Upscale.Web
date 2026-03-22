using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Upscale.Web.Models.Entities
{
    public class UserProfile
    {
        [Key]
        public int ProfileId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstLastName { get; set; }

        [Required]
        [MaxLength(100)]
        public string SecondLastName { get; set; }

        [Required]
        [MaxLength(10)]
        public string DocumentType { get; set; }

        public DateTime BirthDate { get; set; }

        [MaxLength(50)]
        public string Nationality { get; set; } = "Peruvian";

        [MaxLength(20)]
        public string Gender { get; set; }

        [MaxLength(150)]
        public string SecondaryEmail { get; set; }

        [MaxLength(25)]
        public string MobilePhone { get; set; }

        [MaxLength(25)]
        public string SecondaryPhone { get; set; }

        [MaxLength(50)]
        public string ContractType { get; set; }

        public DateTime? HireDate { get; set; }

        [MaxLength(150)]
        public string JobTitle { get; set; }

        [MaxLength(150)]
        public string Organization { get; set; }

        public virtual User User { get; set; }
    }
}