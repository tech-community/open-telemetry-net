﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace demo1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly ActivitySource Activity = new(nameof(WeatherForecastController));
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
            _logger.LogCritical("Crtico");
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            using (var activity = Activity.StartActivity("RabbitMq Publish", ActivityKind.Producer))
            {
                _logger.LogInformation($"Logging current activity:");
                _logger.LogTrace("You call sql save message endpoint");
                var iteracion = 4;

                _logger.LogDebug($"Debug {iteracion}");
                _logger.LogInformation($"Information {iteracion}");
                _logger.LogWarning($"Warning {iteracion}");
                _logger.LogError($"Error {iteracion}");
                _logger.LogCritical($"Critical {iteracion}");
                
                try
                {
                    throw new NotImplementedException();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
            };
            
            

            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("lost")]
        public IActionResult GetWithError()
        {
            throw new Exception("Test");
            return BadRequest("Error en API");
        }
    }
}
