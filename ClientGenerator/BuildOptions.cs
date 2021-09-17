using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientGenerator
{
    public class BuildOptions
    {
        public string OutputPath { get; set; }
        public string[] AssemblyInformation { get; set; }
        public string StrInject { get; set; }
    }
}
