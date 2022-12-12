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
    internal class CalContactPair : CalculixKeyword
    {
        // Variables                                                                                                                
        private ContactPair _contactPair;


        // Properties                                                                                                               


        // Constructor                                                                                                              
        public CalContactPair(ContactPair contactPair)
        {
            _contactPair = contactPair;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            //*CONTACT PAIR, INTERACTION=INTERACTION-1, TYPE=NODE TO SURFACE, SMALL SLIDING, ADJUST=0.001
            //slaveSurfaceName, masterSurfaceName
            StringBuilder sb = new StringBuilder();
            //
            sb.AppendFormat("*Contact pair, Interaction={0}, Type=", _contactPair.SurfaceInteractionName );
            if (_contactPair.Method == ContactPairMethod.NodeToSurface) sb.Append("Node to surface");
            else if (_contactPair.Method == ContactPairMethod.SurfaceToSurface) sb.Append("Surface to surface");
            else if (_contactPair.Method == ContactPairMethod.Mortar) sb.Append("Mortar");
            else if (_contactPair.Method == ContactPairMethod.LinMortar) sb.Append("LinMortar");
            else if (_contactPair.Method == ContactPairMethod.PGLinMortar) sb.Append("PGLinMortar");
            else throw new NotSupportedException();
            //
            if (_contactPair.Method == ContactPairMethod.NodeToSurface && _contactPair.SmallSliding) sb.Append(", Small sliding");
            //
            if (_contactPair.Adjust)
            {
                sb.AppendFormat(", Adjust=");
                if (double.IsNaN(_contactPair.AdjustmentSize)) sb.Append("0");
                else sb.AppendFormat("{0}", _contactPair.AdjustmentSize.ToCalculiX16String());
            }
            //
            sb.AppendLine();
            return sb.ToString();
        }
        public override string GetDataString()
        {
            return string.Format("{0}, {1}{2}", _contactPair.SlaveRegionName, _contactPair.MasterRegionName, Environment.NewLine);
        }
    }
}
