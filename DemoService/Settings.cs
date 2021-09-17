using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoService
{
    public static class Settings
    {
#if DEBUG
        public static string STR_INJECT = "-- Debug Mode Static String --";
#else
        public static string STR_INJECT = "";
#endif
        //public static string STR_INJECT = "";
    }
}
