using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimicAPI.Helpers;
using MimicAPI.V1.Models;
using MimicAPI.V1.Models.DTO;
using MimicAPI.V1.Repositories.Contracts;
using Newtonsoft.Json;

namespace MimicAPI.V1.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class PalavrasController : ControllerBase
    {

        private readonly IMapper _mapper;
        private readonly IPalavraRepository _repository;
        public PalavrasController(IPalavraRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;

        }

        /// <summary>
        /// Operação que pega do banco de dados todas as palavras existentes
        /// </summary>
        /// <param name="query">Filtros de pesquisa</param>
        /// <returns>Listagem de palavras</returns>
        [MapToApiVersion("1.0")]
        [HttpGet("", Name = "ObterTodas")]
        public IActionResult ObterTodasPalavras([FromQuery]PalavraUrlQuery query)
        {
            var item = _repository.ObterTodasPalavras(query);

            if (item.Results.Count == 0)
            {
                return NotFound();
            }

            var lista = _mapper.Map <PaginationList<Palavra>, PaginationList<PalavraDTO>>(item);

            foreach(var palavra in lista.Results)
            { 
                palavra.Links.Add(new LinkDTO(
                "self", Url.Link("ObterPalavra", new { id = palavra.Id }), "GET"));
            }

            lista.Links.Add(new LinkDTO(
                "self", Url.Link("ObterTodas", query), "GET"));

            if (item.Paginacao != null)
            {
                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(item.Paginacao));

                if (query.pagNumero + 1 <= item.Paginacao.TotalPaginas)
                {
                    var queryString = new PalavraUrlQuery() { pagNumero = query.pagNumero + 1, pagRegistro = query.pagRegistro, data = query.data };
                    lista.Links.Add(new LinkDTO(
                    "next", Url.Link("ObterTodas", queryString), "GET"));
                }

                if (query.pagNumero - 1 > 0)
                {
                    var queryString = new PalavraUrlQuery() { pagNumero = query.pagNumero - 1, pagRegistro = query.pagRegistro, data = query.data };
                    lista.Links.Add(new LinkDTO(
                    "previous", Url.Link("ObterTodas", queryString), "GET"));
                }

            }
            return Ok(lista);
            
        }

        /// <summary>
        /// Operação que retorna, a partir do ID, uma única palavra da base de dados
        /// </summary>
        /// <param name="id">código identificador da palavra</param>
        /// <returns>um objeto do tipo palavra</returns>
        [MapToApiVersion("1.0")]
        [HttpGet("{id}", Name = "ObterPalavra")]
        public IActionResult Obter1Palavra(int id)
        {
            var obj = _repository.Obter1Palavra(id);

            if (obj == null)
            {
                return NotFound();
            }

            PalavraDTO palavraDTO = _mapper.Map<Palavra, PalavraDTO>(obj);
            palavraDTO.Links = new List<LinkDTO>();
            palavraDTO.Links.Add(
                new LinkDTO(
                "self", Url.Link("ObterPalavra", new { id = palavraDTO.Id }), "GET"));
            palavraDTO.Links.Add(
                new LinkDTO(
                "update", Url.Link("AtualizarPalavra", new { id = palavraDTO.Id }), "PUT"));
            palavraDTO.Links.Add(
                new LinkDTO(
                "delete", Url.Link("DeletarPalavra", new { id = palavraDTO.Id }), "DELETE"));


            return Ok(palavraDTO);
        }

        /// <summary>
        /// Operação que realiza o cadastro de uma palavra
        /// </summary>
        /// <param name="palavra">Um objeto palavra</param>
        /// <returns>Um objeto palavra com o seu ID</returns>
        [MapToApiVersion("1.0")]
        [Route("")]
        [HttpPost]
        public IActionResult Cadastrar([FromBody]Palavra palavra)
        {
            if (palavra == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }

            palavra.Ativo = true;
            palavra.Criado = DateTime.Now;
            _repository.Cadastrar(palavra);

            PalavraDTO palavraDTO = _mapper.Map<Palavra, PalavraDTO>(palavra);
            palavraDTO.Links.Add(new LinkDTO(
               "self", Url.Link("ObterPalavra", new { id = palavraDTO.Id }), "GET"));

            return Created($"api/palavras/{palavra.Id})", palavraDTO);
        }

        /// <summary>
        /// Operação que realiza a substituição de dados de uma palavra específica
        /// </summary>
        /// <param name="id">Código identificador da palavra a ser alterada</param>
        /// <param name="palavra">Objeto palavra com dados para alteração</param>
        /// <returns></returns>
        [MapToApiVersion("1.0")]
        [HttpPut("{id}", Name = "AtualizarPalavra")]
        public IActionResult Atualizar(int id, [FromBody]Palavra palavra)
        {
            palavra.Criado = DateTime.Now;
            var obj = _repository.Obter1Palavra(id);
            if (obj == null)
            {
                return NotFound();
            }
            if (palavra == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }
            else
            {
                palavra.Ativo = obj.Ativo;
                palavra.Criado = obj.Criado;
                palavra.Atualizado = DateTime.Now;
                palavra.Id = id;
                _repository.Atualizar(palavra);

                PalavraDTO palavraDTO = _mapper.Map<Palavra, PalavraDTO>(palavra);
                palavraDTO.Links.Add(new LinkDTO(
                   "self", Url.Link("ObterPalavra", new { id = palavraDTO.Id }), "GET"));

                return Ok(palavraDTO);
            }
        }

        /// <summary>
        /// Operação que desativa uma palavra do sistema
        /// </summary>
        /// <param name="id">Código identificador da palavra</param>
        /// <returns></returns>
        [MapToApiVersion("1.0")]
        [HttpDelete("{id}", Name = "DeletarPalavra")]
        public IActionResult Deletar(int id)
        {
            var palavra = _repository.Obter1Palavra(id);

            if (palavra == null)
            {
                return NotFound();
            }
            else
            {
                _repository.Deletar(id);


                return NoContent();
            }
        }
    }
}