using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarDeals.Models;
using System.Collections;

namespace CarDeals.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarsController(CarDealerContext context) : ControllerBase
    {
        private readonly CarDealerContext _context = context;
        public static readonly uint MINUTES_ALLOWED = 30;

        // GET: api/Cars
        [HttpGet]
        public async Task<ActionResult<object>> GetCars()
        {
            // Join the Cars and Companies collections based on the matching Id values
            var carList = await _context.Cars.Join(_context.Companies,
                car => car.CompanyId,
                company => company.Id,
                (car, company) => new
                {
                    id = car.Id,
                    make = company.Name,
                    model = car.Model,
                    price = car.Price
                }).ToListAsync(); // Convert the result to a list asynchronously

            // Return the carList as a JSON object
            return Ok(new { carList });
        }

        // GET: api/Cars/dealer/{passwordHash}
        [HttpGet("dealer/{passwordHash}")]
        public async Task<ActionResult<object>> GetCarsByCompany(string passwordHash)
        {
            var foundDealerId = await Utils.GetDealerIdFromHash(_context, passwordHash);

            // Join the DealerCars, Dealer, Car, and Company collections based on the matching Id values
            var carList = await _context.DealersCars
                .Where(dealerCar => dealerCar.DealerId.Equals(foundDealerId))
                .Join(_context.Cars,
                    dealerCar => dealerCar.CarId,
                    car => car.Id,
                    (dealerCar, carData) => new { dealerCar, carData })
                .Join(_context.Companies,
                    joinedTable => joinedTable.carData.CompanyId,
                    company => company.Id,
                    (joinedTable, company) => new
                    {
                        carId = joinedTable.carData.Id,
                        make = company.Name,
                        price = joinedTable.carData.Price,
                        model = joinedTable.carData.Model,
                        stock = joinedTable.dealerCar.Stock
                    }).ToListAsync();

            // Return the carList as a JSON object
            return Ok(new { carList });
        }

        // Post: api/Cars/Search/Make
        [HttpPost("Search/Make")]
        public async Task<ActionResult<object>> SearchByMake(string passwordHash, string makeName)
        {
            var foundDealerId = await Utils.GetDealerIdFromHash(_context, passwordHash);

            // Join the Companies and Car collections based on the matching Id values
            var carList = await _context.Companies
                .Where(company => company.Name.ToLower().StartsWith(makeName.ToLower()))
                .Join(_context.Cars,
                    company => company.Id,
                    car => car.CompanyId,
                    (company, car) => new { company, car })
                .Join(_context.DealersCars,
                    joinedTable => joinedTable.car.Id,
                    dealersCars => dealersCars.CarId,
                    (joinedTable,  dealersCars) => new { joinedTable, dealersCars })
                .Where(formedTable => formedTable.dealersCars.DealerId.Equals(foundDealerId))
                .Select(finalTable => new
                {
                    carId = finalTable.joinedTable.car.Id,
                    make =  finalTable.joinedTable.company.Name,
                    price = finalTable.joinedTable.car.Price,
                    model = finalTable.joinedTable.car.Model,
                    stock = finalTable.dealersCars.Stock
                })
                .ToListAsync();
            // Return the carList as a JSON object
            return Ok(new { carList });
        }

        // POST: api/Cars/Search/Model
        [HttpPost("Search/Model")]
        public async Task<ActionResult<object>> SearchByModel(string passwordHash, string modelName)
        {
            var foundDealerId = await Utils.GetDealerIdFromHash(_context, passwordHash);

            // Join the Companies and Car collections based on the matching Id values
            var carList = await _context.Cars
                .Where(car => car.Model.ToLower().StartsWith(modelName.ToLower()))
                .Join(_context.Companies,
                    car => car.CompanyId,
                    company => company.Id,
                    (car, company) => new { company, car })
                .Join(_context.DealersCars,
                    joinedTable => joinedTable.car.Id,
                    dealersCars => dealersCars.CarId,
                    (joinedTable,  dealersCars) => new
                    {
                        joinedTable,
                        dealersCars
                    })
                .Where(formedTable => formedTable.dealersCars.DealerId.Equals(foundDealerId))
                .Select(finalTable => new
                {
                    companyId = finalTable.joinedTable.company.Id,
                    carId = finalTable.joinedTable.car.Id,
                    make = finalTable.joinedTable.company.Name,
                    model = finalTable.joinedTable.car.Model,
                    price = finalTable.joinedTable.car.Price,
                    stock = finalTable.dealersCars.Stock
                })
                .ToListAsync();
            // Return the carList as a JSON object
            return Ok(new { carList });
        }

        // GET: api/Cars/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Car>> GetCar(int id)
        {
            var car = await _context.Cars.FindAsync(id);

            if (car == null)
            {
                return NotFound();
            }

            return car;
        }

        // PUT: api/Cars/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("Update/Car/{id}")]
        public async Task<IActionResult> UpdateCar(int id, Car car)
        {
            if (id != car.Id)
            {
                return BadRequest();
            }

            _context.Entry(car).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CarExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // PUT: api/Cars/Update/Stock/
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("Update/Stock/")]
        public async Task<IActionResult> UpdateStock(int CarId, int NewStockValue, string PasswordHash)
        {
            if (PasswordHash == null)
            {
                return BadRequest();
            }

            // Make sure the PasswordHash is the only way to get the relevant Id values
            var dealerId = await Utils.GetDealerIdFromHash(_context, PasswordHash);

            // Retrieve a dealerCar value if it exists
            var dealerCar = await _context.DealersCars.FindAsync(dealerId, CarId);
            if (dealerCar == null)
            {
                return NotFound();
            }

            // If no value is returned prior to this statement, it is safe
            // to mutate the Stock value
            dealerCar.Stock = NewStockValue;

            // Values have changed locally, time to change them globally
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return Ok(new { message = "Success" });
        }

        // POST: api/Cars
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Car>> AddCar(Car car)
        {
            _context.Cars.Add(car);
            await _context.SaveChangesAsync();

            return Ok(car);
        }

        // POST: api/Cars/Stock
        [HttpPost("Stock")]
        public async Task<ActionResult<Car>> AddStock(int CarId, string passwordHash, int Stock)
        {
            var dealerId = await Utils.GetDealerIdFromHash(_context, passwordHash);

            // Check if this already exists
            var potentialDealerCar = await _context.DealersCars.FindAsync(dealerId, CarId);
            if (potentialDealerCar != null)
            {
                return new ConflictObjectResult(new { message = "A record like this already exists." });
            }

            var dealerCar = new DealerCars(dealerId, CarId, Stock);

            _context.DealersCars.Add(dealerCar);
            await _context.SaveChangesAsync();

            return Ok(new { status = 200 });
        }

        // POST: api/Cars/Company
        [HttpPost("Company")]
        public async Task<ActionResult<Car>> AddCompany(string Name, string? Address)
        {
            // Check if this already exists
            var potentialDealerCar = await _context.Companies.SingleOrDefaultAsync(
                value => string.Equals(value.Name, Name, StringComparison.OrdinalIgnoreCase)
            );

            if (potentialDealerCar != null)
            {
                return new ConflictObjectResult(new { message = "A record like this already exists." });
            }

            var company = new Company(Name, Address);

            _context.Companies.Add(company);
            await _context.SaveChangesAsync();

            return Ok(new { status = 200, id = company.Id });
        }

        // DELETE: api/Cars/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCar(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }

            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CarExists(int id)
        {
            return _context.Cars.Any(e => e.Id == id);
        }

    }
}
