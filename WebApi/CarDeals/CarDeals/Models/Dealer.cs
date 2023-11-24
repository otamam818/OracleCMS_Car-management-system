// Summary: The login information used for car dealers to access their private
//          data
using System.ComponentModel.DataAnnotations;

namespace CarDeals.Models
{
    public class Dealer(string Name, string PasswordHash)
    {
        [Key]
        public double Id { get; set; } = SecondsSinceEpoch();

        [Required]
        public string Name { get; set; } = Name;

        // It is unsafe to store passwords as they are provided by the user
        // So passwords should be appended with a salt and then stored as hashes
        [Required]
        public string PasswordHash { get; set; } = PasswordHash;

        public static double SecondsSinceEpoch()
        {
            TimeSpan diff = DateTime.Now.ToUniversalTime() - DateTime.UnixEpoch;
            return Math.Floor(diff.TotalSeconds);
        }
    }
}
