// Summary: The login information used for car dealers to access their private
//          data
using System.ComponentModel.DataAnnotations;

namespace CarDeals.Models
{
    public class Dealer(string Name, string PasswordHash)
    {
        [Key]
        public string Id { get; set; } = $"{Utils.SecondsSinceEpoch()}/{Utils.ComputeHashValue(Name)}";

        [Required]
        public string Name { get; set; } = Name;

        // It is unsafe to store passwords as they are provided by the user
        // So passwords should be appended with a salt and then stored as hashes
        [Required]
        public string PasswordHash { get; set; } = PasswordHash;
    }
}
