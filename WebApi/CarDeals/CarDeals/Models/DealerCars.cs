// Summary: Model to allow dealers to store multiple cars with varying stocks per dealer
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CarDeals.Models
{
    [PrimaryKey(nameof(DealerId), nameof(CarId))]
    public class DealerCars
    {
        public DealerCars() { }

        public DealerCars(string dealerId, int carId, int stock)
        {
            DealerId = dealerId;
            CarId = carId;
            Stock = stock;
        }


        // Primary Foreign Keys
        [Required]
        [Column(Order = 0)]
        [ForeignKey(nameof(Dealer.Id))]
        public string DealerId { get; set; } = null!;

        [Required]
        [Column(Order = 1)]
        [ForeignKey(nameof(Car.Id))]
        public int CarId { get; set; }

        // Attributes
        [Required]
        public int Stock { get; set; }
    }
}
