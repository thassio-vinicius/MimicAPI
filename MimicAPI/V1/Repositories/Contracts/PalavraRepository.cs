using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MimicAPI.Database;
using MimicAPI.Helpers;
using MimicAPI.V1.Models;

namespace MimicAPI.V1.Repositories.Contracts
{
    public class PalavraRepository : IPalavraRepository
    {
        private readonly MimicContext _mimicContext;

        public PalavraRepository(MimicContext mimicContext)
        {
            _mimicContext = mimicContext;
           
        }

        public PaginationList<Palavra> ObterTodasPalavras(PalavraUrlQuery query)
        {
           
            var lista = new PaginationList<Palavra>();
            var item = _mimicContext.Palavras.AsNoTracking().AsQueryable();

            if (query.data.HasValue)
            {
                item = item.Where(a => a.Criado > query.data.Value || a.Atualizado > query.data.Value);
            }

            if (query.pagNumero.HasValue)
            {
                var quantidadeTotalRegistros = item.Count();
                item = item.Skip((query.pagNumero.Value - 1) * query.pagRegistro.Value).Take(query.pagRegistro.Value);

                var paginacao = new Paginacao();
                paginacao.NumeroPagina = query.pagNumero.Value;
                paginacao.RegistroPorPagina = query.pagRegistro.Value;
                paginacao.TotalRegistros = quantidadeTotalRegistros;
                paginacao.TotalPaginas = (int)Math.Ceiling((double)quantidadeTotalRegistros / query.pagRegistro.Value);

                lista.Paginacao = paginacao;
            }

            lista.Results.AddRange(item.ToList());

            return lista;
        }

            public Palavra Obter1Palavra(int id)
        {
            return _mimicContext.Palavras.AsNoTracking().FirstOrDefault(a => a.Id == id);
        }

        public void Cadastrar(Palavra palavra)
        {
            _mimicContext.Palavras.Add(palavra);
            _mimicContext.SaveChanges();
        }

        public void Atualizar(Palavra palavra)
        {
            _mimicContext.Palavras.Update(palavra);
            _mimicContext.SaveChanges();
        }

        public void Deletar(int id)
        {
            var palavra = Obter1Palavra(id);
            _mimicContext.Palavras.Update(palavra);
            _mimicContext.SaveChanges();
        }   
    }
}
