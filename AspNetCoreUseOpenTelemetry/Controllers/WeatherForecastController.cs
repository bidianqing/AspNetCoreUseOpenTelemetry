using AspNetCoreUseOpenTelemetry.Models;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
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
        private readonly IConfiguration _configuration;

        public WeatherForecastController(ILogger<WeatherForecastController> logger,
            IHttpClientFactory httpClientFactory,
            ConnectionMultiplexer connectionMultiplexer,
            IConfiguration configuration)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _redis = connectionMultiplexer.GetDatabase();
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            string requestUri = "http://localhost:5002/weatherforecast";

            var httpClient = _httpClientFactory.CreateClient();
            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(requestUri);

            var connection = new MySqlConnection(_configuration.GetConnectionString("MySqlConnectionString"));
            var user = new User
            {
                Name = "bidianqing"
            };
            await connection.InsertAsync(user);

            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
