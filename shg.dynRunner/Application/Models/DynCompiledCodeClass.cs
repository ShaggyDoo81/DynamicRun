using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shg.dynRunner.Application.Models
{
    public class DynClass
    {
        public string ClassName { get; set; }
        public List<DynMethod> Methods { get; set; }
    }

    public class DynMethod
    {
        public string Name { get; set; }    
        public Type? ReturnType { get; set; }
        public List<DynParameter> Parameters { get; set; }
    }

    public class DynParameter
    { 
        public Type Type { get; set; }
        public string Name { get; set; }    
    }
}
