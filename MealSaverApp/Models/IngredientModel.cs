using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MealSaverApp.Models
{
        public class ResponseObject
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public Data Data { get; set; }
            public object Instance { get; set; }
        }

        public class Data
        {
            public List<Ingredient> Ingredients { get; set; }
        }

        public class Ingredient
        {
            public string Owner { get; set; }
            public string Id { get; set; }
            public Details Details { get; set; }
            public DateTime CreatedAt { get; set; } = DateTime.Now;
            public DateTime UpdatedAt { get; set; }
        }

        public class Details
        {
            public string Name { get; set; }
            public int Quantity { get; set; }
            public string QuantityType { get; set; }
            public string KeptAt { get; set; }
            public DateTime UseByDate { get; set; }
        }
    }
