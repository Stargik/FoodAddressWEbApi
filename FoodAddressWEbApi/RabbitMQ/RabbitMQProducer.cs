using System;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;
using Microsoft.Extensions.Options;

namespace FoodAddressWEbApi.RabbitMQ
{
	public class RabbitMQProducer : IRabbitMQProducer
	{
        private readonly RabbitMQSettings rabbitMQSettings;
		public RabbitMQProducer(IOptions<RabbitMQSettings> mapsSettings)
		{
            this.rabbitMQSettings = mapsSettings.Value;
		}

        public void SendMessage<T>(T message)
        {
            var factory = new ConnectionFactory
            {
                HostName = rabbitMQSettings.Host
            };

            var connection = factory.CreateConnection();

            using var channel = connection.CreateModel();

            channel.QueueDeclare(rabbitMQSettings.Queue, exclusive: false);

            var json = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(json);

            channel.BasicPublish(exchange: "", routingKey: rabbitMQSettings.Queue, body: body);
        }
    }
}

