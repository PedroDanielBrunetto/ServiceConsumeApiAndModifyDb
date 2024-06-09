using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServiceConsumePUTabx.Models
{
    public class Column
    {
        public string Nome { get; set; }
        public string Rotulo { get; set; }
        public string Descricao { get; set; }
        public string Tipo { get; set; }
        public int? Tamanho { get; set; }
        public int? CasasDecimais { get; set; }
        public string Formato { get; set; }
        public bool Obrigatorio { get; set; }
        public bool AutoNumerado { get; set; }
        public bool ChaveNegocio { get; set; }
        public bool CampoEstrangeiro { get; set; }
        public bool RestricaoUnicidade { get; set; }
        public bool PossuiDominio { get; set; }
        public string NomeTabelaEstrangeira { get; set; }
        public string Dominios { get; set; }
    }
}