using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class ResumenPorAsesor : ResumenPorUnidad
    {
        public string Ejecutivo { get; set; }
        public int TotalInvalidas { get; set; }
        public decimal PorcentajeInvalidas { get; set; }
    }
}
