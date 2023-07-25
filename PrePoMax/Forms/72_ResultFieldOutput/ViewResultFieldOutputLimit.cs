using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;
using DynamicTypeDescriptor;
using CaeResults;
using CaeModel;

namespace PrePoMax
{
    [Serializable]
    public class ViewResultFieldOutputLimit : ViewResultFieldOutput
    {
        // Variables                                                                                                                
        private ResultFieldOutputLimit _resultFieldOutput;
        private List<LimitPartDataPoint> _partPoints;
        private List<LimitElementSetDataPoint> _elementSetPoints;
        private Dictionary<string, string[]> _filedNameComponentNames;


        // Properties                                                                                                               
        public override string Name { get { return _resultFieldOutput.Name; } set { _resultFieldOutput.Name = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(1, 10, "Field name")]
        [DescriptionAttribute("Filed name for the field output.")]
        public string FieldName
        {
            get { return _resultFieldOutput.FieldName; }
            set
            {
                if (_resultFieldOutput.FieldName != value)
                {
                    _resultFieldOutput.FieldName = value;
                    UpdateComponents();
                }
            }
        }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(2, 10, "Component name")]
        [DescriptionAttribute("Component name for the field output.")]
        public string ComponentName
        {
            get { return _resultFieldOutput.ComponentName; }
            set { _resultFieldOutput.ComponentName = value; }
        }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(2, 10, "Limit based on")]
        [DescriptionAttribute("Select how the limit values will be defined for the field output.")]
        public LimitPlotBasedOnEnum LimitPlotBasedOn
        {
            get { return _resultFieldOutput.LimitPlotBasedOn; }
            set { _resultFieldOutput.LimitPlotBasedOn = value; }
        }
        //
        [Browsable(false)]
        public List<LimitPartDataPoint> PartPoints
        {
            get { return _partPoints; }
            set { _partPoints = value; }
        }
        //
        [Browsable(false)]
        public List<LimitElementSetDataPoint> ElementSetPoints
        {
            get { return _elementSetPoints; }
            set { _elementSetPoints = value; }
        }
        [Browsable(false)]
        public object DataPoints
        {
            get
            {
                if (LimitPlotBasedOn == LimitPlotBasedOnEnum.Parts) return _partPoints;
                if (LimitPlotBasedOn == LimitPlotBasedOnEnum.ElementSets) return _elementSetPoints;
                else throw new NotSupportedException();
            }
        }


        // Constructors                                                                                                             
        public ViewResultFieldOutputLimit(ResultFieldOutputLimit resultFieldOutput, string[] partNames,
                                                 string[] elementSetNames, ref bool propertyChanged)
        {
            // The order is important
            _resultFieldOutput = resultFieldOutput;
            // Parts
            bool valid = true;
            double limit;
            _partPoints = new List<LimitPartDataPoint>();
            foreach (var partName in partNames)
            {
                limit = 0;
                valid &= _resultFieldOutput.ItemNameLimit.TryGetValue(partName, out limit);
                _partPoints.Add(new LimitPartDataPoint(partName, limit));
            }
            // Element sets
            if (elementSetNames.Length == 0) elementSetNames = new string[] { ResultFieldOutputLimit.AllElementsName };
            //
            _elementSetPoints = new List<LimitElementSetDataPoint>();
            foreach (var elementSetName in elementSetNames)
            {
                limit = 0;
                valid &= _resultFieldOutput.ItemNameLimit.TryGetValue(elementSetName, out limit);
                _elementSetPoints.Add(new LimitElementSetDataPoint(elementSetName, limit));
            }
            //
            if (!valid) propertyChanged = true;
            //
            _dctd = ProviderInstaller.Install(this);
        }


        // Methods                                                                                                                  
        public override ResultFieldOutput GetBase()
        {
            _resultFieldOutput.ItemNameLimit.Clear();
            //
            if (LimitPlotBasedOn == LimitPlotBasedOnEnum.Parts)
            {
                foreach (var point in _partPoints)
                {
                    _resultFieldOutput.ItemNameLimit.Add(point.PartName, point.Limit);
                }
            }
            else if (LimitPlotBasedOn == LimitPlotBasedOnEnum.ElementSets)
            {
                foreach (var point in _elementSetPoints)
                {
                    _resultFieldOutput.ItemNameLimit.Add(point.ElementSetName, point.Limit);
                }
            }
            else throw new NotSupportedException();
            //
            return _resultFieldOutput;
        }
        public void PopulateDropDownLists(Dictionary<string, string[]> filedNameComponentNames)
        {
            _filedNameComponentNames = filedNameComponentNames;
            _dctd.PopulateProperty(nameof(FieldName), _filedNameComponentNames.Keys.ToArray());
            UpdateComponents();
        }
        private void UpdateComponents()
        {
            string[] componentNames;
            if (_filedNameComponentNames != null && _filedNameComponentNames.TryGetValue(FieldName, out componentNames) &&
                componentNames != null && componentNames.Length > 0)
            {
                _dctd.PopulateProperty(nameof(ComponentName), componentNames);
                if (!componentNames.Contains(ComponentName)) ComponentName = componentNames[0];
            }
        }
        private void UpdateVisibility()
        {
           
        }
    }



   
}
