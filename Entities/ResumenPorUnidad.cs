using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class ResumenPorUnidad
    {
        public string Unidad { get; set; }
        public string Calificacion { get; set; }
        public int TotalLamadas { get; set; }
        public decimal Porcentaje { get; set; }
        public double NiTotal { get; set; }
    }
}
