// Summary: The atomic unit representing individual cars
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarDeals.Models
{
    public class Car(string CompanyId, float Price, string Model)
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Company.Id))]
        public string CompanyId { get; set; } = CompanyId;

        [Required]
        public float Price { get; set; } = Price;

        [Required]
        public string Model { get; set; } = Model;
    }
}
