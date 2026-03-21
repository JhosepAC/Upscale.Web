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

        public string FirstName { get; set; }
        public string FirstLastName { get; set; }
        public string SecondLastName { get; set; }
        public string DocumentType { get; set; }
        public DateTime BirthDate { get; set; }
        public string Nationality { get; set; } = "Peruvian";
        public string Gender { get; set; }
        public string MobilePhone { get; set; }
        public string JobTitle { get; set; }
        public string Organization { get; set; }

        public virtual User User { get; set; }
    }
}