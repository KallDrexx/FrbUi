using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrbUi
{
    public interface IDisableable
    {
        bool Enabled { get; set; }

        string DisabledAnimationChainName { get; set; }
    }
}
