using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSReciboUnico.Models
{
    public class WARecibo
    {
        public string iacx { get; set; }
        public string referenciax { get; set; }
        public string valorx { get; set; }
        public string fechax { get; set; }
        public int idpersona { get; set; }
        public int idproducto { get; set; }
        public string usuario_wa { get; set; }
        public string password_wa { get; set; }
        public int tipopersona { get; set; }
        public string IdProgramaBeca { get; set; }
    }
}