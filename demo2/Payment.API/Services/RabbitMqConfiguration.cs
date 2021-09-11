namespace Payment.API.Services
{
    public class RabbitMqConfiguration
    {
        public string Hostname { get; set; }
        public string QueueName {get; set;}
        public string QueueName2 { get; set; }
        public bool Enabled { get; set; }
    }
}