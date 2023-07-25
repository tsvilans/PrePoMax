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
using System.Collections;

namespace PrePoMax.Forms
{    
    public partial class FrmSurfaceInteraction : UserControls.PrePoMaxChildForm, IFormBase
    {
        // Variables                                                                                                                
        private string[] _surfraceInteractionNames;
        private string _surfaceInteractionToEditName;
        private SurfaceInteraction _surfaceInteraction;
        private Controller _controller;
        private TabPage[] _pages;
        
        // Properties                                                                                                               
        public SurfaceInteraction SurfaceInteraction 
        { 
            get { return _surfaceInteraction; } 
            set { _surfaceInteraction = value.DeepClone(); } 
        }


        // Constructors                                                                                                             
        public FrmSurfaceInteraction(Controller controller)
        {
            InitializeComponent();
            //
            _controller = controller;
            _surfaceInteraction = null;
            //
            int i = 0;
            _pages = new TabPage[tcProperties.TabPages.Count];
            foreach (TabPage tabPage in tcProperties.TabPages)
            {
                tabPage.Paint += TabPage_Paint;
                _pages[i++] = tabPage;
            }
            //
            int count = 0;
            Dictionary<string, int> namePosition = new Dictionary<string, int>();
            foreach (TreeNode node in tvProperties.Nodes) namePosition.Add(node.Text, count++);
            lvAddedProperties.ListViewItemSorter = new ListViewItemComparer(0, namePosition);
            //
            ClearControls();
        }


        // Event handling
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
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (tvProperties.SelectedNode != null && tvProperties.SelectedNode.Tag != null)
            {
                string propertyName = tvProperties.SelectedNode.Text;
                //
                if (lvAddedProperties.FindItemWithText(propertyName) == null)
                {
                    ListViewItem item = new ListViewItem(propertyName);
                    if (tvProperties.SelectedNode.Tag is SurfaceInteractionProperty sip)
                    {
                        if (sip is SurfaceBehavior sb) item.Tag = new ViewSurfaceBehavior(sb.DeepClone());
                        else if (sip is Friction fr) item.Tag = new ViewFriction(fr.DeepClone());
                        else if (sip is GapConductance gp) item.Tag = new ViewGapConductance(gp.DeepClone());
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
        private void lvAddedProperties_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvAddedProperties.SelectedItems.Count == 1)
            {
                // Clear
                dgvData.DataSource = null;
                dgvData.Columns.Clear();
                tcProperties.TabPages.Clear();
                //
                if (lvAddedProperties.SelectedItems[0].Tag is ViewSurfaceBehavior vsb)
                {
                    tcProperties.TabPages.Add(_pages[0]);   // properties
                    tcProperties.TabPages.Add(_pages[1]);   // data points
                    //
                    SetDataGridViewBinding(vsb.DataPoints);                    
                }
                else if (lvAddedProperties.SelectedItems[0].Tag is ViewFriction vf)
                {
                    tcProperties.TabPages.Add(_pages[0]);   // properties
                }
                else if (lvAddedProperties.SelectedItems[0].Tag is ViewGapConductance vgc)
                {
                    tcProperties.TabPages.Add(_pages[0]);   // properties
                    tcProperties.TabPages.Add(_pages[1]);   // data points
                    //
                    SetDataGridViewBinding(vgc.DataPoints);
                }
                else throw new NotSupportedException();
                //
                propertyGrid.SelectedObject = lvAddedProperties.SelectedItems[0].Tag;
                //
                SetAllGridViewUnits();
                //
                propertyGrid_PropertyValueChanged(null, null);
            }
            lvAddedProperties.Select();
        }
        private void Binding_ListChanged(object sender, ListChangedEventArgs e)
        {
            _propertyItemChanged = true;
        }
        private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (propertyGrid.SelectedObject != null && propertyGrid.SelectedObject is ViewSurfaceBehavior vsb)
            {
                if (vsb.PressureOverclosureType == PressureOverclosureEnum.Tabular && tcProperties.TabPages.Count == 1)
                    tcProperties.TabPages.Add(_pages[1]);
                else if (vsb.PressureOverclosureType != PressureOverclosureEnum.Tabular && tcProperties.TabPages.Count == 2)
                    tcProperties.TabPages.Remove(_pages[1]);
            }
            else if (propertyGrid.SelectedObject != null && propertyGrid.SelectedObject is ViewGapConductance vgc)
            {
                if (vgc.GapConductanceType == GapConductanceEnum.Tabular && tcProperties.TabPages.Count == 1)
                    tcProperties.TabPages.Add(_pages[1]);
                else if (vgc.GapConductanceType != GapConductanceEnum.Tabular && tcProperties.TabPages.Count == 2)
                    tcProperties.TabPages.Remove(_pages[1]);
            }
            //
            propertyGrid.Refresh();
            _propertyItemChanged = true;
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
                CaeGlobals.ExceptionTools.Show(this, ex);
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
            Hide();
        }
        private void FrmSurfaceInteraction_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }


