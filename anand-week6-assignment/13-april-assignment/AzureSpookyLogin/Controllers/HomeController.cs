using AzureSpookyLogin.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace AzureSpookyLogin.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(SpookyRequest spookyrequest)
        {
            if (!ModelState.IsValid)
            {
                return View(spookyrequest);
            }

            var logicAppUrl = _configuration["LogicApp:RequestUrl"];
            if (string.IsNullOrWhiteSpace(logicAppUrl))
            {
                ModelState.AddModelError(string.Empty, "Logic App URL is missing in configuration.");
                return View(spookyrequest);
            }

            spookyrequest.Id = Guid.NewGuid().ToString();

            var payload = new
            {
                id = spookyrequest.Id,
                name = spookyrequest.Name,
                email = spookyrequest.Email,
                phone = spookyrequest.Phone
            };

            using var client = _httpClientFactory.CreateClient();
            var json = JsonConvert.SerializeObject(payload);
            using var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await client.PostAsync(logicAppUrl, content);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Logic App call failed with status code: {StatusCode}", response.StatusCode);
                ModelState.AddModelError(string.Empty, "Could not submit request. Please try again.");
                return View(spookyrequest);
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
