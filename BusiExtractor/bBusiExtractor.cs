using DataExtractor.WebService;
using EntiExtractor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusiExtractor
{
    public class bBusiExtractor
    {
        dwsEmpresa empresaData = new dwsEmpresa();


        public List<bEntiEmpresa> ListadoEmpresa()
        {
            return empresaData.ListadoEmpresa();
        }
    }
}
