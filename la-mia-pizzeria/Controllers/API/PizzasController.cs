using la_mia_pizzeria.Database;
using la_mia_pizzeria.Models;
using la_mia_pizzeria.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;

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
        public IActionResult GetAll()
        {
            List<Pizza> pizzaList = (List<Pizza>) _pizzaManager.GetAll();

            return Ok(pizzaList);
        }

        [HttpGet]
        public IActionResult GetAllContaining(string? name)
        {
            List<Pizza> foundPizzas = _pizzaManager.GetAllContaining(name, true);
            return Ok(foundPizzas);
        }

        [HttpGet("{pizzaId}")]
        public IActionResult GetById(int pizzaId)
        {
            Pizza? foundPizza = _pizzaManager.GetById(pizzaId, true);
            if (foundPizza is null) 
            {
                return NotFound();
            }
            else
            {
                return Ok(foundPizza);
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] Pizza newPizza)
        {
            newPizza.Slug = Helper.GetSlugFromString(newPizza.Name);
            try
            {
                _pizzaManager.Add(newPizza);
                return Ok();
            }catch(Exception ex)
            {
                return BadRequest( new { Message = ex.Message });
            }
        }

        [HttpPut("{pizzaId}")]
        public IActionResult ModifyPizza(int pizzaId,[FromBody] Pizza modifiedPizza)
        {
            Pizza? originalPizza = _pizzaManager.GetById(pizzaId);

            if(originalPizza is null)
            {
                return NotFound();
            }
            else
            {
                try
                {
                    modifiedPizza.Slug = Helper.GetSlugFromString(modifiedPizza.Name);
                    _pizzaManager.Update(originalPizza, modifiedPizza);
                    return Ok();
                }catch (Exception ex)
                {
                    return BadRequest(new { Message = ex.Message });
                }
            }
        }

        [HttpDelete("{pizzaId}")]
        public IActionResult DeletePizza(int pizzaId)
        {
            Pizza? markedPizza = _pizzaManager.GetById(pizzaId);
            if(markedPizza is null)
            {
                return NotFound();
            }
            else
            {
                try
                {
                    _pizzaManager.Delete(markedPizza);
                    return Ok();
                }catch( Exception ex)
                {
                    return BadRequest(new {Message = ex.Message});
                }
            }
        }
    }
}
