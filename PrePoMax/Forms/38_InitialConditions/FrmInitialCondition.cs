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
    class FrmInitialCondition : UserControls.FrmPropertyListView, IFormBase, IFormItemSetDataParent, IFormHighlight
    {
        // Variables                                                                                                                
        private string[] _initialConditionNames;
        private string _initialConditionToEditName;
        private ViewInitialCondition _viewInitialCondition;
        private Controller _controller;


        // Properties                                                                                                               
        public InitialCondition InitialCondition
        {
            get { return _viewInitialCondition != null ? _viewInitialCondition.GetBase() : null; }
            set
            {
                if (value is InitialTemperature it) _viewInitialCondition = new ViewInitialTemperature(it.DeepClone());
                else if (value is InitialVelocity iv) _viewInitialCondition = new ViewInitialVelocity(iv.DeepClone());
                else throw new NotImplementedException();
            }
        }


        // Constructors                                                                                                             
        public FrmInitialCondition(Controller controller)
            : base(1.7)
        {
            InitializeComponent();
            //
            _controller = controller;
            _viewInitialCondition = null;
        }
        private void InitializeComponent()
        {
            this.gbType.SuspendLayout();
            this.gbProperties.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbType
            // 
            this.gbType.Size = new System.Drawing.Size(310, 88);
            // 
            // lvTypes
            // 
            this.lvTypes.Size = new System.Drawing.Size(298, 60);
            // 
            // gbProperties
            // 
            this.gbProperties.Location = new System.Drawing.Point(12, 106);
            this.gbProperties.Size = new System.Drawing.Size(310, 314);
            // 
            // propertyGrid
            // 
            this.propertyGrid.Size = new System.Drawing.Size(298, 286);
            // 
            // FrmInitialCondition
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.ClientSize = new System.Drawing.Size(334, 461);
            this.Name = "FrmInitialCondition";
            this.Text = "Edit Initial Condition";
            this.gbType.ResumeLayout(false);
            this.gbProperties.ResumeLayout(false);
            this.ResumeLayout(false);

        }


        // Overrides                                                                                                                
        protected override void OnListViewTypeSelectedIndexChanged()
        {
            if (lvTypes.SelectedItems != null && lvTypes.SelectedItems.Count > 0)
            {
                object itemTag = lvTypes.SelectedItems[0].Tag;
                if (itemTag is ViewError)  _viewInitialCondition = null;
                else if (itemTag is ViewInitialTemperature vit) _viewInitialCondition = vit;
                else if (itemTag is ViewInitialVelocity viv) _viewInitialCondition = viv;
                else throw new NotImplementedException();
                //
                ShowHideSelectionForm();
                //
                propertyGrid.SelectedObject = itemTag;
                propertyGrid.Select();
                //
                HighlightInitialCondition();
            }
        }
        protected override void OnPropertyGridPropertyValueChanged()
        {
            string property = propertyGrid.SelectedGridItem.PropertyDescriptor.Name;
            //
            if (property == nameof(_viewInitialCondition.RegionType))
            {
                ShowHideSelectionForm();
                //
                HighlightInitialCondition();
            }
            else if (_viewInitialCondition is ViewInitialTemperature vit &&
                     (property == nameof(vit.NodeSetName) ||
                      property == nameof(vit.SurfaceName)))
            {
                HighlightInitialCondition();
            }
            else if (_viewInitialCondition is ViewInitialVelocity viv &&
                     (property == nameof(viv.PartName) ||
                      property == nameof(viv.ElementSetName)))
            {
                HighlightInitialCondition();
            }
            //
            base.OnPropertyGridPropertyValueChanged();
        }
        protected override void OnApply(bool onOkAddNew)
        {
            if (propertyGrid.SelectedObject is ViewError ve) throw new CaeException(ve.Message);
            //
            _viewInitialCondition = (ViewInitialCondition)propertyGrid.SelectedObject;
            //
            if (InitialCondition == null) throw new CaeException("No initial condition was selected.");
            //
            if (InitialCondition.RegionType == RegionTypeEnum.Selection &&
                (InitialCondition.CreationIds == null || InitialCondition.CreationIds.Length == 0))
                throw new CaeException("The initial condition selection must contain at least one item.");
            // Check if the name exists
            CheckName(_initialConditionToEditName, InitialCondition.Name, _initialConditionNames, "initial condition");
            // Create
            if (_initialConditionToEditName == null)
            {
                _controller.AddInitialConditionCommand(InitialCondition);
            }
            // Replace
            else if (_propertyItemChanged)
            {
                _controller.ReplaceInitialConditionCommand(_initialConditionToEditName, InitialCondition);
            }
            // If all is successful close the ItemSetSelectionForm - except for OKAddNew
            if (!onOkAddNew) ItemSetDataEditor.SelectionForm.Hide();
        }
        protected override void OnHideOrClose()
        {
            // Close the ItemSetSelectionForm
            ItemSetDataEditor.SelectionForm.Hide();
            //
            base.OnHideOrClose();
        }
        protected override bool OnPrepareForm(string stepName, string initialConditionToEditName)
        {
            this.btnOkAddNew.Visible = initialConditionToEditName == null;
            //
            _propertyItemChanged = false;
            _stepName = null;
            _initialConditionNames = null;
            _initialConditionToEditName = null;
            _viewInitialCondition = null;
            lvTypes.Items.Clear();
            propertyGrid.SelectedObject = null;
            //
            _stepName = stepName;
            _initialConditionNames = _controller.GetInitialConditionNames();
            _initialConditionToEditName = initialConditionToEditName;
            string[] nodeSetNames = _controller.GetUserNodeSetNames();
            string[] surfaceNames = _controller.GetUserSurfaceNames();
            string[] partNames = _controller.GetModelPartNames();
            string[] elementSetNames = _controller.GetUserElementSetNames();
            //
            if (_initialConditionNames == null)
                throw new CaeException("The initial condition names must be defined first.");
            // Populate list view
            PopulateListOfInitialConditions(partNames, nodeSetNames, elementSetNames, surfaceNames);
            // Create new initial condition
            if (_initialConditionToEditName == null)
            {
                lvTypes.Enabled = true;
                _viewInitialCondition = null;
                if (lvTypes.Items.Count == 1) _preselectIndex = 0;
                //
                HighlightInitialCondition(); // must be here if called from the menu
            }
            else
            // Edit existing initial condition
            {
                // Get and convert a converted initial condition back to selection
                InitialCondition = _controller.GetInitialCondition(_initialConditionToEditName); // to clone
                if (InitialCondition.CreationData != null) InitialCondition.RegionType = RegionTypeEnum.Selection;
                //
                int selectedId;
                if (_viewInitialCondition is ViewInitialTemperature vit)
                {
                    selectedId = 0;
                    // Check for deleted entities
                    if (vit.RegionType == RegionTypeEnum.Selection.ToFriendlyString()) { }
                    else if (vit.RegionType == RegionTypeEnum.NodeSetName.ToFriendlyString())
                        CheckMissingValueRef(ref nodeSetNames, vit.NodeSetName, s => { vit.NodeSetName = s; });
                    else if (vit.RegionType == RegionTypeEnum.SurfaceName.ToFriendlyString())
                        CheckMissingValueRef(ref surfaceNames, vit.SurfaceName, s => { vit.SurfaceName = s; });
                    else throw new NotSupportedException();
                    //
                    vit.PopulateDropDownLists(nodeSetNames, surfaceNames);
                }
                else if (_viewInitialCondition is ViewInitialVelocity viv)
                {
                    selectedId = 1;
                    // Check for deleted entities
                    if (viv.RegionType == RegionTypeEnum.Selection.ToFriendlyString()) { }
                    else if (viv.RegionType == RegionTypeEnum.PartName.ToFriendlyString())
                        CheckMissingValueRef(ref nodeSetNames, viv.PartName, s => { viv.PartName = s; });
                    else if (viv.RegionType == RegionTypeEnum.ElementSetName.ToFriendlyString())
                        CheckMissingValueRef(ref surfaceNames, viv.ElementSetName, s => { viv.ElementSetName = s; });
                    else throw new NotSupportedException();
                    //
                    viv.PopulateDropDownLists(partNames, elementSetNames);
                }
                else throw new NotSupportedException();
                //
                lvTypes.Items[selectedId].Tag = _viewInitialCondition;
                _preselectIndex = selectedId;
            }
            ShowHideSelectionForm();
            //
            return true;
        }


        // Methods                                                                                                                  
        private void PopulateListOfInitialConditions(string[] partNames, string[] nodeSetNames, string[] elementSetNames,
                                                     string[] surfaceNames)
        {
            ListViewItem item;
            bool twoD = _controller.Model.Properties.ModelSpace.IsTwoD();
            // Initial temperature
            string name = "Temperature";
            item = new ListViewItem(name);
            InitialTemperature it = new InitialTemperature(GetInitialConditionName(name), "", RegionTypeEnum.Selection);
            ViewInitialTemperature vit = new ViewInitialTemperature(it);
            vit.PopulateDropDownLists(nodeSetNames, surfaceNames);
            item.Tag = vit;
            lvTypes.Items.Add(item);
            // Initial velocity
            name = "Velocity";
            item = new ListViewItem(name);
            InitialVelocity iv = new InitialVelocity(GetInitialConditionName(name), "", RegionTypeEnum.Selection, 0, 0, 0, twoD);
            ViewInitialVelocity viv = new ViewInitialVelocity(iv);
            viv.PopulateDropDownLists(partNames, elementSetNames);
            item.Tag = viv;
            lvTypes.Items.Add(item);
        }
        private string GetInitialConditionName(string name)
        {
            if (name == null || name == "") name = "Initial Condition";
            name = name.Replace(' ', '_');
            name = _initialConditionNames.GetNextNumberedKey(name);
            //
            return name;
        }
        private void HighlightInitialCondition()
        {
            try
            {
                _controller.ClearSelectionHistory();
                //
                if (_viewInitialCondition == null) { }
                else if (InitialCondition is InitialTemperature || InitialCondition is InitialVelocity)
                {
                    if (InitialCondition.RegionType == RegionTypeEnum.PartName || 
                        InitialCondition.RegionType == RegionTypeEnum.NodeSetName ||
                        InitialCondition.RegionType == RegionTypeEnum.ElementSetName ||
                        InitialCondition.RegionType == RegionTypeEnum.SurfaceName)
                    {
                        _controller.Highlight3DObjects(new object[] { InitialCondition.RegionName });
                    }
                    else if (InitialCondition.RegionType == RegionTypeEnum.Selection)
                    {
                        SetSelectItem();
                        //
                        if (InitialCondition.CreationData != null)
                        {
                            _controller.Selection = InitialCondition.CreationData.DeepClone();
                            _controller.HighlightSelection();
                        }
                    }
                    else throw new NotImplementedException();
                }
                else throw new NotSupportedException();
            }
            catch { }
        }
        private void ShowHideSelectionForm()
        {
            if (InitialCondition != null && InitialCondition.RegionType == RegionTypeEnum.Selection)
                ItemSetDataEditor.SelectionForm.ShowIfHidden(this.Owner);
            else
                ItemSetDataEditor.SelectionForm.Hide();
            //
            SetSelectItem();
        }
        private void SetSelectItem()
        {
            if (InitialCondition != null && InitialCondition.RegionType == RegionTypeEnum.Selection)
            {
                if (InitialCondition is null) { }
                else if (InitialCondition is InitialTemperature) _controller.SetSelectItemToNode();
                else if (InitialCondition is InitialVelocity) _controller.SetSelectItemToPart();
            }
            else _controller.SetSelectByToOff();
        }
        //
        public void SelectionChanged(int[] ids)
        {
            if (InitialCondition != null && InitialCondition.RegionType == RegionTypeEnum.Selection)
            {
                if (InitialCondition is InitialTemperature || InitialCondition is InitialVelocity)
                {
                    InitialCondition.CreationIds = ids;
                    InitialCondition.CreationData = _controller.Selection.DeepClone();
                    //
                    propertyGrid.Refresh();
                    //
                    _propertyItemChanged = true;
                }
                else throw new NotSupportedException();
            }
        }

        // IFormHighlight
        public void Highlight()
        {
            HighlightInitialCondition();
        }

        // IFormItemSetDataParent
        public bool IsSelectionGeometryBased()
        {
            // Prepare ItemSetDataEditor - prepare Geometry or Mesh based selection
            if (InitialCondition == null || InitialCondition.CreationData == null) return true;
            return InitialCondition.CreationData.IsGeometryBased();
        }
    }
}
