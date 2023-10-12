using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using TempSignalRServer.Models;

namespace TempSignalRServer.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class WeatherForecastController : ControllerBase
	{
		private readonly ILogger<WeatherForecastController> _logger;

		private static readonly List<WeatherForecast> _forecasts = new List<WeatherForecast>();
		public WeatherForecastController(ILogger<WeatherForecastController> logger)
		{
			_logger = logger;
		}

		[HttpGet(Name = "GetWeatherForecast")]
		public IEnumerable<WeatherForecast> Get()
		{
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
			try
			{
				if (forecast == null)
				{
					_logger.LogError("Received null forecast data.");
					return BadRequest("Provided forecast data is null.");
				}
				// Lägger till den mottagna prognosen till in-memory listan.
				_forecasts.Add(forecast);

				// Här kan du lägga till koden för att spara din väderprognos, t.ex. i en databas.

				return Ok("Väderprognos har lagts till");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while adding weather forecast.");
				return StatusCode(500, "Internal server error occurred.");
			}
		}
	}
}