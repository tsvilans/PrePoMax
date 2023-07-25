using CaeGlobals;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UserControls
{
    public class DataGridViewCopyPaste : DataGridView
    {
        // Variables                                                                                                                
        private int _xColIndex;
        //
        private ContextMenuStrip cmsCopyPaste;
        private IContainer components;
        private ToolStripMenuItem tsmiCut;
        private ToolStripMenuItem tsmiCopy;
        private ToolStripMenuItem tsmiPaste;
        private ToolStripSeparator tssDivider1;
        private ToolStripMenuItem tsmiPlot;
        private FrmDiagramView frmDiagramView;


        // Properties                                                                                                               
        public int XColIndex 
        {
            get { return _xColIndex; } 
            set
            {
                _xColIndex = value;
                if (_xColIndex < 0) _xColIndex = 0;
            }
        }
        public bool EnableCutMenu { get { return tsmiCut.Enabled; } set { tsmiCut.Enabled = value; } }
        public bool EnablePasteMenu { get { return tsmiPaste.Enabled; } set { tsmiPaste.Enabled = value; } }
        public bool EnablePlotMenu { get { return tsmiPlot.Enabled; } set { tsmiPlot.Enabled = value; } }
        public bool StartPlotAtZero
        {
            get { return frmDiagramView.StartPlotAtZero; }
            set { frmDiagramView.StartPlotAtZero = value; }
        }


        // Constructors                                                                                                             
        public DataGridViewCopyPaste()
        {
            InitializeComponent();
            //
            this.KeyDown += DataGridViewCopyPaste_KeyDown;
            //
            frmDiagramView = new FrmDiagramView();
        }
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataGridViewCopyPaste));
            this.cmsCopyPaste = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiCut = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.tssDivider1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiPlot = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsCopyPaste.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // cmsCopyPaste
            // 
            this.cmsCopyPaste.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiCut,
            this.tsmiCopy,
            this.tsmiPaste,
            this.tssDivider1,
            this.tsmiPlot});
            this.cmsCopyPaste.Name = "cmsCopyPaste";
            this.cmsCopyPaste.Size = new System.Drawing.Size(145, 98);
            // 
            // tsmiCut
            // 
            this.tsmiCut.Image = global::UserControls.Properties.Resources.Cut;
            this.tsmiCut.Name = "tsmiCut";
            this.tsmiCut.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.tsmiCut.Size = new System.Drawing.Size(144, 22);
            this.tsmiCut.Text = "Cut";
            this.tsmiCut.Click += new System.EventHandler(this.tsmiCut_Click);
            // 
            // tsmiCopy
            // 
            this.tsmiCopy.Image = global::UserControls.Properties.Resources.Copy;
            this.tsmiCopy.Name = "tsmiCopy";
            this.tsmiCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.tsmiCopy.Size = new System.Drawing.Size(144, 22);
            this.tsmiCopy.Text = "Copy";
            this.tsmiCopy.Click += new System.EventHandler(this.tsmiCopy_Click);
            // 
            // tsmiPaste
            // 
            this.tsmiPaste.Image = global::UserControls.Properties.Resources.Paste;
            this.tsmiPaste.Name = "tsmiPaste";
            this.tsmiPaste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.tsmiPaste.Size = new System.Drawing.Size(144, 22);
            this.tsmiPaste.Text = "Paste";
            this.tsmiPaste.Click += new System.EventHandler(this.tsmiPaste_Click);
            // 
            // tssDivider1
            // 
            this.tssDivider1.Name = "tssDivider1";
            this.tssDivider1.Size = new System.Drawing.Size(141, 6);
            // 
            // tsmiPlot
            // 
            this.tsmiPlot.Image = ((System.Drawing.Image)(resources.GetObject("tsmiPlot.Image")));
            this.tsmiPlot.Name = "tsmiPlot";
            this.tsmiPlot.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.tsmiPlot.Size = new System.Drawing.Size(144, 22);
            this.tsmiPlot.Text = "Plot";
            this.tsmiPlot.Click += new System.EventHandler(this.tsmiPlot_Click);
            // 
            // DataGridViewCopyPaste
            // 
            this.ContextMenuStrip = this.cmsCopyPaste;
            this.cmsCopyPaste.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }


        // Event handlers                                                                                                           
        private void DataGridViewCopyPaste_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                if (e.KeyCode == Keys.C)
                {
                    tsmiCopy_Click(null, null);
                }
                else if (e.KeyCode == Keys.X)
                {
                    tsmiCut_Click(null, null);
                }
                else if (e.KeyCode == Keys.V)
                {
                    tsmiPaste_Click(null, null);
                }
                else if (e.KeyCode == Keys.P)
                {
                    tsmiPlot_Click(null, null);
                }
            }
        }
        private void DataGridViewCopyPaste_KeyPress(object sender, KeyPressEventArgs e)
        {
        }
        // Context menu
        private void dgvData_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    // Context menu
                    if (e.RowIndex < 0 || e.ColumnIndex < 0)
                    {
                        ContextMenuStrip = null;
                        return;
                    }
                    else ContextMenuStrip = cmsCopyPaste;
                    // Move selection to the new cell
                    DataGridViewCell cell = Rows[e.RowIndex].Cells[e.ColumnIndex];
                    if (!cell.Selected)
                    {
                        EndEdit();
                        ClearSelection();
                        cell.Selected = true;
                    }
                    //
                    Dictionary<int, Dictionary<int, double>> values = GetValues();
                    tsmiPlot.Enabled = IsPlottingPossible(values); 
                }
            }
            catch { }
        }
        private void tsmiCut_Click(object sender, EventArgs e)
        {
            try
            {
                if (tsmiCut.Enabled)
                {
                    // Copy to clipboard
                    CopyToClipboard();
                    // Clear selected cells
                    foreach (DataGridViewCell dgvCell in SelectedCells) dgvCell.Value = 0;
                }
            }
            catch { }
        }
        private void tsmiCopy_Click(object sender, EventArgs e)
        {
            try
            {
                CopyToClipboard();
            }
            catch { }
        }
        private void tsmiPaste_Click(object sender, EventArgs e)
        {
            try
            {
                // Perform paste Operation
                if (tsmiPaste.Enabled) PasteClipboardValue();
            }
            catch { }
        }
        private void tsmiPlot_Click(object sender, EventArgs e)
        {
            try
            {
                Dictionary<int, double> rowValues;
                Dictionary<int, Dictionary<int, double>> values = GetValues();
                if (IsPlottingPossible(values))
                {
                    //
                    HashSet<int> numberOfcolumns = new HashSet<int>();
                    foreach (var entry in values) numberOfcolumns.Add(entry.Value.Count);
                    //
                    double[] xData = new double[values.Count];
                    double[][] yData = new double[numberOfcolumns.First() - 1][];
                    string[] yNames = new string[yData.Length];
                    int yIndex = 0;
                    int[] colIndices = null;
                    int[] rowIndices = values.Keys.ToArray();
                    Array.Sort(rowIndices);
                    // For each row
                    for (int i = 0; i < rowIndices.Length; i++)
                    {
                        // Get row values
                        rowValues = values[rowIndices[i]];
                        colIndices = rowValues.Keys.ToArray();
                        // Sort col indices
                        Array.Sort(colIndices);
                        // For each column
                        for (int j = 0; j < colIndices.Length; j++)
                        {
                            // Get x value
                            if (j == _xColIndex)
                            {
                                xData[i] = rowValues[colIndices[j]];
                            }
                            // Get y value
                            else
                            {
                                if (j < _xColIndex) yIndex = j;
                                else yIndex = j - 1;
                                // First row
                                if (i == 0)
                                {
                                    yData[yIndex] = new double[values.Count];
                                    yNames[yIndex] = Columns[colIndices[j]].HeaderText.Replace("\n", " ");
                                }
                                // Get y data
                                yData[yIndex][i] = rowValues[colIndices[j]];                                
                            }
                        }
                    }
                    //
                    frmDiagramView.CurveNames = yNames;
                    if (colIndices != null) frmDiagramView.XAxisTitle = Columns[colIndices[XColIndex]].HeaderText;
                    if (yData.Length == 1) frmDiagramView.YAxisTitle = yNames[0];
                    else frmDiagramView.YAxisTitle = "Data";
                    // After the axis names were set plot the data
                    frmDiagramView.SetData(xData, yData);
                    //
                    if (!frmDiagramView.Visible) frmDiagramView.Show(this);
                }
                else throw new CaeException("The selected cell range is not valid for plotting.");
            }
            catch (Exception ex)
            {
                MessageBoxes.ShowError(ex.Message);
            }
        }


        // Overrides                                                                                                                
        protected override void OnDataError(bool displayErrorDialogIfNoHandler, DataGridViewDataErrorEventArgs e)
        {
            MessageBoxes.ShowError(e.Exception.Message);
        }


        // Methods                                                                                                                  
        public void HidePlot()
        {
            frmDiagramView.Hide();
        }
        private Dictionary<int, Dictionary<int, double>> GetValues()
        {            
            Dictionary<int, Dictionary<int, double>> values = new Dictionary<int, Dictionary<int, double>>();
            Dictionary<int, double> rowValues;
            double value;
            foreach (DataGridViewCell cell in SelectedCells)
            {
                if (cell.Value == null) value = double.NaN;
                else value = (double)cell.Value;
                //
                if (values.TryGetValue(cell.RowIndex, out rowValues)) rowValues.Add(cell.ColumnIndex, value);
                else values.Add(cell.RowIndex, new Dictionary<int, double>() { { cell.ColumnIndex, value } });
            }
            return values;
        }
        private bool IsPlottingPossible(Dictionary<int, Dictionary<int, double>> values)
        {
            bool plottingPossible = true;
            // Number of rows > 1
            if (values.Count > 1)
            {
                HashSet<int> numberOfcolumns = new HashSet<int>();
                HashSet<int> columnIndices = new HashSet<int>();
                foreach (var entry in values)
                {
                    numberOfcolumns.Add(entry.Value.Count);
                    columnIndices.UnionWith(entry.Value.Keys);
                    // Number of columns must be equal in all rows; number of columns must be > 2
                    if (numberOfcolumns.Count > 1 || entry.Value.Count < 2) plottingPossible = false;
                    // X column index must be within range
                    if (XColIndex >= entry.Value.Count) plottingPossible = false;
                }
                // Allow only plotting if unbroken columns are selected
                if (columnIndices.Count != numberOfcolumns.First()) plottingPossible = false;
            }
            // Number of rows <= 1
            else plottingPossible = false;
            //
            return plottingPossible;
        }
        private void CopyToClipboard()
        {
            // Copy to clipboard
            DataObject dataObj = GetClipboardContent();
            if (dataObj != null) Clipboard.SetDataObject(dataObj);
        }
        private void PasteClipboardValue()
        {
            // Show Error if no cell is selected
            if (SelectedCells.Count == 0)
            {
                MessageBoxes.ShowWarning("Please select a cell");
                return;
            }
            // Get the starting Cell
            DataGridViewCell startCell = GetStartCell();
            // Get the clipboard value in a dictionary
            Dictionary<int, Dictionary<int, string>> cbValues = ClipboardValues(Clipboard.GetText());
            //
            string valueString;
            int iRowIndex = startCell.RowIndex;
            // Add new rows
            int numOfRows = cbValues.Keys.Count;
            int lastRow = iRowIndex + numOfRows;
            BindingSource bindingSource = (BindingSource)DataSource;
            while (RowCount < lastRow + 1) bindingSource.AddNew();
            //
            
            // CONVERTERS
            
            //TypeConverter[] converters = null;
            //if (this.DataSource is BindingSource bs && bs.DataSource is System.Collections.ICollection ic)
            //{
            //    //Get the type you are interested in.
            //    Type myListElementType = ic.GetType().GetGenericArguments().Single();

            //    //Get information about the property you are interested in on the type.
            //    var properties = myListElementType.GetProperties();
            //    //
            //    converters = new TypeConverter[properties.Length];
            //    for (int i = 0; i < properties.Length; i++)
            //    {
            //        //Pull off the TypeConverterAttribute.
            //        var attr = properties[i].GetCustomAttribute<TypeConverterAttribute>();
            //        //The attribute only stores the name of the TypeConverter as a string.
            //        var converterTypeName = attr.ConverterTypeName;

            //        // Get the actual Type of the TypeConverter from the string.
            //        var converterType = Type.GetType(converterTypeName);

            //        //Create an instance of the TypeConverter.
            //        converters[i] = (TypeConverter)Activator.CreateInstance(converterType);
            //    }
            //}


            foreach (int rowKey in cbValues.Keys)
            {
                int iColIndex = startCell.ColumnIndex;
                foreach (int cellKey in cbValues[rowKey].Keys)
                {
                    // Check if the index is within the limit
                    if (iColIndex <= Columns.Count - 1 && iRowIndex <= Rows.Count - 1)
                    {
                        DataGridViewCell cell = this[iColIndex, iRowIndex];
                        //
                        valueString = cbValues[rowKey][cellKey];
                        //
                        try
                        {
                            //if (converters != null) cell.Value = converters[iColIndex].ConvertFrom(valueString);
                            //else 
                            cell.Value = valueString;
                        }
                        catch
                        {
                            cell.Value = double.NaN;
                        }
                        //
                        cell.Selected = true;
                    }
                    iColIndex++;
                }
                iRowIndex++;
            }
        }
        private DataGridViewCell GetStartCell()
        {
            // Get the smallest row,column index
            if (SelectedCells.Count == 0) return null;
            //
            int rowIndex = Rows.Count - 1;
            int colIndex = Columns.Count - 1;
            //
            foreach (DataGridViewCell dgvCell in SelectedCells)
            {
                if (dgvCell.RowIndex < rowIndex) rowIndex = dgvCell.RowIndex;
                if (dgvCell.ColumnIndex < colIndex) colIndex = dgvCell.ColumnIndex;
            }
            //
            return this[colIndex, rowIndex];
        }
        private Dictionary<int, Dictionary<int, string>> ClipboardValues(string clipboardValue)
        {
            Dictionary<int, Dictionary<int, string>> copyValues = new Dictionary<int, Dictionary<int, string>>();
            //
            clipboardValue = clipboardValue.Replace("\r\n", "\n");
            string[] lines = clipboardValue.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            //
            for (int i = 0; i <= lines.Length - 1; i++)
            {
                copyValues[i] = new Dictionary<int, string>();
                string[] lineContent = lines[i].Split('\t');
                // If an empty cell value copied, then set the dictionary with an empty string else Set value to dictionary
                if (lineContent.Length == 0) copyValues[i][0] = string.Empty;
                else
                {
                    for (int j = 0; j <= lineContent.Length - 1; j++) copyValues[i][j] = lineContent[j];
                }
            }
            //
            return copyValues;
        }
        
    }
}
