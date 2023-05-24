﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeModel;
using CaeMesh;

namespace FileInOut.Output.Calculix
{
    [Serializable]
    internal class CalContactFile : CalculixKeyword
    {
        // Variables                                                                                                                
        private ContactFieldOutput _contactFieldOutput;


        // Properties                                                                                                               


        // Constructor                                                                                                              
        public CalContactFile(ContactFieldOutput contactFieldOutput)
        {
            _contactFieldOutput = contactFieldOutput;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            //string frequency = _contactFieldOutput.Frequency > 1 ? ", Frequency=" + _contactFieldOutput.Frequency : "";
            string frequency = ", Frequency=" + _contactFieldOutput.Frequency;
            string lastIterations = _contactFieldOutput.LastIterations ? ", Last iterations" : "";
            //
            return string.Format("*Contact file{0}{1}{2}", frequency, lastIterations, Environment.NewLine);
        }
        public override string GetDataString()
        {
            return string.Format("{0}{1}", _contactFieldOutput.Variables.ToString(), Environment.NewLine);
        }
    }
}
