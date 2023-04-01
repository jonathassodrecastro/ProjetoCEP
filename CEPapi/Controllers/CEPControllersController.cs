using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CEPapi.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;
using ProjetoCEP;

namespace CEPapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CEPControllersController : ControllerBase
    {
        private readonly Contexto _context;
        ManipulaCEP manipulaCEP = new ManipulaCEP(); 

        public CEPControllersController(Contexto context)
        {
            _context = context;
        }

        // GET: api/CEPControllers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CEPController>>> GetController()
        {
          if (_context.Controller == null)
          {
              return NotFound();
          }
            return await _context.Controller.ToListAsync();
        }

        // GET: api/CEPControllers/5
        [HttpGet("{Cep}")]
        public async Task<ActionResult<CEPController>> GetCEPController(string Cep)
        {
          if (_context.Controller == null)
          {
              return NotFound();
          }
            string viaCEPUrl = "https://viacep.com.br/ws/" + Cep + "/json/";
            string result;

            WebClient client = new WebClient();
            result = client.DownloadString(viaCEPUrl);

            JObject jsonRetorno = JsonConvert.DeserializeObject<JObject>(ManipulaCEP.trataCaracteres(result, viaCEPUrl));

            ManipulaCEP.buscaCEP(jsonRetorno);
           


            var cEPController = await _context.Controller.FindAsync(Cep);

            if (cEPController == null)
            {
                return NotFound();
            }

            return cEPController;
        }

        // PUT: api/CEPControllers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCEPController(int id, CEPController cEPController)
        {
            if (id != cEPController.Id)
            {
                return BadRequest();
            }

            _context.Entry(cEPController).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CEPControllerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/CEPControllers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CEPController>> PostCEPController(CEPController cEPController)
        {
          if (_context.Controller == null)
          {
              return Problem("Entity set 'Contexto.Controller'  is null.");
          }
            _context.Controller.Add(cEPController);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCEPController", new { id = cEPController.Id }, cEPController);
        }

        // DELETE: api/CEPControllers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCEPController(int id)
        {
            if (_context.Controller == null)
            {
                return NotFound();
            }
            var cEPController = await _context.Controller.FindAsync(id);
            if (cEPController == null)
            {
                return NotFound();
            }

            _context.Controller.Remove(cEPController);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CEPControllerExists(int id)
        {
            return (_context.Controller?.Any(e => e.Id == id)).GetValueOrDefault();
        }




    }
}
