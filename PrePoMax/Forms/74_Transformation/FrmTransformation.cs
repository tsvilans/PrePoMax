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
using CaeResults;

namespace PrePoMax.Forms
{
    public partial class FrmTransformation : UserControls.PrePoMaxChildForm, IFormBase
    {
        // Variables                                                                                                                
        //private List<Transformation> _transformations;
        private Controller _controller;
        private double[][] _coorNodesToDraw;
        private double[][] _coorLinesToDraw;


        // Properties                                                                                                               


        // Constructors                                                                                                             
        public FrmTransformation(Controller controller)
        {
            InitializeComponent();
            //
            _controller = controller;
            //
            propertyGrid.SetLabelColumnWidth(1.85);
        }


        // Event handling
        private void tvTransformations_DoubleClick(object sender, EventArgs e)
        {
            btnAdd_Click(null, null);
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (tvTransformations.SelectedNode != null && tvTransformations.SelectedNode.Tag != null)
            {
                string propertyName = tvTransformations.SelectedNode.Name;
                //
                ListViewItem item = new ListViewItem(propertyName);
                if (tvTransformations.SelectedNode.Tag is Transformation tr)
                {
                    if (tr is Symmetry sym) item.Tag = new ViewSymmetry(sym.DeepClone());
                    else if (tr is LinearPattern lp) item.Tag = new ViewLinearPattern(lp.DeepClone());
                    else if (tr is CircularPattern cp) item.Tag = new ViewCircularPattern(cp.DeepClone());
                    else throw new NotSupportedException();
                    //
                    item.Text = tr.Name;
                }
                else throw new NotSupportedException();
                //
                lvActiveTransformations.Items.Add(item);                    
                int id = lvActiveTransformations.Items.IndexOf(item);
                lvActiveTransformations.Items[id].Selected = true;
                lvActiveTransformations.Select();
                //
                _propertyItemChanged = true;
            }
        }
        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lvActiveTransformations.SelectedItems.Count == 1)
            {
                ListViewItem item = lvActiveTransformations.SelectedItems[0];
                int index = item.Index;
                if (index == lvActiveTransformations.Items.Count - 1) index--;
                lvActiveTransformations.Items.Remove(item);
                //
                if (lvActiveTransformations.Items.Count > 0) lvActiveTransformations.Items[index].Selected = true;
                else propertyGrid.SelectedObject = null;
            }
        }
        private void lvActiveTransformations_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvActiveTransformations.SelectedItems.Count == 1)
            {
                if (lvActiveTransformations.SelectedItems[0].Tag is ViewTransformation)
                {
                    propertyGrid.SelectedObject = lvActiveTransformations.SelectedItems[0].Tag;
                }
                else throw new NotSupportedException();
                //
                HighlightNodes();
            }
            lvActiveTransformations.Select();
        }
        private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (lvActiveTransformations.SelectedItems.Count == 1 && propertyGrid.SelectedObject is ViewTransformation vt)
            {
                lvActiveTransformations.SelectedItems[0].Text = vt.Name;
            }
            propertyGrid.Refresh();
            //
            HighlightNodes();
            //
            _propertyItemChanged = true;
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                ApplyTransformation();
                //
                Hide();
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }            
        }
        private void bntApply_Click(object sender, EventArgs e)
        {
            try
            {
                _propertyItemChanged = true;
                //
                ApplyTransformation();
                //
                HighlightNodes();
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                _propertyItemChanged = false;   // disable transformation on OK
                //
                _controller.RemoveCurrentTransformations(true);
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
        private void FrmTrasformations_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                //
                Hide();
            }
        }
    

        // Methods                                                                                                                  
        public bool PrepareForm(string stepName, string materialToEditName)
        {
            _propertyItemChanged = false;            
            lvActiveTransformations.Items.Clear();
            propertyGrid.SelectedObject = null;
            //
            List<Transformation> transformations;
            if (_controller.GetTransformations() != null) transformations = _controller.GetTransformations().DeepClone();
            else transformations = new List<Transformation>();
            // Initialize transformations
            tvTransformations.Nodes.Find("X", true)[0].Tag = new Symmetry("Symmetry-X", new double[3], SymmetryPlaneEnum.X);
            tvTransformations.Nodes.Find("Y", true)[0].Tag = new Symmetry("Symmetry-Y", new double[3], SymmetryPlaneEnum.Y);
            tvTransformations.Nodes.Find("Z", true)[0].Tag = new Symmetry("Symmetry-Z", new double[3], SymmetryPlaneEnum.Z);
            tvTransformations.Nodes.Find("Linear", true)[0].Tag = new LinearPattern("Linear", new double[3],
                                                                                    new double[] { 1, 0, 0 }, 2);
            tvTransformations.Nodes.Find("Circular", true)[0].Tag = new CircularPattern("Circular", new double[3],
                                                                                        new double[] { 0, 0, 1 }, 90, 2);
            tvTransformations.ExpandAll();
            //
            if (transformations.Count > 0)
            {
                ListViewItem item;
                ViewTransformation view;
                //
                foreach (var transformation in transformations)
                {
                    if (transformation is Symmetry sym) view = new ViewSymmetry(sym);
                    else if (transformation is LinearPattern lp) view = new ViewLinearPattern(lp);
                    else if (transformation is CircularPattern cp) view = new ViewCircularPattern(cp);
                    else throw new NotSupportedException();
                    //
                    item = new ListViewItem(view.Name);
                    item.Tag = view;
                    lvActiveTransformations.Items.Add(item);
                }
                //
                lvActiveTransformations.Items[0].Selected = true;
                lvActiveTransformations.Select();
            }
            //
            _controller.SetSelectByToOff();
            //
            _controller.ClearSelectionHistory();
            //
            return true;
        }
        public void ApplyTransformation()
        {
            if (_propertyItemChanged)
            {
                List<Transformation> transformations = null;
                if (lvActiveTransformations.Items.Count > 0)
                {
                    transformations = new List<Transformation>();
                    ViewTransformation viewTransformation;
                    Transformation transformation;
                    foreach (ListViewItem item in lvActiveTransformations.Items)
                    {
                        viewTransformation = (ViewTransformation)item.Tag;
                        transformation = viewTransformation.Base;
                        //
                        if (transformation is Symmetry sym)
                        { }
                        else if (transformation is LinearPattern lp)
                        {
                            if (lp.DisplacementLength == 0)
                                throw new CaeException("The pattern displacement must be larger than 0.");
                        }
                        else if (transformation is CircularPattern cp)
                        {
                            if (cp.Angle == 0)
                                throw new CaeException("The pattern angle must be different from 0.");
                            if (cp.AxisNormalLength == 0)
                                throw new CaeException("The pattern axis points coincide.");
                        }
                        else throw new NotSupportedException();
                        //
                        transformations.Add(transformation);
                    }
                }
                //
                _controller.SetTransformations(transformations);
            }
        }
        //
        public void PickedIds(int[] ids)
        {
            this.Enabled = true;
            //
            _controller.SetSelectByToOff();
            _controller.ClearSelectionHistory();
            //
            if (ids != null && ids.Length == 1)
            {
                _propertyItemChanged = true;
                //
                float scale = _controller.GetScale();
                Vec3D deformed = new Vec3D(_controller.GetScaledNode(scale, ids[0]).Coor);
                //
                if (propertyGrid.SelectedObject is ViewSymmetry vs)
                {
                    vs.SymmetryPointX = deformed.X;
                    vs.SymmetryPointY = deformed.Y;
                    vs.SymmetryPointZ = deformed.Z;
                }
                else if (propertyGrid.SelectedObject is ViewLinearPattern vlp)
                {
                    string propertyName = propertyGrid.SelectedGridItem.PropertyDescriptor.Name;
                    //
                    if (propertyName == nameof(vlp.StartPointItemSet))
                    {
                        vlp.StartPointX = deformed.X;
                        vlp.StartPointY = deformed.Y;
                        vlp.StartPointZ = deformed.Z;
                    }
                    else if (propertyName == nameof(vlp.EndPointItemSet))
                    {
                        vlp.EndPointX = deformed.X;
                        vlp.EndPointY = deformed.Y;
                        vlp.EndPointZ = deformed.Z;
                    }
                }
                else if (propertyGrid.SelectedObject is ViewCircularPattern vcp)
                {
                    string propertyName = propertyGrid.SelectedGridItem.PropertyDescriptor.Name;
                    //
                    if (propertyName == nameof(vcp.FirstPointItemSet))
                    {
                        vcp.FirstPointX = deformed.X;
                        vcp.FirstPointY = deformed.Y;
                        vcp.FirstPointZ = deformed.Z;
                    }
                    else if (propertyName == nameof(vcp.SecondPointItemSet))
                    {
                        vcp.SecondPointX = deformed.X;
                        vcp.SecondPointY = deformed.Y;
                        vcp.SecondPointZ = deformed.Z;
                    }
                }
                else throw new NotSupportedException();
                //
                propertyGrid.Refresh();
            }
            //
            HighlightNodes();
        }
        private void HighlightNodes()
        {
            try
            {
                _controller.ClearAllSelection();
                //
                _coorNodesToDraw = new double[1][];
                _coorNodesToDraw[0] = new double[3];
                //
                _coorLinesToDraw = new double[2][];
                _coorLinesToDraw[0] = new double[3];
                //
                if (propertyGrid.SelectedObject == null) { }
                else if (propertyGrid.SelectedObject is ViewSymmetry vs)
                {
                    _coorNodesToDraw[0][0] = vs.SymmetryPointX;
                    _coorNodesToDraw[0][1] = vs.SymmetryPointY;
                    _coorNodesToDraw[0][2] = vs.SymmetryPointZ;
                }
                else if (propertyGrid.SelectedObject is ViewLinearPattern vlp)
                {
                    _coorNodesToDraw[0][0] = vlp.EndPointX;
                    _coorNodesToDraw[0][1] = vlp.EndPointY;
                    _coorNodesToDraw[0][2] = vlp.EndPointZ;
                    //
                    _coorLinesToDraw[0][0] = vlp.StartPointX;
                    _coorLinesToDraw[0][1] = vlp.StartPointY;
                    _coorLinesToDraw[0][2] = vlp.StartPointZ;
                    _coorLinesToDraw[1] = _coorNodesToDraw[0];
                    //
                    _controller.HighlightConnectedLines(_coorLinesToDraw);
                }
                else if (propertyGrid.SelectedObject is ViewCircularPattern vcp)
                {
                    _coorNodesToDraw[0][0] = vcp.SecondPointX;
                    _coorNodesToDraw[0][1] = vcp.SecondPointY;
                    _coorNodesToDraw[0][2] = vcp.SecondPointZ;
                    //
                    _coorLinesToDraw[0][0] = vcp.FirstPointX;
                    _coorLinesToDraw[0][1] = vcp.FirstPointY;
                    _coorLinesToDraw[0][2] = vcp.FirstPointZ;
                    _coorLinesToDraw[1] = _coorNodesToDraw[0];
                    //
                    _controller.HighlightConnectedLines(_coorLinesToDraw);
                }
                else throw new NotSupportedException();
                //
                _controller.HighlightNodes(_coorNodesToDraw);
            }
            catch { }
        }
    }
}
