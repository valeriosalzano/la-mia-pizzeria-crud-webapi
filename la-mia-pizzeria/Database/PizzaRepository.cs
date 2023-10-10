using la_mia_pizzeria.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace la_mia_pizzeria.Database
{
    public class PizzaRepository : EntityFrameworkRepository<Pizza>
    {
        public PizzaRepository(DbContext context) : base(context)
        {
        }

        // GetAllContaining returns all pizzas containing "name" in Name property, returns an empty list if name is null or empty
        public List<Pizza> GetAllContaining(string? name, bool includeEverything = false)
        {
            List<Pizza> foundPizzas = new List<Pizza>();

            if (!string.IsNullOrEmpty(name))
            {
                if (includeEverything)
                    return _dbSet
                        .Where(pizza => pizza.Name!.ToLower().Contains(name.ToLower()))
                        .Include(pizza => pizza.Category)
                        .Include(pizza => pizza.Ingredients)
                        .ToList();
                else
                    return (List<Pizza>) base.GetFilteredList(pizza => pizza.Name!.ToLower().Contains(name.ToLower()));
            }

            return foundPizzas;
        }

        public Pizza? GetById(int id, bool includeEverything)
        {
            if (includeEverything)
                return base._dbSet
                    .Include(pizza => pizza.Category)
                    .Include(pizza => pizza.Ingredients)
                    .Where(pizza => pizza.PizzaId == id)
                    .First();
            else
                return base.GetById(id);
        }

        public IEnumerable<Pizza> GetFilteredList(Func<Pizza, bool> filter, bool includeEverything)
        {
            if(includeEverything)
                return _dbSet
                    .Include(pizza => pizza.Category)
                    .Include(pizza => pizza.Ingredients)
                    .Where(filter)
                    .ToList();
            else
                return base.GetFilteredList(filter);
        }

        public IEnumerable<Pizza> GetAll(bool includeEverything)
        {
            if (includeEverything)
                return _dbSet
                    .Include(pizza => pizza.Category)
                    .Include(pizza => pizza.Ingredients)
                    .ToList();
            else
                return base.GetAll();
        }
    }
}
