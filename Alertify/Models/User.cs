using System.ComponentModel.DataAnnotations;

namespace Alertify.Models
{
    public class User : BaseModel
    {
        [Key]
        public int UserID { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string FirstLastName { get; set; }
        public string SecondLastName { get; set; }
        public string NationalID { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; }
        public DateTime? LastAccess { get; set; }
        public string ProfilePhotoURL { get; set; }
        public DateTime RegistrationDate { get; set; }
        public ICollection<Emergency> Emergencies { get; set; }
        public ICollection<Notification> Notifications { get; set; }
    }
}