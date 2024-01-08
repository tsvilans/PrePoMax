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
    internal class CalElOutput : CalculixKeyword
    {
        // Variables                                                                                                                
        private ElementFieldOutput _elementFieldOutput;


        // Properties                                                                                                               


        // Constructor                                                                                                              
        public CalElOutput(ElementFieldOutput elementFieldOutput)
        {
            _elementFieldOutput = elementFieldOutput;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            string lastIterations = _elementFieldOutput.LastIterations ? ", Last iterations" : "";
            string contactElements = _elementFieldOutput.ContactElements ? ", Contact elements" : "";
            string output = "";
            if (_elementFieldOutput.Output == ElementFieldOutputOutputEnum.TwoD) output += ", Output=2D";
            else if (_elementFieldOutput.Output == ElementFieldOutputOutputEnum.ThreeD) output += ", Output=3D";
            //
            return string.Format("*Element output{0}{1}{2}{3}", lastIterations, contactElements, output,
                                 Environment.NewLine);
        }
        public override string GetDataString()
        {
            return string.Format("{0}{1}", _elementFieldOutput.Variables.ToString(), Environment.NewLine);
        }
    }
}
