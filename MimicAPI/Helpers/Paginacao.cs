using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MimicAPI.Helpers
{
    public class Paginacao
    {
        public int NumeroPagina { get; set; }
        public int RegistroPorPagina { get; set; }
        public int TotalRegistros { get; set; }
        public int TotalPaginas { get; set; }

        public Paginacao()
        {
        }

        public Paginacao(int numeroPagina, int paginaRegistro, int totalRegistros, int totalPaginas)
        {
            NumeroPagina = numeroPagina;
            RegistroPorPagina = paginaRegistro;
            TotalRegistros = totalRegistros;
            TotalPaginas = totalPaginas;
        }
    }
}
