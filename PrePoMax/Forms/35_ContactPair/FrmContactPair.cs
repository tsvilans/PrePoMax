using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CaeMesh;
using CaeGlobals;
using CaeModel;

namespace PrePoMax.Forms
{
    class FrmContactPair : UserControls.FrmProperties, IFormBase, IFormItemSetDataParent, IFormHighlight
    {
        // Variables                                                                                                                
        private string[] _contactPairNames;
        private string _contactPairToEditName;
        private ViewContactPair _viewContactPair;
        private string _prevSelectionFormProperty;
        private Controller _controller;


        // Properties                                                                                                               
        public ContactPair ContactPair
        {
            get { return _viewContactPair != null ? _viewContactPair.GetBase() : null; }
            set { _viewContactPair = new ViewContactPair(value.DeepClone()); }
        }


        // Constructors                                                                                                             
        public FrmContactPair(Controller controller) 
        {
            InitializeComponent();
            //
            _controller = controller;
            _viewContactPair = null;
        }
        private void InitializeComponent()
        {
            this.gbProperties.SuspendLayout();
            this.SuspendLayout();
            // 
            // FrmContactPair
            // 
            this.ClientSize = new System.Drawing.Size(334, 411);
            this.Name = "FrmContactPair";
            this.Text = "Edit Contact Pair";
            this.gbProperties.ResumeLayout(false);
            this.ResumeLayout(false);

        }


        // Overrides                                                                                                                
        protected override void OnPropertyGridSelectedGridItemChanged()
        {
            //if (propertyGrid.SelectedGridItem.PropertyDescriptor == null) return;
            //
            ShowHideSelectionForm();
            //
            HighlightContactPair();
            //
            base.OnPropertyGridSelectedGridItemChanged();
        }
        protected override void OnApply(bool onOkAddNew)
        {
            if (propertyGrid.SelectedObject is ViewError ve) throw new CaeGlobals.CaeException(ve.Message);
            //
            _viewContactPair = (ViewContactPair)propertyGrid.SelectedObject;
            //
            ContactPair cp = ContactPair;
            if (cp == null) throw new CaeException("No contact pair was selected.");
            //
            if (cp.MasterRegionType == RegionTypeEnum.Selection &&
                (cp.MasterCreationIds == null || cp.MasterCreationIds.Length == 0))
                throw new CaeException("The contact pair master region must contain at least one item.");
            //
            if (cp.SlaveRegionType == RegionTypeEnum.Selection &&
                (cp.SlaveCreationIds == null || cp.SlaveCreationIds.Length == 0))
                throw new CaeException("The contact pair slave region must contain at least one item.");
            // Check for errors with constructor
            var tmp = new ContactPair(cp.Name, cp.SurfaceInteractionName, cp.MasterRegionName, cp.MasterRegionType,
                                      cp.SlaveRegionName, cp.SlaveRegionType);
            // Check if the name exists
            CheckName(_contactPairToEditName, cp.Name, _contactPairNames, "contact pair");
            // Create
            if (_contactPairToEditName == null)
            {
                _controller.AddContactPairCommand(cp);
            }
            // Replace
            else if (_propertyItemChanged)
            {
                _controller.ReplaceContactPairCommand(_contactPairToEditName, cp);
                _contactPairToEditName = null; // prevents the execution of toInternal in OnHideOrClose
            }
            // Convert the constraint from internal to show it
            else
            {
                ContactPairInternal(false);
            }
            // If all is successful close the ItemSetSelectionForm - except for OKAddNew
            if (!onOkAddNew) ItemSetDataEditor.SelectionForm.Hide();
        }
        protected override void OnHideOrClose()
        {
            // Close the ItemSetSelectionForm
            ItemSetDataEditor.SelectionForm.Hide();
            // Deactivate selection limits
            _controller.Selection.EnableShellEdgeFaceSelection = false;
            // Convert the contact pair from internal to show it
            ContactPairInternal(false);
            //
            base.OnHideOrClose();
        }       
        protected override bool OnPrepareForm(string stepName, string contactPairToEditName)
        {
            this.btnOkAddNew.Visible = contactPairToEditName == null;
            _controller.Selection.EnableShellEdgeFaceSelection = true;
            //
            _propertyItemChanged = false;
            _contactPairNames = null;
            _contactPairToEditName = null;
            _viewContactPair = null;
            propertyGrid.SelectedObject = null;
            //
            _contactPairNames = _controller.GetContactPairNames();
            _contactPairToEditName = contactPairToEditName;
            //
            string[] surfaceInteractionNames = _controller.GetSurfaceInteractionNames();
            string[] surfaceNames = _controller.GetUserSurfaceNames();
            if (surfaceInteractionNames == null) surfaceInteractionNames = new string[0];
            if (surfaceNames == null) surfaceNames = new string[0];
            //
            if (_contactPairNames == null)
                throw new CaeException("The contact pair names must be defined first.");            
            // Create new contact pair
            if (_contactPairToEditName == null)
            {
                if (surfaceInteractionNames.Length > 0)
                {
                    ContactPair = new ContactPair(GetContactPairName(), surfaceInteractionNames[0], "", RegionTypeEnum.Selection,
                                                  "", RegionTypeEnum.Selection);
                    ContactPair.MasterColor = _controller.Settings.Pre.ConstraintSymbolColor;
                    ContactPair.SlaveColor = ContactPair.MasterColor;
                    //
                    _viewContactPair.PopulateDropDownLists(surfaceInteractionNames, surfaceNames);
                    //
                    propertyGrid.SelectedObject = _viewContactPair;
                    propertyGrid.Select();
                }
                else propertyGrid.SelectedObject =
                        new ViewError("There is no surface interaction defined for the contact pair definition.");
            }
            // Edit existing contact pair
            else
            {
                // Get and convert a converted contact pair back to selection
                ContactPair = _controller.GetContactPair(_contactPairToEditName); // to clone
                // Convert the contact pair to internal to hide it
                ContactPairInternal(true);
                //
                ContactPair cp = ContactPair;
                if (cp.MasterCreationData != null) cp.MasterRegionType = RegionTypeEnum.Selection;
                if (cp.SlaveCreationData != null) cp.SlaveRegionType = RegionTypeEnum.Selection;
                //
                _viewContactPair = new ViewContactPair(cp);
                ViewContactPair vcp = _viewContactPair;
                // Surface interaction
                CheckMissingValueRef(ref surfaceInteractionNames, vcp.SurfaceInteractionName,
                                     s => { vcp.SurfaceInteractionName = s; });
                // Master
                if (vcp.MasterRegionType == RegionTypeEnum.Selection.ToFriendlyString()) { }
                else CheckMissingValueRef(ref surfaceNames, vcp.MasterSurfaceName, s => { vcp.MasterSurfaceName = s; });
                // Slave
                if (vcp.SlaveRegionType == RegionTypeEnum.Selection.ToFriendlyString()) { }
                else CheckMissingValueRef(ref surfaceNames, vcp.SlaveSurfaceName, s => { vcp.SlaveSurfaceName = s; });
                //
                _viewContactPair.PopulateDropDownLists(surfaceInteractionNames, surfaceNames);
                //
                propertyGrid.SelectedObject = _viewContactPair;
                propertyGrid.Select();
            }
            ShowHideSelectionForm();
            //
            HighlightContactPair(); // must be here if called from the menu
            //
            return true;
        }


