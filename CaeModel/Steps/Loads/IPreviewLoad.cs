using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeResults;
using CaeMesh;
using CaeGlobals;

namespace CaeModel
{
    public interface IPreviewable
    {
        FeResults GetPreview(FeMesh targetMesh, string resultName, UnitSystemType unitSystemType);
    }
}
