using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MealSaverApp.Interfaces
{
    public interface IUserService
    {
        Task<bool> CheckForLocalUser(string accessToken);
        Task<bool> CreateLocalUser(string accessToken);
        bool DeleteLocalUser(string userId);
        
    }
}
