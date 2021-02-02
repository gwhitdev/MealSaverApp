using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MealSaverApp.Models;

namespace MealSaverApp.Interfaces
{
    public interface IUserService
    {
        Task<bool> CheckForLocalUser(string accessToken);
        Task<bool> CreateLocalUser(string accessToken);
        Task<bool> UpdateLocalUser(string accessToken, string ingredientId);
        Task<List<Ingredient>> GetUserIngredientsAsync(string accessToken);
        bool DeleteLocalUser(string userId);
        
    }
}