        // Methods                                                                                                                  
        public bool PrepareForm(string stepName, string surfaceInteractionToEditName)
        {
            this.btnOKAddNew.Visible = surfaceInteractionToEditName == null;
            //
            _propertyItemChanged = false;
            _propertyItemChanged = false;
            _surfraceInteractionNames = null;
            _surfaceInteractionToEditName = null;
            _surfaceInteraction = null;
            lvAddedProperties.Items.Clear();
            ClearControls();
            //
            _surfraceInteractionNames = _controller.GetSurfaceInteractionNames();
            _surfaceInteractionToEditName = surfaceInteractionToEditName;
            // Initialize surface interaction properties
            tvProperties.Nodes.Find("Surface Behavior", true)[0].Tag = new SurfaceBehavior();
            tvProperties.Nodes.Find("Friction", true)[0].Tag = new Friction();
            tvProperties.Nodes.Find("Gap Conductance", true)[0].Tag = new GapConductance();
            tvProperties.ExpandAll();
            //
            if (_surfaceInteractionToEditName == null)
            {
                _surfaceInteraction = null;
                tbName.Text = GetSurfaceInteractionName();
            }
            else
            {
                SurfaceInteraction = _controller.GetSurfaceInteraction(_surfaceInteractionToEditName); // to clone
                //
                tbName.Text = _surfaceInteraction.Name;
                if (_surfaceInteraction.Properties.Count > 0)
                {
                    ListViewItem item;
                    ViewSurfaceInteractionProperty view;
                    foreach (var property in _surfaceInteraction.Properties)
                    {
                        if (property is SurfaceBehavior sb) view = new ViewSurfaceBehavior(sb);
                        else if (property is Friction fr) view = new ViewFriction(fr);
                        else if (property is GapConductance gc) view = new ViewGapConductance(gc);
                        else throw new NotSupportedException();
                        //
                        item = new ListViewItem(view.Name);
                        item.Tag = view;
                        lvAddedProperties.Items.Add(item);
                    }
                    //
                    lvAddedProperties.Items[0].Selected = true;
                    lvAddedProperties.Select();
                }
            }
            //
            _controller.SetSelectByToOff();
            //
            return true;
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
            UserControls.FrmProperties.CheckName(_surfaceInteractionToEditName, tbName.Text, _surfraceInteractionNames,
                                                 "surface interaction");
            //
            Friction friction = null;
            SurfaceBehavior surfaceBehavior = null;
            _surfaceInteraction = new SurfaceInteraction(tbName.Text);
            foreach (ListViewItem item in lvAddedProperties.Items)
            {
                _surfaceInteraction.AddProperty(((ViewSurfaceInteractionProperty)(item.Tag)).Base);
                if (item.Tag is ViewSurfaceBehavior vsb) surfaceBehavior = (SurfaceBehavior)vsb.Base;
                if (item.Tag is ViewFriction vf) friction = (Friction)vf.Base;
            }
            if (surfaceBehavior == null)
                throw new CaeException("Surface interaction must define a surface behavior.");
            // Tied contact requires friction definition
            if (surfaceBehavior != null && surfaceBehavior.PressureOverclosureType == PressureOverclosureEnum.Tied
                && friction == null)
            {
                throw new CaeException("A tied contact requires a stick slope definition using the friction interaction model. " + 
                                       "The value of the friction coefficient is irrelevant for tied contact.");
            }
            //
            if (_surfaceInteractionToEditName == null)
            {
                // Create
                _controller.AddSurfaceInteractionCommand(SurfaceInteraction);
            }
            else
            {
                // Replace
                if (_surfaceInteractionToEditName != SurfaceInteraction.Name || _propertyItemChanged)
                {
                    _controller.ReplaceSurfaceInteractionCommand(_surfaceInteractionToEditName, SurfaceInteraction);
                }
            }
        }
        private void SetAllGridViewUnits()
        {
            // Surface behavior
            SetGridViewUnit(nameof(PressureOverclosureDataPoint.Pressure), _controller.Model.UnitSystem.PressureUnitAbbreviation);
            SetGridViewUnit(nameof(PressureOverclosureDataPoint.Overclosure), _controller.Model.UnitSystem.LengthUnitAbbreviation);
            // Gap conductance
            SetGridViewUnit(nameof(GapConductanceDataPoint.Conductance),
                            _controller.Model.UnitSystem.HeatTransferCoefficientUnitAbbreviation);
            SetGridViewUnit(nameof(GapConductanceDataPoint.Pressure), _controller.Model.UnitSystem.PressureUnitAbbreviation);
            SetGridViewUnit(nameof(GapConductanceDataPoint.Temperature), _controller.Model.UnitSystem.TemperatureUnitAbbreviation);
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
        private string GetSurfaceInteractionName()
        {
            return _surfraceInteractionNames.GetNextNumberedKey("Surface_Interaction");
        }
        private void SetDataGridViewBinding(object data)
        {
            BindingSource binding = new BindingSource();
            binding.DataSource = data;
            dgvData.DataSource = binding; // bind datagridview to binding source - enables adding of new lines
            binding.ListChanged += Binding_ListChanged;
        }

    }
}
