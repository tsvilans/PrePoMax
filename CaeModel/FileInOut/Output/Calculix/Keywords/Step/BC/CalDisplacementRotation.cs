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
    internal class CalDisplacementRotation : CalculixKeyword
    {
        // Variables                                                                                                                
        private DisplacementRotation _displacementRotation;
        private Dictionary<string, int[]> _referencePointsNodeIds;
        private string _nodeSetNameOfSurface;
        private ComplexLoadTypeEnum _complexLoadType;


        // Properties                                                                                                               


        // Constructor                                                                                                              
        public CalDisplacementRotation(DisplacementRotation displacementRotation, Dictionary<string, int[]> referencePointsNodeIds,
                                       string nodeSetNameOfSurface, ComplexLoadTypeEnum complexLoadType)
        {
            _displacementRotation = displacementRotation;
            _referencePointsNodeIds = referencePointsNodeIds;
            _nodeSetNameOfSurface = nodeSetNameOfSurface;
            _complexLoadType = complexLoadType;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("** Name: " + _displacementRotation.Name);
            string fixedBc = "";
            if (_displacementRotation.GetFixedDirections().Length > 0) fixedBc = ", Fixed";
            string amplitude = "";
            if (_displacementRotation.AmplitudeName != BoundaryCondition.DefaultAmplitudeName)
                amplitude = ", Amplitude=" + _displacementRotation.AmplitudeName;
            //
            string loadCase = GetComplexLoadCase(_complexLoadType);
            //
            sb.AppendFormat("*Boundary{0}{1}{2}{3}", fixedBc, amplitude, loadCase, Environment.NewLine);
            //
            return sb.ToString();
        }
        public override string GetDataString()
        {
            //                                                                                                  
            //                                                                                                  
            //  Changing the boundary condition definition - change the computation of GetAllZeroDisplacements  
            //                                                                                                  
            //                                                                                                  
            StringBuilder sb = new StringBuilder();
            // *Boundary
            // 6975, 1, 1, 0        node id, start DOF, end DOF, value
            bool fixedBc = true;
            int[] directions = _displacementRotation.GetFixedDirections();
            if (directions.Length == 0)
            {
                fixedBc = false;
                directions = _displacementRotation.GetConstrainedDirections();
            }
            //
            double ratio = GetComplexRatio(_complexLoadType, _displacementRotation.PhaseDeg);
            //
            double[] values = _displacementRotation.GetConstrainValues();
            string[] stringValues = new string[values.Length];
            for (int i = 0; i < values.Length; i++) stringValues[i] = (ratio * values[i]).ToCalculiX16String();
            // Node set
            if (_displacementRotation.RegionType == RegionTypeEnum.NodeSetName)
            {
                for (int i = 0; i < directions.Length; i++)
                {
                    sb.AppendFormat("{0}, {1}, {2}", _displacementRotation.RegionName, directions[i], directions[i]);
                    if (!fixedBc) sb.AppendFormat(", {0}", stringValues[i]);
                    sb.AppendLine();
                }
            }
            // Surface
            else if (_displacementRotation.RegionType == RegionTypeEnum.SurfaceName)
            {
                if (_nodeSetNameOfSurface == null) throw new ArgumentException();
                for (int i = 0; i < directions.Length; i++)
                {
                    sb.AppendFormat("{0}, {1}, {2}", _nodeSetNameOfSurface, directions[i], directions[i]);
                    if (!fixedBc) sb.AppendFormat(", {0}", stringValues[i]);
                    sb.AppendLine();
                }
            }
            // Reference point
            else if (_displacementRotation.RegionType == RegionTypeEnum.ReferencePointName)
            {
                int[] rpNodeIds = _referencePointsNodeIds[_displacementRotation.RegionName];
                for (int i = 0; i < directions.Length; i++)
                {
                    // Translational directions - first node id:        6975, 1, 1, 0
                    if (directions[i] <= 3) sb.AppendFormat("{0}, {1}, {2}", rpNodeIds[0], directions[i], directions[i]);
                    // Rotational directions - second node id:          6976, 2, 2, 0
                    else sb.AppendFormat("{0}, {1}, {2}", rpNodeIds[1], directions[i] - 3, directions[i] - 3);
                    if (!fixedBc) sb.AppendFormat(", {0}", stringValues[i]);
                    sb.AppendLine();
                }
            }
            else throw new NotSupportedException();
            //
            return sb.ToString();
        }
    }
}
