using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TempServer.Models;

namespace TempServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            // Generera och returnera några slumpmässiga väderprognoser här
            var rng = new Random();
            var forecasts = new List<WeatherForecast>();
            for (var i = 1; i <= 5; i++)
            {
                var forecast = new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(i),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = "Sunny",
                };
                forecasts.Add(forecast);
            }
            return forecasts;
        }

        [HttpPost(Name = "AddWeatherForecast")]
        public IActionResult Post(WeatherForecast forecast)
        {
            return Ok("Väderprognos har lagts till");
        }
    }
}
