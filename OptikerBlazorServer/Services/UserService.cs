using BarBuddy.Server.DataContext;
using BarBuddy.Server.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BarBuddyBackend.Services
{
    public class UserService
    {
        private readonly string _salt = "$2a$11$78B4w1CY64crLACSasMKee";

        public async Task<bool> Login(string username, string password)
        {
            using var context = new ApplicationDBContext();

            var user = await context.Bars.Where(s => s.Credentials.Login == username.ToLower()).FirstOrDefaultAsync();
            if(user != default )
            {
                //location.Credentials.Login = newLocation.Login.ToLower();
               var PasswordHash = BCrypt.Net.BCrypt.HashPassword(password, _salt, true, BCrypt.Net.HashType.SHA384);
                if (user.Credentials.PasswordHash == PasswordHash)
                    return true;
            }

            return false;

        }
    }
}
