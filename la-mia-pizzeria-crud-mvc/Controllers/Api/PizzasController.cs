using la_mia_pizzeria_crud_mvc.Database;
using la_mia_pizzeria_crud_mvc.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace la_mia_pizzeria_crud_mvc.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class PizzasController : ControllerBase
    {
        private PizzaContext _myDatabase;

        public PizzasController(PizzaContext myDatabase)
        {
            _myDatabase = myDatabase;
        }

        [HttpGet("all")]
        public IActionResult GetAllPizzas()
        {
            IQueryable<Pizza> pizzas = _myDatabase.Pizzas;
            return Ok(pizzas.ToList());
        }

        [HttpGet("byName")]
        public IActionResult GetPizzasByName(string? searchText)
        {
            if (searchText == null)
            {
                return BadRequest(new { Message = "Invalid search text" });
            }

            List<Pizza> foundedPizzas = _myDatabase.Pizzas.Where(pizza => pizza.Name.ToLower().Contains(searchText.ToLower())).ToList();

            return Ok(foundedPizzas);
        }
    }
}
