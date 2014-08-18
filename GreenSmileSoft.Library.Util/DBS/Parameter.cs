using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenSmileSoft.Library.Util.DBS
{
    [Serializable]
    public class Parameter
    {
        public string Name { get; set; }
        public object Value { get; set; }
    }
}
