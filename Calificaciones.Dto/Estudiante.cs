using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calificaciones.Dto
{
    public class Estudiante
    {
        public string Nombres { get; set; }

        public string ApellidoPaterno { get; set; }

        public string ApellidoMaterno { get; set; }

        public int Grado { get; set; }

        public string Grupo { get; set; }

        public decimal Calificacion { get; set; }
    }
}
