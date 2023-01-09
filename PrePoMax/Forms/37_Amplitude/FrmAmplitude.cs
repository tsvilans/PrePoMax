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

namespace PrePoMax.Forms
{
    class FrmAmplitude : UserControls.FrmPropertyDataListView, IFormBase
    {
        // Variables                                                                                                                
        private string[] _amplitudeNames;
        private string _amplitudeToEditName;
        private ViewAmplitude _viewAmplitude;
        private Controller _controller;
        private TabPage[] _pages;


        // Properties                                                                                                               
        public Amplitude Amplitude
        {
            get { return _viewAmplitude.Base; }
            set
            {
                var clone = value.DeepClone();
                if (clone == null) _viewAmplitude = null;
                else if (clone is AmplitudeTabular at) _viewAmplitude = new ViewAmplitudeTabular(at);
                else throw new NotImplementedException();
            }
        }


        // Constructors                                                                                                             
        public FrmAmplitude(Controller controller)
        {
            InitializeComponent();
            //
            _controller = controller;
            _viewAmplitude = null;
            //
            propertyGrid.SetParent(this);   // for the Tab key to work
            //
            int i = 0;
            _pages = new TabPage[tcProperties.TabPages.Count];
            foreach (TabPage tabPage in tcProperties.TabPages)
            {
                tabPage.Paint += TabPage_Paint;
                _pages[i++] = tabPage;
            }
            
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
            // dgvData
            //
            this.dgvData.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgvData_DataError);
            // 
            // FrmAmplitude
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.ClientSize = new System.Drawing.Size(334, 461);
            this.Name = "FrmAmplitude";
            this.Text = "Edit Amplitude";
            this.VisibleChanged += new System.EventHandler(this.FrmAmplitude_VisibleChanged);
            this.tcProperties.ResumeLayout(false);
            this.tpProperties.ResumeLayout(false);
            this.gbType.ResumeLayout(false);
            this.ResumeLayout(false);

        }


        // Event hadlers                                                                                                            
        private void FrmAmplitude_VisibleChanged(object sender, EventArgs e)
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
                dgvData[0, 0].Selected = true;
            }
        }
        private void Binding_ListChanged(object sender, ListChangedEventArgs e)
        {
            _propertyItemChanged = true;
        }
        private void dgvData_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBoxes.ShowError(e.Exception.Message);
        }


        // Overrides                                                                                                                
        protected override void OnListViewTypeSelectedIndexChanged()
        {
            if (lvTypes.SelectedItems != null && lvTypes.SelectedItems.Count == 1)
            {
                // Clear
                dgvData.DataSource = null;
                dgvData.Columns.Clear();
                tcProperties.TabPages.Clear();
                //
                object itemTag = lvTypes.SelectedItems[0].Tag;
                if (itemTag is ViewAmplitudeTabular vat)
                {
                    tcProperties.TabPages.Add(_pages[0]);   // properites
                    tcProperties.TabPages.Add(_pages[1]);   // data points
                    //
                    SetDataGridViewBinding(vat.DataPoints);
                    //
                    _viewAmplitude = vat;
                }
                else throw new NotImplementedException();
                //
                propertyGrid.SelectedObject = lvTypes.SelectedItems[0].Tag;
                propertyGrid.Select();
                //
                SetAllGridViewUnits();
            }
        }
        protected override void OnApply(bool onOkAddNew)
        {
            if (propertyGrid.SelectedObject == null) throw new CaeException("No item selected.");
            //
            _viewAmplitude = (ViewAmplitude)propertyGrid.SelectedObject;
            // Check if the name exists
            CheckName(_amplitudeToEditName, _viewAmplitude.Name, _amplitudeNames, "amplitude");
            // Create
            if (_amplitudeToEditName == null)
            {
                _controller.AddAmplitudeCommand(Amplitude);
            }
            // Replace
            else if(_propertyItemChanged)
            {
                _controller.ReplaceAmplitudeCommand(_amplitudeToEditName, Amplitude);
            }
        }
        protected override bool OnPrepareForm(string stepName, string amplitudeToEditName)
        {
            this.btnOkAddNew.Visible = amplitudeToEditName == null;
            //
            _propertyItemChanged = false;
            _stepName = null;
            _amplitudeNames = null;
            _amplitudeToEditName = null;
            _viewAmplitude = null;
            lvTypes.Items.Clear();
            propertyGrid.SelectedObject = null;
            //
            _stepName = stepName;
            _amplitudeNames = _controller.GetAmplitudeNames();
            _amplitudeToEditName = amplitudeToEditName;
            //
            if (_amplitudeNames == null)
                throw new CaeException("The amplitude names must be defined first.");
            //
            PopulateListOfAmplitudes();
            //
            if (amplitudeToEditName == null)
            {
                lvTypes.Enabled = true;
                _viewAmplitude = null;
                if (lvTypes.Items.Count == 1) _preselectIndex = 0;
            }
            else
            {
                Amplitude = _controller.GetAmplitude(amplitudeToEditName); // to clone
                //
                int selectedId;
                if (_viewAmplitude.Base is AmplitudeTabular) selectedId = 0;
                else throw new NotSupportedException();
                //
                lvTypes.Items[selectedId].Tag = _viewAmplitude;
                _preselectIndex = selectedId;
            }
            //
            _controller.SetSelectByToOff();
            //
            return true;
        }
        

        // Methods                                                                                                                  
        private void PopulateListOfAmplitudes()
        {
            // Populate list view
            ListViewItem item;
            // Tabular
            item = new ListViewItem("Tabular");
            ViewAmplitudeTabular vat = new ViewAmplitudeTabular(new AmplitudeTabular(GetAmplitudeName("Tabular"),
                                                                                     new double[][] { new double[2] }));
            item.Tag = vat;
            lvTypes.Items.Add(item);
        }
        private string GetAmplitudeName(string name)
        {
            if (name == null || name == "") name = "Amplitude";
            name = name.Replace(' ', '_');
            name = _amplitudeNames.GetNextNumberedKey(name);
            //
            return name;
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
