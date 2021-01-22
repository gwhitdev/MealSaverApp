using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MealSaverApp.Models;

namespace MealSaverApp.Interfaces
{
    public interface IIngredientService
    {
        Task<List<Ingredient>> GetIngredientsAsync();
        Task<Ingredient> GetIngredientAsync(string id);

        Task<bool> CreateIngredientAsync(Ingredient ingredient);
        Task<bool> DeleteIngredientAsync(string Id);
        // void UpdateIngredient(string Id);
    }
}
