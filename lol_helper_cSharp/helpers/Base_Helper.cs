using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lol_helper_cSharp.helpers
{
    public class Base_Helper
    {
        public riot_apis.RiotApiManager ApiManager = riot_apis.RiotApiManager.GetInstance();
        public virtual async Task<bool> Run() {
            return true;
        }
        public virtual void GetConfigForConfigFiles()
        {
        }
    }
}
