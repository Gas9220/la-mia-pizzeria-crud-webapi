using Azure;
using la_mia_pizzeria_crud_mvc.CustomLoggers;
using la_mia_pizzeria_crud_mvc.Database;
using la_mia_pizzeria_crud_mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace la_mia_pizzeria_crud_mvc.Controllers
{
    [Authorize(Roles = "ADMIN, USER")]
    public class PizzaController : Controller
    {
        private ICustomLogger _myLogger;
        private PizzaContext _myDatabase;

        public PizzaController(ICustomLogger _logger, PizzaContext myDatabase)
        {
            _myLogger = _logger;
            _myDatabase = myDatabase;
        }

        public IActionResult Index()
        {
            _myLogger.WriteLog("Admin visit index page", "READ");

            List<Pizza> pizzas = _myDatabase.Pizzas.ToList<Pizza>();

            return View("Index", pizzas);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Details(int id)
        {
            _myLogger.WriteLog($"Admin visit details page for {id}", "READ");

            Pizza? foundedPizza = _myDatabase.Pizzas.Where(pizza => pizza.Id == id).Include(pizza => pizza.Category).Include(pizza => pizza.Ingredients).FirstOrDefault();

            if (foundedPizza == null)
            {
                return NotFound($"No pizza found with id: {id} ");
            }
            else
            {
                return View("Details", foundedPizza);
            }
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PizzaFormModel data)
        {
            _myLogger.WriteLog("Admin create new pizza", "CREATE");

            if (!ModelState.IsValid)
            {
                List<Category> categories = _myDatabase.Categories.ToList();
                data.Categories = categories;

                List<SelectListItem> ingredientsList = new List<SelectListItem>();
                List<Ingredient> databaseIngredients = _myDatabase.Ingredients.ToList();

                foreach (Ingredient ingredient in databaseIngredients)
                {
                    ingredientsList.Add(new SelectListItem()
                    {
                        Text = ingredient.Name,
                        Value = ingredient.Id.ToString()
                    });
                }

                data.Ingredients = ingredientsList;
                return View("Create", data);
            }

            Pizza newPizza = new Pizza();

            newPizza.Name = data.Pizza.Name;
            newPizza.Description = data.Pizza.Description;
            newPizza.Price = data.Pizza.Price;
            newPizza.PhotoUrl = data.Pizza.PhotoUrl;
            newPizza.Ingredients = new List<Ingredient>();
            newPizza.CategoryId = data.Pizza.CategoryId;

            if (data.SelectedIngredientsId != null)
            {
                foreach (string selectedIngredientId in data.SelectedIngredientsId)
                {
                    int selectedIntIngredientId = int.Parse(selectedIngredientId);
                    Ingredient? ingredient = _myDatabase.Ingredients.Where(ingredient => ingredient.Id == selectedIntIngredientId).FirstOrDefault();

                    if (ingredient != null)
                    {
                        newPizza.Ingredients.Add(ingredient);
                    }
                }
            }

            _myDatabase.Pizzas.Add(newPizza);
            _myDatabase.SaveChanges();

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public IActionResult Create()
        {
            _myLogger.WriteLog("Admin visit create new pizza page", "CREATE");

            List<Category> categories = _myDatabase.Categories.ToList();
            List<Ingredient> ingredients = _myDatabase.Ingredients.ToList();
            List<SelectListItem> ingredientsList = new List<SelectListItem>();

            foreach (Ingredient ingredient in ingredients)
            {
                ingredientsList.Add(new SelectListItem()
                {
                    Text = ingredient.Name,
                    Value = ingredient.Id.ToString()
                });
            }

            PizzaFormModel model = new PizzaFormModel();
            model.Pizza = new Pizza();
            model.Categories = categories;
            model.Ingredients = ingredientsList;

            return View("Create", model);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            _myLogger.WriteLog("Admin visit edit new pizza page", "EDIT");

            Pizza? pizzaToEdit = _myDatabase.Pizzas.Where(pizza => pizza.Id == id).Include(pizza => pizza.Ingredients).FirstOrDefault();

            if (pizzaToEdit == null)
            {
                return NotFound();
            }
            else
            {
                List<Category> categories = _myDatabase.Categories.ToList();

                List<Ingredient> ingredientsList = _myDatabase.Ingredients.ToList();
                List<SelectListItem> selectListIngredients = new List<SelectListItem>();

                foreach (Ingredient ingredient in ingredientsList)
                {
                    selectListIngredients.Add(new SelectListItem
                    {
                        Value = ingredient.Id.ToString(),
                        Text = ingredient.Name,
                        Selected = pizzaToEdit.Ingredients.Any(tagId => tagId.Id == ingredient.Id)
                    });

                }

                PizzaFormModel model
                    = new PizzaFormModel
                    {
                        Pizza = pizzaToEdit,
                        Categories = categories,
                        Ingredients = selectListIngredients
                    };

                return View("Edit", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, PizzaFormModel data)
        {
            _myLogger.WriteLog($"Admin edit pizza with {id}", "EDIT");

            if (!ModelState.IsValid)
            {
                List<Category> categories = _myDatabase.Categories.ToList();
                data.Categories = categories;

                List<Ingredient> databaseIngredients = _myDatabase.Ingredients.ToList();
                List<SelectListItem> ingredientsList = new List<SelectListItem>();

                foreach (Ingredient ingredient in databaseIngredients)
                {
                    ingredientsList.Add(new SelectListItem()
                    {
                        Text = ingredient.Name,
                        Value = ingredient.Id.ToString()
                    });
                }

                data.Ingredients = ingredientsList;
                return View("Edit", data);
            }

            Pizza? pizzaToEdit = _myDatabase.Pizzas.Where(pizza => pizza.Id == id).Include(pizza => pizza.Ingredients).FirstOrDefault();

            if (pizzaToEdit != null)
            {
                pizzaToEdit.Ingredients.Clear();

                pizzaToEdit.Name = data.Pizza.Name;
                pizzaToEdit.Description = data.Pizza.Description;
                pizzaToEdit.Price = data.Pizza.Price;
                pizzaToEdit.PhotoUrl = data.Pizza.PhotoUrl;
                pizzaToEdit.CategoryId = data.Pizza.CategoryId;

                if (data.SelectedIngredientsId != null)
                {
                    foreach (string selectedIngredientId in data.SelectedIngredientsId)
                    {
                        int intSelectedIngredientId = int.Parse(selectedIngredientId);

                        Ingredient? IngredientInDb = _myDatabase.Ingredients.Where(Ingredient => Ingredient.Id == intSelectedIngredientId).FirstOrDefault();

                        if (IngredientInDb != null)
                        {
                            pizzaToEdit.Ingredients.Add(IngredientInDb);
                        }
                    }
                }

                _myDatabase.SaveChanges();

                return RedirectToAction("Details", "Pizza", new { id = pizzaToEdit.Id });
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            _myLogger.WriteLog($"Admin delete pizza with {id}", "DELETE");

            Pizza? pizzaToDelete = _myDatabase.Pizzas.Where(pizza => pizza.Id == id).FirstOrDefault();

            if (pizzaToDelete != null)
            {
                _myDatabase.Pizzas.Remove(pizzaToDelete);
                _myDatabase.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return NotFound();
            }
        }
    }
}
