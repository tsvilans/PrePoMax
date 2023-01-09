using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeModel;
using CaeMesh;
using CaeGlobals;

namespace FileInOut.Output.Calculix
{
    [Serializable]
    internal class CalInitialTemperature : CalculixKeyword
    {
        // Variables                                                                                                                
        private string _regionName;
        private InitialTemperature _initialTemperature;


        // Properties                                                                                                               


        // Constructor                                                                                                              
        public CalInitialTemperature(FeModel model, InitialTemperature initialTemperature)
        {
            _initialTemperature = initialTemperature;
            //
            _regionName = "";
            if (_initialTemperature.RegionType == RegionTypeEnum.NodeSetName)
                _regionName += _initialTemperature.RegionName;
            else if (_initialTemperature.RegionType == RegionTypeEnum.SurfaceName)
                _regionName += model.Mesh.Surfaces[_initialTemperature.RegionName].NodeSetName;
            else if (_initialTemperature.RegionType == RegionTypeEnum.Selection)
            { }
            else throw new NotSupportedException();
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("** Name: " + _initialTemperature.Name);
            sb.AppendLine("*Initial conditions, Type=Temperature");
            return sb.ToString();
        }
        public override string GetDataString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0}, {1}{2}", _regionName, _initialTemperature.Temperature.ToCalculiX16String(), Environment.NewLine);
            return sb.ToString();
        }
    }
}
