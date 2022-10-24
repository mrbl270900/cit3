using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitServer
{
    internal class Request
    {
        public String? Method { get; set; }
        public String? Path { get; set; }
        public String? Date { get; set; }
        public String? Body { get; set; }

    }
}
