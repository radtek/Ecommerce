using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortalEducacional.Models;
using System.Diagnostics;

namespace PortalEducacional.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {       
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "A sua página de descrição da aplicação.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Sua página de contato.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
