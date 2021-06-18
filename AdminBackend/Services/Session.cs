using BarBuddyBackend.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BarBuddyBackend.Services
{
    public class Session
    {
        public delegate void LoggedinEventArgs(bool loggedIn);
        public delegate void UserMessageEventArgs(string message, UserMessageType type);

        public event LoggedinEventArgs OnLoggedIn;
        public event LoggedinEventArgs OnLoggedOut;
        public event UserMessageEventArgs OnUserMessage;
        

        private UserService _userService;

        public Session( UserService userService)
        {
            _userService = userService;
        }

        public long LoggedInEntityId { get; set; } = -1;
        public bool LoggedIn { get; set; } = false;

        public BarBuddy.DTOs.Enums.ApplicationEnum LoggedInApplication { get; set; } = BarBuddy.DTOs.Enums.ApplicationEnum.NotLoggedIn;
        
        public string LoginName { get; set; }

        public async Task Login(string user, string password)
        {
            LoggedInEntityId = -1;
            if (user=="baradmin" && password=="12345678")
            {
                LoggedIn = true;
                LoggedInApplication = BarBuddy.DTOs.Enums.ApplicationEnum.Admin;
                OnLoggedIn?.Invoke(true);
            }
            else
            {
                var result = await _userService.Login(user, password);
                if (result.Item1)
                {
                    LoggedIn = true;
                    LoggedInApplication = (BarBuddy.DTOs.Enums.ApplicationEnum) result.Item2;
                    LoggedInEntityId = result.Item3;
                    OnLoggedIn?.Invoke(true);
                }
                else
                {
                    LoggedIn = false;
                    LoggedInApplication = BarBuddy.DTOs.Enums.ApplicationEnum.NotLoggedIn;
                    OnLoggedIn?.Invoke(false);
                }                
            }            
        }

        public async Task Logout()
        {
            LoggedIn = false;
            LoggedInEntityId = -1;
            // User = "";
            OnLoggedOut(true);
        }

        public void UserMessage(string msg, UserMessageType type)
        {
            OnUserMessage?.Invoke(msg, type);
        }

    }
}
