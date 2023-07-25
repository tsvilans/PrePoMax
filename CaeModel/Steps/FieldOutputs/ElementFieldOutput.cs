using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;

namespace CaeModel
{
    [Serializable]
    [Flags]
    public enum ElementFieldVariable
    {
        // Must start at 1 for the UI to work
        S = 1,
        PHS = 2,
        E = 4,
        ME = 8,
        PEEQ = 16,
        ENER = 32,
        // Thermal
        HFL = 64,
        // Error
        ERR = 128,
        HER = 256,
        ZZS = 512
    }

    [Serializable]
    public enum ElementFieldOutputOutputEnum
    {
        Default,
        [DynamicTypeDescriptor.StandardValue("TwoD", DisplayName = "2D")]
        TwoD,
        [DynamicTypeDescriptor.StandardValue("ThreeD", DisplayName = "3D")]
        ThreeD
    }

    [Serializable]
    public class ElementFieldOutput : FieldOutput
    {
        // Variables                                                                                                                
        private ElementFieldOutputOutputEnum _output;
        private ElementFieldVariable _variables;


        // Properties                                                                                                               
        public ElementFieldOutputOutputEnum Output { get { return _output; } set { _output = value; } }
        public ElementFieldVariable Variables { get { return _variables; } set { _variables = value; } }


        // Constructors                                                                                                             
        public ElementFieldOutput(string name, ElementFieldVariable variables)
            : base(name) 
        {
            _variables |= variables;
            _output = ElementFieldOutputOutputEnum.Default;
        }


        // Methods                                                                                                                  
    }
}