        // Methods                                                                                                                  
        private string GetContactPairName()
        {
            return _contactPairNames.GetNextNumberedKey("Contact_pair");
        }
        private void HighlightContactPair()
        {
            try
            {
                if (propertyGrid.SelectedGridItem == null || propertyGrid.SelectedGridItem.PropertyDescriptor == null) return;
                //
                string property = propertyGrid.SelectedGridItem.PropertyDescriptor.Name;
                //
                _controller.ClearSelectionHistory();
                //
                if (ContactPair == null) { }
                else if (ContactPair is ContactPair cp)
                {
                    if (property == nameof(ViewTie.MasterRegionType))
                    {
                        HighlightRegion(cp.MasterRegionType, cp.MasterRegionName, cp.MasterCreationData, true, false);      // master
                    }
                    else if (property == nameof(ViewTie.SlaveRegionType))
                    {
                        HighlightRegion(cp.SlaveRegionType, cp.SlaveRegionName, cp.SlaveCreationData, true, true);          // slave
                    }
                    else
                    {
                        HighlightRegion(cp.MasterRegionType, cp.MasterRegionName, cp.MasterCreationData, true, false);      // master
                        HighlightRegion(cp.SlaveRegionType, cp.SlaveRegionName, cp.SlaveCreationData, false, true);         // slave
                    }
                }
                else throw new NotSupportedException();
            }
            catch { }
        }
        private void HighlightRegion(RegionTypeEnum regionType, string regionName, Selection creationData,
                                    bool clear, bool useSecondaryHighlightColor)
        {
            if (regionType == RegionTypeEnum.NodeSetName) _controller.HighlightNodeSets(new string[] { regionName }, useSecondaryHighlightColor);
            else if (regionType == RegionTypeEnum.SurfaceName) _controller.HighlightSurfaces(new string[] { regionName }, useSecondaryHighlightColor);
            else if (regionType == RegionTypeEnum.Selection)
            {
                SetSelectItem();
                //
                if (creationData != null)
                {
                    _controller.Selection = creationData.DeepClone();
                    _controller.HighlightSelection(clear, true, useSecondaryHighlightColor);
                }
            }
        }
        private void ShowHideSelectionForm()
        {
            if (propertyGrid.SelectedGridItem == null || propertyGrid.SelectedGridItem.PropertyDescriptor == null) return;
            //
            string property = propertyGrid.SelectedGridItem.PropertyDescriptor.Name;
            //
            if (property != _prevSelectionFormProperty) ItemSetDataEditor.SelectionForm.ResetSelection(false);
            _prevSelectionFormProperty = property;
            //
            if (ContactPair != null && ContactPair is ContactPair cp)
            {
                if (property == nameof(ViewTie.MasterRegionType) && cp.MasterRegionType == RegionTypeEnum.Selection)
                    ItemSetDataEditor.SelectionForm.ShowIfHidden(this.Owner);
                else if (property == nameof(ViewTie.SlaveRegionType) && cp.SlaveRegionType == RegionTypeEnum.Selection)
                    ItemSetDataEditor.SelectionForm.ShowIfHidden(this.Owner);
                else ItemSetDataEditor.SelectionForm.Hide();
            }
            else ItemSetDataEditor.SelectionForm.Hide();
            //
            SetSelectItem();
        }
        private void SetSelectItem()
        {
            if (propertyGrid.SelectedGridItem == null || propertyGrid.SelectedGridItem.PropertyDescriptor == null) return;
            //
            string property = propertyGrid.SelectedGridItem.PropertyDescriptor.Name;
            //
            if (ContactPair == null) { }
            else if (ContactPair is ContactPair cp)
            {
                if ((cp.MasterRegionType == RegionTypeEnum.Selection && property == nameof(ViewTie.MasterRegionType)) ||
                    (cp.SlaveRegionType == RegionTypeEnum.Selection && property == nameof(ViewTie.SlaveRegionType)))
                    _controller.SetSelectItemToSurface();
                else
                    _controller.SetSelectByToOff();
            }
        }
        private void ContactPairInternal(bool toInternal)
        {
            if (_contactPairToEditName != null)
            {
                // Convert the contact pair from/to internal to hide/show it
                _controller.GetContactPair(_contactPairToEditName).Internal = toInternal;
                _controller.FeModelUpdate(UpdateType.RedrawSymbols);
            }
        }
        //
        public void SelectionChanged(int[] ids)
        {
            if (propertyGrid.SelectedGridItem == null || propertyGrid.SelectedGridItem.PropertyDescriptor == null) return;
            //
            string property = propertyGrid.SelectedGridItem.PropertyDescriptor.Name;
            //
            bool changed = false;
            if (ContactPair != null && ContactPair is ContactPair cp)
            {
                if (property == nameof(ViewTie.MasterRegionType) && cp.MasterRegionType == RegionTypeEnum.Selection)
                {
                    cp.MasterCreationIds = ids;
                    cp.MasterCreationData = _controller.Selection.DeepClone();
                    changed = true;
                }
                else if (property == nameof(ViewTie.SlaveRegionType) && cp.SlaveRegionType == RegionTypeEnum.Selection)
                {
                    cp.SlaveCreationIds = ids;
                    cp.SlaveCreationData = _controller.Selection.DeepClone();
                    changed = true;
                }
                //
                if (changed)
                {
                    propertyGrid.Refresh();
                    //
                    _propertyItemChanged = true;
                }
            }
        }

        // IFormHighlight
        public void Highlight()
        {
            HighlightContactPair();
        }

        // IFormItemSetDataParent
        public bool IsSelectionGeometryBased()
        {
            // Prepare ItemSetDataEditor - prepare Geometry or Mesh based selection
            if (propertyGrid.SelectedGridItem == null || propertyGrid.SelectedGridItem.PropertyDescriptor == null) return true;
            //
            string property = propertyGrid.SelectedGridItem.PropertyDescriptor.Name;
            //
            if (ContactPair != null)
            {
                if (ContactPair is ContactPair cp)
                {
                    if (property == nameof(ViewTie.MasterRegionType) && cp.MasterRegionType == RegionTypeEnum.Selection)
                    {
                        if (cp.MasterCreationData != null) return cp.MasterCreationData.IsGeometryBased();
                        else return true;
                    }
                    else if (property == nameof(ViewTie.SlaveRegionType) && cp.SlaveRegionType == RegionTypeEnum.Selection)
                    {
                        if (cp.SlaveCreationData != null) return cp.SlaveCreationData.IsGeometryBased();
                        else return true;
                    }
                }
                else throw new NotSupportedException();
            }
            return true;
        }

    }


}
