﻿using la_mia_pizzeria.Database;
using la_mia_pizzeria.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using la_mia_pizzeria.Utility;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;
using Microsoft.AspNetCore.Mvc.Rendering;
using Azure;
using Microsoft.AspNetCore.Authorization;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;
using la_mia_pizzeria.CustomLoggers;

namespace la_mia_pizzeria.Controllers
{
    [Authorize(Roles = "ADMIN,SUPERADMIN")]
    public class PizzaController : Controller
    {
        private readonly ICustomLogger _logger;
        private readonly PizzeriaContext _database;

        public PizzaController(ICustomLogger logger, PizzeriaContext db)
        {
            _logger = logger;
            _database = db;
        }

        // GET: PizzaController
        [Authorize]
        [HttpGet]
        public ActionResult Index()
        {
            try
            {
                List<Pizza> pizzas = _database.Pizzas.ToList<Pizza>();
                return View("Index", pizzas);
            }
            catch
            {
                _logger.WriteLog("Catching exception at Pizza>Index");
                return NotFound();
            }
        }

        // GET: PizzaController/Details/pizza-slug
        public ActionResult Details(string slug)
        {
            try
            {
                Pizza? pizza = _database.Pizzas.Where(pizza => pizza.Slug == slug).
                    Include(pizza => pizza.Category).
                    Include(pizza => pizza.Ingredients).
                    FirstOrDefault();

                if (pizza is null)
                {
                    _logger.WriteLog($"Catching a null reference at Pizza>Details>{slug}");
                    return NotFound("Can't find the pizza.");
                }
                else
                    return View("Details", pizza);
            }
            catch
            {
                _logger.WriteLog($"Catching exception at Pizza>Details>{slug}");
                return NotFound();
            }
        }

        // GET: PizzaController/Create
        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public ActionResult Create()
        {
            try
            {
                PizzaFormModel formModel = new PizzaFormModel {
                    Pizza = new Pizza(),
                };
                PrepareFormModel( formModel );

                return View("Create", formModel);
            }
            catch
            {
                _logger.WriteLog($"Catching exception at GET Pizza>Create");
                return NotFound();
            }
        }

        // POST: PizzaController/Create
        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PizzaFormModel formData)
        {
            try
            {
                ModelState.Clear();
                PreparePizzaForValidation(formData.Pizza);
                if (!TryValidateModel(formData))
                {
                    PrepareFormModel(formData);
                    return View(nameof(Create), formData);
                }

                formData.Pizza.Ingredients = new List<Ingredient>();
                if(formData.SelectedIngredientsId != null)
                {
                    foreach(string selectedId in formData.SelectedIngredientsId)
                    {
                        int selectedIngredientId = int.Parse(selectedId);
                        Ingredient fetchedIngredient = _database.Ingredients.Where(ingredient => ingredient.IngredientId == selectedIngredientId).First();
                        formData.Pizza.Ingredients.Add(fetchedIngredient);
                    }
                }

                _database.Add(formData.Pizza);
                _database.SaveChanges();
                return RedirectToAction(nameof(Details), new { slug = formData.Pizza.Slug });
            }
            catch
            {
                _logger.WriteLog($"Catching exception at POST Pizza>Create");
                return NotFound();
            }
        }

        // GET: PizzaController/Edit/pizza-slug
        [Authorize(Roles = "SUPERADMIN")]
        public ActionResult Edit(string slug)
        {
            try
            {
                Pizza? pizza = _database.Pizzas.Where(pizza => pizza.Slug == slug)
                    .Include(pizza => pizza.Category)
                    .Include(pizza => pizza.Ingredients)
                    .FirstOrDefault();

                if (pizza is null)
                {
                    _logger.WriteLog($"Catching a null reference at GET Pizza>Edit>{slug}");
                    return NotFound("Can't find the pizza.");
                }
                else
                {
                    PizzaFormModel formModel = new PizzaFormModel{ Pizza = pizza };
                    PrepareFormModel( formModel );

                    return View(nameof(Edit),formModel);
                }
            }
            catch
            {
                _logger.WriteLog($"Catching exception at GET Pizza>Edit>{slug}");
                return NotFound();
            }
        }

        // POST: PizzaController/Edit/pizza-slug
        [Authorize(Roles = "SUPERADMIN")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string slug, PizzaFormModel formData)
        {
            try
            {
                ModelState.Clear();
                PreparePizzaForValidation(formData.Pizza);
                if (!TryValidateModel(formData))
                {
                    PrepareFormModel(formData);
                    return View(nameof(Edit), formData);
                }

                Pizza originalPizza = _database.Pizzas.Where(pizza => pizza.Slug == slug).Include(pizza => pizza.Ingredients).First();

                originalPizza.Ingredients!.Clear();

                if (formData.SelectedIngredientsId != null)
                {
                    foreach (string selectedId in formData.SelectedIngredientsId)
                    {
                        int selectedIngredientId = int.Parse(selectedId);
                        Ingredient fetchedIngredient = _database.Ingredients.Where(ingredient => ingredient.IngredientId == selectedIngredientId).First();
                        originalPizza.Ingredients.Add(fetchedIngredient);
                    }
                }

                
                EntityEntry<Pizza> originalPizzaEntity = _database.Entry(originalPizza);
                originalPizzaEntity.CurrentValues.SetValues(formData.Pizza);

                
                _database.SaveChanges();
                return RedirectToAction(nameof(Details), new {slug = formData.Pizza.Slug});
            }
            catch
            {
                _logger.WriteLog($"Catching exception at POST Pizza>Edit>{slug}");
                return NotFound();
            }
            
        }

        // POST: PizzaController/Delete/pizza-slug
        [Authorize(Roles = "SUPERADMIN")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string slug)
        {
            try
            {
                Pizza deletedPizza = _database.Pizzas.Where(pizza => pizza.Slug == slug).First();
                _database.Remove(deletedPizza);
                _database.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                _logger.WriteLog($"Catching exception at Pizza>Delete>{slug}");
                return NotFound("Can't find the desired pizza.");
            }
        }

        // POST: PizzaController/PopulateDb
        [Authorize(Roles = "ADMIN")]
        public ActionResult PopulateDb()
        {
            try
            {
                DataSeeder.PopulateDb();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View("Index");
            }
        }

        private void PreparePizzaForValidation(Pizza pizza)
        {
            pizza.Description ??= "";
            pizza.ImgPath ??= "";
            pizza.Slug = Helper.GetSlugFromString(pizza.Name);
        }
        private void PrepareFormModel(PizzaFormModel formData)
        {
            // CATEGORY LIST
            List<Category> categories = _database.Categories.ToList();
            formData.Categories = categories;

            // INGREDIENTS LIST
            List<SelectListItem> selectIngredients = new List<SelectListItem>();
            List<Ingredient> dbIngredients = _database.Ingredients.OrderBy(ingredient => ingredient.Name).ToList();
            foreach (Ingredient ingredient in dbIngredients)
            {
                selectIngredients.Add(new SelectListItem { Text = ingredient.Name, Value = ingredient.IngredientId.ToString() });
            }
            formData.Ingredients = selectIngredients;
        }
    }
}
