using la_mia_pizzeria_static.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Linq;

namespace la_mia_pizzeria_static.Data
{
    public static class PizzaManager
    {
        public static int CountAllPizzas()
        {
            using (PizzaDbContext db = new PizzaDbContext())
            {
                return db.Pizzas.Count();
            }
        }

        public static List<Pizza> GetAllPizzas()
        {
            using (PizzaDbContext db = new PizzaDbContext())
            {
                return db.Pizzas.Include(p => p.Category).OrderByDescending(p => p.Id).ToList();
            }
        }

        public static Ingredient GetIngredient(int id)
        {
            using (var db = new PizzaDbContext())
            {
                return db.Ingredients.FirstOrDefault(i => i.Id == id);
            }
        }

        public static Pizza GetPizza(int id)
        {
            using (PizzaDbContext db = new PizzaDbContext())
            {
                return db.Pizzas
                         .Include(p => p.Category)
                         .Include(p => p.PizzaIngredients)
                             .ThenInclude(pi => pi.Ingredient)
                         .FirstOrDefault(p => p.Id == id);
            }
        }
        public static void InsertPizza(Pizza pizza, int[] selectedIngredients)
        {
            if (pizza == null)
            {
                // Gestisci il caso in cui pizza è null (ad esempio, solleva un'eccezione o esegui un'altra azione)
                throw new ArgumentNullException(nameof(pizza), "La pizza non può essere nulla.");
            }

            using (PizzaDbContext db = new PizzaDbContext())
            {
                // Aggiungi la pizza al contesto del database
                db.Pizzas.Add(pizza);

                // Aggiungi gli ingredienti selezionati alla pizza
                if (selectedIngredients != null)
                {
                    foreach (var ingredientId in selectedIngredients)
                    {
                        var ingredient = db.Ingredients.Find(ingredientId);
                        if (ingredient != null)
                        {
                            // Assicurati che pizza.PizzaIngredients non sia null
                            pizza.PizzaIngredients ??= new List<PizzaIngredient>();
                            pizza.PizzaIngredients.Add(new PizzaIngredient { IngredientId = ingredientId });
                        }
                    }
                }

                // Salva le modifiche nel database
                db.SaveChanges();
            }
        }



        public static void UpdatePizza(Pizza pizza, int[] selectedIngredients)
        {
            using (PizzaDbContext db = new PizzaDbContext())
            {
                var originalPizza = db.Pizzas
                                      .Include(p => p.PizzaIngredients)
                                      .SingleOrDefault(p => p.Id == pizza.Id);

                if (originalPizza == null)
                {
                    // Pizza non trovata, gestisci di conseguenza (ad esempio, restituisci o solleva un'eccezione)
                    return;
                }

                // Aggiorna le proprietà della pizza originale con quelle della pizza modificata
                originalPizza.Name = pizza.Name;
                originalPizza.Price = pizza.Price;
                originalPizza.Description = pizza.Description;
                originalPizza.CategoryId = pizza.CategoryId;

                // Rimuovi gli ingredienti esistenti associati alla pizza
                originalPizza.PizzaIngredients.Clear();

                // Aggiungi o aggiorna gli ingredienti selezionati
                if (selectedIngredients != null)
                {
                    foreach (var ingredientId in selectedIngredients)
                    {
                        var ingredient = db.Ingredients.Find(ingredientId);
                        if (ingredient != null)
                        {
                            originalPizza.PizzaIngredients.Add(new PizzaIngredient { IngredientId = ingredientId });
                        }
                    }
                }

                // Esegui l'aggiornamento nel contesto del database
                db.SaveChanges();
            }
        }


        public static void DeletePizza(int id)
        {
            using (PizzaDbContext db = new PizzaDbContext())
            {
                var pizza = db.Pizzas.FirstOrDefault(p => p.Id == id);
                if (pizza != null)
                {
                    db.Pizzas.Remove(pizza);
                    db.SaveChanges();
                }
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
        public static void SeedPizzaIngredient()
        {
            using (PizzaDbContext db = new PizzaDbContext())
            {
                if (db.PizzaIngredients.Count() == 0)
                {
                    var margherita = db.Pizzas.First(p => p.Name == "Margherita");
                    var marinara = db.Pizzas.First(p => p.Name == "Marinara");
                    var diavola = db.Pizzas.First(p => p.Name == "Diavola");

                    var farina = db.Ingredients.First(i => i.Name == "Farina 00");
                    var acqua = db.Ingredients.First(i => i.Name == "Acqua");
                    var sale = db.Ingredients.First(i => i.Name == "Sale");
                    var lievito = db.Ingredients.First(i => i.Name == "Lievito madre");
                    var mozzarella = db.Ingredients.First(i => i.Name == "Mozzarella");

                    db.PizzaIngredients.AddRange(
                        new PizzaIngredient { PizzaId = margherita.Id, IngredientId = farina.Id },
                        new PizzaIngredient { PizzaId = margherita.Id, IngredientId = acqua.Id },
                        new PizzaIngredient { PizzaId = margherita.Id, IngredientId = sale.Id },
                        new PizzaIngredient { PizzaId = margherita.Id, IngredientId = lievito.Id },
                        new PizzaIngredient { PizzaId = margherita.Id, IngredientId = mozzarella.Id },
                        new PizzaIngredient { PizzaId = marinara.Id, IngredientId = farina.Id },
                        new PizzaIngredient { PizzaId = marinara.Id, IngredientId = acqua.Id },
                        new PizzaIngredient { PizzaId = marinara.Id, IngredientId = sale.Id },
                        new PizzaIngredient { PizzaId = marinara.Id, IngredientId = lievito.Id },
                        new PizzaIngredient { PizzaId = diavola.Id, IngredientId = farina.Id },
                        new PizzaIngredient { PizzaId = diavola.Id, IngredientId = acqua.Id },
                        new PizzaIngredient { PizzaId = diavola.Id, IngredientId = sale.Id },
                        new PizzaIngredient { PizzaId = diavola.Id, IngredientId = lievito.Id },
                        new PizzaIngredient { PizzaId = diavola.Id, IngredientId = mozzarella.Id }
                    );

                    db.SaveChanges();
                }
            }
        }

    }
    
}
