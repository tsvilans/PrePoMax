using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrePoMax
{
    public partial class FrmViewResultHistoryOutput : Form
    {
        // Variables                                                                                                                
        private Controller _controller;


        // Properties                                                                                                               


        // Constructors                                                                                                             
        public FrmViewResultHistoryOutput(Controller controller)
        {
            InitializeComponent();
            //
            _controller = controller;
            dgvHistory.EnableCutMenu = false;
            dgvHistory.EnablePasteMenu = false;
        }


        // Event handlers                                                                                                           
        private void FrmHistoryOutput_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                dgvHistory.HidePlot();
                Hide();
            }
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            dgvHistory.HidePlot();
            Hide();
        }


        // Methods                                                                                                                  
        public void SetData(string[] columnNames, object[][] rowBasedData)
        {
            dgvHistory.Rows.Clear();
            dgvHistory.Columns.Clear();
            //
            DataGridViewTextBoxCell cell;
            cell = new DataGridViewTextBoxCell();
            DataGridViewColumn column;
            int maxNumCol = _controller.Settings.Post.MaxHistoryEntriesToShow;
            if (maxNumCol < 1)
                throw new NotSupportedException("The maximum number of history output entries to show is too small." +
                                                " Use Tools -> Settings to increase it.");
            //
            if (columnNames == null) return;
            //
            if (columnNames.Length > maxNumCol + 1)
            {
                CaeGlobals.MessageBoxes.ShowWarning("Only first " + maxNumCol + " columns of " + columnNames.Length + 
                                                    " will be displayed.");
            }
            //
            for (int i = 0; i < Math.Min(columnNames.Length, maxNumCol + 1); i++)
            {
                column = new DataGridViewColumn(cell);
                column.HeaderText = columnNames[i];
                column.FillWeight = 1;
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;
                // Enable sorting
                column.SortMode = DataGridViewColumnSortMode.Automatic;
                // Auto size
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                // Add
                dgvHistory.Columns.Add(column);
            }
            //
            if (rowBasedData.Length <= 0) return;
            foreach (var row in rowBasedData) dgvHistory.Rows.Add(row);
            // Remove column auto sizing
            int width;
            for (int i = 0; i <= dgvHistory.Columns.Count - 1; i++)
            {
                width = dgvHistory.Columns[i].Width;
                // Remove AutoSizing:
                dgvHistory.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgvHistory.Columns[i].Width = width;
            }
            //
            return;
        }
    }
}
