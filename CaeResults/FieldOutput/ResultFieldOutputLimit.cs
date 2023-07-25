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
    public enum LimitPlotBasedOnEnum
    {
        [StandardValue("Parts", DisplayName = "Parts", Description = "Parts")]
        Parts,
        [StandardValue("ElementSets", DisplayName = "Element sets", Description = "Element sets")]
        ElementSets
    }


    [Serializable]
    public class ResultFieldOutputLimit : ResultFieldOutput
    {
        // Variables                                                                                                                
        public static readonly string AllElementsName = "All elements";
        private string _fieldName;
        private string _componentName;
        private LimitPlotBasedOnEnum _limitPlotBasedOn;
        private OrderedDictionary<string, double> _itemNameLimit;


        // Properties                                                                                                               
        public string FieldName { get { return _fieldName; } set { _fieldName = value; } }
        public string ComponentName { get { return _componentName; } set { _componentName = value; } }
        public LimitPlotBasedOnEnum LimitPlotBasedOn
        {
            get { return _limitPlotBasedOn; }
            set { _limitPlotBasedOn = value; }
        }
        public OrderedDictionary<string, double> ItemNameLimit
        {
            get { return _itemNameLimit; }
            set { _itemNameLimit = value; }
        }


        // Constructors                                                                                                             
        public ResultFieldOutputLimit(string name, string filedName, string componentName)
            : base(name)
        {
            _fieldName = filedName;
            _componentName = componentName;
            _limitPlotBasedOn = LimitPlotBasedOnEnum.Parts;
            _itemNameLimit = new OrderedDictionary<string, double>("ItemNameLimit");
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
            return new string[] { FOComponentNames.Ratio, FOComponentNames.SafetyFactor };
        }
    }
}
