﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MealSaverApp.Interfaces;
using MealSaverApp.Models;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace MealSaverApp.Controllers
{
    public class IngredientsController : Controller
    {
        private string AccessToken { get; set; }
        private readonly IIngredientService _ingredientService;
        private readonly IUserService _userService;
        private readonly ILogger _logger;
        public IngredientsController(IIngredientService ingredientService, IUserService userService, ILoggerFactory loggerFactory)
        {
            _ingredientService = ingredientService;
            _userService = userService;
            _logger = loggerFactory.CreateLogger<IngredientsController>();
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
            GetAccessToken();

            var ownerId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value; //Owner ID to send to API as part of ingredient
            var role = User.Claims.FirstOrDefault(c => c.Type == "https://mealsaverapp/roles").Value;
            //_logger.LogDebug($"{role}");
            Ingredient ingredient = new Ingredient()
            {
                Owner = ownerId,
                Details = details
            };

            try
            {
                await _ingredientService.CreateIngredientAsync(ingredient, AccessToken);
                if (role == "admin") return RedirectToAction("Admin", "Home");
                return RedirectToAction("Profile","Home");
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

        public async Task<IActionResult> Delete(string id)
        {
            GetAccessToken();
            bool deletedIngredient = await _ingredientService.DeleteIngredientAsync(AccessToken, id);
            _logger.LogDebug($"RETURNED deletedIngredient? {deletedIngredient}");
            if (deletedIngredient)
            {
                TempData["IngredientDeleted"] = "Ingredient deleted";
            }
            else
            {
                TempData["IngredientDeleted"] = "Ingredient not deleted";
            }
            
            return RedirectToAction("Admin", "Home");
        }
    }
}
