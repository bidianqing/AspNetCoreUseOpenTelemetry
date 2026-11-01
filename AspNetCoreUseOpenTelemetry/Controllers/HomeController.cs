using Microsoft.AspNetCore.Mvc;


namespace AspNetCoreUseOpenTelemetry.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
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
