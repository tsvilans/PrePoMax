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
    public partial class FrmSectionView : UserControls.PrePoMaxChildForm, IFormBase
    {
        // Variables                                                                                                                
        private Controller _controller;
        private SectionViewParameters _sectionViewParameters;
        private bool _pause;
        private Vec3D _projHalfSize;
        private Octree.Plane _plane;

        // Properties                                                                                                               


        // Constructors                                                                                                             
        public FrmSectionView(Controller controller)
        {
            InitializeComponent();
            //
            _controller = controller;
            _sectionViewParameters = new SectionViewParameters();
            propertyGrid.SelectedObject = _sectionViewParameters;
            _pause = false;
            //
            propertyGrid.SetLabelColumnWidth(1.75);
        }


        // Event handlers                                                                                                           
        private void FrmSectionView_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                btnCancel_Click(null, null);
            }
        }
        private void FrmSectionView_VisibleChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.Visible)
                {
                    _pause = true;
                    _plane = _controller.GetSectionViewPlane();
                    if (_plane == null)
                    {
                        double[] vpn = _controller.GetViewPlaneNormal();
                        double max = 0;
                        int id = -1;
                        for (int i = 0; i < 3; i++)
                        {
                            if (Math.Abs(vpn[i]) > max)
                            {
                                max = Math.Abs(vpn[i]);
                                id = i;
                            }
                        }
                        for (int i = 0; i < 3; i++)
                        {
                            if (i == id) vpn[i] = -Math.Round(vpn[i], MidpointRounding.AwayFromZero);
                            else vpn[i] = 0;
                        }
                        //
                        _sectionViewParameters.Point = GetBBCenter().Coor;
                        _sectionViewParameters.Normal = vpn;
                        //
                        PointOrNormalChanged();
                    }
                    else
                    {
                        _sectionViewParameters.Point = _plane.Point.Coor.ToArray();      // keep plane data for Cancel
                        _sectionViewParameters.Normal = _plane.Normal.Coor.ToArray();    // keep plane data for Cancel
                        SetScrollBarPositionFromPoint();
                    }
                    //
                    _controller.ApplySectionView(_sectionViewParameters.Point, _sectionViewParameters.Normal);
                }
                else
                {
                    if (DialogResult == DialogResult.Abort) _controller.RemoveSectionView();
                    else if (DialogResult == DialogResult.Cancel || DialogResult == DialogResult.None)
                    {
                        if (_plane == null)
                        {
                            _controller.RemoveSectionView();
                        }
                        else
                        {
                            _sectionViewParameters.Point = _plane.Point.Coor;
                            _sectionViewParameters.Normal = _plane.Normal.Coor;
                            UpdateSectionView();
                        }
                    }
                }
            }
            catch
            { }
            finally
            {
                _pause = false;
            }
        }

        private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            PointOrNormalChanged();
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _sectionViewParameters.Clear();
            _sectionViewParameters.Point = GetBBCenter().Coor;
            //
            PointOrNormalChanged();
        }
        private void xDirectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _sectionViewParameters.Normal = new double[] { 1, 0, 0 };
            //
            PointOrNormalChanged();
        }
        private void yDirectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _sectionViewParameters.Normal = new double[] { 0, 1, 0 };
            //
            PointOrNormalChanged();
        }
        private void zDirectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _sectionViewParameters.Normal = new double[] { 0, 0, 1 };
            //
            PointOrNormalChanged();
        }
        private void reverseDirectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _sectionViewParameters.Normal[0] = -_sectionViewParameters.Normal[0];
            _sectionViewParameters.Normal[1] = -_sectionViewParameters.Normal[1];
            _sectionViewParameters.Normal[2] = -_sectionViewParameters.Normal[2];
            //
            PointOrNormalChanged();
        }

        private void hsbPosition_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.Type == ScrollEventType.EndScroll) return;
            //
            UpdateProjHalfSize();
            //
            double ratio = (double)(hsbPosition.Value - hsbPosition.Minimum) / (hsbPosition.Maximum - hsbPosition.Minimum);
            ratio = 2 * ratio - 1;
            Vec3D point = GetBBCenter() + _projHalfSize * ratio;
            point.X = CaeGlobals.Tools.RoundToSignificantDigits(point.X, 6);
            point.Y = CaeGlobals.Tools.RoundToSignificantDigits(point.Y, 6);
            point.Z = CaeGlobals.Tools.RoundToSignificantDigits(point.Z, 6);
            _sectionViewParameters.Point = point.Coor;
            //
            timerUpdate.Start();    // use timer to speed things up
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

        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            timerUpdate.Stop();
            UpdateSectionView();
        }

        // Methods                                                                                                                  

        // IFormBase
        public bool PrepareForm(string stepName, string partToEditName)
        {
            this.DialogResult = DialogResult.None;
            // Clear selection
            _controller.ClearSelectionHistoryAndCallSelectionChanged();
            // Disable selection
            _controller.SetSelectByToOff();
            //
            _sectionViewParameters.Clear();
            // Get start point grid item
            GridItem gi = propertyGrid.EnumerateAllItems().First((item) =>
                          item.PropertyDescriptor != null &&
                          item.PropertyDescriptor.Name == nameof(_sectionViewParameters.PointItemSet));
            // Select it
            gi.Select();
            //
            propertyGrid.Refresh();
            //
            return true;
        }
        
        public void PickedIds(int[] ids)
        {
            bool selectionFinished = false;
            if (ids != null)
            {
                string propertyName = propertyGrid.SelectedGridItem.PropertyDescriptor.Name;
                if (propertyName == nameof(_sectionViewParameters.PointItemSet))
                {
                    if (ids.Length == 0)
                    {
                        selectionFinished = true;
                    }
                    else if (ids.Length == 1)
                    {
                        FeNode node = _controller.DisplayedMesh.Nodes[ids[0]];
                        //
                        _sectionViewParameters.X = CaeGlobals.Tools.RoundToSignificantDigits(node.X, 6);
                        _sectionViewParameters.Y = CaeGlobals.Tools.RoundToSignificantDigits(node.Y, 6);
                        _sectionViewParameters.Z = CaeGlobals.Tools.RoundToSignificantDigits(node.Z, 6);
                        //
                        selectionFinished = true;
                    }
                }
                else if (propertyName == nameof(_sectionViewParameters.NormalItemSet)) 
                {
                    if (ids.Length == 2)
                    {
                        FeNode start = _controller.DisplayedMesh.Nodes[ids[0]];
                        FeNode end = _controller.DisplayedMesh.Nodes[ids[1]];
                        //
                        _sectionViewParameters.Nx = CaeGlobals.Tools.RoundToSignificantDigits(end.X - start.X, 6);
                        _sectionViewParameters.Ny = CaeGlobals.Tools.RoundToSignificantDigits(end.Y - start.Y, 6);
                        _sectionViewParameters.Nz = CaeGlobals.Tools.RoundToSignificantDigits(end.Z - start.Z, 6);
                        //
                        selectionFinished = true;
                    }
                }
            }
            //
            if (selectionFinished)
            {
                this.Enabled = true;
                _controller.ClearSelectionHistoryAndCallSelectionChanged();
                _controller.SetSelectByToOff();
                _controller.Selection.SelectItem = vtkSelectItem.None;
                //
                PointOrNormalChanged();
                //
                UpdateSectionView();
            }
        }

        private void PointOrNormalChanged()
        {
            SetScrollBarPositionFromPoint();
            timerUpdate.Start();
        }
        private void UpdateProjHalfSize()
        {
            double[] box = _controller.GetBoundingBox();
            //
            Vec3D v = new Vec3D(); // half box diagonal
            v.X = (box[1] - box[0]) / 2;
            v.Y = (box[3] - box[2]) / 2;
            v.Z = (box[5] - box[4]) / 2;
            //
            Vec3D n = new Vec3D(_sectionViewParameters.Normal);
            n.Abs();
            n.Normalize();
            // Project 1/2 diagonal on the positive normal
            double l = Vec3D.DotProduct(v, n) * 1.05;
            if (l == 0) l = 1;  // 2D
            //
            _projHalfSize = new Vec3D(_sectionViewParameters.Normal);
            _projHalfSize.Normalize();
            _projHalfSize = l * _projHalfSize;
        }
        private Vec3D GetBBCenter()
        {
            double[] box = _controller.GetBoundingBox();
            Vec3D center = new Vec3D();
            center.X = CaeGlobals.Tools.RoundToSignificantDigits((box[0] + box[1]) / 2, 6);
            center.Y = CaeGlobals.Tools.RoundToSignificantDigits((box[2] + box[3]) / 2, 6);
            center.Z = CaeGlobals.Tools.RoundToSignificantDigits((box[4] + box[5]) / 2, 6);
            return center;
        }
        private void SetScrollBarPositionFromPoint()
        {
            try
            {
                UpdateProjHalfSize();
                //
                Vec3D c = GetBBCenter();
                Vec3D n = new Vec3D(_sectionViewParameters.Normal);
                n.Normalize();
                //
                Vec3D p = new Vec3D(_sectionViewParameters.Point);
                Vec3D v = p - c;
                //
                double l = Vec3D.DotProduct(v, n);
                double ratio = l / _projHalfSize.Len;
                //
                if (ratio > 1) ratio = 1;
                else if (ratio < -1) ratio = -1;
                //
                ratio = (ratio + 1) / 2;        // to iterval from 0 to 1;
                //
                hsbPosition.Value = (int)Math.Round((hsbPosition.Maximum - hsbPosition.Minimum) * ratio, 0);
            }
            catch
            { }
        }
        private void UpdateSectionView()
        {
            try
            {
                if (_pause) return;
                //
                propertyGrid.Refresh();     // must be here to update values
                //
                System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
                watch.Start();
                //
                UpdateProjHalfSize();
                //
                _sectionViewParameters = (SectionViewParameters)propertyGrid.SelectedObject;
                double[] point = _sectionViewParameters.Point;
                double[] normal = _sectionViewParameters.Normal;
                //
                _controller.UpdateSectionView(point, normal);
                //
                //System.Diagnostics.Debug.WriteLine("Section cut time: " + DateTime.Now.ToLongTimeString() + "   Duration: " + watch.ElapsedMilliseconds);
            }
            catch
            { }
        }
       
    }
}
