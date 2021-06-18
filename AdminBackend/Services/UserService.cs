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

        public async Task<(bool, BarBuddy.DTOs.Enums.ApplicationEnum, long)> Login(string username, string password)
        {
            using var context = new ApplicationDBContext();

            BarBuddy.DTOs.Enums.ApplicationEnum application = BarBuddy.DTOs.Enums.ApplicationEnum.NotLoggedIn;

            var userBar = await context.Bars.Where(s => s.Credentials.Login == username.ToLower()).FirstOrDefaultAsync();
            if(userBar != default )
            {
                //location.Credentials.Login = newLocation.Login.ToLower();
               var PasswordHash = BCrypt.Net.BCrypt.HashPassword(password, _salt, true, BCrypt.Net.HashType.SHA384);
                if (userBar.Credentials.PasswordHash == PasswordHash)
                {
                    return (true, BarBuddy.DTOs.Enums.ApplicationEnum.Bar, userBar.Id);        // Bei Bars eingeloggt 
                }
            }
            
            var userOptiker = await context.Optikerlist.Where(s => s.Credentials.Login == username.ToLower()).FirstOrDefaultAsync();
            if (userOptiker != default)
            {
                //location.Credentials.Login = newLocation.Login.ToLower();
                var PasswordHash = BCrypt.Net.BCrypt.HashPassword(password, _salt, true, BCrypt.Net.HashType.SHA384);
                if (userOptiker.Credentials.PasswordHash == PasswordHash)
                {
                    return (true, BarBuddy.DTOs.Enums.ApplicationEnum.Optiker, userOptiker.Id);        // Bei Optiker eingeloggt 
                }
            }
            
            var userAugenarzt = await context.Augenarztlist.Where(s => s.Credentials.Login == username.ToLower()).FirstOrDefaultAsync();
            if (userAugenarzt != default)
            {
                //location.Credentials.Login = newLocation.Login.ToLower();
                var PasswordHash = BCrypt.Net.BCrypt.HashPassword(password, _salt, true, BCrypt.Net.HashType.SHA384);
                if (userAugenarzt.Credentials.PasswordHash == PasswordHash)
                {
                    return (true, BarBuddy.DTOs.Enums.ApplicationEnum.Augenarzt, userAugenarzt.Id);        // Bei Augenarzt eingeloggt 
                }
            }
            return (false, BarBuddy.DTOs.Enums.ApplicationEnum.NotLoggedIn,-1);        // nicht eingeloggt
        }
    }
}
