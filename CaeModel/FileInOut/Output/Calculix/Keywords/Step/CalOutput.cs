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
    internal class CalOutput : CalculixKeyword
    {
        // Variables                                                                                                                
        private int _outputFrequency;


        // Properties                                                                                                               


        // Constructor                                                                                                              
        public CalOutput(int outputFrequency)
        {
            _outputFrequency = outputFrequency;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            string frequency = ", Frequency=";
            if (_outputFrequency == int.MinValue) frequency += "1";
            else frequency += _outputFrequency;
            //
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("*Output{0}{1}", frequency, Environment.NewLine);
            return sb.ToString();
        }
        public override string GetDataString()
        {
            return "";
        }
    }
}
