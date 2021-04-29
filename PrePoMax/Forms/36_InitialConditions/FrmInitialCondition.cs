﻿using System;
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
                else throw new NotImplementedException();
            }
        }


        // Constructors                                                                                                             
        public FrmInitialCondition(Controller controller)
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
            this.gbType.Size = new System.Drawing.Size(310, 97);
            // 
            // lvTypes
            // 
            this.lvTypes.Size = new System.Drawing.Size(298, 69);
            // 
            // gbProperties
            // 
            this.gbProperties.Location = new System.Drawing.Point(12, 115);
            this.gbProperties.Size = new System.Drawing.Size(310, 305);
            // 
            // propertyGrid
            // 
            this.propertyGrid.Size = new System.Drawing.Size(298, 277);
            // 
            // FrmInitialConditions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.ClientSize = new System.Drawing.Size(334, 461);
            this.Name = "FrmInitialConditions";
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
                else if (itemTag is ViewInitialTemperature vi) _viewInitialCondition = vi;
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
                throw new CaeException("The initial condition must contain at least one item.");
            //
            if ((_initialConditionToEditName == null &&
                 _initialConditionNames.Contains(InitialCondition.Name)) ||   // named to existing name
                (InitialCondition.Name != _initialConditionToEditName &&
                 _initialConditionNames.Contains(InitialCondition.Name)))     // renamed to existing name
                throw new CaeException("The selected initial condition name already exists.");            
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
            //
            if (_initialConditionNames == null)
                throw new CaeException("The initial condition names must be defined first.");
            // Populate list view
            PopulateListOfHistoryOutputs(nodeSetNames, surfaceNames);
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
                    vit.PopululateDropDownLists(nodeSetNames, surfaceNames);
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
        private void PopulateListOfHistoryOutputs(string[] nodeSetNames, string[] surfaceNames)
        {
            ListViewItem item;
            // Initial temperature
            string name = "Initial temperature";
            item = new ListViewItem(name);
            InitialTemperature it = new InitialTemperature(GetInitialConditionName(name), "", RegionTypeEnum.Selection);
            ViewInitialTemperature vit = new ViewInitialTemperature(it);
            vit.PopululateDropDownLists(nodeSetNames, surfaceNames);
            item.Tag = vit;
            lvTypes.Items.Add(item);
        }
        private string GetInitialConditionName(string name)
        {
            if (name == null || name == "") name = "Initial condition";
            name = name.Replace(' ', '_');
            if (name[name.Length - 1] != '-') name += '-';
            name = NamedClass.GetNewValueName(_initialConditionNames, name);
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
                else if (InitialCondition is InitialTemperature)
                {
                    if (InitialCondition.RegionType == RegionTypeEnum.NodeSetName ||
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
            }
            else _controller.SetSelectByToOff();
        }
        //
        public void SelectionChanged(int[] ids)
        {
            if (InitialCondition != null && InitialCondition.RegionType == RegionTypeEnum.Selection)
            {
                if (InitialCondition is InitialTemperature)
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