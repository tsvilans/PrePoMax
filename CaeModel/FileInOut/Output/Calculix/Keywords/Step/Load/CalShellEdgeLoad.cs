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
    internal class CalShellEdgeLoad : CalculixKeyword
    {
        // Variables                                                                                                                
        private ShellEdgeLoad _load;
        private FeSurface _surface;
        private ComplexLoadTypeEnum _complexLoadType;


        // Properties                                                                                                               


        // Constructor                                                                                                              
        public CalShellEdgeLoad(ShellEdgeLoad load, FeSurface surface, ComplexLoadTypeEnum complexLoadType)
        {
            _load = load;
            _surface = surface;
            _complexLoadType = complexLoadType;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("** Name: " +_load.Name);
            string amplitude = "";
            if (_load.AmplitudeName != Load.DefaultAmplitudeName) amplitude = ", Amplitude=" + _load.AmplitudeName;
            //
            string loadCase = GetComplexLoadCase(_complexLoadType);
            //
            sb.AppendFormat("*Dload{0}{1}{2}", amplitude, loadCase, Environment.NewLine);
            //
            return sb.ToString();
        }
        public override string GetDataString()
        {
            //*Dload
            //_obremenitev_el_surf_S3, EDNOR1, 1
            //_obremenitev_el_surf_S4, EDNOR2, 1
            StringBuilder sb = new StringBuilder();
            FeFaceName faceName;
            string faceKey = "";
            double magnitude;
            //
            double ratio = GetComplexRatio(_complexLoadType, _load.PhaseDeg);
            //
            foreach (var entry in _surface.ElementFaces)
            {
                faceName = entry.Key;
                if (_load.TwoD) throw new NotSupportedException();
                else
                {
                    if (faceName == FeFaceName.S1 || faceName == FeFaceName.S2) throw new NotSupportedException();
                    else if (faceName == FeFaceName.S3) faceKey = "EDNOR1";
                    else if (faceName == FeFaceName.S4) faceKey = "EDNOR2";
                    else if (faceName == FeFaceName.S5) faceKey = "EDNOR3";
                    else if (faceName == FeFaceName.S6) faceKey = "EDNOR4";
                }
                //
                magnitude = ratio * _load.Magnitude;
                sb.AppendFormat("{0}, {1}, {2}", entry.Value, faceKey, magnitude.ToCalculiX16String()).AppendLine();
            }
            return sb.ToString();
        }
    }
}
