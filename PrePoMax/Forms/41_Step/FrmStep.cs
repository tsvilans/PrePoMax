using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeModel;
using CaeGlobals;
using System.Windows.Forms;

namespace PrePoMax.Forms
{
    class FrmStep : UserControls.FrmPropertyListView, IFormBase
    {
        // Variables                                                                                                                
        private string[] _stepNames;
        private string _stepToEditName;
        private ViewStep _viewStep;
        private Controller _controller;


        // Properties                                                                                                               
        public string[] StepNames { get { return _stepNames; } set { _stepNames = value; } }
        public Step Step
        {
            get { return _viewStep.GetBase(); }
            set
            {
                if (value.GetType() == typeof(StaticStep))  // use this form due to inheritance
                    _viewStep = new ViewStaticStep((value as StaticStep).DeepClone());  
                else if (value is SlipWearStep sws)
                    _viewStep = new ViewSlipWearStep(sws.DeepClone());
                else if (value is BoundaryDisplacementStep bds)
                    _viewStep = new ViewBoundaryDisplacementStep(bds.DeepClone());
                else if (value is FrequencyStep fs)
                    _viewStep = new ViewFrequencyStep(fs.DeepClone());
                else if (value is BuckleStep bs)
                    _viewStep = new ViewBuckleStep(bs.DeepClone());
                else if (value.GetType() == typeof(DynamicStep))    // use this form due to inheritance
                    _viewStep = new ViewDynamicStep((value as DynamicStep).DeepClone());
                // Thermal
                else if (value.GetType() == typeof(HeatTransferStep))   // use this form due to inheritance
                    _viewStep = new ViewHeatTransferStep((value as HeatTransferStep).DeepClone());
                else if (value.GetType() == typeof(UncoupledTempDispStep))  // use this form due to inheritance
                    _viewStep = new ViewUncoupledTempDispStep((value as UncoupledTempDispStep).DeepClone());
                else if (value.GetType() == typeof(CoupledTempDispStep))    // use this form due to inheritance
                    _viewStep = new ViewCoupledTempDispStep((value as CoupledTempDispStep).DeepClone());
                else throw new NotImplementedException();
            }
        }


        // Constructors                                                                                                             
        public FrmStep(Controller controller)
            : base(1.7)
        {
            InitializeComponent();
            //
            _controller = controller;
            _viewStep = null;
        }
        private void InitializeComponent()
        {
            this.gbType.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbType
            // 
            this.gbType.Size = new System.Drawing.Size(310, 108);
            // 
            // lvTypes
            // 
            this.lvTypes.Size = new System.Drawing.Size(298, 80);
            // 
            // gbProperties
            // 
            this.gbProperties.Location = new System.Drawing.Point(12, 126);
            this.gbProperties.Size = new System.Drawing.Size(310, 354);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(160, 486);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(241, 486);
            // 
            // btnOkAddNew
            // 
            this.btnOkAddNew.Location = new System.Drawing.Point(79, 486);
            // 
            // FrmStep
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.ClientSize = new System.Drawing.Size(334, 521);
            this.Name = "FrmStep";
            this.Text = "Edit Step";
            this.gbType.ResumeLayout(false);
            this.ResumeLayout(false);

        }


        // Overrides                                                                                                                
        protected override void OnListViewTypeSelectedIndexChanged()
        {
            if (lvTypes.SelectedItems != null && lvTypes.SelectedItems.Count > 0)
            {
                propertyGrid.SelectedObject = lvTypes.SelectedItems[0].Tag;
                propertyGrid.Select();
            }
        }
        protected override void OnPropertyGridPropertyValueChanged()
        {
            (propertyGrid.SelectedObject as ViewStep).UpdateVisibility();
            //
            base.OnPropertyGridPropertyValueChanged();
        }
        protected override void OnApply(bool onOkAddNew)
        {
            _viewStep = (ViewStep)propertyGrid.SelectedObject;
            //
            if (_viewStep == null) throw new CaeException("No step selected.");
            // Check if the name exists
            CheckName(_stepToEditName, Step.Name, _stepNames, "step");
            // Create
            if (_stepToEditName == null)
            {
                bool copyBCsAndLoads = true;
                if (Step is BoundaryDisplacementStep) copyBCsAndLoads = false;
                //
                _controller.AddStepCommand(Step, copyBCsAndLoads);
            }
            // Replace
            else
            {
                if (_propertyItemChanged) _controller.ReplaceStepCommand(_stepToEditName, this.Step);
            }
        }
        protected override bool OnPrepareForm(string stepName, string stepToEditName)
        {
            this.btnOkAddNew.Visible = stepToEditName == null;
            //
            _propertyItemChanged = false;
            _stepNames = null;
            _stepToEditName = null;
            _viewStep = null;
            lvTypes.Items.Clear();
            propertyGrid.SelectedObject = null;
            //
            _stepNames = _controller.GetStepNames();
            _stepToEditName = stepToEditName;
            //
            if (_stepNames == null)
                throw new CaeException("The step names must be defined first.");
            //
            PopulateListOfSteps();
            //
            if (_stepToEditName == null)
            {
                lvTypes.Enabled = true;
                _viewStep = null;
                //
                if (lvTypes.Items.Count == 1) lvTypes.Items[0].Selected = true;
            }
            else
            {
                Step = _controller.GetStep(_stepToEditName);    // to clone and set _viewStep
                // Select the appropriate load in the list view
                int selectedId = 0;
                foreach (ListViewItem item in lvTypes.Items)
                {
                    if (item.Tag != null && item.Tag.GetType() == _viewStep.GetType())
                    {
                        lvTypes.Items[selectedId].Tag = _viewStep;
                        _preselectIndex = selectedId;
                        break;
                    }
                    selectedId++;
                }
            }
            //
            _controller.SetSelectByToOff();
            //
            return true;
        }


