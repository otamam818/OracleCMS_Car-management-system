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
                    companyName = company.Name,
                    model = car.Model,
                    price = car.Price
                }).ToListAsync(); // Convert the result to a list asynchronously

            // Return the carList as a JSON object
            return Ok(new { carList });
        }

        // GET: api/Cars/car/{passwordHash}
        [HttpGet("company/{passwordHash}")]
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
                        companyName = company.Name,
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

        // Post: api/Cars/Search/Model
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
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCar(int id, Car car)
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
                                 select dealer.Id) ?? throw new Exception("Not found");
            var foundDealerId = await foundDealerQuery.SingleAsync();

            return foundDealerId;
        }
    }
}
