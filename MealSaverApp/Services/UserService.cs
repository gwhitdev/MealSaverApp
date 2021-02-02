using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;
using MealSaverApp.Interfaces;
using MealSaverApp.Models;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;

namespace MealSaverApp.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _userClient;
        private readonly ILogger _logger;

        public UserService(HttpClient userClient, ILoggerFactory loggerFactory)
        {
            _userClient = userClient;
            _logger = loggerFactory.CreateLogger<UserService>();

        }
        public async Task<bool> CheckForLocalUser(string accessToken)
        {
            string url = "users";
            _userClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            HttpResponseMessage response = await _userClient.GetAsync(url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return true;
            }

            return false;

        }

        public async Task<User> Get(string accessToken)
        {
            string url = "users";
            _userClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            HttpResponseMessage response = await _userClient.GetAsync(url);
            var result = await response.Content.ReadAsStringAsync();
            JToken convertedResult = JToken.Parse(result);
            JToken confirmedUser = convertedResult[0]["data"]["user"];
            User user = confirmedUser.ToObject<User>();
            return user;
        }

        public async Task<List<Ingredient>> GetUserIngredientsAsync(string accessToken)
        {
            string url = "users/GetUserIngredients";
            _userClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            HttpResponseMessage response = await _userClient.GetAsync(url);

            List<Ingredient> foundIngredients = new List<Ingredient>();

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                JToken convertedResult = JArray.Parse(result);
                List<JToken> results = convertedResult[0]["data"]["ingredients"].ToList();
                
                foreach (JToken foundResult in results)
                {
                    Ingredient ingredient = foundResult.ToObject<Ingredient>();
                    foundIngredients.Add(ingredient);
                }
            }
            
            var detailsContainsNull = foundIngredients.Exists(i => i.Details == null);
            Details nulledIngredientReplacement = new Details()
            {
                Name = "No ingredients found"
            };

            if(detailsContainsNull)
            {
                var detailsToReplace = foundIngredients.FindAll(i => i.Details == null);
                foreach(var ingredient in detailsToReplace)
                {
                    ingredient.Details = nulledIngredientReplacement;
                }
            }

            return foundIngredients;

        }
        public async Task<bool> CreateLocalUser(string accessToken)
        {
            string url = $"users/CreateUser";
            _userClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            HttpResponseMessage response = await _userClient.GetAsync(url);
            if (response.IsSuccessStatusCode) return true;
            return false;
        }

        public async Task<bool> UpdateLocalUser(string accessToken, string ingredientId)
        {
            User user = await Get(accessToken);

            User updatedUser = new User()
            {
                UserId = user.UserId,
                UserIngredients = new List<string>() { ingredientId }
            };

            var url = "users/UpdateUserIngredientList";
            string json = JsonConvert.SerializeObject(updatedUser);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            _userClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _userClient.PutAsync(url, content);
            var result = await response.Content.ReadAsStringAsync();

            JArray convertedResponse = JArray.Parse(result);
            User returnedUser = convertedResponse[0]["data"]["user"].ToObject<User>();

            if (returnedUser.UserIngredients.Contains(updatedUser.UserIngredients[0])) return true;
            return false;
        }

        public bool DeleteLocalUser(string userId)
        {
            throw new NotImplementedException();
        }
    }
}
 