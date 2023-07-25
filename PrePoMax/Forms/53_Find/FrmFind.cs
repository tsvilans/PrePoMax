using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CaeGlobals;
using CaeMesh;

namespace PrePoMax.Forms
{
    public partial class FrmFind : UserControls.PrePoMaxChildForm
    {
        // Variables                                                                                                                
        private Controller _controller;


        // Callbacks                                                                                                               
        public Action<object, EventArgs> Form_RemoveAnnotations;


        // Constructors                                                                                                             
        public FrmFind()
        {
            InitializeComponent();
        }


        // Event hadlers                                                                                                            
        private void rbItem_CheckedChanged(object sender, EventArgs e)
        {
        }
        private void tbItemId_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                btnFind_Click(null, null);
            }
        }
        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                // Clear
                _controller.ClearSelectionHistoryAndCallSelectionChanged();
                //
                int id = int.Parse(tbItemId.Text);
                //
                if (rbVertexNode.Checked)
                {
                    _controller.HighlightNode(id);
                    //
                    if (cbAddAnnotation.Checked) _controller.Annotations.AddNodeAnnotation(id);
                }
                else if (rbFacetElement.Checked)
                {
                    _controller.HighlightElement(id);
                    //
                    if (cbAddAnnotation.Checked) _controller.Annotations.AddElementAnnotation(id);
                }
                else if (rbEdge.Checked)
                {
                    // GeometryId = itemId * 100000 + typeId * 10000 + partId
                    id--;   // the numbering of edges starts with 0
                    int typeId = (int)GeometryType.Edge;
                    int[] partIds = _controller.DisplayedMesh.GetVisiblePartIds();
                    int edgeId;
                    for (int i = 0; i < partIds.Length; i++)
                    {
                        edgeId = id * 100000 + typeId * 10000 + partIds[i];
                        try
                        {
                            _controller.HighlightItemsByGeometryEdgeIds(new int[] { edgeId }, false);
                            if (cbAddAnnotation.Checked) _controller.Annotations.AddEdgeAnnotation(edgeId);
                        }
                        catch { }
                    }
                }
                else if (rbSurface.Checked)
                {
                    // GeometryId = itemId * 100000 + typeId * 10000 + partId
                    id--;   // the numbering of surfaces starts with 0
                    int typeId = (int)GeometryType.SolidSurface;
                    int[] partIds = _controller.DisplayedMesh.GetVisiblePartIds();
                    int surfaceId;
                    for (int i = 0; i < partIds.Length; i++)
                    {
                        surfaceId = id * 100000 + typeId * 10000 + partIds[i];
                        try
                        {
                            _controller.HighlightItemsBySurfaceIds(new int[] { surfaceId }, false);
                            if (cbAddAnnotation.Checked) _controller.Annotations.AddSurfaceAnnotation(surfaceId);
                        }
                        catch { }
                    }
                }
                else if (rbPart.Checked)
                {
                    BasePart part = _controller.DisplayedMesh.GetPartById(id);
                    if (part != null)
                    {
                        _controller.Highlight3DObjects(new object[] { part });
                        if (part.Visible && cbAddAnnotation.Checked) _controller.Annotations.AddPartAnnotation(part.Name);
                    }
                }
            }
            catch { }
            finally { PrepareIdTextBox(); }
        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            // Annotations
            Form_RemoveAnnotations?.Invoke(null, null);
            // Selection
            _controller.ClearSelectionHistoryAndCallSelectionChanged();
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            Hide();
        }
        private void FrmQuery_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }
        private void FrmQuery_VisibleChanged(object sender, EventArgs e)
        {
            // This is called if some other form is shown to close all other forms
            // This is called after the form visibility changes
            if (Visible)
            {
                rbVertexNode.Checked = true;
                PrepareIdTextBox();
            }
            // The form was hidden 
            else
            {
                // Clear
                _controller.ClearSelectionHistoryAndCallSelectionChanged();
            }                
        }


        // Methods                                                                                                                  
        public void PrepareForm(Controller controller)
        {
            _controller = controller;
        }
        public void PrepareIdTextBox()
        {
            tbItemId.Focus();
            tbItemId.SelectAll();
        }

      
    }
}