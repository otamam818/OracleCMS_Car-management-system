// Summary: The login information used for car dealers to access their private
//          data
namespace CarDeals.Models
{
    internal static class Utils
    {

        public static double SecondsSinceEpoch()
        {
            TimeSpan diff = DateTime.Now.ToUniversalTime() - DateTime.UnixEpoch;
            return Math.Floor(diff.TotalSeconds);
        }
    }
}