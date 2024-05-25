using la_mia_pizzeria_static.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Linq;

namespace la_mia_pizzeria_static.Data
{
    public enum ResultType
    {
        OK,
        Exception,
        NotFound
    }
    public static class PizzaManager
    {
        public static int CountAllPizzas()
        {
            using PizzaDbContext db = new PizzaDbContext();
            return db.Pizzas.Count();
        }

        public static List<Pizza> GetAllPizzas()
        {
            using PizzaDbContext db = new PizzaDbContext();
            return db.Pizzas.ToList();
        }

        public static List<Pizza> GetPizza(int id, bool includeReferences = true)
        {
            using PizzaDbContext db = new PizzaDbContext();
            if (includeReferences)
                return db.Pizzas.Where(x => x.Id == id).Include(p => p.Category).Include(p => p.Ingredients).ToList();
            return new List<Pizza> { db.Pizzas.FirstOrDefault(p => p.Id == id) };
        }


        public static Pizza GetPizzaByName(string name)
        {
            using PizzaDbContext db = new PizzaDbContext();
            return db.Pizzas.FirstOrDefault(p => p.Name == name);
        }

        public static List<Category> GetAllCategories()
        {
            using PizzaDbContext db = new PizzaDbContext();
            return db.Categories.ToList();
        }
        public static List<Ingredient> GetAllIngredients()
        {
            using PizzaDbContext db = new PizzaDbContext();
            return db.Ingredients.ToList();
        }


        public static void InsertPizza(Pizza pizza, List<string> selectedIngredients)
        {
            using PizzaDbContext db = new PizzaDbContext();
            pizza.Ingredients = new List<Ingredient>();
            if (selectedIngredients != null)
            {
                // Trasformiamo gli ID scelti in ingredienti da aggiungere tra i riferimenti in Pizza
                foreach (var ingredient in selectedIngredients)
                {
                    int id = int.Parse(ingredient);
                    // NON usiamo un GetIngredientById() perché userebbe un db context diverso
                    // e ciò causerebbe errore in fase di salvataggio - usiamo lo stesso context all'interno della stessa operazione
                    var ingredientFromDb = db.Ingredients.FirstOrDefault(x => x.Id == id);
                    if (ingredientFromDb != null)
                    {
                        pizza.Ingredients.Add(ingredientFromDb);
                    }
                }
            }
            db.Pizzas.Add(pizza);
            db.SaveChanges();
        }

