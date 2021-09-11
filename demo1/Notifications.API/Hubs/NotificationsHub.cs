using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Notifications.API.Hubs
{
    public class NotificationsHub : Hub
    {
        private readonly ILogger<NotificationsHub> _logger;
         public NotificationsHub(ILogger<NotificationsHub> logger)
        {
            _logger = logger;
        }
        public Task SendMessage(string message)
        {
            _logger.LogInformation("Notificación: " + message);
            return Clients.All.SendAsync("notifications", message);
        }
    }
}