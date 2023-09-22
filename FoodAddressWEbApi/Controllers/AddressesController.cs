using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoodAddressWEbApi.Data;
using FoodAddressWEbApi.Models;
using FoodAddressWEbApi.RabbitMQ;
using MassTransit;
using Microsoft.AspNetCore.Http.HttpResults;
using static FoodAddressWEbApi.Contracts;

namespace FoodAddressWEbApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressesController : ControllerBase
    {
        private readonly FoodAddressDbContext _context;
        private readonly IPublishEndpoint publishEndpoint;

        public AddressesController(FoodAddressDbContext context, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            this.publishEndpoint = publishEndpoint;
        }

        // GET: api/Addresses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Address>>> GetAddresses()
        {
          if (_context.Addresses == null)
          {
              return NotFound();
          }
            var addresses = await _context.Addresses.ToListAsync();
            return Ok(addresses);
        }

        // GET: api/Addresses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Address>> GetAddress(int id)
        {
          if (_context.Addresses == null)
          {
              return NotFound();
          }
            var address = await _context.Addresses.FindAsync(id);

            if (address == null)
            {
                return NotFound();
            }

            return Ok(address);
        }

        // PUT: api/Addresses/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAddress(int id, Address address)
        {
            if (id != address.Id)
            {
                return BadRequest();
            }

            _context.Entry(address).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AddressExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            await publishEndpoint.Publish(new AddressUpdated(address.Id, address.Name, address.Latitude, address.Longitude));

            return NoContent();
        }

        // POST: api/Addresses
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Address>> PostAddress(Address address)
        {
          if (_context.Addresses == null)
          {
              return Problem("Entity set 'FoodAddressDbContext.Addresses'  is null.");
          }
            var result = _context.Addresses.Add(address);
            await _context.SaveChangesAsync();

            await publishEndpoint.Publish(new AddressCreated(result.Entity.Id, result.Entity.Name, result.Entity.Latitude, result.Entity.Longitude));

            return CreatedAtAction("GetAddress", new { id = address.Id }, address);
        }

        // DELETE: api/Addresses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            if (_context.Addresses == null)
            {
                return NotFound();
            }
            var address = await _context.Addresses.FindAsync(id);
            if (address == null)
            {
                return NotFound();
            }

            var result = _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();

            await publishEndpoint.Publish(new AddressDeleted(result.Entity.Id));

            return NoContent();
        }

        private bool AddressExists(int id)
        {
            return (_context.Addresses?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
