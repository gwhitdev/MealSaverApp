using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using MealSaverApp.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Security.Claims;
using MealSaverApp.Models;
using MealSaverApp.Interfaces;
namespace MealSaverApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IIngredientService _ingredientService;
        public HomeController(IIngredientService ingredientService)
        {
            _ingredientService = ingredientService;
        }
        public async Task<IActionResult> Index()
        {
            // If the user is authenticated, then this is how you can get the access_token and id_token
            if (User.Identity.IsAuthenticated)
            {
                string accessToken = await HttpContext.GetTokenAsync("access_token");

                // if you need to check the access token expiration time, use this value
                // provided on the authorization response and stored.
                // do not attempt to inspect/decode the access token
                DateTime accessTokenExpiresAt = DateTime.Parse(
                    await HttpContext.GetTokenAsync("expires_at"),
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.RoundtripKind);

                string idToken = await HttpContext.GetTokenAsync("id_token");

                // Now you can use them. For more info on when and how to use the
                // access_token and id_token, see https://auth0.com/docs/tokens
            }

            return View();
        }
        [Authorize(Roles = "admin")]
        public IActionResult Admin()
        {
            
            return View(new UserProfileViewModel()
            {
                Name = User.Identity.Name,
                EmailAddress = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value,
                ProfileImage = User.Claims.FirstOrDefault(c => c.Type == "picture")?.Value,
                Role = User.Claims.FirstOrDefault(c => c.Type == "https://mealsaverapp/roles").Value
            }); ;
        }
        [Authorize(Roles = "user")]
        public async Task<IActionResult> Profile()
        {
            return View(new UserProfileViewModel()
            {
                Name = User.Identity.Name,
                EmailAddress = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value,
                ProfileImage = User.Claims.FirstOrDefault(c => c.Type == "picture")?.Value,
                Role = User.Claims.FirstOrDefault(c => c.Type == "https://mealsaverapp/roles").Value,
                Ingredients = await _ingredientService.GetIngredientsAsync(),
                Verified = User.Claims.FirstOrDefault(c => c.Type == "email_verified").Value
            }) ;
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
