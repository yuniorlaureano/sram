using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class ResumenPorDatoVital
    {
        public string Unidad { get; set; }
        public int TotalLlamadas { get; set; }
        public int CargoTotal { get; set; }
        public int Grabaciones { get; set; }
        public int CargoInvalido { get; set; }
        public int InvalidasVsTotalAuditadas { get; set; }
        public int CargoInvalidoVsTotalCargo { get; set; }
        public int Pregunta3 { get; set; }
        public int Pregunta4 { get; set; }
        public int Pregunta5 { get; set; }
        public int Pregunta6 { get; set; }
        public int Pregunta7 { get; set; }
        public int Pregunta8 { get; set; }
        public int Pregunta9 { get; set; }
        public int TotalDeIncidencias { get; set; }
    }
}
