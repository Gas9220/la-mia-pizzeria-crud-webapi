using la_mia_pizzeria_crud_mvc.Database;
using la_mia_pizzeria_crud_mvc.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            }
            else
            {

                Pizza? foundedPizza = _myDatabase.Pizzas.Where(pizza => pizza.Id == id).FirstOrDefault();

                if (foundedPizza != null && _myDatabase.Pizzas.Contains(foundedPizza))
                {
                    return Ok(foundedPizza);
                }
                else
                {
                    return BadRequest(new { Message = $"No pizzas with id: {id}" });
                }
            }
        }

        [HttpPost("create")]
        public IActionResult CreatePizza(Pizza newPizza)
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

        [HttpPut("update/{id}")]
        public IActionResult UpdatePizza(int id, Pizza updatedPizzaData)
        {
            Pizza? pizzaToUpdate = _myDatabase.Pizzas.Where(pizza => pizza.Id == id).Include(pizza => pizza.Ingredients).FirstOrDefault();

            if (pizzaToUpdate != null)
            {

                pizzaToUpdate.Name = updatedPizzaData.Name;
                pizzaToUpdate.Description = updatedPizzaData.Description;
                pizzaToUpdate.PhotoUrl = updatedPizzaData.PhotoUrl;
                pizzaToUpdate.Price = updatedPizzaData.Price;
                pizzaToUpdate.CategoryId = updatedPizzaData.CategoryId;


                if (updatedPizzaData.Ingredients != null)
                {
                    if (pizzaToUpdate.Ingredients != null)
                    {
                        pizzaToUpdate.Ingredients.Clear();
                    }
                    else
                    {
                        pizzaToUpdate.Ingredients = new List<Ingredient>();
                    }

                    foreach(Ingredient ingredient in updatedPizzaData.Ingredients)
                    {
                        pizzaToUpdate.Ingredients.Add(ingredient);
                    }
                }

                _myDatabase.SaveChanges();

                return Ok("Success");
            }
            else
            {
                return BadRequest("Unable to update this pizza");
            }
        }
    }
}


