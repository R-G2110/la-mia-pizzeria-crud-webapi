﻿using la_mia_pizzeria_static.Models;
using la_mia_pizzeria_static.Data;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;

namespace LaMiaPizzeria.Controllers
{
    [Authorize]
    public class PizzaController : Controller
    {
        private readonly ILogger<PizzaController> _logger;

        public PizzaController(ILogger<PizzaController> logger)
        {
            _logger = logger;
        }

        
        public IActionResult Index()
        {
            return View("Index");
        }

        [HttpGet]
        [Authorize(Roles = "USER,ADMIN")]
        public IActionResult GetAllPizzas()
        {
            return View(PizzaManager.GetAllPizzas());
        }

        [HttpGet]
        [Authorize(Roles = "USER,ADMIN")]
        public IActionResult GetPizza(int id)
        {
            try
            {
                var pizza = PizzaManager.GetPizza(id);
                if (pizza != null)
                    return View(pizza);
                else
                    //return NotFound();
                    return View("Errore", new ErrorViewModel($"La pizza {id} non è stata trovata!"));
            }
            catch (Exception e)
            {
                return View("Errore", new ErrorViewModel(e.Message));
                //return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public IActionResult CreatePizza() // Restituisce la form per la creazione di pizze
        {
            
            List<Category> categories = PizzaManager.GetAllCategories();
            PizzaFormModel model = new PizzaFormModel( categories);
            model.CreateIngredients();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMIN")]
        public IActionResult CreatePizza(PizzaFormModel pizzaDaInserire)
        {
            if (ModelState.IsValid == false)
            {
                // Ritorno la form di prima con i dati della pizza
                // precompilati dall'utente
                pizzaDaInserire.Categories = PizzaManager.GetAllCategories();
                pizzaDaInserire.CreateIngredients();
                return View("CreatePizza", pizzaDaInserire);
            }

            PizzaManager.InsertPizza(pizzaDaInserire.Pizza, pizzaDaInserire.SelectedIngredients);
            // Richiamiamo la action Index affinché vengano mostrate tutte le pizze
            // inclusa quella nuova
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        // Mi serve l'ID della pizza per:
        // 1) Indicare alla view QUALE pizza devo modificare
        // 2) Popolare la form della view coi dati della pizza che sto per modificare
        public IActionResult UpdatePizza(int id) // Restituisce la form per l'update di una pizza
        {
            var pizza = PizzaManager.GetPizza(id);
            if (pizza == null)
                return NotFound();
            PizzaFormModel model = new PizzaFormModel(pizza, PizzaManager.GetAllCategories());
            model.CreateIngredients();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMIN")]
        public IActionResult UpdatePizza(int id, PizzaFormModel pizzaDaModificare) // Restituisce la form per la creazione di pizze
        {
            if (ModelState.IsValid == false)
            {
                // Ritorno la form di prima con i dati della pizza
                // precompilati dall'utente
                pizzaDaModificare.Categories = PizzaManager.GetAllCategories();
                pizzaDaModificare.CreateIngredients();
                return View("UpdatePizza", pizzaDaModificare);
            }

            var modified = PizzaManager.UpdatePizza(id, pizzaDaModificare.Pizza, pizzaDaModificare.SelectedIngredients);
            if (modified)
            {
                // Richiamiamo la action Index affinché vengano mostrate tutte le pizze
                return RedirectToAction("Index");
            }
            else
                return NotFound();

            /*
            var state = PizzaManager.UpdatePizzaWithEnum(id, pizzaDaModificare);
            switch (state)
            {
                case ResultType.OK:
                    return RedirectToAction("Index");
                    break;
                case ResultType.NotFound:
                    return NotFound();
                    break;
                case ResultType.Exception:
                    return NotFound();
                    break;
            }
            */
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMIN")]
        public IActionResult DeletePizza(int id)
        {
            var deleted = PizzaManager.DeletePizza(id);
            if (deleted)
            {
                // Richiamiamo la action Index affinché vengano mostrate tutte le pizze
                return RedirectToAction("Index");
            }
            else
                return NotFound();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
