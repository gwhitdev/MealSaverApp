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
