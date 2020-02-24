using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntiExtractor
{
    public class bEntiServExtractor
    {
        public enum lnEstusEvento : int
        {
            EnEjecucion = 0,
            ProcesadoError = 1,
            Procesado = 2
        }

        public string Estatus { get; set; }
        public DateTime FechaEvento { get; set; }
        public DateTime HoraEvento { get; set; }
        public lnEstusEvento EstusEvento { get; set; }
        public DateTime FechaConfigurador { get; set; }
        public DateTime HoraConfigurador { get; set; }
        public int IdConfExtraccion { get; set; }
        public int RowNumber { get; set; }
        public int FolioEvento { get; set; }
        public string ClaveLogin { get; set; }   
                   
    }
}
