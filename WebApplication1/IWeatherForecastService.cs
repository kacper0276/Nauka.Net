namespace WebApplication1
{
    public interface IWeatherForecastService
    {
        IEnumerable<WeatherForecast> Get();
    }
}