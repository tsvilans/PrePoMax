using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CaeMesh;
using System.Reflection;
using CaeGlobals;
using DynamicTypeDescriptor;
using CaeJob;

namespace PrePoMax.Forms
{
    public partial class FrmGetDampingRatios : Form
    {
        // Variables                                                                                                                
        private List<DampingRatioAndRange> _dampingRatiosAndRange;
        private bool _valueChanged;


        // Properties                                                                                                               
        public List<DampingRatioAndRange> DampingRatiosAndRange
        {
            get
            {
                // if value changed return new object to detect value changed in the property grid
                if (_valueChanged) return new List<DampingRatioAndRange>(_dampingRatiosAndRange); 
                else return _dampingRatiosAndRange;
            }
        }


        // Constructors                                                                                                             
        public FrmGetDampingRatios(List<DampingRatioAndRange> dampingRatiosAndRange)
        {
            InitializeComponent();
            //
            _valueChanged = false;
            if (dampingRatiosAndRange != null) _dampingRatiosAndRange = dampingRatiosAndRange;
            else _dampingRatiosAndRange = new List<DampingRatioAndRange>();
            //
            BindingSource binding = new BindingSource();
            binding.DataSource = _dampingRatiosAndRange;
            dgvData.DataSource = binding; //bind datagridview to binding source - enables adding of new lines
            //
            dgvData.Columns["LowestMode"].Width = 103;
            dgvData.Columns["HighestMode"].Width = 105;
            dgvData.Columns["DampingRatio"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            //
            foreach (DataGridViewColumn column in dgvData.Columns)
            {
                column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }


        // Event hadlers                                                                                                            
        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                CheckForMissingData();
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void dgvData_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            _valueChanged = true;
        }
        private void dgvData_ColumnAdded(object sender, DataGridViewColumnEventArgs e)
        {
            _valueChanged = true;
        }
        private void dgvData_ColumnRemoved(object sender, DataGridViewColumnEventArgs e)
        {
            _valueChanged = true;
        }


        // Methods                                                                                                                  
        private void CheckForMissingData()
        {
            int  rowId = 1;
            foreach (DataGridViewRow row in dgvData.Rows)
            {
                if (rowId == dgvData.Rows.Count) continue; // skip the last row
                //
                if (row.Cells[0].Value == null || row.Cells[0].Value.ToString().Trim() == "" ||
                    row.Cells[1].Value == null || row.Cells[1].Value.ToString().Trim() == "" ||
                    row.Cells[2].Value == null || row.Cells[2].Value.ToString().Trim() == "")
                    throw new CaeException("Row " + rowId.ToString() + " contains empty data. Add missing data or delete entire row.");
                //
                if ((int)row.Cells[1].Value < (int)row.Cells[0].Value)
                    throw new CaeException("Row " + rowId.ToString() + 
                                           ": the highest mode value must be equal or larger than the lowest mode value.");
                rowId++;
            }
        }
        
    }
}
