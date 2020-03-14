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
    class FrmSection : UserControls.FrmPropertyListView, IFormBase
    {
        // Variables                                                                                                                
        private string[] _sectionNames;
        private string _sectionToEditName;
        private ViewSection _viewSection;
        private Controller _controller;


        // Properties                                                                                                               
        public Section Section
        {
            get { return _viewSection.GetBase(); }
            set
            {
                if (value is SolidSection) _viewSection = new ViewSolidSection((SolidSection)value.DeepClone());
                else throw new NotImplementedException();
            }
        }


        // Constructors                                                                                                             
        public FrmSection(Controller controller)
        {
            InitializeComponent();
            //
            _controller = controller;
            _viewSection = null;
            //
            _selectedPropertyGridItemChangedEventActive = true;
        }
        private void InitializeComponent()
        {
            this.gbType.SuspendLayout();
            this.gbProperties.SuspendLayout();
            this.SuspendLayout();
            // 
            // FrmSection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.ClientSize = new System.Drawing.Size(334, 461);
            this.Name = "FrmSection";
            this.Text = "Edit Section";
            this.gbType.ResumeLayout(false);
            this.gbProperties.ResumeLayout(false);
            this.ResumeLayout(false);
        }


        // Overrides                                                                                                                
        protected override void OnListViewTypeSelectedIndexChanged()
        {
            if (lvTypes.Enabled && lvTypes.SelectedItems != null && lvTypes.SelectedItems.Count > 0)
            {
                if (lvTypes.SelectedItems[0].Tag is ViewError) { }
                else if (lvTypes.SelectedItems[0].Tag is ViewSection vs)
                {
                    _viewSection = vs;
                }
                //
                propertyGrid.SelectedObject = lvTypes.SelectedItems[0].Tag;
                propertyGrid.Select();
            }
        }
        protected override void OnPropertyGridSelectedGridItemChanged()
        {
            object value = propertyGrid.SelectedGridItem.Value;
            if (value != null)
            {
                string valueString = value.ToString();
                //
                if (propertyGrid.SelectedObject == null) { }
                else if (propertyGrid.SelectedObject is ViewError) { }
                else if (propertyGrid.SelectedObject is ViewSolidSection) HighlightSection();
                else throw new NotImplementedException();
            }
        }
        protected override void Apply()
        {
            _viewSection = (ViewSection)propertyGrid.SelectedObject;
            //
            if ((_sectionToEditName == null && _sectionNames.Contains(_viewSection.Name)) ||                // create
                (_viewSection.Name != _sectionToEditName && _sectionNames.Contains(_viewSection.Name)))     // edit
                throw new CaeGlobals.CaeException("The selected section name already exists.");
            //
            if (_sectionToEditName == null)
            {
                // Create
                _controller.AddSectionCommand(Section);
            }
            else
            {
                // Replace
                if (_propertyItemChanged) _controller.ReplaceSectionCommand(_sectionToEditName, Section);
            }
        }
        protected override bool OnPrepareForm(string stepName, string sectionToEditName)
        {
            // To prevent clear of the selection
            _selectedPropertyGridItemChangedEventActive = false;
            // To prevent the call to frmMain.itemForm_VisibleChanged when minimized
            this.DialogResult = DialogResult.None;      
            this.btnOkAddNew.Visible = sectionToEditName == null;
            //
            _propertyItemChanged = false;
            _sectionNames = null;
            _sectionToEditName = null;
            _viewSection = null;
            lvTypes.Items.Clear();
            propertyGrid.SelectedObject = null;
            //
            _controller.SetSelectItemToPart();
            //
            _sectionNames = _controller.GetSectionNames();
            _sectionToEditName = sectionToEditName;
            string[] materialNames = _controller.GetMaterialNames();
            string[] solidPartNames = _controller.GetModelPartNames<CaeMesh.FeElement3D>();
            string[] solidElementSetNames = _controller.GetUserElementSetNames<CaeMesh.FeElement3D>();
            //
            if (_sectionNames == null)
                throw new CaeGlobals.CaeException("The section names must be defined first.");
            //
            PopulateListOfSections(materialNames, solidPartNames, solidElementSetNames);
            // Add sectios
            if (_sectionToEditName == null)
            {
                lvTypes.Enabled = true;
                _viewSection = null;
                //
                if (lvTypes.Items.Count == 1)
                {
                    lvTypes.Items[0].Selected = true;
                    // ??? The code in the previous line does not call OnListViewTypeSelectedIndexChanged ???
                    if (lvTypes.Items[0].Tag is ViewSection vs)
                    {
                        _viewSection = vs;
                        propertyGrid.SelectedObject = _viewSection;
                        propertyGrid.Select();
                        ItemSetDataEditor.SelectionForm.ItemSetData = _viewSection.Selection;
                        ItemSetDataEditor.SelectionForm.Show(this);
                    }
                }
            }
            else
            {
                Section = _controller.GetSection(_sectionToEditName); // to clone
                // Select the appropriate constraint in the list view - disable event SelectedIndexChanged
                _lvTypesSelectedIndexChangedEventActive = false;
                if (_viewSection is ViewSolidSection) lvTypes.Items[0].Selected = true;
                lvTypes.Enabled = false;
                _lvTypesSelectedIndexChangedEventActive = true;
                //
                if (_viewSection is ViewSolidSection vss)
                {
                    // Check for deleted entities
                    CheckMissingValueRef(ref materialNames, vss.MaterialName, s => { vss.MaterialName = s; });
                    if (vss.RegionType == RegionTypeEnum.Selection.ToFriendlyString()) {}
                    else if (vss.RegionType == RegionTypeEnum.PartName.ToFriendlyString())
                        CheckMissingValueRef(ref solidPartNames, vss.PartName, s => { vss.PartName = s; });
                    else if (vss.RegionType == RegionTypeEnum.ElementSetName.ToFriendlyString())
                        CheckMissingValueRef(ref solidElementSetNames, vss.ElementSetName, s => { vss.ElementSetName = s; });
                    else throw new NotSupportedException();
                    //
                    _viewSection.PopululateDropDownLists(materialNames, solidPartNames, solidElementSetNames);
                }
                propertyGrid.SelectedObject = _viewSection;
                propertyGrid.Select();
            }
            _selectedPropertyGridItemChangedEventActive = true;
            //
            return true;
        }
        protected override void OnEnabledChanged()
        {
            // Form is Enabled On and Off by the itemSetForm
            if (this.Enabled)
            {
                // The FrmItemSet was closed with OK
                if (this.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    Section.CreationData = _controller.Selection.DeepClone();
                    _propertyItemChanged = true;
                }
                // The FrmItemSet was closed with Cancel
                else if (this.DialogResult == System.Windows.Forms.DialogResult.Cancel)
                {
                    if (Section.CreationData != null)
                    {
                        _controller.Selection.CopySelectonData(Section.CreationData);
                        HighlightSection();
                    }
                }
            }
            this.DialogResult = System.Windows.Forms.DialogResult.None;
        }


        // Methods                                                                                                                  
        public bool PrepareForm(string stepName, string sectionToEditName)
        {
            return OnPrepareForm(stepName, sectionToEditName);
        }
        private void PopulateListOfSections(string[] materialNames, string[] partNames, string[] elementSetNames)
        {
            ListViewItem item;
            // Initialize sections
            item = new ListViewItem("Solid section");
            if (materialNames.Length > 0)
            {
                if (partNames.Length + elementSetNames.Length >= 1)
                {
                    SolidSection ss = new SolidSection(GetSectionName(), materialNames[0], "", RegionTypeEnum.Selection);
                    ViewSolidSection vss = new ViewSolidSection(ss);
                    vss.PopululateDropDownLists(materialNames, partNames, elementSetNames);
                    item.Tag = vss;
                }
                else item.Tag = new ViewError("There is no part/element set defined for the solid section definition.");
            }
            else item.Tag = new ViewError("There is no material defined for the solid section definition.");
            lvTypes.Items.Add(item);
        }
        private string GetSectionName()
        {
            return NamedClass.GetNewValueName(_sectionNames, "Section-");
        }
        private void HighlightSection()
        {
            try
            {
                if (propertyGrid.SelectedObject is ViewSolidSection vss)
                {
                    if (vss.RegionType == RegionTypeEnum.PartName.ToFriendlyString())
                    {
                        _controller.Highlight3DObjects(new object[] { vss.PartName });
                    }
                    else if (vss.RegionType == RegionTypeEnum.ElementSetName.ToFriendlyString())
                    {
                        _controller.Highlight3DObjects(new object[] { vss.ElementSetName });
                    }
                    else if (vss.RegionType == RegionTypeEnum.Selection.ToFriendlyString())
                    {
                        _controller.Selection.SelectItem = vtkSelectItem.Part;
                        if (Section.CreationData != null) _controller.Selection.CopySelectonData(Section.CreationData); // deep copy to not clear
                        _controller.HighlightSelection();
                    }
                    else throw new NotSupportedException();
                }
            }
            catch { }
        }
    }
}
