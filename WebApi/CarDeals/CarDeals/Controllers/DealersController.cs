// Summary: Handles the CRUD operations related to every dealer
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarDeals.Models;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Runtime.Intrinsics.Arm;
using System.Text;

namespace CarDeals.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DealersController(CarDealerContext context) : ControllerBase
    {
        private readonly string SALT = "SALT_VALUE";
        private readonly CarDealerContext _context = context;

        // GET: api/Dealers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> GetDealers()
        {
            return await _context.Deals.Select(u => u.Name).ToListAsync();
        }

        // POST: api/Dealers/SignUp
        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUpDealer(string username, string password)
        {
            // We can never be too safe from invalid input, but we can always prevent null
            // values from contributing to it
            if (username == null || password == null)
            {
                return BadRequest();
            }

            if (_context.Deals.Select(table => table.Name).Contains(username))
            {
                return Conflict(new { message = "Conflict: NameExists" });
            }

            /* The user would be entering their password, not the passwordHash itself
             * This is where a hash function would be run on the Password+SALT combination
             */
            password = ComputeHashValue(password + SALT);

            // No conflicts have arisen, it can now be added to the database
            var dealer = new Dealer(username, password);
            _context.Deals.Add(dealer).State = EntityState.Added;

            // Sign Up was a success, they should be logged in now
            _context.SignedInUsers.Add(dealer.Name, DateTime.Now);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return Ok(new { message = "Success", hashValue = dealer.PasswordHash } );
        }

        // POST: api/Dealers/SignIn
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("SignIn")]
        public async Task<IActionResult> SignInDealer(string username, string password)
        {
            // Before we sign them in, we must check if the passwords match
            var ExpectedPassword = password + SALT;
            var ExecutedQuery = from dealers in _context.Deals
                            where dealers.PasswordHash == ExpectedPassword && dealers.Name == username
                            select dealers;
            var FoundDealer = ExecutedQuery.First();
            if (FoundDealer == null)
            {
                return NotFound();
            }

            _context.SignedInUsers.Add(username, DateTime.Now);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return Ok(new { message = "Success", hashValue = FoundDealer.PasswordHash } );
        }

        // DELETE: api/Dealers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDealer(int id)
        {
            var dealer = await _context.Deals.FindAsync(id);
            if (dealer == null)
            {
                return NotFound();
            }

            _context.Deals.Remove(dealer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private static string ComputeHashValue(string value)
        {
            string HashString;
            // Convert the string to a byte array
            byte[] messageBytes = Encoding.UTF8.GetBytes(value);

            // Compute the hash from the byte array
            byte[] hashValue = SHA256.HashData(messageBytes);

            // Convert the hash value to a hexadecimal string
            HashString = BitConverter.ToString(hashValue).Replace("-", "");
            return HashString;
        }
    }
}
