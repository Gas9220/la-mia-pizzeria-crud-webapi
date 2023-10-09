using la_mia_pizzeria_crud_mvc.CustomLoggers;
using la_mia_pizzeria_crud_mvc.Database;
using la_mia_pizzeria_crud_mvc.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace la_mia_pizzeria_crud_mvc.Controllers
{
    public class HomeController : Controller
    {
        private ICustomLogger _myLogger;
        private PizzaContext _myDatabase;

        public HomeController(ICustomLogger _logger, PizzaContext myDatabase)
        {
            _myLogger = _logger;
            _myDatabase = myDatabase;
        }

        public IActionResult Index()
        {
            _myLogger.WriteLog("User visit index page", "READ");

            List<Pizza> pizzas = _myDatabase.Pizzas.ToList<Pizza>();

            return View("Index", pizzas);
        }

        public IActionResult Details(int id)
        {
            _myLogger.WriteLog($"User visit details page for id {id}", "READ");

            Pizza? foundedPizza = _myDatabase.Pizzas.Where(pizza => pizza.Id == id).FirstOrDefault();

            if (foundedPizza == null)
            {
                return NotFound($"Nessuna pizza trovata con l'id {id} ");
            }
            else
            {
                return View("Details", foundedPizza);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}