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
using CaeGlobals;


namespace PrePoMax.Forms
{    
    public partial class FrmExplodedView : UserControls.PrePoMaxChildForm, IFormBase
    {
        // Variables                                                                                                                
        private Controller _controller;
        private ViewExplodedViewParameters _viewExplodedViewParameters;
        private ExplodedViewParameters _cancelParam;
        private ExplodedViewParameters _defaultParam;
        private string _drawSymbolsForStep;
        private Octree.Plane _sectionViewPlane;
        private bool _continueExplodedView;
        private Dictionary<string, double[]> _partOffsets;


        // Properties                                                                                                               
        public void SetExplodedViewParameters(ExplodedViewParameters parameters)
        {
            _cancelParam = parameters.DeepClone();
            //
            if (parameters.ScaleFactor == -1) parameters.ScaleFactor = 0.5;   // -1 stands for disabled scale factor
            _viewExplodedViewParameters.Parameters = parameters.DeepClone();
            //
            UpdateScrollbarPosition(false);
        }


        // Callbacks
        public Action Clear3D;


        // Constructors                                                                                                             
        public FrmExplodedView(Controller controller)
        {
            InitializeComponent();
            //
            _viewExplodedViewParameters = new ViewExplodedViewParameters();
            propertyGrid.SelectedObject = _viewExplodedViewParameters;
            //
            _controller = controller;
            //
            propertyGrid.SetLabelColumnWidth(1.75);
            //
            _defaultParam = new ExplodedViewParameters();
            _defaultParam.ScaleFactor = 0;
        }


        // Event handlers                                                                                                           
        private void FrmExplodedView_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                btnCancel_Click(null, null);
            }
        }
        private void FrmExplodedView_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                // Suppress section view
                _sectionViewPlane = _controller.GetSectionViewPlane();
                if (_sectionViewPlane != null) _controller.RemoveSectionView();
                // Suppress symbols
                _drawSymbolsForStep = _controller.GetDrawSymbolsForStep();
                _controller.DrawSymbolsForStep("None", false);
                // Suppress annotations
                _controller.Annotations.SuppressCurrentAnnotations();
                // Set exploded view
                if (_cancelParam.ScaleFactor == -1)
                {
                    _continueExplodedView = false;
                }
                else
                {
                    // Animation of exploded view is not needed
                    _continueExplodedView = true;
                    // This redraws the scene and redraws selection
                    _partOffsets = _controller.RemoveExplodedView(true);
                    _controller.PreviewExplodedView(_viewExplodedViewParameters.Parameters, false, _partOffsets);
                }
                // Update properties
                _viewExplodedViewParameters.UpdateVisibility();
                // Animate
                UpdateScrollbarPosition(true);
            }
            else
            {
                if (DialogResult == DialogResult.OK) _controller.ApplyExplodedView(_viewExplodedViewParameters.Parameters);
                else if (DialogResult == DialogResult.Abort) Cancel(true);
                else if (DialogResult == DialogResult.Cancel) Cancel(_cancelParam.ScaleFactor == -1);
                // the form was closed from frmMain.CloseAllForms
                else if (DialogResult == DialogResult.None) Cancel(_cancelParam.ScaleFactor == -1);
                // Resume symbols
                _controller.DrawSymbolsForStep(_drawSymbolsForStep, false);
                // Resume annotations
                _controller.Annotations.ResumeCurrentAnnotations();
                // Resume section view
                if (_sectionViewPlane != null) _controller.ApplySectionView(_sectionViewPlane.Point.Coor,
                                                                            _sectionViewPlane.Normal.Coor);
            }
        }
        private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            UpdateScrollbarPosition(true);
        }
        //
        private void hsbPosition_Scroll(object sender, ScrollEventArgs e)
        {
            UpdateExplodedViewFromScrollBar(false);
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            Hide();
        }
        private void btnDisable_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Abort;
            Hide();
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Hide();
        }
        

        // Methods                                                                                                                  

        // IFormBase
        public bool PrepareForm(string stepName, string itemToEditName)
        {
            this.DialogResult = DialogResult.None;
            //
            propertyGrid.Refresh();
            //
            _controller.ClearSelectionHistory();
            _controller.SetSelectByToOff();
            //
            return true;
        }
        //
        private void Cancel(bool cancelToDefault)
        {
            if (cancelToDefault)
            {
                //System.Diagnostics.Debug.WriteLine("PreviewExplodedView");
                _controller.PreviewExplodedView(_defaultParam, true);
                //System.Diagnostics.Debug.WriteLine("RemoveExplodedView");
                _controller.RemoveExplodedView(true);
            }
            else
            {
                _controller.PreviewExplodedView(_cancelParam, true);
                _controller.ApplyExplodedView(_cancelParam);
            }
        }
        private void UpdateScrollbarPosition(bool animate)
        {
            hsbPosition.Value = (int)(_viewExplodedViewParameters.ScaleFactor * 1000);
            //
            UpdateExplodedViewFromScrollBar(animate);
            //
            HighlightCenterPoint();
        }
        private void UpdateExplodedViewFromScrollBar(bool animate)
        {
            try
            {
                double scaleFactor = (double)hsbPosition.Value / hsbPosition.Maximum;
                _viewExplodedViewParameters.ScaleFactor = scaleFactor;
                propertyGrid.Refresh();
                //
                if (this.Visible)
                {
                    if (_continueExplodedView)
                    {
                        animate = false;    // animation of exploded view is not needed
                        _continueExplodedView = false;  // next time animate
                    }
                    //
                    _controller.PreviewExplodedView(_viewExplodedViewParameters.Parameters, animate);
                }
            }
            catch
            { }
        }
        private void HighlightCenterPoint()
        {
            _controller.ClearSelectionHistory();
            //
            if (_viewExplodedViewParameters.Method == ExplodedViewMethodEnum.Default) { }
            else if (_viewExplodedViewParameters.Method == ExplodedViewMethodEnum.CenterPoint)
            {
                double[][] _coorNodesToDraw = new double[1][];
                _coorNodesToDraw[0] = new double[3];
                _coorNodesToDraw[0][0] = _viewExplodedViewParameters.CenterX;
                _coorNodesToDraw[0][1] = _viewExplodedViewParameters.CenterY;
                _coorNodesToDraw[0][2] = _viewExplodedViewParameters.CenterZ;
                //
                _controller.HighlightNodes(_coorNodesToDraw);
            }
        }
    }
}
