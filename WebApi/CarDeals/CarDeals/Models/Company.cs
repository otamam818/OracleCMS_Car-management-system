// Summary: As multiple cars can belong to one company,
//          it makes sense to model for separate companies
using System.ComponentModel.DataAnnotations;

namespace CarDeals.Models
{
    public class Company
    {
        public Company() { }

        public Company(string Name, string? Address)
        {
            Id = $"{Utils.SecondsSinceEpoch()}/{Utils.ComputeHashValue(Name)}";
            this.Name = Name;
            this.Address = Address;
        }

        [Key]
        public string Id { get; set; } = null!;

        [Required]
        public string Name { get; set; } = null!;

        public string? Address { get; set; }
    }
}
