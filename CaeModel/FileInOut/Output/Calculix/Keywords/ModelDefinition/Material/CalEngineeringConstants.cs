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
    internal class CalEngineeringConstants : CalculixKeyword
    {
        // Variables                                                                                                                
        private EngineeringConstants _engineeringConstants;
        private bool _temperatureDependent;


        // Properties                                                                                                               


        // Constructor                                                                                                              
        public CalEngineeringConstants(EngineeringConstants engineeringConstants, bool temperatureDependent)
        {
            _engineeringConstants = engineeringConstants;
            _temperatureDependent = temperatureDependent;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            return string.Format("*Elastic, Type=Engineering Constants{0}", Environment.NewLine);
        }
        public override string GetDataString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0:0.##E+00}, {1:0.##E+00}, {2:0.##E+00}, {3}, {4}, {5}, {6:0.##E+00}, {7:0.##E+00},{8}",
                _engineeringConstants.YoungsModuli[0], _engineeringConstants.YoungsModuli[1], _engineeringConstants.YoungsModuli[2],
                _engineeringConstants.PoissonsRatios[0], _engineeringConstants.PoissonsRatios[1], _engineeringConstants.PoissonsRatios[2],
                _engineeringConstants.ShearModuli[0], _engineeringConstants.ShearModuli[1], Environment.NewLine
                );
            sb.AppendFormat("{0:0.##E+00}{1}", _engineeringConstants.ShearModuli[2], Environment.NewLine);

            return sb.ToString();
        }
    }
}
