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

        // GET: api/Dealers/Name/{passwordHash}
        [HttpGet("Name/{passwordHash}")]
        public async Task<ActionResult<IEnumerable<string>>> GetDealerName(string passwordHash)
        {
            var dealerId = await Utils.GetDealerIdFromHash(_context, passwordHash);
            var dealer = await _context.Deals.FindAsync(dealerId);
            if (dealer == null)
            {
                return NotFound();
            }

            return Ok(new { name = dealer.Name });
        }

        // GET: api/Dealers/SignInStatus/
        [HttpGet("SignInStatus/{passwordHash}")]
        public async Task<ActionResult<IEnumerable<string>>> SignInStatus(string passwordHash)
        {
            var foundUser = await _context.SignedInUsers.FindAsync(passwordHash);
            if (foundUser != null)
            {
                if ((DateTime.Now - foundUser.LastSignedIn).TotalMinutes > CarsController.MINUTES_ALLOWED)
                {
                    // If it's expired, there is no need to keep this record anymore
                    _context.SignedInUsers.Remove(foundUser);
                    return Ok(new { status = "SessionExpired" });
                }

                // Renew the session counter (since the user interacted with it)
                foundUser.LastSignedIn = DateTime.Now;
                _context.SignedInUsers.Update(foundUser);

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }

                return Ok(new { status = "SessionExists" });
            }
            return Ok(new { status = "NotSignedIn" });
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
            password = Utils.ComputeHashValue(password + SALT);

            // No conflicts have arisen, it can now be added to the database
            var dealer = new Dealer(username, password);
            _context.Deals.Add(dealer).State = EntityState.Added;

            // Sign Up was a success, they should be logged in now
            _context.SignedInUsers.Add(new SignedInUser(dealer.PasswordHash));

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return Ok(new {
                hashValue = dealer.PasswordHash,
                status = 200
            } );
        }

        // POST: api/Dealers/SignIn
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("SignIn")]
        public async Task<IActionResult> SignInDealer(string username, string password)
        {
            // Before we sign them in, we must check if the passwords match
            var ExpectedPassword = Utils.ComputeHashValue(password + SALT);
            var ExecutedQuery = from dealers in _context.Deals
                            where dealers.PasswordHash == ExpectedPassword && dealers.Name == username
                            select dealers;
            try
            {
                var FoundDealer = await ExecutedQuery.SingleAsync();
                var CurrSignedInUser = _context.SignedInUsers.Find(FoundDealer.PasswordHash);
                if (CurrSignedInUser != null)
                {
                    CurrSignedInUser.LastSignedIn = DateTime.Now;
                    _context.SignedInUsers.Update(CurrSignedInUser);
                }
                else
                {
                    _context.SignedInUsers.Add(new SignedInUser(FoundDealer.PasswordHash));
                }

                await _context.SaveChangesAsync();

                return Ok(new {
                    hashValue = FoundDealer.PasswordHash,
                    status = 200
                } );
            } catch (InvalidOperationException)
            {
                throw;
            } catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        // POST: api/Dealers/SignOut
        [HttpPost("SignOut")]
        public async Task<IActionResult> SignOutDealer(string passwordHash)
        {
            // Before we sign them in, we must check if the passwords match
            try
            {
                var FoundAccount = await _context.SignedInUsers.FindAsync(passwordHash);
                if (FoundAccount != null)
                {
                    _context.SignedInUsers.Remove(FoundAccount);
                }

                await _context.SaveChangesAsync();

                return Ok(new {
                    status = 200
                } );
            } catch (InvalidOperationException)
            {
                return NotFound();
            } catch (DbUpdateConcurrencyException)
            {
                throw;
            }
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

    }
}
