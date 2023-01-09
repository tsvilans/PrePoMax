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
    internal class CalInitialVelocity : CalculixKeyword
    {
        // Variables                                                                                                                
        private InitialVelocity _initialVelocity;


        // Properties                                                                                                               


        // Constructor                                                                                                              
        public CalInitialVelocity(FeModel model, InitialVelocity initialVelocity)
        {
            _initialVelocity = initialVelocity;
            //
            if (initialVelocity.NodeSetName == null) throw new NotSupportedException();
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("** Name: " + _initialVelocity.Name);
            sb.AppendLine("*Initial conditions, Type=Velocity");
            return sb.ToString();
        }
        public override string GetDataString()
        {
            StringBuilder sb = new StringBuilder();
            if (_initialVelocity.V1 != 0)
                sb.AppendFormat("{0}, {1}, {2}{3}", _initialVelocity.NodeSetName, 1, _initialVelocity.V1.ToCalculiX16String(),
                                Environment.NewLine);
            if (_initialVelocity.V2 != 0)
                sb.AppendFormat("{0}, {1}, {2}{3}", _initialVelocity.NodeSetName, 2, _initialVelocity.V2.ToCalculiX16String(),
                                Environment.NewLine);
            if (_initialVelocity.V3 != 0)
                sb.AppendFormat("{0}, {1}, {2}{3}", _initialVelocity.NodeSetName, 3, _initialVelocity.V3.ToCalculiX16String(),
                                Environment.NewLine);
            return sb.ToString();
        }
    }
}
