using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using MealSaverApp.Interfaces;
using MealSaverApp.Models;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;

namespace MealSaverApp.Services
{
    public class IngredientHttpService : IIngredientService
    {
        private readonly HttpClient _client;
        private readonly IUserService _userService;
        private readonly ILogger _logger;
        private List<Ingredient> SearchResults { get; set; }
        private Ingredient Ingredient { get; set; }

        public IngredientHttpService(HttpClient client, IUserService userService, ILoggerFactory loggerFactory)
        {
            // Injected HttpClient
            _client = client;
            _userService = userService;
            _logger = loggerFactory.CreateLogger<IngredientHttpService>();
        }

        /// <summary>
        /// Method to get all ingredients from API
        /// </summary>
        /// <returns>Enumarble list of ingredients</returns>
        public async Task<List<Ingredient>> GetIngredientsAsync(string accessToken)
        {

            var url = "Ingredients";

            try
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                HttpResponseMessage response = await _client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsStringAsync();
                JArray convertedResult = JArray.Parse(result);
                IList<JToken> results = convertedResult[0]["data"]["ingredients"].Children().ToList();

                // serialize Json results into .Net objects
                SearchResults = new List<Ingredient>();

                foreach (JToken searchResult in results)
                {
                    Ingredient ingredient = searchResult.ToObject<Ingredient>();
                    SearchResults.Add(ingredient);
                }

                var detailsContainsNull = SearchResults.Exists(i => i.Details == null);
                Details nulledIngredientReplacement = new Details()
                {
                    Name = "no info",
                };
                if (detailsContainsNull)
                {
                    var detailsToReplace = SearchResults.FindAll(i => i.Details == null);
                    foreach(var ingredient in detailsToReplace)
                    {
                        ingredient.Details = nulledIngredientReplacement;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return SearchResults;
        }

        /// <summary>
        /// Method to get a specific ingredient from the API
        /// </summary>
        /// <param name="id"></param>
        /// <returns>An async ingredient</returns>
        public async Task<Ingredient> GetIngredientAsync(string id)
        {
            var url = $"Ingredients/{id}";

            try
            {
                HttpResponseMessage response = await _client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsStringAsync();

                // Parses the result
                JArray convertedResult = JArray.Parse(result);

                // Extracts just the specific ingredient details from the results
                List<JToken> results = convertedResult[0]["data"]["ingredients"].Children().ToList();

                // Converts the specific ingredient to an Ingredient object
                Ingredient = results[0].ToObject<Ingredient>();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return Ingredient;
        }

        public async Task<bool> CreateIngredientAsync(Ingredient ingredient, string accessToken)
        {
            var url = $"Ingredients";
            var json = JsonConvert.SerializeObject(ingredient);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _client.PostAsync(url, content);
            var result = await response.Content.ReadAsStringAsync();
            JToken convertedResult = JToken.Parse(result);
            //List<JToken> results = convertedResult[0]["data"]["ingredients"].Children().ToList();
            Ingredient = convertedResult.ToObject<Ingredient>();
            var ingredientId = Ingredient.Id;
            
            if(response.IsSuccessStatusCode)
            {
                await _userService.UpdateLocalUser(accessToken, ingredientId);
            }
            if (response.StatusCode == HttpStatusCode.Created)
            {
                return true;
            }

            return false;

        }

        public async Task<bool> DeleteIngredientAsync(string ingredientId)
        {
            var url = "Ingredients";
            var response = await _client.DeleteAsync($"{url}/{ingredientId}");

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return true;
            }

            return false;
        }
    }
}
