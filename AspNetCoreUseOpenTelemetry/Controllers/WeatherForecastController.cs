using AspNetCoreUseOpenTelemetry.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;


namespace AspNetCoreUseOpenTelemetry.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IDatabase _redis;

        public WeatherForecastController(ILogger<WeatherForecastController> logger,
            IHttpClientFactory httpClientFactory,
            ConnectionMultiplexer connectionMultiplexer)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _redis = connectionMultiplexer.GetDatabase();
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost]
        public async Task<Person> Post([FromBody] Person person)
        {
            var httpClient = _httpClientFactory.CreateClient();
            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync("http://localhost:5001/weatherforecast");
            var content = await httpResponseMessage.Content.ReadAsStringAsync();

            await _redis.StringSetAsync("name", "bidianqing");

            return person;
        }
    }
}
