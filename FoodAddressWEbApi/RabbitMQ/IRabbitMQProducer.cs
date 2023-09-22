using System;
namespace FoodAddressWEbApi.RabbitMQ
{
	public interface IRabbitMQProducer
	{
        public void SendMessage<T>(T message);
    }
}

