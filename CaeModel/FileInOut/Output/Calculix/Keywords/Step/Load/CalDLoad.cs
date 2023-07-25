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
    internal class CalDLoad : CalculixKeyword
    {
        // Variables                                                                                                                
        private DLoad _load;
        private FeSurface _surface;
        private ComplexLoadTypeEnum _complexLoadType;


        // Properties                                                                                                               


        // Constructor                                                                                                              
        public CalDLoad(DLoad load, FeSurface surface, ComplexLoadTypeEnum complexLoadType)
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
            // *Dload
            // _obremenitev_el_surf_S3, P3, 1
            // _obremenitev_el_surf_S4, P4, 1
            // _obremenitev_el_surf_S1, P1, 1
            // _obremenitev_el_surf_S2, P2, 1
            StringBuilder sb = new StringBuilder();
            FeFaceName faceName;
            string faceKey = "";
            double magnitude;
            foreach (var entry in _surface.ElementFaces)
            {
                faceName = entry.Key;
                if (_load.TwoD)
                {
                    if (faceName == FeFaceName.S1 || faceName == FeFaceName.S2) throw new NotSupportedException();
                    else if (faceName == FeFaceName.S3) faceKey = "P1";
                    else if (faceName == FeFaceName.S4) faceKey = "P2";
                    else if (faceName == FeFaceName.S5) faceKey = "P3";
                    else if (faceName == FeFaceName.S6) faceKey = "P4";
                }
                else
                {
                    faceKey = "P" + faceName.ToString()[1];
                }
                //
                double ratio = GetComplexRatio(_complexLoadType, _load.PhaseDeg);
                //
                magnitude = ratio * _load.Magnitude;
                if (_surface.SurfaceFaceTypes == FeSurfaceFaceTypes.ShellFaces && faceName == FeFaceName.S2) magnitude *= -1;
                //
                sb.AppendFormat("{0}, {1}, {2}", entry.Value, faceKey, magnitude.ToCalculiX16String()).AppendLine();
            }
            return sb.ToString();
        }
    }
}
