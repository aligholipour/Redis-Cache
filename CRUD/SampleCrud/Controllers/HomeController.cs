using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using SampleCrud.Models;
using System.Diagnostics;
using System.Text.Json;

namespace SampleCrud.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDistributedCache _cache;
        public HomeController(ILogger<HomeController> logger, IDistributedCache cache)
        {
            _logger = logger;
            _cache = cache;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> CreateProduct()
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1),
                SlidingExpiration = TimeSpan.FromMinutes(10),
            };

            var product = new ProductModel { Id = 1, Name = "Laptop", Price = 100, Quantity = 1 };

            await _cache.SetStringAsync("Product_" + product.Id, JsonSerializer.Serialize(product), options);

            return Ok();
        }
        public async Task<IActionResult> GetProduct()
        {
            var productId = 1;

            var result = await _cache.GetStringAsync("Product_" + productId);

            var product = JsonSerializer.Deserialize<ProductModel>(result);

            return Ok();
        }

        public async Task<IActionResult> DeleteProduct()
        {
            var productId = 1;

            await _cache.RemoveAsync("Product_" + productId);

            return Ok();
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