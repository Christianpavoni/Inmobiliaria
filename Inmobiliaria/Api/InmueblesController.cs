using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Inmobiliaria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;

namespace Inmobiliaria.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class InmueblesController : ControllerBase
    {
        private readonly DbContexto _context;
        private readonly IConfiguration _config;

        public InmueblesController(DbContexto context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Inmueble>>> GetInmuebles()
        {
            try
            {
                var usuario = User.Identity.Name;
                //var res = await _context.Inmuebles.Where(e => e.Propietario.Email == usuario).ToListAsync();                
                var list = await _context.Inmuebles.Include(e => e.Propietario).Where(e => e.Propietario.Email == usuario).ToListAsync();
                return Ok(list);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


        // GET: api/Inmuebles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Inmueble>> GetInmueble(int id)
        {            
            try
            {                
                var inmueble = await _context.Inmuebles.FindAsync(id);
                return Ok(inmueble);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // PUT: api/Inmuebles/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut]
        public async Task<IActionResult> PutInmueble(Inmueble inmueble)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    _context.Inmuebles.Update(inmueble);
                    await _context.SaveChangesAsync();

                    return Ok(inmueble);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // POST: api/Inmuebles
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Inmueble>> PostInmueble(Inmueble inmueble)
        {
            _context.Inmuebles.Add(inmueble);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetInmueble", new { id = inmueble.IdInmueble }, inmueble);
        }

        // DELETE: api/Inmuebles/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Inmueble>> DeleteInmueble(int id)
        {
            var inmueble = await _context.Inmuebles.FindAsync(id);
            if (inmueble == null)
            {
                return NotFound();
            }

            _context.Inmuebles.Remove(inmueble);
            await _context.SaveChangesAsync();

            return inmueble;
        }

        private bool InmuebleExists(int id)
        {
            return _context.Inmuebles.Any(e => e.IdInmueble == id);
        }
    }
}
