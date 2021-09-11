using System;

namespace Frontend.Models
{
    public class NotificationDto
    {
        public string Type {get; set;}
        public string Message {get; set;}
        public int OrderID {get;set;}
        public DateTime Date {get;set;}
    }
}
