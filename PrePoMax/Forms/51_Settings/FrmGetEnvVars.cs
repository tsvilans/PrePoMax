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
    public partial class FrmGetEnvVars : Form
    {
        // Variables                                                                                                                
        private List<EnvironmentVariable> _environmentVariables;
        private bool _valueChanged;


        // Properties                                                                                                               
        public List<EnvironmentVariable> EnvironmentVariables
        {
            get
            {
                // if value changed return new object to detect value changed in the property grid
                if (_valueChanged) return new List<EnvironmentVariable>(_environmentVariables); 
                else return _environmentVariables;
            }
        }


        // Constructors                                                                                                             
        public FrmGetEnvVars(List<EnvironmentVariable> environmentVariables)
        {
            

            InitializeComponent();

            _valueChanged = false;
            if (environmentVariables != null) _environmentVariables = environmentVariables;
            else _environmentVariables = new List<EnvironmentVariable>();

            BindingSource binding = new BindingSource();
            binding.DataSource = _environmentVariables;
            dgvEnvironmentVariables.DataSource = binding; //bind datagridview to binding source - enables adding of new lines

            dgvEnvironmentVariables.Columns["Name"].Width = 150;
            dgvEnvironmentVariables.Columns["Value"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }


        // Event hadlers                                                                                                            
        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                CheckForMissingData();
                DialogResult = System.Windows.Forms.DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }
        private void dgvEnvironmentVariables_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            _valueChanged = true;
        }
        private void dgvEnvironmentVariables_ColumnAdded(object sender, DataGridViewColumnEventArgs e)
        {
            _valueChanged = true;
        }
        private void dgvEnvironmentVariables_ColumnRemoved(object sender, DataGridViewColumnEventArgs e)
        {
            _valueChanged = true;
        }


        // Methods                                                                                                                  
        private void CheckForMissingData()
        {
            int  rowId = 1;
            foreach (DataGridViewRow row in dgvEnvironmentVariables.Rows)
            {
                if (rowId == dgvEnvironmentVariables.Rows.Count) continue; // skip the last row

                if (row.Cells[0].Value == null || row.Cells[0].Value.ToString().Trim() == "" || row.Cells[1].Value == null || row.Cells[1].Value.ToString().Trim() == "")
                    throw new CaeException("Row " + rowId.ToString() + " contains empty data. Add missing data or delete entire row.");

                rowId++;
            }
        }













    }
}
