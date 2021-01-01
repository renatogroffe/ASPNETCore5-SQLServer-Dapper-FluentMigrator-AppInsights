using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Dapper;
using APIIndicadores.Models;

namespace APIIndicadores.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IndicadoresController : ControllerBase
    {
        private readonly ILogger<IndicadoresController> _logger;

        public IndicadoresController(ILogger<IndicadoresController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<Indicador> GetAll(
            [FromServices]IConfiguration configuration)
        {
            using (SqlConnection conexao = new (
                configuration.GetConnectionString("BaseIndicadores")))
            {
                var dados = conexao.Query<Indicador>(
                    "SELECT * FROM dbo.Indicadores");
                _logger.LogInformation($"{nameof(GetAll)}: {dados.Count()} " +
                    "registro(1) encontrado(s)");
                return dados;
            }
        }    

        [HttpGet("{indicador}")]
        public ActionResult<Indicador> GetIndicador(
            [FromServices]IConfiguration configuration,
            string indicador)
        {
            Indicador resultado = null;

            using (SqlConnection conexao = new (
                configuration.GetConnectionString("BaseIndicadores")))
            {
                resultado = conexao.QueryFirstOrDefault<Indicador>(
                    "SELECT * FROM dbo.Indicadores " +
                    "WHERE Sigla = @siglaIndicador",
                    new { siglaIndicador = indicador });
            }

            if (resultado != null)
            {
                _logger.LogInformation(
                    $"{nameof(GetIndicador)}: encontrado o indicador {resultado.Sigla}");
                return resultado;
            }
            else
            {
                string msgNotFound = $"{indicador} - indicador inválido ou inexistente.";
                _logger.LogError($"{nameof(GetIndicador)}: {msgNotFound}");
                
                return NotFound(
                    new {
                            Mensagem = msgNotFound
                        });
            }
        }
    }
}