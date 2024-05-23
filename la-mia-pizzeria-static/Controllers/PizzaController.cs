using la_mia_pizzeria_static.Models;
using la_mia_pizzeria_static.Data;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace LaMiaPizzeria.Controllers
{
    [Authorize(Roles = "ADMIN,USER")]
    public class PizzaController : Controller
    {
        public IActionResult Index()
        {
            return View(PizzaManager.GetAllPizzas());
        }

        [HttpPost]
        public IActionResult Search(string searchString)
        {
            var pizzas = PizzaManager.GetAllPizzas();

            if (!string.IsNullOrEmpty(searchString))
            {
                pizzas = pizzas.Where(p => p.Name.ToLower().Contains(searchString.ToLower()) || p.Description.ToLower().Contains(searchString.ToLower())).ToList();
            }

            if (!pizzas.Any())
            {
                ViewData["SearchMessage"] = $"Nessuna pizza '{searchString}' è stata trovata.";
            }

            return View("Index", pizzas);
        }

        public IActionResult GetPizza(int id)
        {
            var pizza = PizzaManager.GetPizza(id);
            if (pizza != null)
                return View(pizza);
            else
                return View("errore");
        }

        [Authorize(Roles = "ADMIN")]
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(GetCategories(), "Id", "Name");
            ViewBag.Ingredients = GetIngredients();
            return View("PizzaForm", new Pizza());
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Pizza pizza, int[] selectedIngredients)
        {
            if (ModelState.IsValid)
            {
                // Chiamata al metodo InsertPizza con l'array selectedIngredients
                PizzaManager.InsertPizza(pizza, selectedIngredients);
                TempData["SuccessMessage"] = $"La pizza {pizza.Name} è stata creata con successo!";
                return RedirectToAction("Index");
            }

            ViewBag.Categories = new SelectList(GetCategories(), "Id", "Name");
            ViewBag.Ingredients = GetIngredients();

            // Aggiungi gli ingredienti selezionati al modello
            if (selectedIngredients != null)
            {
                pizza.PizzaIngredients = selectedIngredients.Select(id => new PizzaIngredient { IngredientId = id }).ToList();
            }

            return View("PizzaForm", pizza);
        }

        [Authorize(Roles = "ADMIN")]
        public IActionResult Edit(int id)
        {
            var pizza = PizzaManager.GetPizza(id);
            if (pizza != null)
            {
                ViewBag.Categories = new SelectList(GetCategories(), "Id", "Name", pizza.CategoryId);
                ViewBag.Ingredients = GetIngredients();
                return View("PizzaForm", pizza);
            }
            else
            {
                return View("errore");
            }
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Pizza pizza, int[] selectedIngredients)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(GetCategories(), "Id", "Name", pizza.CategoryId);
                ViewBag.Ingredients = GetIngredients();

                // Aggiungi gli ingredienti selezionati al modello
                if (selectedIngredients != null)
                {
                    pizza.PizzaIngredients = selectedIngredients.Select(id => new PizzaIngredient { IngredientId = id }).ToList();
                }

                return View("PizzaForm", pizza);
            }

            // Chiamata al metodo UpdatePizza con l'array selectedIngredients
            PizzaManager.UpdatePizza(pizza, selectedIngredients);

            TempData["SuccessMessage"] = $"La pizza {pizza.Name} è stata modificata con successo!";
            return RedirectToAction("Index");
        }

        private List<Ingredient> GetIngredients()
        {
            using (var db = new PizzaDbContext())
            {
                return db.Ingredients.ToList();
            }
        }

        [Authorize]
        public IActionResult Delete(int id)
        {
            var pizza = PizzaManager.GetPizza(id);
            if (pizza == null)
            {
                TempData["ErrorMessage"] = $"La pizza {pizza?.Name} non è stata trovata!";
                return RedirectToAction("Index");
            }

            PizzaManager.DeletePizza(id);
            TempData["SuccessMessage"] = $"La pizza {pizza.Name} è stata eliminata con successo!";
            return RedirectToAction("Index");
        }

        private List<Category> GetCategories()
        {
            using (var db = new PizzaDbContext())
            {
                return db.Categories.ToList();
            }
        }
    }
}
