using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrePoMax
{
    [Serializable]
    [Flags]
    public enum UpdateType
    {
        Check = 1,
        DrawModel = 2,
        DrawResults = 4,
        ResetCamera = 8,
        RedrawSymbols = 16
    }
}
