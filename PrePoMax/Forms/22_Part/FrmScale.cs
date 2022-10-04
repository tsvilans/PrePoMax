using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;
using System.Windows.Forms;
using System.Drawing;

namespace PrePoMax.Forms
{
    class FrmScale : UserControls.FrmProperties, IFormBase
    {
        // Variables                                                                                                                
        private ScaleParameters _scaleParameters;
        private Controller _controller;
        private string[] _partNames;
        private ContextMenuStrip cmsPropertyGrid;
        private System.ComponentModel.IContainer components;
        private ToolStripMenuItem tsmiResetAll;
        private double[][] _coorNodesToDraw;
        
        
        // Callbacks                                                                                                                
        public Func<string[], double[], double[], bool, Task> ScaleGeometryPartsAsync;


        // Properties                                                                                                               
        public string[] PartNames { get { return _partNames; } set { _partNames = value; } }
        public double[] ScaleCenter
        {
            get
            {
                return new double[] { _scaleParameters.CenterX, _scaleParameters.CenterY, _scaleParameters.CenterZ };
            }
        }
        public double[] ScaleFactors
        {
            get
            {
                return new double[] { _scaleParameters.FactorX, _scaleParameters.FactorY, _scaleParameters.FactorZ };
            }
        }


        // Constructors                                                                                                             
        public FrmScale(Controller controller) 
            : base(1.7)
        {
            InitializeComponent();
            //
            _controller = controller;
            //
            _coorNodesToDraw = new double[1][];
            _coorNodesToDraw[0] = new double[3];
            //
            btnOK.Visible = false;
            btnOkAddNew.Width = btnOK.Width;
            btnOkAddNew.Left = btnOK.Left;
            btnOkAddNew.Text = "Apply";
            btnCancel.Text = "Close";
        }
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.cmsPropertyGrid = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiResetAll = new System.Windows.Forms.ToolStripMenuItem();
            this.gbProperties.SuspendLayout();
            this.cmsPropertyGrid.SuspendLayout();
            this.SuspendLayout();
            // 
            // propertyGrid
            // 
            this.propertyGrid.ContextMenuStrip = this.cmsPropertyGrid;
            // 
            // cmsPropertyGrid
            // 
            this.cmsPropertyGrid.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiResetAll});
            this.cmsPropertyGrid.Name = "cmsPropertyGrid";
            this.cmsPropertyGrid.Size = new System.Drawing.Size(181, 48);
            // 
            // tsmiResetAll
            // 
            this.tsmiResetAll.Name = "tsmiResetAll";
            this.tsmiResetAll.Size = new System.Drawing.Size(180, 22);
            this.tsmiResetAll.Text = "Reset all";
            this.tsmiResetAll.Click += new System.EventHandler(this.tsmiResetAll_Click);
            // 
            // FrmScale
            // 
            this.ClientSize = new System.Drawing.Size(334, 411);
            this.Name = "FrmScale";
            this.Text = "Scale Parameters";
            this.Controls.SetChildIndex(this.gbProperties, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.btnOK, 0);
            this.Controls.SetChildIndex(this.btnOkAddNew, 0);
            this.gbProperties.ResumeLayout(false);
            this.cmsPropertyGrid.ResumeLayout(false);
            this.ResumeLayout(false);

        }


        // Event handlers                                                                                                           
        protected override void OnPropertyGridPropertyValueChanged()
        {
            HighlightNodes();
            //
            base.OnPropertyGridPropertyValueChanged();
        }
        protected override void OnApply(bool onOkAddNew)
        {
            if (_controller.CurrentView == ViewGeometryModelResults.Geometry)
                ScaleGeometryParts();
            else if (_controller.CurrentView == ViewGeometryModelResults.Model)
                _controller.ScaleModelPartsCommand(_partNames, ScaleCenter, ScaleFactors, _scaleParameters.Copy);
            else throw new NotSupportedException();
            //
            HighlightNodes();
        }
        async private void ScaleGeometryParts()
        {
            try
            {
                Enabled = false;
                await ScaleGeometryPartsAsync?.Invoke(_partNames, ScaleCenter, ScaleFactors, _scaleParameters.Copy);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
            finally
            {
                Enabled = true;
            }
            
        }
        protected override bool OnPrepareForm(string stepName, string itemToEditName)
        {
            // Clear
            tsmiResetAll_Click(null, null);
            _controller.ClearSelectionHistoryAndCallSelectionChanged();
            _scaleParameters.Clear();
            // Disable selection
            _controller.SetSelectByToOff();
            // Get center point grid item
            GridItem gi = propertyGrid.EnumerateAllItems().First((item) =>
                              item.PropertyDescriptor != null &&
                              item.PropertyDescriptor.Name == "ScaleCenterItemSet");
            // Select it
            gi.Select();
            //
            propertyGrid.Refresh();
            //
            HighlightNodes();
            //
            return true;
        }
        private void tsmiResetAll_Click(object sender, EventArgs e)
        {
            _scaleParameters = new ScaleParameters(_controller.Model.Properties.ModelSpace);
            propertyGrid.SelectedObject = _scaleParameters;
            _controller.ClearAllSelection();
        }


        // Methods                                                                                                                  
        public void PickedIds(int[] ids)
        {
            this.Enabled = true;
            // Disable selection
            _controller.SetSelectByToOff();
            _controller.Selection.SelectItem = vtkSelectItem.None;
            _controller.ClearSelectionHistoryAndCallSelectionChanged();
            //
            if (ids != null && ids.Length == 1)
            {
                FeNode node = _controller.DisplayedMesh.Nodes[ids[0]];
                //
                _scaleParameters.CenterX = node.X;
                _scaleParameters.CenterY = node.Y;
                _scaleParameters.CenterZ = node.Z;
                //
                propertyGrid.Refresh();
                //
                HighlightNodes();
            }
        }
        private void HighlightNodes()
        {
            _coorNodesToDraw[0][0] = _scaleParameters.CenterX;
            _coorNodesToDraw[0][1] = _scaleParameters.CenterY;
            _coorNodesToDraw[0][2] = _scaleParameters.CenterZ;
            //
            _controller.HighlightNodes(_coorNodesToDraw);
        }

        
    }
}
