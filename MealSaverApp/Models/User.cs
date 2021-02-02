using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MealSaverApp.Models
{
    public class User
    {
        public string UserId { get; set; }
        public string AuthId { get; set; }
        public List<string> UserIngredients { get; set; } = new List<string>();
        public DateTime CreatedAt { get; set; }
    }

    public class UserIngredients
    {
        public string IngredientId { get; set; }
    }
}