        // Methods                                                                                                                  
        private void PopulateListOfSteps()
        {
            Step prevOrLastStep = GetPreviousOrLastStep();
            bool addStaticStep = false;
            bool addSlipWearStep = false;
            bool addBoundaryDisplacementStep = false;
            bool addFrequencyStep = false;
            bool addBuckleStep = false;
            bool addDynamicStep = false;
            bool addHeatTransferStep = false;
            bool addUncoupledTemDispStep = false;
            bool addCoupledTemDispStep = false;
            bool cannotAdd;
            //
            if (prevOrLastStep is BoundaryDisplacementStep)
            {
                // no possible steps to add
            }
            else if (prevOrLastStep is SlipWearStep)
            {
                addSlipWearStep = true;
                addBoundaryDisplacementStep = true; // only one possibility
            }
            else
            {
                if (prevOrLastStep == null || prevOrLastStep.GetType() == typeof(StaticStep) ||
                    prevOrLastStep is HeatTransferStep)
                {
                    addStaticStep = true;
                    addSlipWearStep = true;
                }
                if (!(prevOrLastStep is FrequencyStep)) addFrequencyStep = true;
                if (!(prevOrLastStep is BuckleStep)) addBuckleStep = true;
                //
                addDynamicStep = true;
                addHeatTransferStep = true;
                addUncoupledTemDispStep = true;
                addCoupledTemDispStep = true;
            }
            //
            cannotAdd = !(addStaticStep || addSlipWearStep || addBoundaryDisplacementStep || addFrequencyStep || addBuckleStep ||
                          addHeatTransferStep || addUncoupledTemDispStep || addCoupledTemDispStep);
            //
            ListViewItem item;
            if (cannotAdd)
            {
                item = new ListViewItem("No steps can be added after the last step type.");
                item.Tag = null;
                lvTypes.Items.Add(item);
            }
            else
            {
                SolverTypeEnum defaultSolverType = _controller.Settings.Calculix.DefaultSolverType;
                if (addStaticStep)
                {
                    // Static step
                    item = new ListViewItem("Static step");
                    StaticStep staticStep = (StaticStep)CreateNewOrCloneLast(typeof(StaticStep));
                    staticStep.SolverType = defaultSolverType;
                    item.Tag = new ViewStaticStep(staticStep);
                    lvTypes.Items.Add(item);
                }
                if (addSlipWearStep)
                {
                    // Slip wear step
                    item = new ListViewItem("Slip wear step");
                    //SlipWearStep slipWearStep = new SlipWearStep(GetStepName());
                    SlipWearStep slipWearStep = (SlipWearStep)CreateNewOrCloneLast(typeof(SlipWearStep));
                    slipWearStep.SolverType = defaultSolverType;
                    item.Tag = new ViewSlipWearStep(slipWearStep);
                    lvTypes.Items.Add(item);
                }
                if (addBoundaryDisplacementStep)
                {
                    // Boundary displacement step
                    item = new ListViewItem("Boundary displacement step");
                    BoundaryDisplacementStep boundaryDisplacementStep = new BoundaryDisplacementStep(GetStepName());
                    item.Tag = new ViewBoundaryDisplacementStep(boundaryDisplacementStep);
                    lvTypes.Items.Add(item);
                }
                if (addFrequencyStep)
                {
                    // Frequency step
                    item = new ListViewItem("Frequency step");
                    FrequencyStep frequencyStep = new FrequencyStep(GetStepName());
                    frequencyStep.SolverType = defaultSolverType;
                    item.Tag = new ViewFrequencyStep(frequencyStep);
                    lvTypes.Items.Add(item);
                }
                if (addBuckleStep)
                {
                    // Frequency step
                    item = new ListViewItem("Buckle step");
                    BuckleStep buckleStep = new BuckleStep(GetStepName());
                    buckleStep.SolverType = defaultSolverType;
                    item.Tag = new ViewBuckleStep(buckleStep);
                    lvTypes.Items.Add(item);
                }
                if (addDynamicStep)
                {
                    // Dynamic step
                    item = new ListViewItem("Dynamic step");
                    DynamicStep dynamicStep = new DynamicStep(GetStepName());
                    dynamicStep.SolverType = defaultSolverType;
                    item.Tag = new ViewDynamicStep(dynamicStep);
                    lvTypes.Items.Add(item);
                }
                if (addHeatTransferStep)
                {
                    // Heat transfer step
                    item = new ListViewItem("Heat transfer step");
                    HeatTransferStep heatTransferStep = new HeatTransferStep(GetStepName());
                    heatTransferStep.SolverType = defaultSolverType;
                    item.Tag = new ViewHeatTransferStep(heatTransferStep);
                    lvTypes.Items.Add(item);
                }
                if (addUncoupledTemDispStep)
                {
                    // Uncoupled temperature-displacement step
                    item = new ListViewItem("Uncoupled temperature-displacement step");
                    UncoupledTempDispStep uncoupledTempDispStep = new UncoupledTempDispStep(GetStepName());
                    uncoupledTempDispStep.SolverType = defaultSolverType;
                    item.Tag = new ViewUncoupledTempDispStep(uncoupledTempDispStep);
                    lvTypes.Items.Add(item);
                }
                if (addCoupledTemDispStep)
                {
                    // Coupled temperature-displacement step
                    item = new ListViewItem("Coupled temperature-displacement step");
                    CoupledTempDispStep coupledTempDispStep = new CoupledTempDispStep(GetStepName());
                    coupledTempDispStep.SolverType = defaultSolverType;
                    item.Tag = new ViewCoupledTempDispStep(coupledTempDispStep);
                    lvTypes.Items.Add(item);
                }
            }
            
        }
        private Step CreateNewOrCloneLast(Type stepTypeToCreate)
        {
            if (stepTypeToCreate == null) throw new ArgumentNullException();
            //
            Step newStep;
            Step prevOrLastStep = GetPreviousOrLastStep();
            //
            if (prevOrLastStep == null || prevOrLastStep.GetType() != stepTypeToCreate)
            {
                newStep = (Step)Activator.CreateInstance(stepTypeToCreate, new object[] { GetStepName() });
                //
                if (prevOrLastStep != null)
                {
                    newStep.Perturbation = prevOrLastStep.Perturbation;
                    newStep.Nlgeom = prevOrLastStep.Nlgeom;
                    newStep.MaxIncrements = prevOrLastStep.MaxIncrements;
                    newStep.IncrementationType = prevOrLastStep.IncrementationType;
                    newStep.SolverType = prevOrLastStep.SolverType;
                }
            }
            else
            {
                newStep = prevOrLastStep.DeepClone();
                //
                newStep.BoundaryConditions.Clear(); // this gets added at the step collection
                newStep.Loads.Clear();              // this gets added at the step collection
                newStep.DefinedFields.Clear();      // this gets added at the step collection
                //
                newStep.Name = GetStepName();
            }
            return newStep;
        }
        private Step GetPreviousOrLastStep()
        {
            Step prevOrLastStep = null;
            // Find previous step
            if (_stepToEditName != null)
            {
                Step[] steps = _controller.GetAllSteps();
                int prevStepId = -1;
                for (int i = 0; i < steps.Length; i++)
                {
                    if (steps[i].Name == _stepToEditName)
                    {
                        prevStepId = i - 1;
                        break;
                    }
                }
                if (prevStepId >= 0) prevOrLastStep = steps[prevStepId];
            }
            // Find last step
            else if (_stepNames.Length > 0)
            {
                prevOrLastStep = _controller.GetAllSteps().Last();
            }
            //
            return prevOrLastStep;
        }
        private string GetStepName()
        {
            return _stepNames.GetNextNumberedKey("Step");
        }
    }
}
