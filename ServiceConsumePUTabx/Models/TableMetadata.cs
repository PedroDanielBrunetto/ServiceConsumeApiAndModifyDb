using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServiceConsumePUTabx.Models
{
    public class TableMetadata
    {
        public string Nome { get; set; }
        public string Rotulo { get; set; }
        public string Descricao { get; set; }
        public List<Column> Campos { get; set; }
    }
}