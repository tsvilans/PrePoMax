using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CaeModel;
using CaeGlobals;
using PrePoMax.PropertyViews;
using System.Runtime.CompilerServices;
using System.Reflection;

namespace PrePoMax.Forms
{
    public partial class FrmMaterial : UserControls.PrePoMaxChildForm, IFormBase
    {
        // Variables                                                                                                                
        private string[] _materialNames;
        private string _materialToEditName;
        private Material _material;
        private Controller _controller;
        private TabPage[] _pages;
        private bool _useSimpleEditor;      // a switch to change the form into a simple material editor form
        private bool _preview;              // a switch to change the form into a preview material form


        // Properties                                                                                                               
        public Material Material
        {
            get { return _material; }
            set
            {
                _material = value.DeepClone();
                // Save selected property
                int selectedId = -1;
                if (lvAddedProperties.SelectedIndices.Count > 0)
                    selectedId = lvAddedProperties.SelectedIndices[0];
                //
                ShowMaterial();
                // Select previous property
                if (selectedId >= 0 && selectedId < lvAddedProperties.Items.Count) 
                    lvAddedProperties.Items[selectedId].Selected = true;
            }
        }
        public bool UseSimpleEditor { get { return _useSimpleEditor; } set { _useSimpleEditor = value.DeepClone(); } }


        // Constructors                                                                                                             
        public FrmMaterial(Controller controller)
        {
            InitializeComponent();
            //
            _controller = controller;
            _material = null;
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
            //
            ClearControls();
            //
            _useSimpleEditor = false;
        }


