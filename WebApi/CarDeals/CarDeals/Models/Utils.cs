// Summary: The login information used for car dealers to access their private
//          data
using System.Security.Cryptography;
using System.Text;
using CarDeals.Controllers;
using Microsoft.EntityFrameworkCore;

namespace CarDeals.Models
{
    internal static class Utils
    {
        public static async Task<string> GetDealerIdFromHash(CarDealerContext _context, string passwordHash)
        {
            // Find the ID associated with the passwordHash
            var foundDealerQuery = from dealer in _context.Deals
                                 where dealer.PasswordHash == passwordHash
                                 select dealer;
            var foundDealer = await foundDealerQuery.SingleAsync();
            if (foundDealer.PasswordHash != passwordHash)
            {
                throw new Exception("PasswordHash does not match");
            }

            var foundUser = await _context.SignedInUsers.FindAsync(passwordHash);
            if (foundUser != null)
            {
                // Check if the session has lasted for too long
                var MinutesElapsed = (DateTime.Now - foundUser.LastSignedIn).TotalMinutes;
                if (MinutesElapsed > CarsController.MINUTES_ALLOWED)
                {
                    throw new Exception("Session expired");
                } else
                {
                    // Renew the session counter (since the user interacted with it)
                    foundUser.LastSignedIn = DateTime.Now;
                    _context.SignedInUsers.Update(foundUser);
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

        public static string ComputeHashValue(string value)
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

        public static double SecondsSinceEpoch()
        {
            TimeSpan diff = DateTime.Now.ToUniversalTime() - DateTime.UnixEpoch;
            return Math.Floor(diff.TotalSeconds);
        }
    }
}