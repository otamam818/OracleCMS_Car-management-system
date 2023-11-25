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
        private readonly uint MINUTES_ALLOWED = 30;

        // GET: api/Cars
        [HttpGet]
        public async Task<ActionResult<JsonResult>> GetCars()
        {
            // Join the Cars and Companies collections based on the matching Id values
            var carList = await _context.Cars.Join(_context.Companies,
                car => car.Id,
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
        public async Task<ActionResult<JsonResult>> GetCarsByCompany(string passwordHash)
        {
            var foundDealerId = await GetDealerIdFromHash(passwordHash);

            // Join the DealerCars, Dealer, Car, and Company collections based on the matching Id values
            var carList = await _context.DealersCars
                .Where(dealerCar => dealerCar.GetDealerId().Equals(foundDealerId))
                .Join(_context.Cars,
                    dealerCar => dealerCar.GetCarId(),
                    car => car.Id.ToString(),
                    (dealerCar, carData) => new { dealerCar, carData })
                .Join(_context.Companies,
                    joinedTable => joinedTable.carData.Id,
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
        public async Task<ActionResult<JsonResult>> SearchByMake(string passwordHash, string makeName)
        {
            var foundDealerId = await GetDealerIdFromHash(passwordHash);

            // Join the Companies and Car collections based on the matching Id values
            var carList = await _context.Companies
                .Where(company => company.Name.StartsWith(makeName))
                .Join(_context.Cars,
                    company => company.Id,
                    car => car.CompanyId,
                    (company, car) => new { company, car })
                .Join(_context.DealersCars,
                    joinedTable => joinedTable.car.Id.ToString(),
                    dealersCars => dealersCars.GetCarId(),
                    (joinedTable,  dealersCars) => new
                    {
                        dealerId = dealersCars.GetDealerId(),
                        companyId = joinedTable.company.Id,
                        carId = joinedTable.car.Id,
                        make = joinedTable.company.Name,
                        model = joinedTable.car.Model,
                        price = joinedTable.car.Price
                    })
                .Where(finalTable => finalTable.dealerId.Equals(foundDealerId))
                .ToListAsync();
            // Return the carList as a JSON object
            return Ok(new { carList });
        }

        // POST: api/Cars/Search/Model
        [HttpPost("Search/Model")]
        public async Task<ActionResult<JsonResult>> SearchByModel(string passwordHash, string modelName)
        {
            var foundDealerId = await GetDealerIdFromHash(passwordHash);

            // Join the Companies and Car collections based on the matching Id values
            var carList = await _context.Cars
                .Where(car => car.Model.StartsWith(modelName))
                .Join(_context.Companies,
                    car => car.CompanyId,
                    company => company.Id,
                    (car, company) => new { company, car })
                .Join(_context.DealersCars,
                    joinedTable => joinedTable.car.Id.ToString(),
                    dealersCars => dealersCars.GetCarId(),
                    (joinedTable,  dealersCars) => new
                    {
                        dealerId = dealersCars.GetDealerId(),
                        companyId = joinedTable.company.Id,
                        carId = joinedTable.car.Id,
                        make = joinedTable.company.Name,
                        model = joinedTable.car.Model,
                        price = joinedTable.car.Price
                    })
                .Where(finalTable => finalTable.dealerId.Equals(foundDealerId))
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
            var dealerId = await GetDealerIdFromHash(PasswordHash);
            var dealerCarId = DealerCars.MakeDealerId(dealerId.ToString(), CarId.ToString());

            // Retrieve a dealerCar value if it exists
            var dealerCar = await _context.DealersCars.FindAsync(dealerCarId);
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
        public async Task<ActionResult<Car>> PostCar(Car car)
        {
            _context.Cars.Add(car);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCar", new { id = car.Id }, car);
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

        private async Task<double> GetDealerIdFromHash(string passwordHash)
        {
            // Find the ID associated with the passwordHash
            var foundDealerQuery = (from dealer in _context.Deals
                                 where dealer.PasswordHash == passwordHash
                                 select dealer) ?? throw new Exception("Not found");
            var foundDealer = await foundDealerQuery.SingleAsync();
            if (_context.SignedInUsers.TryGetValue(foundDealer.Name, out DateTime timeLastSignedIn))
            {
                // Check if the session has lasted for too long
                var MinutesElapsed = (DateTime.Now - timeLastSignedIn).TotalMinutes;
                if (MinutesElapsed > MINUTES_ALLOWED)
                {
                    throw new Exception("Session expired");
                } else
                {
                    // Renew the session counter (since the user interacted with it)
                    _context.SignedInUsers[foundDealer.Name] = DateTime.Now;
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return foundDealer.Id;
        }
    }
}