        // Event hadlers                                                                                                            
        private void FrmMaterial_VisibleChanged(object sender, EventArgs e)
        {
            cbTemperatureDependent.Enabled = !(_useSimpleEditor || _preview);
            tvProperties.Visible = !_useSimpleEditor;
            lvAddedProperties.Visible = !_useSimpleEditor;
            //
            if (Visible) { }
            else
            {
                dgvData.HidePlot();
            }
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
        private void tvProperties_DoubleClick(object sender, EventArgs e)
        {
            btnAdd_Click(null, null);
        }
        private void tbName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;  // no beep
            }
        }
        private void cbTemperatureDependent_CheckedChanged(object sender, EventArgs e)
        {
            HideShowTemperature();
            _propertyItemChanged = true;
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!_preview && tvProperties.SelectedNodes != null)
            {
                foreach (TreeNode treeNode in tvProperties.SelectedNodes)
                {
                    if (treeNode.Tag != null)
                    {
                        string propertyText = treeNode.Text;
                        //
                        ListViewItem existingItem = null;
                        if (lvAddedProperties.Items.Count > 0)
                            existingItem = lvAddedProperties.FindItemWithText(propertyText, true, 0, false);
                        //
                        if (existingItem == null)
                        {
                            ListViewItem item = new ListViewItem(propertyText);
                            if (treeNode.Tag is MaterialProperty mp)
                            {
                                if (mp is Density de)
                                    item.Tag = new ViewDensity(de.DeepClone());
                                else if (mp is SlipWear sw)
                                    item.Tag = new ViewSlipWear(sw.DeepClone());
                                else if (mp is Elastic el)
                                    item.Tag = new ViewElastic(el.DeepClone());
                                else if (mp is Plastic pl)
                                    item.Tag = new ViewPlastic(pl.DeepClone());
                                else if (mp is ThermalExpansion te)
                                    item.Tag = new ViewThermalExpansion(te.DeepClone(), cbTemperatureDependent.Checked);
                                else if (mp is ThermalConductivity tc)
                                    item.Tag = new ViewThermalConductivity(tc.DeepClone());
                                else if (mp is SpecificHeat sh)
                                    item.Tag = new ViewSpecificHeat(sh.DeepClone());
                                else throw new NotSupportedException();
                            }
                            else throw new NotSupportedException();
                            //
                            lvAddedProperties.Items.Add(item);
                            int id = lvAddedProperties.Items.IndexOf(item);
                            lvAddedProperties.Items[id].Selected = true;
                            lvAddedProperties.Select();
                        }
                    }
                }
                tvProperties.SelectedNodes.Clear();
            }
            _propertyItemChanged = true;
        }
        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lvAddedProperties.SelectedItems.Count == 1)
            {
                ListViewItem item = lvAddedProperties.SelectedItems[0];                
                int index = item.Index;
                if (index == lvAddedProperties.Items.Count - 1) index--;
                lvAddedProperties.Items.Remove(item);
                //
                if (lvAddedProperties.Items.Count > 0) lvAddedProperties.Items[index].Selected = true;
                else ClearControls();
            }
            _propertyItemChanged = true;
        }
        private void btnMoveUp_Click(object sender, EventArgs e)
        {
            try
            {
                int currentIndex = lvAddedProperties.SelectedItems[0].Index;
                ListViewItem item = lvAddedProperties.Items[currentIndex];
                if (currentIndex > 0)
                {
                    lvAddedProperties.Items.RemoveAt(currentIndex);
                    lvAddedProperties.Items.Insert(currentIndex - 1, item);
                }
            }
            catch
            { }
        }
        private void btnMoveDown_Click(object sender, EventArgs e)
        {
            try
            {
                int currentIndex = lvAddedProperties.SelectedItems[0].Index;
                ListViewItem item = lvAddedProperties.Items[currentIndex];
                if (currentIndex < lvAddedProperties.Items.Count - 1)
                {
                    lvAddedProperties.Items.RemoveAt(currentIndex);
                    lvAddedProperties.Items.Insert(currentIndex + 1, item);
                }
            }
            catch
            { }
        }
        //
        private void lvAddedProperties_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvAddedProperties.SelectedItems.Count == 1)
            {
                // Clear
                dgvData.DataSource = null;      
                dgvData.Columns.Clear();
                tcProperties.TabPages.Clear();
                //
                if (lvAddedProperties.SelectedItems[0].Tag is ViewDensity vd)
                {
                    tcProperties.TabPages.Add(_pages[0]);   // properites
                    tcProperties.TabPages.Add(_pages[1]);   // data points
                    //
                    SetDataGridViewBinding(vd.DataPoints);
                }
                else if (lvAddedProperties.SelectedItems[0].Tag is ViewElastic ve)
                {
                    tcProperties.TabPages.Add(_pages[0]);   // properites
                    tcProperties.TabPages.Add(_pages[1]);   // data points
                    //
                    SetDataGridViewBinding(ve.DataPoints);
                }
                else if (lvAddedProperties.SelectedItems[0].Tag is ViewElasticWithDensity)
                {
                    tcProperties.TabPages.Add(_pages[0]);
                }
                else if (lvAddedProperties.SelectedItems[0].Tag is ViewPlastic vp)
                {
                    tcProperties.TabPages.Add(_pages[1]);   // data points
                    tcProperties.TabPages.Add(_pages[0]);   // properites
                    //
                    SetDataGridViewBinding(vp.DataPoints);
                }
                else if (lvAddedProperties.SelectedItems[0].Tag is ViewThermalExpansion vte)
                {
                    tcProperties.TabPages.Add(_pages[0]);   // properites
                    tcProperties.TabPages.Add(_pages[1]);   // data points
                    //
                    SetDataGridViewBinding(vte.DataPoints);
                }
                else if (lvAddedProperties.SelectedItems[0].Tag is ViewThermalConductivity vtc)
                {
                    tcProperties.TabPages.Add(_pages[0]);   // properites
                    tcProperties.TabPages.Add(_pages[1]);   // data points
                    //
                    SetDataGridViewBinding(vtc.DataPoints);
                }
                else if (lvAddedProperties.SelectedItems[0].Tag is ViewSpecificHeat vsh)
                {
                    tcProperties.TabPages.Add(_pages[0]);   // properites
                    tcProperties.TabPages.Add(_pages[1]);   // data points
                    //
                    SetDataGridViewBinding(vsh.DataPoints);
                }
                else if (lvAddedProperties.SelectedItems[0].Tag is ViewSlipWear vsw)
                {
                    tcProperties.TabPages.Add(_pages[0]);
                }
                else throw new NotSupportedException();
                //
                propertyGrid.SelectedObject = lvAddedProperties.SelectedItems[0].Tag;
                //
                SetAllGridViewUnits();
                //
                HideShowTemperature();
            }
            lvAddedProperties.Select();
        }        
        private void Binding_ListChanged(object sender, ListChangedEventArgs e)
        {
            _propertyItemChanged = true;
        }
        private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            propertyGrid.Refresh();
            _propertyItemChanged = true;
        }
        private void dgvData_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBoxes.ShowError(e.Exception.Message);
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                Add();
                //
                Hide();
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
            
        }
        private void btnOKAddNew_Click(object sender, EventArgs e)
        {
            try
            {
                Add();
                //
                
                PrepareForm(null, null);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            _useSimpleEditor = false;
            //
            Hide();
        }
        private void FrmMaterial_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                btnCancel_Click(null, null);
            }
        }
    

        // Methods                                                                                                                  
        public bool PrepareForm(string stepName, string materialToEditName)
        {
            this.btnOKAddNew.Visible = materialToEditName == null;
            //
            _propertyItemChanged = false;
            _materialNames = null;
            _materialToEditName = null;
            _material = null;
            _preview = false;
            lvAddedProperties.Items.Clear();
            ClearControls();
            SetControlStates();
            //
            _materialNames = _controller.GetMaterialNames();
            _materialToEditName = materialToEditName;
            // Initialize material properties
            TreeNode node;
            node = tvProperties.Nodes.Find("Density", true)[0];
            node.Tag = new Density(new double[][] { new double[] { 0, 0 } });
            node = tvProperties.Nodes.Find("Elastic", true)[0];
            node.Tag = new Elastic(new double[][] { new double[] { 0, 0, 0 } });
            node = tvProperties.Nodes.Find("Plastic", true)[0];
            node.Tag = new Plastic(new double[][] { new double[] { 0, 0, 0 } });
            node = tvProperties.Nodes.Find("ThermalExpansion", true)[0];
            node.Tag = new ThermalExpansion(new double[][] { new double[] { 0, 0 } });
            node = tvProperties.Nodes.Find("ThermalConductivity", true)[0];
            node.Tag = new ThermalConductivity(new double[][] { new double[] { 0, 0 } });
            node = tvProperties.Nodes.Find("SpecificHeat", true)[0];
            node.Tag = new SpecificHeat(new double[][] { new double[] { 0, 0 } });
            node = tvProperties.Nodes.Find("SlipWear", true)[0];
            node.Tag = new SlipWear(0, 0);
            //
            tvProperties.ExpandAll();
            //
            if (_materialToEditName == null)
            {
                _material = null;
                tbName.Text = GetMaterialName();
                tbDescription.Text = "";
                cbTemperatureDependent.Checked = false;
            }
            else
            {
                Material = _controller.GetMaterial(_materialToEditName); // to clone
                ShowMaterial();
            }
            // Simple material editor
            int delta;
            if (_useSimpleEditor)
            {                
                if (_materialToEditName == null)
                {
                    ViewMaterialProperty view = new ViewElasticWithDensity(new ElasticWithDensity(0, 0, 0));
                    ListViewItem item = new ListViewItem(view.Name);
                    item.Tag = view;
                    lvAddedProperties.Items.Add(item);
                    lvAddedProperties.Items[0].Selected = true;
                    lvAddedProperties.Select();
                    lvAddedProperties_SelectedIndexChanged(null, null);
                }
                delta = tcProperties.Top - labAvailable.Top;
                tcProperties.Top = labAvailable.Top;
                tcProperties.Height += delta;
                this.Height -= delta;
            }
            else
            {
                delta = (tvProperties.Bottom + 5) - tcProperties.Top;
                tcProperties.Top = tvProperties.Bottom + 5;
                tcProperties.Height -= delta;
                this.Height += delta;
            }
            //
            _controller.SetSelectByToOff();
            //
            return true;
        }
        public void ShowMaterial()
        {
            lvAddedProperties.Items.Clear();
            //
            tbName.Text = _material.Name;
            tbDescription.Text = _material.Description;
            cbTemperatureDependent.Checked = _material.TemperatureDependent;
            //
            if (_material.Properties.Count > 0)
            {
                ListViewItem item;
                ViewMaterialProperty view = null;
                foreach (var property in _material.Properties)
                {
                    if (property is Density den) view = new ViewDensity(den);
                    else if (property is SlipWear sw) view = new ViewSlipWear(sw);
                    else if (property is Elastic el) view = new ViewElastic(el);
                    else if (property is ElasticWithDensity ewd)
                    {
                        view = new ViewElasticWithDensity(ewd);
                        _useSimpleEditor = true;
                    }
                    else if (property is Plastic pl) view = new ViewPlastic(pl);
                    else if (property is ThermalExpansion te)
                        view = new ViewThermalExpansion(te, cbTemperatureDependent.Checked);
                    else if (property is ThermalConductivity tc) view = new ViewThermalConductivity(tc);
                    else if (property is SpecificHeat sh) view = new ViewSpecificHeat(sh);
                    else throw new NotSupportedException();
                    //
                    item = new ListViewItem(view.Name);
                    item.Tag = view;
                    item = lvAddedProperties.Items.Add(item);
                }
                //
                lvAddedProperties.Items[0].Selected = true;
                lvAddedProperties.Select();
            }
        }
        public void PrepareFormForPreview()
        {
            PrepareForm(null, null);
            tbName.Text = "";
            //
            _preview = true;
            SetControlStates();
        }
        private void SetControlStates()
        {
            tbName.ReadOnly = _preview;
            tbName.BackColor = SystemColors.Window;
            tbDescription.ReadOnly = _preview;
            tbDescription.BackColor = SystemColors.Window;
            // Buttons
            btnAdd.Visible = !_preview;
            btnMoveUp.Visible = !_preview;
            btnMoveDown.Visible = !_preview;
            btnRemove.Visible = !_preview;
            // All models
            tvProperties.Visible = !_preview;
            // Added models
            if (_preview)
            {
                lvAddedProperties.Left = tvProperties.Left;
                lvAddedProperties.Width = (btnMoveUp.Left + btnMoveUp.Width) - lvAddedProperties.Left;
                lvAddedProperties.Top = labAvailable.Top;
                lvAddedProperties.Height = tvProperties.Bottom - lvAddedProperties.Top;
                lvAddedProperties.BringToFront();
            }
            // Property grid
            propertyGrid.ReadOnly = _preview;
            // Data grid
            dgvData.AllowUserToAddRows = !_preview;
            dgvData.AllowUserToDeleteRows = !_preview;
            dgvData.ReadOnly = _preview;
            // Buttons
            btnOKAddNew.Visible = !_preview;
            btnOK.Visible = !_preview;
            //btnCancel.Visible = !_preview;
        }
        private void ClearControls()
        {
            propertyGrid.SelectedObject = null;
            dgvData.DataSource = null;
            //
            tcProperties.TabPages.Clear();
            tcProperties.TabPages.Add(_pages[0]);
        }
        public void Add()
        {
            // Check if the name exists
            UserControls.FrmProperties.CheckName(_materialToEditName, tbName.Text, _materialNames, "material");
            //
            _material = new Material(tbName.Text);
            _material.Description = tbDescription.Text;
            _material.TemperatureDependent = cbTemperatureDependent.Checked;
            //
            ViewMaterialProperty property;
            foreach (ListViewItem item in lvAddedProperties.Items)
            {
                property = (ViewMaterialProperty)item.Tag;
                if (property is ViewDensity vd)
                {
                    for (int i = 0; i < vd.DataPoints.Count; i++)
                    {
                        if (vd.DataPoints[i].Density <= 0) throw new CaeException("The density must be larger than 0.");
                    }
                }
                else if (property is ViewElastic ve && ve.YoungsModulus <= 0)
                {
                    throw new CaeException("The Young's modulus must be larger than 0.");
                }
                else if (property is ViewElasticWithDensity ewd)
                {
                    if (ewd.YoungsModulus <= 0) throw new CaeGlobals.CaeException("The Young's modulus must be larger than 0.");
                    if (ewd.Density <= 0) throw new CaeGlobals.CaeException("The density must be larger than 0.");
                }
                else if (property is ViewThermalExpansion vex)
                {
                    for (int i = 0; i < vex.DataPoints.Count; i++)
                    {
                        if (vex.DataPoints[i].ThermalExpansion <= 0)
                            throw new CaeException("The thermal expansion coefficient must be larger than 0.");
                    }
                }
                else if (property is ViewThermalConductivity vtc)
                {
                    for (int i = 0; i < vtc.DataPoints.Count; i++)
                    {
                        if (vtc.DataPoints[i].ThermalConductivity <= 0)
                            throw new CaeException("The thermal conductivity coefficient must be larger than 0.");
                    }
                }
                else if (property is ViewSpecificHeat vsh)
                {
                    for (int i = 0; i < vsh.DataPoints.Count; i++)
                    {
                        if (vsh.DataPoints[i].SpecificHeat <= 0)
                            throw new CaeException("The thermal conductivity coefficient must be larger than 0.");
                    }
                }
                else if (property is ViewSlipWear vsw)
                {
                    if (vsw.Hardness <= 0) throw new CaeException("The hardness must be larger than 0.");
                    if (vsw.WearCoefficient <= 0) throw new CaeException("The wear coefficient must be larger than 0.");
                }
                //
                _material.AddProperty(property.Base);
            }
            //
            if (_materialToEditName == null)
            {
                // Create
                _controller.AddMaterialCommand(Material);
            }
            else
            {
                // Replace
                if (_materialToEditName != Material.Name || _propertyItemChanged)
                {
                    _controller.ReplaceMaterialCommand(_materialToEditName, Material);
                }
            }
        }
        private void SetAllGridViewUnits()
        {
            string noUnit = "/";
            // Density
            SetGridViewUnit(nameof(DensityDataPoint.Density), _controller.Model.UnitSystem.DensityUnitAbbreviation);
            // Elastic
            SetGridViewUnit(nameof(ElasticDataPoint.YoungsModulus), _controller.Model.UnitSystem.PressureUnitAbbreviation);
            SetGridViewUnit(nameof(ElasticDataPoint.PoissonsRatio), noUnit);
            // Plastic
            SetGridViewUnit(nameof(PlasticDataPoint.Stress), _controller.Model.UnitSystem.PressureUnitAbbreviation);
            SetGridViewUnit(nameof(PlasticDataPoint.Strain), noUnit);
            // Thermal expansion
            SetGridViewUnit(nameof(ThermalExpansionDataPoint.ThermalExpansion),
                            _controller.Model.UnitSystem.ThermalExpansionUnitAbbreviation);
            // Thermal conductivity
            SetGridViewUnit(nameof(ThermalConductivityDataPoint.ThermalConductivity),
                            _controller.Model.UnitSystem.ThermalConductivityUnitAbbreviation);
            // Specific heat
            SetGridViewUnit(nameof(SpecificHeatDataPoint.SpecificHeat), _controller.Model.UnitSystem.SpecificHeatUnitAbbreviation);
            // Temperature
            SetGridViewUnit(nameof(TempDataPoint.Temperature), _controller.Model.UnitSystem.TemperatureUnitAbbreviation);
            //
            dgvData.XColIndex = 1;
            dgvData.StartPlotAtZero = true;
        }
        private void SetGridViewUnit(string columnName, string unit)
        {
            DataGridViewColumn col = dgvData.Columns[columnName];
            if (col != null)
            {
                // Unit
                if (col.HeaderText != null) col.HeaderText = col.HeaderText.Replace("?", unit);
                // Alignment
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;
            }
        }
        private string GetMaterialName()
        {
            return _materialNames.GetNextNumberedKey("Material");
        }
        private void SetDataGridViewBinding(object data)
        {
            BindingSource binding = new BindingSource();
            binding.DataSource = data;
            dgvData.DataSource = binding; // bind datagridview to binding source - enables adding of new lines
            binding.ListChanged += Binding_ListChanged;
        }
        private void HideShowTemperature()
        {
            if (lvAddedProperties.SelectedItems.Count > 0)
            {
                if (lvAddedProperties.SelectedItems[0].Tag is ViewDensity ||
                    lvAddedProperties.SelectedItems[0].Tag is ViewElastic ||
                    lvAddedProperties.SelectedItems[0].Tag is ViewThermalConductivity ||
                    lvAddedProperties.SelectedItems[0].Tag is ViewSpecificHeat)
                {
                    tcProperties.TabPages.Clear();
                    // Properites
                    if (!cbTemperatureDependent.Checked) tcProperties.TabPages.Add(_pages[0]);
                    // Data points
                    if (cbTemperatureDependent.Checked) tcProperties.TabPages.Add(_pages[1]);
                }
                else if (lvAddedProperties.SelectedItems[0].Tag is ViewThermalExpansion vte)
                {
                    tcProperties.TabPages.Clear();
                    // Properites
                    tcProperties.TabPages.Add(_pages[0]);
                    // Data points
                    if (cbTemperatureDependent.Checked) tcProperties.TabPages.Add(_pages[1]);
                    //
                    vte.SetTemperatureDependence(cbTemperatureDependent.Checked);
                    propertyGrid.Refresh();
                }
                else if (lvAddedProperties.SelectedItems[0].Tag is ViewSlipWear vsw)
                {
                    tcProperties.TabPages.Clear();
                    tcProperties.TabPages.Add(_pages[0]);
                }
            }
            //
            string temperatureName = nameof(TempDataPoint.Temperature);
            DataGridViewColumn col = dgvData.Columns[temperatureName];
            if (col != null) col.Visible = cbTemperatureDependent.Checked;
        }

        
    }
}
