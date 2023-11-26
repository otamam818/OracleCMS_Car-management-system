using System.ComponentModel.DataAnnotations;

namespace CarDeals.Models
{
    public class SignedInUser
    {
        public SignedInUser() { }
        public SignedInUser(string passwordHash)
        {
            PasswordHash = passwordHash;
            LastSignedIn = DateTime.Now;
        }

        [Key]
        public string PasswordHash { get; set; } = null!;

        [Required]
        public DateTime LastSignedIn { get; set; }

    }
}
