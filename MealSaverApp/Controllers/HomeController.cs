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
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
namespace MealSaverApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IIngredientService _ingredientService;
        private readonly IUserService _userService;
        private readonly ILogger _logger;
        private string AccessToken { get; set; }
        public HomeController(IIngredientService ingredientService, IUserService userService, ILoggerFactory loggerFactory)
        {
            _ingredientService = ingredientService;
            _userService = userService;
            _logger = loggerFactory.CreateLogger<HomeController>();
        }
        
        public async void GetAccessToken()
        {
            AccessToken = await HttpContext.GetTokenAsync("access_token");
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
        public async Task<IActionResult> Admin()
        {
            GetAccessToken();

            var userIngredients = new List<Ingredient>();
            bool userInDatabase = await _userService.CheckForLocalUser(AccessToken);

            if(!userInDatabase) await _userService.CreateLocalUser(AccessToken);
           
            if (User.Identity.IsAuthenticated && userInDatabase)
            {
                var ingredients = await _userService.GetUserIngredientsAsync(AccessToken);
                userIngredients = ingredients;
            }

            return View(new UserProfileViewModel()
            {
                Name = User.Identity.Name,
                EmailAddress = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value,
                ProfileImage = User.Claims.FirstOrDefault(c => c.Type == "picture")?.Value,
                Role = User.Claims.FirstOrDefault(c => c.Type == "https://mealsaverapp/roles").Value,
                Ingredients = userIngredients,
                ExistsInDb = userInDatabase // could be removed once initial development is finished
            });
        }

        [Authorize(Roles = "user")]
        public async Task<IActionResult> Profile()
        {

            List<Ingredient> userIngredients = new List<Ingredient>();

            string accessToken = "";
            //var errorMsg = "None";
            var tokenAccepted = false;
            if (User.Identity.IsAuthenticated)
            {
                accessToken = await HttpContext.GetTokenAsync("access_token");
            }

            if (!String.IsNullOrEmpty(accessToken))
            {
                List<Ingredient> fetchIngredients = await _ingredientService.GetIngredientsAsync(accessToken);
                if (fetchIngredients != null )
                {
                    //userIngredients = fetchIngredients;
                    tokenAccepted = true;
                }
                
            }

                return View(new UserProfileViewModel()
            {
                Name = User.Identity.Name,
                EmailAddress = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value,
                ProfileImage = User.Claims.FirstOrDefault(c => c.Type == "picture")?.Value,
                Role = User.Claims.FirstOrDefault(c => c.Type == "https://mealsaverapp/roles").Value,
                //Ingredients = userIngredients,
                //Error = errorMsg,
                Authorized = tokenAccepted,
                Verified = User.Claims.FirstOrDefault(c => c.Type == "email_verified").Value
            }) ;
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
