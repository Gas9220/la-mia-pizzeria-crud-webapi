using la_mia_pizzeria_crud_mvc.Database;
using la_mia_pizzeria_crud_mvc.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace la_mia_pizzeria_crud_mvc.Controllers.Api
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PizzasController : ControllerBase
    {
        private PizzaContext _myDatabase;

        public PizzasController(PizzaContext myDatabase)
        {
            _myDatabase = myDatabase;
        }

        public IActionResult GetAllPizzas()
        {
            IQueryable<Pizza> pizzas = _myDatabase.Pizzas;
            return Ok(pizzas.ToList());
        }
    }
}
