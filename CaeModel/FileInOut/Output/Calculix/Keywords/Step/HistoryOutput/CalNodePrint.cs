using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeModel;
using CaeMesh;

namespace FileInOut.Output.Calculix
{
    [Serializable]
    internal class CalNodePrint : CalculixKeyword
    {
        // Variables                                                                                                                
        private string _regionName;
        private readonly NodalHistoryOutput _nodalHistoryOutput;


        // Properties                                                                                                               


        // Constructors                                                                                                             
        public CalNodePrint(FeModel model, NodalHistoryOutput nodalHistoryOutput)
        {
            _nodalHistoryOutput = nodalHistoryOutput;   // set this first
            //
            _regionName = ", Nset=";
            if (_nodalHistoryOutput.RegionType == CaeGlobals.RegionTypeEnum.NodeSetName)
                _regionName += _nodalHistoryOutput.RegionName;
            else if (_nodalHistoryOutput.RegionType == CaeGlobals.RegionTypeEnum.SurfaceName)
                _regionName += model.Mesh.Surfaces[_nodalHistoryOutput.RegionName].NodeSetName;
            else if (_nodalHistoryOutput.RegionType == CaeGlobals.RegionTypeEnum.Selection)
            { }
            else throw new NotSupportedException();
        }
        public CalNodePrint(FeModel model, NodalHistoryOutput nodalHistoryOutput, string regionName)
        {
            _regionName = ", Nset=" + regionName;
            _nodalHistoryOutput = nodalHistoryOutput;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            string totals = "";
            if (_nodalHistoryOutput.TotalsType == TotalsTypeEnum.Yes) totals = ", Totals=Yes";
            else if (_nodalHistoryOutput.TotalsType == TotalsTypeEnum.Only) totals = ", Totals=Only";
            return string.Format("*Node print{0}{1}{2}", _regionName, totals, Environment.NewLine);
        }
        public override string GetDataString()
        {
            return string.Format("{0}{1}", _nodalHistoryOutput.Variables.ToString(), Environment.NewLine);
        }
    }
}
