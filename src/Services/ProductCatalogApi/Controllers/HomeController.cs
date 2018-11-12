using Microsoft.AspNetCore.Mvc;

namespace ProductCatalogApi.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return new RedirectResult("~/swagger");
        }
    }
}