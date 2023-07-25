using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeModel;
using CaeGlobals;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using CaeResults;
using CaeMesh;
using System.Data.Common;

namespace PrePoMax.Forms
{
    class FrmResultFieldOutput : UserControls.FrmPropertyDataListView, IFormBase
    {
        // Variables                                                                                                                
        private string[] _resultFieldOutputNames;
        private string _resultFieldOutputToEditName;
        private ViewResultFieldOutput _viewResultFieldOutput;
        private Controller _controller;
        private TabPage[] _pages;


        // Properties                                                                                                               
        public ResultFieldOutput ResultFieldOutput
        {
            get { return _viewResultFieldOutput.GetBase(); }
            set
            {
                var clone = value.DeepClone();
                if (clone == null) _viewResultFieldOutput = null;
                else if (clone is ResultFieldOutputLimit rfol)
                    _viewResultFieldOutput = new ViewResultFieldOutputLimit(rfol, _controller.GetResultPartNames(),
                                                                            _controller.GetResultUserElementSetNames(),
                                                                            ref _propertyItemChanged);
                else if (clone is ResultFieldOutputEnvelope rfoe)
                    _viewResultFieldOutput = new ViewResultFieldOutputEnvelope(rfoe);
                else throw new NotImplementedException();
            }
        }


        // Constructors                                                                                                             
        public FrmResultFieldOutput(Controller controller)
        {
            InitializeComponent();
            //
            _controller = controller;
            _viewResultFieldOutput = null;
            //
            int i = 0;
            _pages = new TabPage[tcProperties.TabPages.Count];
            foreach (TabPage tabPage in tcProperties.TabPages)
            {
                tabPage.Paint += TabPage_Paint;
                _pages[i++] = tabPage;
            }
            //
            dgvData.EnableCutMenu = false;
            dgvData.EnablePasteMenu = false;
            dgvData.EnablePlotMenu = false;
        }
        private void InitializeComponent()
        {
            this.tcProperties.SuspendLayout();
            this.tpProperties.SuspendLayout();
            this.gbType.SuspendLayout();
            this.SuspendLayout();
            // 
            // tpProperties
            // 
            this.tpProperties.Size = new System.Drawing.Size(302, 284);
            // 
            // tpDataPoints
            // 
            this.tpDataPoints.Size = new System.Drawing.Size(302, 284);
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
            this.propertyGrid.Size = new System.Drawing.Size(296, 278);
            // 
            // FrmAmplitude
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.ClientSize = new System.Drawing.Size(334, 461);
            this.Name = "FrmResultFieldOutput";
            this.Text = "Edit Field Output";
            this.VisibleChanged += new System.EventHandler(this.FrmResultFieldOutput_VisibleChanged);
            this.tcProperties.ResumeLayout(false);
            this.tpProperties.ResumeLayout(false);
            this.gbType.ResumeLayout(false);
            this.ResumeLayout(false);

        }


