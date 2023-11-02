namespace WebApplication1
{
    public interface IWeatherForecastService
    {
        IEnumerable<WeatherForecast> GetWithParams(int count, int min, int max);

        IEnumerable<WeatherForecast> Get();
    }
}