using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntiExtractor
{
    public class bEntiServExtractorDet
    {
        public enum lnEstusEvento : int
        {
            EnEjecucion = 0,
            ProcesadoError = 1,
            Procesado = 2
        }

        public int IdDatosEntrada { get; set; }
        public int IdEmpresa { get; set; }
        public int IdAlarma { get; set; }
        public int IdServExt { get; set; }
        public int NumEjercicio { get; set; }
        public int NumPeriodo { get; set; }
        public string ClaveEmpresa { get; set; }
        public string ClaveLogin { get; set; }
        public string Mensaje { get; set; }
        public DateTime FechaFinEvento { get; set; }
        public DateTime HoraFinEvento { get; set; }
        public DateTime FechaAlta { get; set; }
        public DateTime HoraAlta { get; set; }
        public DateTime FechaEvento { get; set; }
        public DateTime HoraEvento { get; set; }
        public bool GeneroAlarma { get; set; }
        public string Estatus { get; set; }
        public lnEstusEvento EstusEvento { get; set; }
        
    }
}
