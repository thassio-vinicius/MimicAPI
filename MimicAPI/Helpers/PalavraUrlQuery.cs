using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MimicAPI.Helpers
{
    public class PalavraUrlQuery
    {
        public DateTime? data { get; set; }
        public int? pagNumero { get; set; }
        public int? pagRegistro { get; set; }

        public PalavraUrlQuery()
        {
        }

        public PalavraUrlQuery(DateTime? data, int? pagNumero, int? pagRegistro)
        {
            this.data = data;
            this.pagNumero = pagNumero;
            this.pagRegistro = pagRegistro;
        }
    }
}
