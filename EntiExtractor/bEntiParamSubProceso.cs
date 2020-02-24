using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace EntiExtractor
{
    public class bEntiParamSubProceso
    {
        public bEntiServExtractorDet oServExtracDet { get; set; }
        public string CveDatoEntrada { get; set; }
        public string ClaveEmpresa { get; set; }
        public int Ejercicio { get; set; }
        public int Periodo { get; set; }
        public string Usuario { get; set; }
        public string Ambiente { get; set; }
        public int IdConfExtraccion { get; set; }
    }
}
