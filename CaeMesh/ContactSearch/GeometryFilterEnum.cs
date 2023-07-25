using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeMesh.ContactSearchNamespace
{
    [Flags]
    public enum GeometryFilterEnum
    {
        None = 0,
        Solid = 1,
        Shell = 2,
        ShellEdge = 4,
        IgnoreHidden = 8
    }
}
