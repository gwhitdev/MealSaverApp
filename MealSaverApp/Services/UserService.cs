using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MealSaverApp.Controllers;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;
using MealSaverApp.Interfaces;


namespace MealSaverApp.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _userClient;

        public UserService(HttpClient userClient)
        {
            _userClient = userClient;

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

        public async Task<bool> CreateLocalUser(string accessToken)
        {
            string url = $"users/CreateUser";
            _userClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            HttpResponseMessage response = await _userClient.GetAsync(url);
            if (response.IsSuccessStatusCode) return true;
            return false;
        }

        public bool DeleteLocalUser(string userId)
        {
            throw new NotImplementedException();
        }
    }
}
