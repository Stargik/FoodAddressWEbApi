using System;
using FoodAddressWEbApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodAddressWEbApi.Data
{
	public class FoodAddressDbContext : DbContext
	{
		public FoodAddressDbContext(DbContextOptions<FoodAddressDbContext> options) : base(options)
        {
		}
		public DbSet<Address> Addresses { get; set; }
	}
}

