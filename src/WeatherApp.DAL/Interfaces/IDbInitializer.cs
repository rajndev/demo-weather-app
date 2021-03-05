using System.Threading.Tasks;

namespace WeatherApp.DAL.Interfaces
{
    public interface IDbInitializer
    {
        /* Applies any pending migrations for the context to the database.
         Will create the database if it does not already exist. */

        void Initialize();

        // add seed data to the database
        Task SeedData();
    }
}