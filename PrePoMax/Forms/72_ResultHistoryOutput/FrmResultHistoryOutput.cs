using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeResults;
using CaeGlobals;
using System.Windows.Forms;

namespace PrePoMax.Forms
{
    class FrmResultHistoryOutput : UserControls.FrmPropertyListView, IFormBase, IFormItemSetDataParent, IFormHighlight
    {
        // Variables                                                                                                                
        private string[] _historyOutputSetNames;
        private string _historyOutputToEditName;
        private ViewResultHistoryOutput _viewHistoryOutput;
        private Controller _controller;


        // Properties                                                                                                               
        public  ResultHistoryOutput ResultHistoryOutput
        {
            get { return _viewHistoryOutput != null ? _viewHistoryOutput.GetBase() : null; }
            set
            {
                if (value is ResultHistoryOutputFromField rhoff) _viewHistoryOutput =
                        new ViewResultHistoryOutputFromField(rhoff.DeepClone());
                else throw new NotImplementedException();
            }
        }


        // Constructors                                                                                                             
        public FrmResultHistoryOutput(Controller controller)
        {
            InitializeComponent();
            //
            _controller = controller;
            _viewHistoryOutput = null;
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
            // FrmHistoryOutput
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.ClientSize = new System.Drawing.Size(334, 461);
            this.Name = "FrmResultHistoryOutput";
            this.Text = "Edit History Output";
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
                if (itemTag is ViewError)  _viewHistoryOutput = null;
                else if (itemTag is ViewResultHistoryOutputFromField vrhoff) _viewHistoryOutput = vrhoff;
                else throw new NotImplementedException();
                //
                ShowHideSelectionForm();
                //
                propertyGrid.SelectedObject = itemTag;
                propertyGrid.Select();
                //
                HighlightHistoryOutput();
            }
        }
        protected override void OnPropertyGridPropertyValueChanged()
        {
            string property = propertyGrid.SelectedGridItem.PropertyDescriptor.Name;
            //
            if (property == nameof(_viewHistoryOutput.RegionType))
            {
                ShowHideSelectionForm();
                //
                HighlightHistoryOutput();
            }
            else if (_viewHistoryOutput is ViewResultHistoryOutputFromField vrhoff &&
                     (property == nameof(vrhoff.NodeSetName) ||
                      property == nameof(vrhoff.SurfaceName)))
            {
                HighlightHistoryOutput();
            }
            //
            base.OnPropertyGridPropertyValueChanged();
        }
        protected override void OnApply(bool onOkAddNew)
        {
            if (propertyGrid.SelectedObject is ViewError ve) throw new CaeException(ve.Message);
            //
            _viewHistoryOutput = (ViewResultHistoryOutput)propertyGrid.SelectedObject;
            //
            if (ResultHistoryOutput == null) throw new CaeException("No history output was selected.");
            //
            if (ResultHistoryOutput.RegionType == RegionTypeEnum.Selection &&
                (ResultHistoryOutput.CreationIds == null || ResultHistoryOutput.CreationIds.Length == 0))
                throw new CaeException("The history output must contain at least one item.");
            // Check if the name exists
            CheckName(_historyOutputToEditName, ResultHistoryOutput.Name, _historyOutputSetNames, "history output");
            // Create
            if (_historyOutputToEditName == null)
            {
                _controller.AddResultHistoryOutput(ResultHistoryOutput);
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
        protected override bool OnPrepareForm(string stepName, string historyOutputToEditName)
        {
            this.btnOkAddNew.Visible = historyOutputToEditName == null;
            //
            _propertyItemChanged = false;
            _stepName = null;
            _historyOutputSetNames = null;
            _historyOutputToEditName = null;
            _viewHistoryOutput = null;
            lvTypes.Items.Clear();
            propertyGrid.SelectedObject = null;
            //
            _stepName = stepName;
            _historyOutputSetNames = _controller.GetResultHistoryOutputSetNames();
            _historyOutputToEditName = historyOutputToEditName;
            string[] nodeSetNames = _controller.GetResultUserNodeSetNames();
            string[] surfaceNames = _controller.GetResultUserSurfaceNames();
            Dictionary<string, string[]> filedNameComponentNames = _controller.CurrentResult.GetAllFiledNameComponentNames();
            //
            if (_historyOutputSetNames == null)
                throw new CaeException("The history output names must be defined first.");
            // Populate list view
            PopulateListOfHistoryOutputs(nodeSetNames, surfaceNames, filedNameComponentNames);
            // Create new history output
            if (_historyOutputToEditName == null)
            {
                lvTypes.Enabled = true;
                _viewHistoryOutput = null;
                //
                if (lvTypes.Items.Count == 1) _preselectIndex = 0;
                //
                HighlightHistoryOutput(); // must be here if called from the menu
            }
            // Edit existing history output
            else throw new NotSupportedException();
            //
            ShowHideSelectionForm();
            //
            return true;
        }


        // Methods                                                                                                                  
        private void PopulateListOfHistoryOutputs(string[] nodeSetNames, string[] surfaceNames,
                                                  Dictionary<string, string[]> filedNameComponentNames)
        {
            ListViewItem item;
            // Node output
            item = new ListViewItem("History output from field output");
            CaeResults.FieldData fieldData = _controller.CurrentFieldData;
            ResultHistoryOutputFromField rhoff = new ResultHistoryOutputFromField(GetHistoryOutputName("FF"),
                                                                                  fieldData.Name, fieldData.Component,
                                                                                  "", RegionTypeEnum.Selection);
            ViewResultHistoryOutputFromField vrhoff = new ViewResultHistoryOutputFromField(rhoff);
            vrhoff.PopulateDropDownLists(nodeSetNames, surfaceNames, filedNameComponentNames);
            item.Tag = vrhoff;
            lvTypes.Items.Add(item);
        }
        private string GetHistoryOutputName(string prefix)
        {
            return _historyOutputSetNames.GetNextNumberedKey(prefix + "H_output");
        }
        private void HighlightHistoryOutput()
        {
            try
            {
                _controller.ClearSelectionHistory();
                //
                if (_viewHistoryOutput == null) { }
                else if (ResultHistoryOutput is ResultHistoryOutputFromField)
                {
                    if (ResultHistoryOutput.RegionType == RegionTypeEnum.NodeSetName ||
                        ResultHistoryOutput.RegionType == RegionTypeEnum.SurfaceName)
                    {
                        _controller.Highlight3DObjects(new object[] { ResultHistoryOutput.RegionName });
                    }
                    else if (ResultHistoryOutput.RegionType == RegionTypeEnum.Selection)
                    {
                        SetSelectItem();
                        //
                        if (ResultHistoryOutput.CreationData != null)
                        {
                            _controller.Selection = ResultHistoryOutput.CreationData.DeepClone();
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
            if (ResultHistoryOutput != null && ResultHistoryOutput.RegionType == RegionTypeEnum.Selection)
                ItemSetDataEditor.SelectionForm.ShowIfHidden(this.Owner);
            else
                ItemSetDataEditor.SelectionForm.Hide();
            //
            SetSelectItem();
        }
        private void SetSelectItem()
        {
            if (ResultHistoryOutput != null && ResultHistoryOutput.RegionType == RegionTypeEnum.Selection)
            {
                if (ResultHistoryOutput is null) { }
                else if (ResultHistoryOutput is ResultHistoryOutputFromField) _controller.SetSelectItemToNode();
            }
            else _controller.SetSelectByToOff();
        }
        //
        public void SelectionChanged(int[] ids)
        {
            if (ResultHistoryOutput != null && ResultHistoryOutput.RegionType == RegionTypeEnum.Selection)
            {
                if (ResultHistoryOutput is ResultHistoryOutputFromField)
                {
                    ResultHistoryOutput.CreationIds = ids;
                    ResultHistoryOutput.CreationData = _controller.Selection.DeepClone();
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
            HighlightHistoryOutput();
        }

        // IFormItemSetDataParent
        public bool IsSelectionGeometryBased()
        {
            // Prepare ItemSetDataEditor - prepare Geometry or Mesh based selection
            if (ResultHistoryOutput == null || ResultHistoryOutput.CreationData == null) return true;
            return ResultHistoryOutput.CreationData.IsGeometryBased();
        }
    }
}
