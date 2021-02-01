using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MealSaverApp.Models.ViewModels
{
    public class UserProfileViewModel
    {
        public string EmailAddress { get; set; }
        public string Name { get; set; }
        public string ProfileImage { get; set; }
        public string Role { get; set; }
        public List<Ingredient> Ingredients { get; set; }
        public string Verified { get; set; }
        public string Error { get; set; }
        public bool Authorized { get; set; }
        public bool ExistsInDb { get; set; }
    }
}
