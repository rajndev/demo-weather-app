namespace WeatherApp.Data.Provider.Interfaces
{
    public interface IDbInitializer
    {
        void Initialize();
        Task SeedData();
    }
}