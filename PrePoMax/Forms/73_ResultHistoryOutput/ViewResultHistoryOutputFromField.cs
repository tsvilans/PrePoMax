using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;
using DynamicTypeDescriptor;
using CaeResults;

namespace PrePoMax
{
    [Serializable]
    public class ViewResultHistoryOutputFromField : ViewResultHistoryOutput
    {
        // Variables                                                                                                                
        private readonly static string _all = "All";
        private bool _complexVisible;
        private ResultHistoryOutputFromField _historyOutput;
        private Dictionary<string, string[]> _filedNameComponentNames;
        private Dictionary<string, string[]> _stepIdStepIncrementIds;


        // Properties                                                                                                               
        public override string Name { get { return _historyOutput.Name; } set { _historyOutput.Name = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(1, 10, "Field name")]
        [DescriptionAttribute("Filed name for the history output.")]
        public string FieldName
        {
            get { return _historyOutput.FieldName; }
            set
            {
                if (_historyOutput.FieldName != value)
                {
                    _historyOutput.FieldName = value;
                    UpdateComponents();
                }
            }
        }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(2, 10, "Component name")]
        [DescriptionAttribute("Component name for the history output.")]
        public string ComponentName { get { return _historyOutput.ComponentName; } set { _historyOutput.ComponentName = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(3, 10, "Complex")]
        [DescriptionAttribute("Complex component for the history output.")]
        public ComplexResultTypeEnum ComplexResultType
        {
            get { return _historyOutput.ComplexResultType; }
            set
            {
                _historyOutput.ComplexResultType = value;
                UpdateVisibility();
            }
        }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(4, 10, "Angle")]
        [DescriptionAttribute("Angle for the history output.")]
        [TypeConverter(typeof(StringAngleDegConverter))]
        public double ComplexAngleDeg
        {
            get { return _historyOutput.ComplexAngleDeg; }
            set { _historyOutput.ComplexAngleDeg = value; }
        }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(5, 10, "Step id")]
        [DescriptionAttribute("Step id for the history output.")]
        public string StepId
        {
            get
            {
                if (_historyOutput.StepId == -1) return _all;
                else return _historyOutput.StepId.ToString();
            }
            set
            {
                if (value == _all) _historyOutput.StepId = -1;
                else
                {
                    if (int.TryParse(value, out int stepId)) _historyOutput.StepId = stepId;
                    else throw new NotSupportedException();
                }
                UpdateStepIncrements();
            }
        }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(6, 10, "Increment id")]
        [DescriptionAttribute("Increment id for the history output.")]
        public string StepIncrementId
        {
            get
            {
                if (_historyOutput.StepIncrementId == -1) return _all;
                else return _historyOutput.StepIncrementId.ToString();
            }
            set
            {
                if (value == _all) _historyOutput.StepIncrementId = -1;
                else
                {
                    if (int.TryParse(value, out int incrementId)) _historyOutput.StepIncrementId = incrementId;
                    else throw new NotSupportedException();
                    //
                    UpdateVisibility();
                }
            }
        }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(7, 10, "Harmonic")]
        [DescriptionAttribute("Output harmonic oscillations as the history output.")]
        public bool Harmonic { get { return _historyOutput.Harmonic; } set { _historyOutput.Harmonic = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(8, 10, "Node coordinates")]
        [DescriptionAttribute("Output node coordinates in the history output.")]
        public OutputNodeCoordinatesEnum OutputNodeCoordinates
        {
            get { return _historyOutput.OutputNodeCoordinates; }
            set { _historyOutput.OutputNodeCoordinates = value; }
        }
        //
        [CategoryAttribute("Region")]
        [OrderedDisplayName(2, 10, "Node set")]
        [DescriptionAttribute("Select the node set for the creation of the history output.")]
        public string NodeSetName { get { return _historyOutput.RegionName; } set { _historyOutput.RegionName = value; } }
        //
        [CategoryAttribute("Region")]
        [OrderedDisplayName(3, 10, "Surface")]
        [DescriptionAttribute("Select the surface for the creation of the history output.")]
        public string SurfaceName { get { return _historyOutput.RegionName; } set { _historyOutput.RegionName = value; } }

       
        // Constructors                                                                                                             
        public ViewResultHistoryOutputFromField(ResultHistoryOutputFromField historyOutput, bool complexVisible)
        {
            // The order is important
            _historyOutput = historyOutput;
            _complexVisible = complexVisible;
            //
            Dictionary<RegionTypeEnum, string> regionTypePropertyNamePairs = new Dictionary<RegionTypeEnum, string>();
            regionTypePropertyNamePairs.Add(RegionTypeEnum.Selection, nameof(SelectionHidden));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.NodeSetName, nameof(NodeSetName));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.SurfaceName, nameof(SurfaceName));
            //
            SetBase(_historyOutput, regionTypePropertyNamePairs);
            DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
            //
            DynamicCustomTypeDescriptor.GetProperty(nameof(ComplexResultType)).SetIsBrowsable(_complexVisible);
            //
            DynamicCustomTypeDescriptor.RenameBooleanPropertyToYesNo(nameof(Harmonic));
        }


        // Methods                                                                                                                  
        public override ResultHistoryOutput GetBase()
        {
            return _historyOutput;
        }
        public void PopulateDropDownLists(string[] nodeSetNames, string[] surfaceNames,
                                          Dictionary<string, string[]> filedNameComponentNames,
                                          Dictionary<int, int[]> stepIdStepIncrementIds)
        {
            Dictionary<RegionTypeEnum, string[]> regionTypeListItemsPairs = new Dictionary<RegionTypeEnum, string[]>();
            regionTypeListItemsPairs.Add(RegionTypeEnum.Selection, new string[] { "Hidden" });
            regionTypeListItemsPairs.Add(RegionTypeEnum.NodeSetName, nodeSetNames);
            regionTypeListItemsPairs.Add(RegionTypeEnum.SurfaceName, surfaceNames);
            base.PopulateDropDownLists(regionTypeListItemsPairs);
            //
            _filedNameComponentNames = filedNameComponentNames;
            DynamicCustomTypeDescriptor.PopulateProperty(nameof(FieldName), _filedNameComponentNames.Keys.ToArray());
            UpdateComponents();
            //
            List<string> incrementIds;
            _stepIdStepIncrementIds = new Dictionary<string, string[]> { { _all, new string[] { _all } } };
            foreach (var stepEntry in stepIdStepIncrementIds)
            {
                incrementIds = new List<string>() { _all };
                foreach (var incrementId in stepEntry.Value) incrementIds.Add(incrementId.ToString());
                _stepIdStepIncrementIds.Add(stepEntry.Key.ToString(), incrementIds.ToArray());
            }
            DynamicCustomTypeDescriptor.PopulateProperty(nameof(StepId), _stepIdStepIncrementIds.Keys.ToArray());
            //
            UpdateStepIncrements();
        }
        private void UpdateComponents()
        {
            string[] componentNames;
            if (_filedNameComponentNames.TryGetValue(FieldName, out componentNames) && componentNames.Length > 0)
            {
                DynamicCustomTypeDescriptor.PopulateProperty(nameof(ComponentName), componentNames);
                if (!componentNames.Contains(ComponentName)) ComponentName = componentNames[0];
            }
        }
        private void UpdateStepIncrements()
        {
            string[] incrementIds;
            if (_stepIdStepIncrementIds.TryGetValue(StepId, out incrementIds) && incrementIds.Length > 1)
            {
                DynamicCustomTypeDescriptor.PopulateProperty(nameof(StepIncrementId), incrementIds);
                if (!incrementIds.Contains(StepIncrementId)) StepIncrementId = incrementIds[0];
            }
            //
            UpdateVisibility();
        }
        private void UpdateVisibility()
        {
            DynamicCustomTypeDescriptor dctd = DynamicCustomTypeDescriptor;
            bool visible = ComplexResultType == ComplexResultTypeEnum.RealAtAngle;
            //
            dctd.GetProperty(nameof(ComplexAngleDeg)).SetIsBrowsable(visible);
            visible = StepId != _all;
            dctd.GetProperty(nameof(StepIncrementId)).SetIsBrowsable(visible);
            visible = _complexVisible && ComplexResultType == ComplexResultTypeEnum.Real &&
                      visible && StepIncrementId != _all;
            dctd.GetProperty(nameof(Harmonic)).SetIsBrowsable(visible);
        }
    }



   
}
