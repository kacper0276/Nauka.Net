using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {

        private readonly IWeatherForecastService _service;
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IWeatherForecastService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet("innaSciezka")] // /WeatherForecast/innaSciezka
        public IEnumerable<WeatherForecast> Get()
        {
            var result = _service.Get();
            return result;
        }

        [HttpGet]
        [Route("currentDay/{max}")] // /WeatherForecast/currentDay/15?take=10
        public IEnumerable<WeatherForecast> Get2([FromQuery]int take, [FromRoute]int max)
        {
            var result = _service.Get();
            return result;
        }

        [HttpPost] // Niesprecyzowana œcie¿ka to /WeatherForecast
        public ActionResult<string> Hello([FromBody]string text)
        {
            // HttpContext.Response.StatusCode = 401; // Jaki kod zwracamy <- pierwszy sposób
            //return $"Hello {text}";
            return StatusCode(401, $"Hello {text}"); // <- Drugi sposób
            // return NotFound($"Hello {text}"); <- Zwróci 404
        }

        [HttpPost("generate")]
        public ActionResult<IEnumerable<WeatherForecast>> Generate([FromQuery] int count, [FromBody] TemperatureRequest temperature)
        {
            if(count < 0 || temperature.Max < temperature.Min)
            {
                return BadRequest();
            }

            var result = _service.GetWithParams(count, temperature.Min, temperature.Max);
            return Ok(result);
        }
    }
}