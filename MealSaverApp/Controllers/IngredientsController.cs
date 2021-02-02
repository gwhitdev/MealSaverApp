using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MealSaverApp.Interfaces;
using MealSaverApp.Models;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace MealSaverApp.Controllers
{
    public class IngredientsController : Controller
    {
        private string AccessToken { get; set; }
        private readonly IIngredientService _ingredientService;
        private readonly IUserService _userService;
        public IngredientsController(IIngredientService ingredientService, IUserService userService)
        {
            _ingredientService = ingredientService;
            _userService = userService;
        }
        private async void GetAccessToken()
        {
            if (User.Identity.IsAuthenticated)
            {
                AccessToken = await HttpContext.GetTokenAsync("access_token");
            }
        }
        public IActionResult Index()
        {
            return View();
        }
         // Get create form
        public IActionResult Create() 
        {
            return View();
        }
        [HttpPost] // Post to create form
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Details details)
        {
            var ownerId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value; //Owner ID to send to API as part of ingredient
            Ingredient ingredient = new Ingredient()
            {
                Owner = ownerId,
                Details = details
            };

            GetAccessToken();

            try
            {

                await _ingredientService.CreateIngredientAsync(ingredient, AccessToken);
                return RedirectToAction(nameof(Create));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return View();
            }
        }

        public IActionResult Update()
        {
            return View();
        }

        public IActionResult Delete()
        {
            return View();
        }
    }
}
