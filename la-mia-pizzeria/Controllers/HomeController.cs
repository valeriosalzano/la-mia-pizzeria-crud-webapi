using la_mia_pizzeria.CustomLoggers;
using la_mia_pizzeria.Database;
using la_mia_pizzeria.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace la_mia_pizzeria_static.Controllers
{

    public class HomeController : Controller
    {
        private readonly ICustomLogger _logger;
        private readonly PizzeriaContext _database;

        public HomeController(ICustomLogger logger, PizzeriaContext db)
        {
            _logger = logger;
            _database = db;
        }

        public IActionResult Index()
        {
            try
            {
                //List<Pizza> pizzas = _database.Pizzas.ToList<Pizza>();
                return View("Index");
            }
            catch
            {
                _logger.WriteLog("Catching exception at Home>Index");
                return NotFound();
            }
        }

        public IActionResult PopulateDb()
        {
            try
            {
                DataSeeder.PopulateDb();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View("Index");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}