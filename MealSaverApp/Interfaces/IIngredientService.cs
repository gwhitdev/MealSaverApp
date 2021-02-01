using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MealSaverApp.Models;

namespace MealSaverApp.Interfaces
{
    public interface IIngredientService
    {
        Task<List<Ingredient>> GetIngredientsAsync(string accessToken);
        Task<Ingredient> GetIngredientAsync(string id);

        Task<bool> CreateIngredientAsync(Ingredient ingredient, string accessToken);
        Task<bool> DeleteIngredientAsync(string Id);
        // void UpdateIngredient(string Id);
    }
}
