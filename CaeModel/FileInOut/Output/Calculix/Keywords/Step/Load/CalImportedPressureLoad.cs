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
    internal class CalImportedPressureLoad : CalculixKeyword
    {
        // Variables                                                                                                                
        private ImportedPressure _load;
        private CLoad[] _cLoads;


        // Properties                                                                                                               


        // Constructor                                                                                                              
        public CalImportedPressureLoad(FeModel model, ImportedPressure load)
        {
            _load = load;
            //
            _load.ImportPressure();
            _cLoads = model.GetNodalLoadsFromVariablePressureLoad(_load);
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("** Name: " + _load.Name);
            string amplitude = "";
            if (_load.AmplitudeName != Load.DefaultAmplitudeName) amplitude = ", Amplitude=" + _load.AmplitudeName;
            //
            sb.AppendFormat("*Cload{0}{1}", amplitude, Environment.NewLine);
            //
            return sb.ToString();
        }
        public override string GetDataString()
        {
            StringBuilder sb = new StringBuilder();
            if (_cLoads != null)
            {
                List<int> directions = new List<int>();
                foreach (var cLoad in _cLoads)
                {
                    directions.Clear();
                    if (cLoad.F1 != 0) directions.Add(1);
                    if (cLoad.F2 != 0) directions.Add(2);
                    if (cLoad.F3 != 0) directions.Add(3);
                    //
                    foreach (var dir in directions)
                    {
                        sb.AppendFormat("{0}, {1}, {2}", cLoad.NodeId, dir, cLoad.GetComponent(dir - 1).ToCalculiX16String());
                        
                        sb.AppendLine();
                    }
                }
            }
            return sb.ToString();
        }
    }
}
