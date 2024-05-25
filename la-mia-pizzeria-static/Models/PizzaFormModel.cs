using la_mia_pizzeria_static.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Hosting;

namespace la_mia_pizzeria_static.Models
{
    public class PizzaFormModel
    {
        public List<Category>? Categories { get; set; }
        public Pizza Pizza { get; set; }
        public List<SelectListItem>? Ingredients { get; set; } // Gli elementi degli ingredienti selezionabili per la select (analogo a Categories)
        public List<string>? SelectedIngredients { get; set; } // Gli ID degli elementi effettivamente selezionati

        public PizzaFormModel(List<Pizza> pizza) { }

        public PizzaFormModel(Pizza pizza, List<Category>? categories)
        {
            Pizza = pizza;
            Categories = categories;
            SelectedIngredients = new List<string>();
            if (Pizza.Ingredients != null)
                foreach (var i in Pizza.Ingredients)
                    SelectedIngredients.Add(i.Id.ToString());
        }

        public PizzaFormModel(List<Category> categories)
        {
            Categories = categories;
        }

        public PizzaFormModel(List<Pizza> pizza, List<Category> categories) : this(pizza)
        {
        }

        public void CreateIngredients()
        {
            this.Ingredients = new List<SelectListItem>();
            if (this.SelectedIngredients == null)
                this.SelectedIngredients = new List<string>();
            var ingredientsFromDB = PizzaManager.GetAllIngredients();
            foreach (var ingrendient in ingredientsFromDB) 
            {
                bool isSelected = this.SelectedIngredients.Contains(ingrendient.Id.ToString()); 
                this.Ingredients.Add(new SelectListItem() 
                {
                    Text = ingrendient.Name, 
                    Value = ingrendient.Id.ToString(), 
                    Selected = isSelected 
                });
                 
            }
        }
    }
}
