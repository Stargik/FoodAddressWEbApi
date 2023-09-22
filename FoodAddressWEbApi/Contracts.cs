using System;
namespace FoodAddressWEbApi
{
	public class Contracts
	{
		public record AddressCreated(int Id, string Name, string Latitude, string Longitude);
        public record AddressUpdated(int Id, string Name, string Latitude, string Longitude);
        public record AddressDeleted(int Id);
    }
}

