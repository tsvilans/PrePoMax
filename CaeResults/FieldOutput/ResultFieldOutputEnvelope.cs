using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;
using DynamicTypeDescriptor;

namespace CaeResults
{
    [Serializable]
    public enum EnvelopeTypeEnum
    {
        [StandardValue("Min", DisplayName = "Min", Description = "Min")]
        Min,
        [StandardValue("Max", DisplayName = "Max", Description = "Max")]
        Max,
        [StandardValue("Average", DisplayName = "Average", Description = "Average")]
        Average
    }


    [Serializable]
    public class ResultFieldOutputEnvelope : ResultFieldOutput
    {
        // Variables                                                                                                                
        private string _fieldName;
        private string _componentName;
        //private EnvelopeTypeEnum _envelopeType;


        // Properties                                                                                                               
        public string FieldName { get { return _fieldName; } set { _fieldName = value; } }
        public string ComponentName { get { return _componentName; } set { _componentName = value; } }
        //public EnvelopeTypeEnum EnvelopeType { get { return _envelopeType; } set { _envelopeType = value; } }


        // Constructors                                                                                                             
        public ResultFieldOutputEnvelope(string name, string filedName, string componentName)
            : base(name)
        {
            _fieldName = filedName;
            _componentName = componentName;
            //_envelopeType = EnvelopeTypeEnum.Max;
        }


        // Methods                                                                                                                  
        public override string[] GetParentFieldNames()
        {
            return new string[] { _fieldName };
        }
        public override string[] GetParentComponentNames()
        {
            return new string[] { _componentName };
        }
        public override string[] GetComponentNames()
        {
            return new string[] { FOComponentNames.Max, FOComponentNames.Min, FOComponentNames.Average };
        }
    }
}
