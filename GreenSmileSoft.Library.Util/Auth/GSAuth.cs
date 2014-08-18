using GreenSmileSoft.Library.Util.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenSmileSoft.Library.Util.Auth
{
    public class GSAuth :NotifyPropertyChanged
    {
        public int ModuleID { get; set; }
        public string ModuleName
        {
            get
            {
                return TextTable._[GSAuthMgr.TEXTRULE, ModuleID];
            }
        }
        private GSAuthType _AuthType = GSAuthType.None;
        public GSAuthType AuthType
        {
            get
            {
                return _AuthType;
            }
            set
            {
                _AuthType = value;
                OnPropertyChanged("AuthType");
            }
        }
        public string Navigation { get; set; }
    }
}
