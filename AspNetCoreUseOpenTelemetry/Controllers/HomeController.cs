using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Net.Http;
using System.Threading.Tasks;


namespace AspNetCoreUseOpenTelemetry.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IDatabase _redis;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger,
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
        public async Task Get()
        {
            _logger.LogInformation("Test Logging");

            string requestUri = "https://www.baidu.com";

            var httpClient = _httpClientFactory.CreateClient();
            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(requestUri);

            //var connection = new MySqlConnection(_configuration.GetConnectionString("MySqlConnectionString"));
            //var user = new User
            //{
            //    Name = "bidianqing"
            //};
            //await connection.InsertAsync(user);
        }
    }
}
