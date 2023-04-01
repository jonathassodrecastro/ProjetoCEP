using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CEPapi.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;
using ProjetoCEP;
using AutoMapper;
using System.Runtime.ConstrainedExecution;
using Microsoft.Data.SqlClient;
using System.Data;

namespace CEPapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CEPControllersController : ControllerBase
    {
        private Contexto _context;
        //private IMapper _mapper;



        ManipulaCEP manipulaCEP = new ManipulaCEP();

        public CEPControllersController(Contexto context)
        {
            _context = context;
        }

        // GET: api/CEPControllers
        ///<summary>
        /// Retorna uma lista de todos os CEPs cadastrados no sistema.
        /// </summary>
        /// <remarks>
        /// Este método retorna uma lista de todos os CEPs cadastrados no sistema. Caso não haja CEPs cadastrados, o método retorna uma lista vazia.
        /// </remarks>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CEP>>> GetController()
        {
            if (_context.CEP == null)
            {
                return NotFound();
            }
            return await _context.CEP.ToListAsync();
        }

        /// <summary>
        /// Retorna um endereço a partir do CEP informado.
        /// </summary>
        /// <remarks>
        /// O CEP deve ser informado no formato XXXXXXXX apenas com os números.
        /// </remarks>
        [HttpGet("{Cep}")]
        public async Task<ActionResult<CEP>> GetCEPController(string Cep)
        {
            if (_context.CEP == null)
            {
                return NotFound();
            }

            Cep = Cep.Substring(0, 5) + "-" + Cep.Substring(5);

            var cEPController = await _context.CEP.Where(c => c.Cep == Cep).FirstOrDefaultAsync(); ;

            if (cEPController == null)
            {
                return NotFound();
            }

            return cEPController;
        }


        /// <summary>
        /// Cadastra um endereço a partir do CEP informado.
        /// </summary>
        /// <remarks>
        /// O CEP deve ser informado no formato XXXXXXXX apenas com os números.
        /// </remarks>
        [HttpPost("{Cep}")]
        public async Task<IActionResult> BuscaCEP(string Cep)
        {
            try
            {
                string viaCEPUrl = "https://viacep.com.br/ws/" + Cep + "/json/";

                WebClient client = new WebClient();
                string result = client.DownloadString(viaCEPUrl);

                CEP jsonRetorno = JsonConvert.DeserializeObject<CEP>(ManipulaCEP.trataCaracteres(result, viaCEPUrl));

                //var cepExistente = await _context.CEP.FirstOrDefaultAsync(c => c.Cep == jsonRetorno.Cep);

                var cepExistente = await _context.CEP.AnyAsync(c => c.Cep == jsonRetorno.Cep);

                if (cepExistente)
                {
                    return BadRequest("CEP já cadastrado.");
                }

                var novoCep = new CEP                              
                {
                    Cep = jsonRetorno.Cep,
                    Logradouro = jsonRetorno.Logradouro,
                    Complemento = jsonRetorno.Complemento,
                    Bairro = jsonRetorno.Bairro,
                    Localidade = jsonRetorno.Localidade,
                    Uf = jsonRetorno.Uf,
                    Unidade = jsonRetorno.Unidade,
                    Ibge = jsonRetorno.Ibge,
                    Gia = jsonRetorno.Gia
                };

                _context.CEP.Add(novoCep);
                await _context.SaveChangesAsync();

                return Ok("Endereço cadastrado com sucesso.");

            }
            catch (Exception ex)
            {

                return BadRequest("Ocorreu um erro ao buscar o CEP." + ex.Message);
            }
        }



        // DELETE: api/CEPControllers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCEPController(int id)
        {
            if (_context.CEP == null)
            {
                return NotFound();
            }
            var cEPController = await _context.CEP.FindAsync(id);
            if (cEPController == null)
            {
                return NotFound();
            }

            _context.CEP.Remove(cEPController);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CEPControllerExists(string Cep)
        {
            return (_context.CEP?.Any(e => e.Cep == Cep)).GetValueOrDefault();
        }





    }
}
