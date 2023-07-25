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
    internal class CalOrientation : CalculixKeyword
    {
        // Variables                                                                                                                
        private FeOrientation _orientation;

        public FeOrientation Orientation
        {
            get { return _orientation; }
            set { _orientation = value; }
        }


        // Properties                                                                                                               


        // Constructor                                                                                                              
        public CalOrientation(FeOrientation orientation)
        {
            _orientation = orientation;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            //
            return string.Format("*Orientation, Name={0}{1}", _orientation.Name, Environment.NewLine, _orientation.Distribution.Name, Environment.NewLine);
        }
        public override string GetDataString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0}{1}", _orientation.Distribution.Name, Environment.NewLine);

            return sb.ToString();
        }
    }
}
