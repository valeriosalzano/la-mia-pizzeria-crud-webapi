using la_mia_pizzeria.Database;
using la_mia_pizzeria.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace la_mia_pizzeria.Controllers.API
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PizzasController : ControllerBase
    {
        private PizzaRepository _pizzaManager;

        public PizzasController(PizzaRepository pizzaManager) 
        { 
            _pizzaManager = pizzaManager;
        }

        [HttpGet]
        public IActionResult GetPizzas()
        {
            List<Pizza> pizzas = (List<Pizza>) _pizzaManager.GetAll();

            return Ok(pizzas);
        }

        [HttpGet]
        public IActionResult FindPizzasByName(string? name)
        {
            return Ok(_pizzaManager.GetAllContaining(name));
        }

        [HttpGet("{id}")]
        public IActionResult PizzaById(int id)
        {
            Pizza? foundPizza = _pizzaManager.GetById(id);
            if (foundPizza is null) 
            {
                return NotFound();
            }
            else
            {
                return Ok(foundPizza);
            }
        }

    }
}
