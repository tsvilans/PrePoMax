using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeResults
{
    [Serializable]
    enum FrdFeDescriptorId
    {
        SolidLinearHexahedron = 1,
        SolidLinearWedge = 2,
        SolidLinearTetrahedron = 3,
        SolidParabolicHexahedron = 4,
        SolidParabolicWedge = 5,
        SolidParabolicTetrahedron = 6,
        ShellLinearTriangle = 7,
        ShellParabolicTriangle = 8,
        ShellLinearQuadrilateral = 9,
        ShellParabolicQuadrilateral = 10,
        //
        BeamLinear = 11
    }


}