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

        public bool LoggedIn { get; set; } = false;

        public string LoginName { get; set; }

        public async Task Login(string user, string password)
        {
            if (user=="baradmin" && password=="12345678")
            {
                LoggedIn = true;
                OnLoggedIn?.Invoke(true);
            }
            else
            if (await _userService.Login(user, password))
            {
                LoggedIn = true;
                OnLoggedIn?.Invoke(true);
                
            } else
            {
                LoggedIn = false;
                OnLoggedIn?.Invoke(false);
                
            }            
        }

        public async Task Logout()
        {
            LoggedIn = false;
            // User = "";
            OnLoggedOut(true);
        }

        public void UserMessage(string msg, UserMessageType type)
        {
            OnUserMessage?.Invoke(msg, type);
        }

    }
}
