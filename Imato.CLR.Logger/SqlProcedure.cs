using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imato.CLR.Logger
{
    public class SqlProcedure
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public bool IsSuccess { get; set; }
        public string Error { get; set; }
    }
}