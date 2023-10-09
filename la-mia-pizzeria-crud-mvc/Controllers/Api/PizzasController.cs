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

        [HttpGet("byId")]
        public IActionResult GetPizzasById(int? id)
        {
            if (id == null)
            {
                return BadRequest(new { Message = "Invalid id" });
            } else
            {

            Pizza? foundedPizza = _myDatabase.Pizzas.Where(pizza => pizza.Id == id).FirstOrDefault();

                if (foundedPizza != null && _myDatabase.Pizzas.Contains(foundedPizza))
                {
                    return Ok(foundedPizza);
                } else
                {
                    return BadRequest(new { Message = $"No pizzas with id: {id}" });
                }
            }
        }

        [HttpPost("create")]
        public IActionResult Create(Pizza newPizza)
        {
            try
            {
                _myDatabase.Pizzas.Add(newPizza);
                _myDatabase.SaveChanges();

                return Ok("Success");
            }
            catch
            {
                return BadRequest("Unable to add this pizza");
            }
        }
    }
}


