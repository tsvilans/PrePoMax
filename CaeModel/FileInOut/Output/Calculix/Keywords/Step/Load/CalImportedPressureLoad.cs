using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeModel;
using CaeMesh;
using CaeGlobals;
using System.Runtime.InteropServices;

namespace FileInOut.Output.Calculix
{
    [Serializable]
    internal class CalImportedPressureLoad : CalculixKeyword
    {
        // Variables                                                                                                                
        private ImportedPressure _load;
        private CLoad[] _cLoads;
        private DLoad[] _dLoads;
        private ComplexLoadTypeEnum _complexLoadType;
        private FeSurfaceFaceTypes _surfaceFaceType;


        // Properties                                                                                                               


        // Constructor                                                                                                              
        public CalImportedPressureLoad(FeModel model, ImportedPressure load, ComplexLoadTypeEnum complexLoadType)
        {
            _load = load;
            _load.ImportPressure();
            //_cLoads = model.GetNodalCLoadsFromVariablePressureLoad(_load);
            _dLoads = model.GetNodalDLoadsFromVariablePressureLoad(_load);
            _complexLoadType = complexLoadType;
            //
            _surfaceFaceType = model.Mesh.Surfaces[load.SurfaceName].SurfaceFaceTypes;
        }


        // Methods                                                                                                                  
        //public override string GetKeywordString()
        //{
        //    StringBuilder sb = new StringBuilder();
        //    sb.AppendLine("** Name: " + _load.Name);
        //    string amplitude = "";
        //    if (_load.AmplitudeName != Load.DefaultAmplitudeName) amplitude = ", Amplitude=" + _load.AmplitudeName;
        //    //
        //    string loadCase = GetComplexLoadCase(_complexLoadType);
        //    //
        //    sb.AppendFormat("*Cload{0}{1}{2}", amplitude, loadCase, Environment.NewLine);
        //    //
        //    return sb.ToString();
        //}
        //public override string GetDataString()
        //{
        //    StringBuilder sb = new StringBuilder();
        //    //
        //    double ratio = GetComplexRatio(_complexLoadType, _load.PhaseDeg);
        //    //
        //    if (_cLoads != null)
        //    {
        //        List<int> directions = new List<int>();
        //        foreach (var cLoad in _cLoads)
        //        {
        //            directions.Clear();
        //            if (cLoad.F1 != 0) directions.Add(1);
        //            if (cLoad.F2 != 0) directions.Add(2);
        //            if (cLoad.F3 != 0) directions.Add(3);
        //            //
        //            foreach (var dir in directions)
        //            {
        //                sb.AppendFormat("{0}, {1}, {2}", cLoad.NodeId, dir,
        //                                (ratio * cLoad.GetComponent(dir - 1)).ToCalculiX16String());
        //                sb.AppendLine();
        //            }
        //        }
        //    }
        //    return sb.ToString();
        //}



        public override string GetKeywordString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("** Name: " + _load.Name);
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
            StringBuilder sb = new StringBuilder();
            //
            double ratio = GetComplexRatio(_complexLoadType, _load.PhaseDeg);
            //
            if (_dLoads != null)
            {
                string faceKey = "";
                FeFaceName faceName;
                double magnitude;
                //
                foreach (var dLoad in _dLoads)
                {
                    faceName = (FeFaceName)Enum.Parse(typeof(FeFaceName), dLoad.Name);
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
                    magnitude = ratio * dLoad.Magnitude;
                    if (_surfaceFaceType == FeSurfaceFaceTypes.ShellFaces && faceName == FeFaceName.S2) magnitude *= -1;
                    //
                    sb.AppendFormat("{0}, {1}, {2}", dLoad.SurfaceName, faceKey, magnitude.ToCalculiX16String()).AppendLine();
                }
            }
            return sb.ToString();
        }
    }
}