        // Event hadlers                                                                                                            
        private void FrmResultFieldOutput_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible) { }
            else dgvData.HidePlot();
        }
        private void TabPage_Paint(object sender, PaintEventArgs e)
        {
            SolidBrush fillBrush = new SolidBrush(((TabPage)sender).BackColor);
            e.Graphics.FillRectangle(fillBrush, e.ClipRectangle);
            // Enable copy/paste without first selecting the cell 0,0
            if (sender == tpDataPoints)
            {
                ActiveControl = dgvData;
                dgvData.ClearSelection();
                if (dgvData.RowCount > 0 && dgvData.Columns.Count > 0) dgvData[0, 0].Selected = true;
            }
        }
        private void Binding_ListChanged(object sender, ListChangedEventArgs e)
        {
            _propertyItemChanged = true;
        }


        // Overrides                                                                                                                
        protected override void OnPropertyGridPropertyValueChanged()
        {
            string property = propertyGrid.SelectedGridItem.PropertyDescriptor.Name;
            //
            if (_viewResultFieldOutput is ViewResultFieldOutputLimit vrfol)
            {
                if (property == nameof(vrfol.LimitPlotBasedOn))
                {
                    SetTabPages(vrfol);
                }
            }
            else if (_viewResultFieldOutput is ViewResultFieldOutputEnvelope vrfoe)
            {
            }
            //
            base.OnPropertyGridPropertyValueChanged();
        }
        protected override void OnListViewTypeSelectedIndexChanged()
        {
            if (lvTypes.SelectedItems != null && lvTypes.SelectedItems.Count == 1)
            {
                object itemTag = lvTypes.SelectedItems[0].Tag;
                SetTabPages(itemTag);
                //
                propertyGrid.SelectedObject = lvTypes.SelectedItems[0].Tag;
                propertyGrid.Select();
            }
        }
        protected override void OnApply(bool onOkAddNew)
        {
            if (propertyGrid.SelectedObject == null) throw new CaeException("No item selected.");
            //
            _viewResultFieldOutput = (ViewResultFieldOutput)propertyGrid.SelectedObject;
            // Check if the name exists
            CheckName(_resultFieldOutputToEditName, _viewResultFieldOutput.Name, _resultFieldOutputNames, "field output");
            // Cyclic reference
            if (_controller.CurrentResult.AreResultFieldOutputsInCyclicDependance(
                _resultFieldOutputToEditName, _viewResultFieldOutput.GetBase()))
            {
                throw new CaeException("The selected dependent field output creates a cyclic reference!");
            }
            // Check for zero limits
            if (_viewResultFieldOutput is ViewResultFieldOutputLimit vrfosf)
            {
                ResultFieldOutputLimit rfosf = (ResultFieldOutputLimit)vrfosf.GetBase();
                foreach (var entry in rfosf.ItemNameLimit)
                {
                    if (entry.Value == 0) throw new CaeException("All limit values must be different from 0.");
                }
            }
            // Create
            if (_resultFieldOutputToEditName == null)
            {
                _controller.AddResultFieldOutput(ResultFieldOutput);
            }
            // Replace
            else if (_propertyItemChanged)
            {
                _controller.ReplaceResultFieldOutput(_resultFieldOutputToEditName, ResultFieldOutput);
            }
        }
        protected override bool OnPrepareForm(string stepName, string resultFieldOutputToEditName)
        {
            this.btnOkAddNew.Visible = resultFieldOutputToEditName == null;
            //
            _propertyItemChanged = false;
            _stepName = null;
            _resultFieldOutputNames = null;
            _resultFieldOutputToEditName = null;
            _viewResultFieldOutput = null;
            lvTypes.Items.Clear();
            propertyGrid.SelectedObject = null;
            dgvData.DataSource = null;
            dgvData.Columns.Clear();
            //
            _stepName = stepName;
            _resultFieldOutputNames = _controller.GetResultFieldOutputNames();
            _resultFieldOutputToEditName = resultFieldOutputToEditName;
            Dictionary<string, string[]> filedNameComponentNames =
                _controller.CurrentResult.GetAllVisibleFiledNameComponentNames();
            // Remove self
            if (resultFieldOutputToEditName != null) filedNameComponentNames.Remove(resultFieldOutputToEditName);
            //
            string[] partNames = _controller.GetResultPartNames();
            string[] elementSetNames = _controller.GetResultUserElementSetNames();
            //
            if (_resultFieldOutputNames == null)
                throw new CaeException("The field output names must be defined first.");
            //
            PopulateListOfResultFieldOutputs(filedNameComponentNames, partNames, elementSetNames);
            //
            if (resultFieldOutputToEditName == null)
            {
                lvTypes.Enabled = true;
                _viewResultFieldOutput = null;
                if (lvTypes.Items.Count == 1) _preselectIndex = 0;
                // Show only propertes tab
                tcProperties.TabPages.Clear();
                tcProperties.TabPages.Add(_pages[0]);   // properites
            }
            else
            {
                ResultFieldOutput = _controller.GetResultFieldOutput(resultFieldOutputToEditName); // to clone
                _propertyItemChanged = !ResultFieldOutput.Valid;
                //
                int selectedId;
                if (_viewResultFieldOutput is ViewResultFieldOutputLimit vrfol)
                {
                    selectedId = 0;
                    // Check
                    string[] fieldNames = filedNameComponentNames.Keys.ToArray();
                    CheckMissingValueRef(ref fieldNames, vrfol.FieldName, s => { vrfol.FieldName = s; });
                    string[] componentNames = filedNameComponentNames[vrfol.FieldName];
                    CheckMissingValueRef(ref componentNames, vrfol.ComponentName, s => { vrfol.ComponentName = s; });
                    //
                    vrfol.PopulateDropDownLists(filedNameComponentNames);
                }
                else if (_viewResultFieldOutput is ViewResultFieldOutputEnvelope vrfoe)
                {
                    selectedId = 1;
                    // Check
                    string[] fieldNames = filedNameComponentNames.Keys.ToArray();
                    CheckMissingValueRef(ref fieldNames, vrfoe.FieldName, s => { vrfoe.FieldName = s; });
                    string[] componentNames = filedNameComponentNames[vrfoe.FieldName];
                    CheckMissingValueRef(ref componentNames, vrfoe.ComponentName, s => { vrfoe.ComponentName = s; });
                    //
                    vrfoe.PopulateDropDownLists(filedNameComponentNames);
                }
                else throw new NotSupportedException();
                //

                //
                lvTypes.Items[selectedId].Tag = _viewResultFieldOutput;
                _preselectIndex = selectedId;
            }
            //
            _controller.SetSelectByToOff();
            //
            return true;
        }
        

        // Methods                                                                                                                  
        private void PopulateListOfResultFieldOutputs(Dictionary<string, string[]> filedNameComponentNames,
                                                      string[] partNames, string[] elementSetNames)
        {
            // Populate list view
            ListViewItem item;
            // Limit
            item = new ListViewItem("Limit");
            ResultFieldOutputLimit rfosf = new ResultFieldOutputLimit(GetResultFieldOutputName("Limit"),
                                                                                    FOFieldNames.Stress,
                                                                                    FOComponentNames.Mises);
            ViewResultFieldOutputLimit vrfosf = new ViewResultFieldOutputLimit(rfosf, partNames, elementSetNames,
                                                                                             ref _propertyItemChanged);
            vrfosf.PopulateDropDownLists(filedNameComponentNames);
            item.Tag = vrfosf;
            lvTypes.Items.Add(item);
            // Envelope
            item = new ListViewItem("Envelope");
            ResultFieldOutputEnvelope rfoe = new ResultFieldOutputEnvelope(GetResultFieldOutputName("Envelope"),
                                                                           FOFieldNames.Stress,
                                                                           FOComponentNames.Mises);
            ViewResultFieldOutputEnvelope vrfoe = new ViewResultFieldOutputEnvelope(rfoe);
            vrfoe.PopulateDropDownLists(filedNameComponentNames);
            item.Tag = vrfoe;
            lvTypes.Items.Add(item);
        }
        private string GetResultFieldOutputName(string name)
        {
            if (name == null || name == "") name = "FieldOutput";
            name = name.Replace(' ', '_');
            name = _resultFieldOutputNames.GetNextNumberedKey(name);
            //
            return name;
        }
        private void SetTabPages(object item)
        {
            if (item is ViewResultFieldOutputLimit vrfol)
            {
                // Clear
                dgvData.DataSource = null;
                dgvData.Columns.Clear();
                tcProperties.TabPages.Clear();
                //
                tcProperties.TabPages.Add(_pages[0]);   // properites
                tcProperties.TabPages.Add(_pages[1]);   // data points
                _pages[1].Text = "Limit Values";
                //
                SetDataGridViewBinding(vrfol.DataPoints);
                //
                dgvData.AllowUserToAddRows = false;
                dgvData.AllowUserToDeleteRows = false;
                dgvData.Columns[0].ReadOnly = true;
                dgvData.Columns[0].Width = 150;
                dgvData.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                //
                dgvData.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                //
                _viewResultFieldOutput = vrfol;
            }
            else if (item is ViewResultFieldOutputEnvelope vrfoe)
            {
                // Clear
                dgvData.DataSource = null;
                dgvData.Columns.Clear();
                tcProperties.TabPages.Clear();
                //
                tcProperties.TabPages.Add(_pages[0]);   // properites
                //
                _viewResultFieldOutput = vrfoe;
            }
            else throw new NotImplementedException();
        }
        private void SetDataGridViewBinding(object data)
        {
            BindingSource binding = new BindingSource();
            binding.DataSource = data;
            dgvData.DataSource = binding; // bind datagridview to binding source - enables adding of new lines
            binding.ListChanged += Binding_ListChanged;
        }
        private void SetAllGridViewUnits()
        {
            string noUnit = "/";
            // Amplitude
            SetGridViewUnit(nameof(AmplitudeDataPoint.Time), _controller.Model.UnitSystem.TimeUnitAbbreviation,
                            _controller.Model.UnitSystem.FrequencyUnitAbbreviation);
            SetGridViewUnit(nameof(AmplitudeDataPoint.Amplitude), noUnit, null);
            //
            dgvData.XColIndex = 0;
            dgvData.StartPlotAtZero = true;
        }
        private void SetGridViewUnit(string columnName, string unit1, string unit2)
        {
            DataGridViewColumn col = dgvData.Columns[columnName];
            if (col != null)
            {
                // Unit
                if (col.HeaderText != null)
                {
                    col.HeaderText = col.HeaderText.ReplaceFirst("?", unit1);
                    if (unit2 != null) col.HeaderText = col.HeaderText.ReplaceFirst("?", unit2);
                }
                // Alignment
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;
                // Width
                col.Width += 10;
            }
        }

       
    }
}