        public static bool UpdatePizza(int id, Pizza pizza, List<string> selectedIngredients)
        {
            try
            {
                // Non posso riusare GetPizza()
                // perché il DbContext deve continuare a vivere
                // affinché possa accorgersi di quali modifiche deve salvare
                using PizzaDbContext db = new PizzaDbContext();
                var pizzaDaModificare = db.Pizzas.Where(p => p.Id == id).Include(p => p.Ingredients).FirstOrDefault();
                if (pizzaDaModificare == null)
                    return false;
                pizzaDaModificare.Name = pizza.Name;
                pizzaDaModificare.Description = pizza.Description;
                pizzaDaModificare.Price = pizza.Price;
                pizzaDaModificare.CategoryId = pizza.CategoryId;

                // Prima svuoto così da salvare solo le informazioni che l'utente ha scelto, NON le aggiungiamo ai vecchi dati
                pizzaDaModificare.Ingredients.Clear();
                if (selectedIngredients != null)
                {
                    foreach (var ingredient in selectedIngredients)
                    {
                        int ingredientId = int.Parse(ingredient);
                        var ingredientFromDb = db.Ingredients.FirstOrDefault(x => x.Id == ingredientId);
                        if (ingredientFromDb != null)
                            pizzaDaModificare.Ingredients.Add(ingredientFromDb);
                    }
                }

                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static ResultType UpdatePizzaWithEnum(int id, Pizza pizza) // solo a scopo didattico
        {
            try
            {
                using PizzaDbContext db = new PizzaDbContext();
                var pizzaDaModificare = db.Pizzas.FirstOrDefault(p => p.Id == id);
                if (pizzaDaModificare == null)
                    return ResultType.NotFound;
                pizzaDaModificare.Name = pizza.Name;
                pizzaDaModificare.Description = pizza.Description;
                pizzaDaModificare.Price = pizza.Price;

                db.SaveChanges();
                return ResultType.OK;
            }
            catch (Exception ex)
            {
                return ResultType.Exception;
            }
        }

        public static bool DeletePizza(int id)
        {
            try
            {
                using (PizzaDbContext db = new PizzaDbContext())
                {
                    var pizzaDaCancellare = db.Pizzas.FirstOrDefault(p => p.Id == id);
                    if (pizzaDaCancellare == null)
                        return false;

                    db.Remove(pizzaDaCancellare);
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public static void SeedPizza()
        {
            using (PizzaDbContext db = new PizzaDbContext())
            {
                if (db.Pizzas.Count() == 0)
                {
                    db.Pizzas.AddRange(
                        new Pizza("Margherita", "Una pizza classica con mozzarella, pomodoro, basilico", "~/img/margherita.jpg", 5.99m, 1),
                        new Pizza("Marinara", "Semplice e deliziosa, con pomodoro, aglio, origano e olio extravergine di oliva.", "~/img/marinara.jpg", 4.99m, 1),
                        new Pizza("Diavola", "Una pizza piccante con salame piccante, peperoncino e mozzarella.", "~/img/diavola.jpg", 6.49m, 1),
                        new Pizza("Mortadella, Stracchino e Pesto", "Una combinazione di mortadella, stracchino e pesto", "~/img/mortadella&stracchino.jpg", 7.99m, 2),
                        new Pizza("Capricciosa", "Una pizza classica con prosciutto cotto, funghi, carciofi, olive e mozzarella.", "~/img/capricciosa.jpg", 8.99m, 1),
                        new Pizza("Salsiccia e Friarielli", "Una pizza tipica della tradizione napoletana, con salsiccia e friarielli, condita con mozzarella di bufala.", "~/img/salsiccia&friarielli.jpg", 8.99m, 2)
                    );
                    db.SaveChanges();
                }
            }
        }

        public static void SeedCategory()
        {
            using (PizzaDbContext db = new PizzaDbContext())
            {
                if (db.Categories.Count() == 0)
                {
                    db.Categories.AddRange(
                        new Category("Pizze classiche"),
                        new Category("Pizze bianche"),
                        new Category("Pizze vegetariane"),
                        new Category("Pizze di mare"),
                        new Category("Pizze speciali")
                    );
                    db.SaveChanges();
                }
            }
        }
        public static void SeedIngredient()
        {
            using (PizzaDbContext db = new PizzaDbContext())
            {
                if (db.Ingredients.Count() == 0)
                {
                    db.Ingredients.AddRange(
                        new Ingredient("Farina 00"),
                        new Ingredient("Acqua"),
                        new Ingredient("Sale"),
                        new Ingredient("Lievito madre"),
                        new Ingredient("Pomodoro"),
                        new Ingredient("Mozzarella"),
                        new Ingredient("Olio EVO"),
                        new Ingredient("Basilico"),
                        new Ingredient("Aglio"),
                        new Ingredient("Origano"),
                        new Ingredient("Salame piccante"),
                        new Ingredient("Peperoncino"),
                        new Ingredient("Mortadella"),
                        new Ingredient("Stracchino"),
                        new Ingredient("Pesto"),
                        new Ingredient("Carciofi"),
                        new Ingredient("Prosciutto cotto"),
                        new Ingredient("Funghi"),
                        new Ingredient("Olive"),
                        new Ingredient("Salsiccia"),
                        new Ingredient("Friarielli"),
                        new Ingredient("Mozzarella di bufala")
                    );
                    db.SaveChanges();
                }
            }
        }
        

    }
    
}
