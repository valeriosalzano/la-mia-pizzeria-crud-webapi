using la_mia_pizzeria.Utility;
using la_mia_pizzeria.Models;
using System.Net;
using System.Reflection.Metadata;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;

namespace la_mia_pizzeria.Database
{
    public class DataSeeder
    {
        private readonly UserManager<IdentityUser> _userManager;

        public DataSeeder(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public static void PopulateDb()
        {
            using (PizzeriaContext db = new PizzeriaContext())
            {
                #region CHECK CATEGORY TABLE
                db.Database.EnsureCreated();

                int testCategory = db.Categories.Count();
                if (testCategory == 0)
                {
                    db.Add(new Category { Name = "Classics" });
                    db.Add(new Category { Name = "Specialty" });
                    db.Add(new Category { Name = "Innovations" });
                    db.Add(new Category { Name = "Appetizers" });
                    db.SaveChanges();
                }

                #endregion
                #region CHECK INGREDIENTS TABLE

                db.Database.EnsureCreated();
                int testIngredient = db.Ingredients.Count();
                if (testIngredient == 0)
                {
                    // GET FILE CONTENT
                    string filePath = Path.GetFullPath(@"./Database/Seeder/ingredients.csv");
                    List<string[]> fileContent = Helper.GetCSVContent(filePath, ";");

                    // PARSING TO INGREDIENTS
                    List<Ingredient> ingredientsList = GetIngredientsFromFile(fileContent);

                    foreach (Ingredient ingredient in ingredientsList)
                    {
                        Debug.WriteLine($"Adding {ingredient.Name} ingredient.");
                        try
                        {
                            db.Add(ingredient);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                            Debug.WriteLine(ex.InnerException);
                        }
                    }
                    db.SaveChanges();
                }

                #endregion
                #region CHECK PIZZAS TABLE

                db.Database.EnsureCreated();
                int testPizza = db.Pizzas.Count();
                if (testPizza == 0)
                {
                    // GET FILE CONTENT
                    string filePath = Path.GetFullPath(@"./Database/Seeder/pizzas.csv");
                    List<string[]> fileContent = Helper.GetCSVContent(filePath, ";");

                    // PARSING TO PIZZAS
                    List<Pizza> pizzasList = GetPizzasFromFile(fileContent);

                    try
                    {
                        int pizzacounter = 1;
                        foreach (Pizza pizza in pizzasList)
                        {
                            Debug.WriteLine($"Adding {pizza.Name} pizza.");

                            string[] ingredientsIds = fileContent[pizzacounter][4].Split(",");
                            foreach (string ingredientId in ingredientsIds)
                            {
                                Ingredient ingredient = db.Ingredients.Where(i => i.IngredientId == int.Parse(ingredientId)).First();
                                pizza.Ingredients?.Add(ingredient);
                            }
                            db.Add(pizza);
                            pizzacounter++;
                        }
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                        Debug.WriteLine(ex.InnerException);
                    }
                }
                #endregion
                #region CHECK ROLES TABLE
                db.Database.EnsureCreated();

                int testRoles = db.Roles.Count();
                if (testRoles == 0)
                {
                    db.Roles.Add(new IdentityRole() { Id = "1", Name="ADMIN", NormalizedName="ADMIN", ConcurrencyStamp="9/10/2023"});
                    db.Roles.Add(new IdentityRole() { Id = "2", Name = "USER", NormalizedName = "USER", ConcurrencyStamp = "9/10/2023" });
                    db.Roles.Add(new IdentityRole() { Id = "3", Name = "SUPERADMIN", NormalizedName = "SUPERADMIN", ConcurrencyStamp = "9/10/2023" });
                    db.SaveChanges();
                }

                #endregion
               
            }
        }
        public async void CheckUsers()
        {
            using (PizzeriaContext db = new PizzeriaContext()) 
            {
                #region CHECK USERS TABLE
                db.Database.EnsureCreated();

                int testUsers = db.Users.Count();
                if (testUsers == 0)
                {
                    var user1 = new IdentityUser() { UserName = "superadmin@superadmin.com", Email = "superadmin@superadmin.com", EmailConfirmed = true };
                    await _userManager.CreateAsync(user1, "Password1!");
                    await _userManager.AddToRoleAsync(user1, "SUPERADMIN");

                    var user2 = new IdentityUser() { UserName = "admin@admin.com", Email = "admin@admin.com", EmailConfirmed = true };
                    await _userManager.CreateAsync(user2, "Password1!");
                    await _userManager.AddToRoleAsync(user2, "ADMIN");

                    var user3 = new IdentityUser() { UserName = "user@user.com", Email = "user@user.com", EmailConfirmed = true };
                    await _userManager.CreateAsync(user3, "Password1!");
                    await _userManager.AddToRoleAsync(user3, "USER");

                    db.SaveChanges();
                }
            }
            #endregion
        }
        public static List<Pizza> GetPizzasFromFile(List<string[]> fileContent)
        {
            List<Pizza> pizzasList = new List<Pizza>();

            int rowCounter = 0;
            foreach (string[] row in fileContent)
            {
                if (rowCounter > 0)
                {
                    if (row.Length != 5)
                    {
                        Debug.WriteLine("Wrong format. Row " + rowCounter);
                    }
                    else
                    {
                        try
                        {

                            Pizza newPizza = new Pizza
                            {
                                Name = row[0],
                                Slug = Helper.GetSlugFromString(row[0]),
                                Price = decimal.Parse(row[1]),
                                Description = row[2],
                                CategoryId = int.Parse(row[3]),
                                Ingredients = new List<Ingredient>(),
                                ImgPath = $"/img/pizza-{rowCounter}.png",
                            };

                            pizzasList.Add(newPizza);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Could not create a pizza with row {rowCounter} data.");
                            Debug.WriteLine(ex.Message);
                        }
                    }
                }
                rowCounter++;
            }
            return pizzasList;
        }

        public static List<Ingredient> GetIngredientsFromFile(List<string[]> fileContent)
        {
            List<Ingredient> ingredientsList = new List<Ingredient>();

            int rowCounter = 0;
            foreach (string[] row in fileContent)
            {
                if (rowCounter > 0)
                {
                    if (row.Length != 2)
                    {
                        Debug.WriteLine("Wrong format. Row " + rowCounter);
                    }
                    else
                    {
                        try
                        {
                            ingredientsList.Add(new Ingredient
                            {
                                Name = row[1],
                            });
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Could not create an ingredient with row {rowCounter} data.");
                            Debug.WriteLine(ex.Message);
                        }
                    }
                }
                rowCounter++;
            }
            return ingredientsList;
        }
    }
}
