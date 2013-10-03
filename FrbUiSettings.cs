using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FrbUi
{
    public static class FrbUiSettings
    {
        public static Version Version
        {
            get
            {
                return Assembly.GetAssembly(typeof (FrbUiSettings))
                               .GetName()
                               .Version;
            }
        }
    }
}
