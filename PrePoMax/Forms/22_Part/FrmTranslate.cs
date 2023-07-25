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
    class FrmTranslate : UserControls.FrmProperties, IFormBase
    {
        // Variables                                                                                                                
        private TranslateParameters _translateParameters;
        private Controller _controller;
        private string[] _partNames;
        private double[][] _coorNodesToDraw;
        private ContextMenuStrip cmsPropertyGrid;
        private System.ComponentModel.IContainer components;
        private ToolStripMenuItem tsmiResetAll;
        private double[][] _coorLinesToDraw;


        // Properties                                                                                                               
        public string[] PartNames { get { return _partNames; } set { _partNames = value; } }
        public double[] TranslateVector
        {
            get
            {
                return new double[] { _translateParameters.X2 - _translateParameters.X1,
                                      _translateParameters.Y2 - _translateParameters.Y1,
                                      _translateParameters.Z2 - _translateParameters.Z1};
            }
        }


        // Constructors                                                                                                             
        public FrmTranslate(Controller controller) 
            : base(1.7)
        {
            InitializeComponent();
            //
            _controller = controller;            
            //
            _coorNodesToDraw = new double[1][];
            _coorNodesToDraw[0] = new double[3];
            //
            _coorLinesToDraw = new double[2][];
            _coorLinesToDraw[0] = new double[3];
            _coorLinesToDraw[1] = new double[3];
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
            this.cmsPropertyGrid.Size = new System.Drawing.Size(118, 26);
            // 
            // tsmiResetAll
            // 
            this.tsmiResetAll.Name = "tsmiResetAll";
            this.tsmiResetAll.Size = new System.Drawing.Size(117, 22);
            this.tsmiResetAll.Text = "Reset all";
            this.tsmiResetAll.Click += new System.EventHandler(this.tsmiResetAll_Click);
            // 
            // FrmTranslate
            // 
            this.ClientSize = new System.Drawing.Size(334, 411);
            this.Name = "FrmTranslate";
            this.Text = "Translate Parameters";
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
            _translateParameters = (TranslateParameters)propertyGrid.SelectedObject;
            //
            double[] translateVector = TranslateVector;
            _controller.TranslateModelPartsCommand(_partNames, translateVector, _translateParameters.Copy);
            //
            HighlightNodes();
        }
        protected override bool OnPrepareForm(string stepName, string itemToEditName)
        {
            // Clear
            tsmiResetAll_Click(null, null);
            _controller.ClearSelectionHistoryAndCallSelectionChanged();
            _translateParameters.Clear();
            // Disable selection
            _controller.SetSelectByToOff();
            // Get start point grid item
            GridItem gi = propertyGrid.EnumerateAllItems().First((item) =>
                              item.PropertyDescriptor != null &&
                              item.PropertyDescriptor.Name == nameof(_translateParameters.StartPointItemSet));
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
            _translateParameters = new TranslateParameters(_controller.Model.Properties.ModelSpace);
            propertyGrid.SelectedObject = _translateParameters;
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
                FeNode node = _controller.Model.Mesh.Nodes[ids[0]];
                string propertyName = propertyGrid.SelectedGridItem.PropertyDescriptor.Name;
                if (propertyName == nameof(_translateParameters.StartPointItemSet))
                {
                    _translateParameters.X1 = node.X;
                    _translateParameters.Y1 = node.Y;
                    _translateParameters.Z1 = node.Z;
                }
                else if(propertyName == nameof(_translateParameters.EndPointItemSet))
                {
                    _translateParameters.X2 = node.X;
                    _translateParameters.Y2 = node.Y;
                    _translateParameters.Z2 = node.Z;
                }

                propertyGrid.Refresh();

                HighlightNodes();
            }
        }
        private void HighlightNodes()
        {
            _controller.ClearAllSelection();
            //
            _coorNodesToDraw[0][0] = _translateParameters.X2;
            _coorNodesToDraw[0][1] = _translateParameters.Y2;
            _coorNodesToDraw[0][2] = _translateParameters.Z2;
            //
            _coorLinesToDraw[0][0] = _translateParameters.X1;
            _coorLinesToDraw[0][1] = _translateParameters.Y1;
            _coorLinesToDraw[0][2] = _translateParameters.Z1;
            _coorLinesToDraw[1] = _coorNodesToDraw[0];
            //
            _controller.HighlightNodes(_coorNodesToDraw);
            _controller.HighlightConnectedLines(_coorLinesToDraw);
        }
    }
}
