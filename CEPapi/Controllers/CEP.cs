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
    public class CEP : ControllerBase
    {
        private Contexto _context;
        //private IMapper _mapper;



        ManipulaCEP manipulaCEP = new ManipulaCEP();

        public CEP(Contexto context)
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
        public async Task<ActionResult<IEnumerable<Models.CEP>>> GetController()
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
        public async Task<ActionResult<Models.CEP>> GetCEPController(string Cep)
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
        /// Retorna uma lista de todos os endereços listados com a UF informada
        /// </summary>
        /// <remarks>
        /// Informar apenas duas letras representando o estado. Exemplo: São Paulo - informar SP.
        /// </remarks>
        [HttpGet("uf/{Uf}")]
        public async Task<IActionResult> GetByUF(string Uf)
        {
            try
            {
                var enderecos = await _context.CEP.Where(e => e.Uf == Uf).ToListAsync();

                if (enderecos == null || enderecos.Count == 0)
                {
                    return NotFound();
                }

                return Ok(enderecos);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao buscar endereços por UF: {ex.Message}");
            }
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

                Models.CEP jsonRetorno = JsonConvert.DeserializeObject<Models.CEP>(ManipulaCEP.trataCaracteres(result, viaCEPUrl));

                //var cepExistente = await _context.CEP.FirstOrDefaultAsync(c => c.Cep == jsonRetorno.Cep);

                var cepExistente = await _context.CEP.AnyAsync(c => c.Cep == jsonRetorno.Cep);

                if (cepExistente)
                {
                    return BadRequest("CEP já cadastrado.");
                }

                var novoCep = new Models.CEP                              
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

                return Ok($"Endereço cadastrado com sucesso. {novoCep.Cep} - {novoCep.Logradouro}, {novoCep.Bairro} - {novoCep.Localidade}/{novoCep.Uf}");

            }
            catch (Exception ex)
            {

                return BadRequest("Ocorreu um erro ao buscar o CEP." + ex.Message);
            }
        }



        // DELETE: api/CEPControllers/5
        ///<summary>
        /// Apaga do banco todos os endereços com o CEP informado. 
        /// </summary>
        /// <remarks>
        /// Informe o CEP no formato XXXXXXXX sem hífen. Caso não haja CEPs cadastrados, o método retorna uma lista vazia.
        /// </remarks>
        [HttpDelete("{Cep}")]
        public async Task<IActionResult> Delete(string Cep)
        {
            Cep = Cep.Substring(0, 5) + "-" + Cep.Substring(5);
            try
            {
                var cepToDelete = await _context.CEP.Where(c => c.Cep == Cep).FirstOrDefaultAsync();
                if (cepToDelete == null)
                {
                    return NotFound();
                }

                _context.CEP.Remove(cepToDelete);
                await _context.SaveChangesAsync();

                return Ok(cepToDelete);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        


        private bool CEPControllerExists(string Cep)
        {
            return (_context.CEP?.Any(e => e.Cep == Cep)).GetValueOrDefault();
        }





    }
}
