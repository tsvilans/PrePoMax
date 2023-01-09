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
    class FrmDefinedField : UserControls.FrmPropertyListView, IFormBase, IFormItemSetDataParent, IFormHighlight
    {
        // Variables                                                                                                                
        private string[] _definedFieldNames;
        private string _definedFieldToEditName;
        private ViewDefinedField _viewDefinedField;
        private Controller _controller;


        // Properties                                                                                                               
        public DefinedField DefinedField
        {
            get { return _viewDefinedField != null ? _viewDefinedField.GetBase() : null; }
            set
            {
                if (value is DefinedTemperature dt) _viewDefinedField = new ViewDefinedTemperature(dt.DeepClone());
                else throw new NotImplementedException();
            }
        }


        // Constructors                                                                                                             
        public FrmDefinedField(Controller controller)
            : base (1.7)
        {
            InitializeComponent();
            //
            _controller = controller;
            _viewDefinedField = null;
        }
        private void InitializeComponent()
        {
            this.gbType.SuspendLayout();
            this.gbProperties.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbType
            // 
            this.gbType.Size = new System.Drawing.Size(310, 89);
            // 
            // lvTypes
            // 
            this.lvTypes.Size = new System.Drawing.Size(298, 61);
            // 
            // gbProperties
            // 
            this.gbProperties.Location = new System.Drawing.Point(12, 107);
            this.gbProperties.Size = new System.Drawing.Size(310, 313);
            // 
            // propertyGrid
            // 
            this.propertyGrid.Size = new System.Drawing.Size(298, 285);
            // 
            // FrmDefinedField
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.ClientSize = new System.Drawing.Size(334, 461);
            this.Name = "FrmDefinedField";
            this.Text = "Edit Defined Field";
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
                if (itemTag is ViewError)  _viewDefinedField = null;
                else if (itemTag is ViewDefinedTemperature vdt) _viewDefinedField = vdt;
                else throw new NotImplementedException();
                //
                ShowHideSelectionForm();
                //
                propertyGrid.SelectedObject = itemTag;
                propertyGrid.Select();
                //
                HighlightDefinedField();
            }
        }
        protected override void OnPropertyGridPropertyValueChanged()
        {
            string property = propertyGrid.SelectedGridItem.PropertyDescriptor.Name;
            //
            if (property == nameof(_viewDefinedField.RegionType))
            {
                ShowHideSelectionForm();
                HighlightDefinedField();
            }
            else if (_viewDefinedField is ViewDefinedTemperature vdt)
            {
                if (property == nameof(vdt.DefinedTemperatureType))
                {
                    ShowHideSelectionForm();
                    HighlightDefinedField();
                }
                else if (property == nameof(vdt.NodeSetName) || property == nameof(vdt.SurfaceName))
                {
                    HighlightDefinedField();
                }
            }
            //
            base.OnPropertyGridPropertyValueChanged();
        }
        protected override void OnApply(bool onOkAddNew)
        {
            if (propertyGrid.SelectedObject is ViewError ve) throw new CaeException(ve.Message);
            //
            _viewDefinedField = (ViewDefinedField)propertyGrid.SelectedObject;
            //
            if (DefinedField == null) throw new CaeException("No defined field was selected.");
            //
            if (DefinedField is DefinedTemperature dt)
            {
                // Empty selection
                if (dt.Type == DefinedTemperatureTypeEnum.ByValue && DefinedField.RegionType == RegionTypeEnum.Selection &&
                    (DefinedField.CreationIds == null || DefinedField.CreationIds.Length == 0))
                    throw new CaeException("The defined field must contain at least one item.");
                // Empty file name
                else if (dt.Type == DefinedTemperatureTypeEnum.FromFile && dt.FileName == null && dt.FileName.Length == 0)
                    throw new CaeException("The file name to read the temperature from is missing.");
            }
            // Check if the name exists
            CheckName(_definedFieldToEditName, DefinedField.Name, _definedFieldNames, "defined field");
            // Create
            if (_definedFieldToEditName == null)
            {
                _controller.AddDefinedFieldCommand(_stepName, DefinedField);
            }
            // Replace
            else if (_propertyItemChanged)
            {
                _controller.ReplaceDefinedFieldCommand(_stepName, _definedFieldToEditName, DefinedField);
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
        protected override bool OnPrepareForm(string stepName, string definedFieldToEditName)
        {
            this.btnOkAddNew.Visible = definedFieldToEditName == null;
            //
            _propertyItemChanged = false;
            _stepName = null;
            _definedFieldNames = null;
            _definedFieldToEditName = null;
            _viewDefinedField = null;
            lvTypes.Items.Clear();
            propertyGrid.SelectedObject = null;
            //
            _stepName = stepName;
            _definedFieldNames = _controller.GetDefinedFieldNamesForStep(_stepName);
            _definedFieldToEditName = definedFieldToEditName;
            string[] nodeSetNames = _controller.GetUserNodeSetNames();
            string[] surfaceNames = _controller.GetUserSurfaceNames();
            //
            if (_definedFieldNames == null)
                throw new CaeException("The defined field names must be defined first.");
            // Populate list view
            PopulateListOfDefinedFields(nodeSetNames, surfaceNames);
            // Check if this step supports any defined fields
            if (lvTypes.Items.Count == 0) return false;
            // Create new defined field
            if (_definedFieldToEditName == null)
            {
                lvTypes.Enabled = true;
                _viewDefinedField = null;
                if (lvTypes.Items.Count == 1) _preselectIndex = 0;
                //
                HighlightDefinedField(); // must be here if called from the menu
            }
            else
            // Edit existing defined field
            {
                // Get and convert a converted history output back to selection
                DefinedField = _controller.GetDefinedField(_stepName, _definedFieldToEditName); // to clone
                if (DefinedField.CreationData != null)
                {
                    if (!DefinedField.Valid || !_controller.Model.RegionValid(DefinedField))
                    {
                        DefinedField.CreationData = null;
                        DefinedField.CreationIds = null;
                        _propertyItemChanged = true;
                    }
                    DefinedField.RegionType = RegionTypeEnum.Selection;
                }
                //
                int selectedId;
                if (_viewDefinedField is ViewDefinedTemperature vdt)
                {
                    selectedId = lvTypes.FindItemWithText("Temperature").Index;
                    // Check for deleted entities
                    if (vdt.RegionType == RegionTypeEnum.Selection.ToFriendlyString()) { }
                    else if (vdt.RegionType == RegionTypeEnum.NodeSetName.ToFriendlyString())
                        CheckMissingValueRef(ref nodeSetNames, vdt.NodeSetName, s => { vdt.NodeSetName = s; });
                    else if (vdt.RegionType == RegionTypeEnum.SurfaceName.ToFriendlyString())
                        CheckMissingValueRef(ref surfaceNames, vdt.SurfaceName, s => { vdt.SurfaceName = s; });
                    else throw new NotSupportedException();
                    //
                    vdt.PopulateDropDownLists(nodeSetNames, surfaceNames);
                }
                else throw new NotSupportedException();
                //
                lvTypes.Items[selectedId].Tag = _viewDefinedField;
                _preselectIndex = selectedId;
            }
            ShowHideSelectionForm();
            //
            return true;
        }


        // Methods                                                                                                                  
        private void PopulateListOfDefinedFields(string[] nodeSetNames, string[] surfaceNames)
        {
            Step step = _controller.GetStep(_stepName);
            // Populate list view
            ListViewItem item;
            // Defined temperature
            string name = "Temperature";
            item = new ListViewItem(name);
            DefinedTemperature definedTemperature = new DefinedTemperature(GetDefinedFieldName(name), "",
                                                                           RegionTypeEnum.Selection, 0);
            if (step.IsDefinedFieldSupported(definedTemperature))
            {
                ViewDefinedTemperature vdt = new ViewDefinedTemperature(definedTemperature);
                vdt.PopulateDropDownLists(nodeSetNames, surfaceNames);
                item.Tag = vdt;
                lvTypes.Items.Add(item);
            }
        }
        private string GetDefinedFieldName(string name)
        {
            if (name == null || name == "") name = "Defined field";
            name = name.Replace(' ', '_');
            name = _definedFieldNames.GetNextNumberedKey(name);
            return name;
        }
        private void HighlightDefinedField()
        {
            try
            {
                _controller.ClearSelectionHistory();
                //
                if (_viewDefinedField == null) { }
                else if (DefinedField is DefinedTemperature dt)
                {
                    if (dt.Type == DefinedTemperatureTypeEnum.ByValue)
                    {
                        if (DefinedField.RegionType == RegionTypeEnum.NodeSetName ||
                            DefinedField.RegionType == RegionTypeEnum.SurfaceName)
                        {
                            _controller.Highlight3DObjects(new object[] { DefinedField.RegionName });
                        }
                        else if (DefinedField.RegionType == RegionTypeEnum.Selection)
                        {
                            SetSelectItem();
                            //
                            if (DefinedField.CreationData != null)
                            {
                                _controller.Selection = DefinedField.CreationData.DeepClone();
                                _controller.HighlightSelection();
                            }
                        }
                        else throw new NotImplementedException();
                    }
                }
                else throw new NotSupportedException();
            }
            catch { }
        }
        private void ShowHideSelectionForm()
        {
            if (DefinedField != null && DefinedField.RegionType == RegionTypeEnum.Selection)
            {
                if (DefinedField is DefinedTemperature dt && dt.Type == DefinedTemperatureTypeEnum.FromFile)
                    ItemSetDataEditor.SelectionForm.Hide();
                else
                    ItemSetDataEditor.SelectionForm.ShowIfHidden(this.Owner);
            }
            else
                ItemSetDataEditor.SelectionForm.Hide();
            //
            SetSelectItem();
        }
        private void SetSelectItem()
        {
            if (DefinedField != null && DefinedField.RegionType == RegionTypeEnum.Selection)
            {
                if (DefinedField is null) { }
                else if (DefinedField is DefinedTemperature) _controller.SetSelectItemToNode();
            }
            else _controller.SetSelectByToOff();
            //
            if (DefinedField is DefinedTemperature dt)
            {
                if (dt.Type == DefinedTemperatureTypeEnum.FromFile) _controller.SetSelectByToOff();
            }
        }
        //
        public void SelectionChanged(int[] ids)
        {
            if (DefinedField != null && DefinedField.RegionType == RegionTypeEnum.Selection)
            {
                if (DefinedField is DefinedTemperature)
                {
                    DefinedField.CreationIds = ids;
                    DefinedField.CreationData = _controller.Selection.DeepClone();
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
            HighlightDefinedField();
        }

        // IFormItemSetDataParent
        public bool IsSelectionGeometryBased()
        {
            // Prepare ItemSetDataEditor - prepare Geometry or Mesh based selection
            if (DefinedField == null || DefinedField.CreationData == null) return true;
            return DefinedField.CreationData.IsGeometryBased();
        }
    }
}
