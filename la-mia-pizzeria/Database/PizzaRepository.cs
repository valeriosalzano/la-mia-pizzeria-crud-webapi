using la_mia_pizzeria.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace la_mia_pizzeria.Database
{
    public class PizzaRepository : EntityFrameworkRepository<Pizza>, IRepository<Pizza>
    {
        public PizzaRepository(PizzeriaContext context) : base(context)
        {
        }

        // GetAllContaining returns all pizzas containing "name" in Name property, returns an empty list if name is null or empty
        public List<Pizza> GetAllContaining(string? name)
        {
            List<Pizza> foundPizzas = new List<Pizza>();

            if (!string.IsNullOrEmpty(name))
                foundPizzas = (List<Pizza>)GetFilteredList(pizza => pizza.Name!.ToLower().Contains(name.ToLower()));

            return foundPizzas;
        }
    }
}
