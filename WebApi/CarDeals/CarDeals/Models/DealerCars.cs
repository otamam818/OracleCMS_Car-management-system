// Summary: Model to allow dealers to store multiple cars with varying stocks per dealer
using System.ComponentModel.DataAnnotations;

namespace CarDeals.Models
{
    public class DealerCars(string dealerId, string carId)
    {
        // Primary Foreign Key Made from Dealer.Id and Car.Id
        // Format: `{Dealer.Id} | {Car.Id}`
        [Key]
        public string Id { get; set; } = $"{dealerId} | {carId}";

        // Attributes
        [Required]
        public int Stock { get; set; }

        public string GetDealerId()
        {
            return Id.Split(" | ", StringSplitOptions.None)[0];
        }

        public string GetCarId()
        {
            return Id.Split(" | ", StringSplitOptions.None)[1];
        }
    }
}
