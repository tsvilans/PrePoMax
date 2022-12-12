using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;
using System.ComponentModel;
using DynamicTypeDescriptor;

namespace PrePoMax.Forms
{
    [Serializable]
    public class ViewMeshingParameters
    {
        // Variables                                                                                                                
        protected MeshingParameters _parameters;
        protected readonly DynamicCustomTypeDescriptor _dctd;


        // Properties                                                                                                               
        [Category("Mesh size")]
        [OrderedDisplayName(0, 10, "Max element size")]
        [Description("The value for the maximum element size.")]
        [TypeConverter(typeof(StringLengthConverter))]
        [Id(1, 1)]
        public virtual double MaxH { get { return _parameters.MaxH; } set { _parameters.MaxH = value; } }
        //
        [Category("Mesh size")]
        [OrderedDisplayName(1, 10, "Min element size")]
        [Description("The value for the minimum element size.")]
        [TypeConverter(typeof(StringLengthConverter))]
        [Id(2, 1)]
        public virtual double MinH { get { return _parameters.MinH; } set { _parameters.MinH = value; } }
        //
        [Category("Mesh size")]
        [OrderedDisplayName(2, 10, "Grading")]
        [Description("The value of the mesh grading (0 => uniform mesh; 1 => aggressive local grading).")]
        [TypeConverter(typeof(StringDoubleConverter))]
        [Id(3, 1)]
        public double Grading { get { return _parameters.Grading; } set { _parameters.Grading = value; } }
        //
        [Category("Mesh size")]
        [OrderedDisplayName(3, 10, "Elements per edge")]
        [Description("Number of elements to generate per edge of the geometry.")]
        [TypeConverter(typeof(StringDoubleConverter))]
        [Id(4, 1)]
        public double Elementsperedge { get { return _parameters.Elementsperedge; } set { _parameters.Elementsperedge = value; } }
        //
        [Category("Mesh size")]
        [OrderedDisplayName(4, 10, "Elements per curvature")]
        [Description("Number of elements to generate per curvature radius.")]
        [TypeConverter(typeof(StringDoubleConverter))]
        [Id(5, 1)]
        public double Elementspercurve { get { return _parameters.Elementspercurve; } set { _parameters.Elementspercurve = value; } }
        // Maximal Hausdorff distance for the boundaries approximation.
        [Category("Mesh size")]
        [OrderedDisplayName(5, 10, "Hausdorff")]
        [Description("Maximal Hausdorff distance for the boundaries approximation. " +
                              "A value of 0.01 is a suitable value for an object of size 1 in each direction.")]
        //
        [TypeConverter(typeof(StringLengthConverter))]
        [Id(6, 1)]
        public double Hausdorff { get { return _parameters.Hausdorff; } set { _parameters.Hausdorff = value; } }
        //
        //
        [Category("Mesh optimization")]
        [OrderedDisplayName(0, 10, "Optimize steps 2D")]
        [Description("Number of optimize steps to use for 2-D mesh optimization.")]
        [Id(1, 2)]
        public int OptimizeSteps2D { get { return _parameters.OptimizeSteps2D; } set { _parameters.OptimizeSteps2D = value; } }
        //
        [Category("Mesh optimization")]
        [OrderedDisplayName(1, 10, "Optimize steps 3D")]
        [Description("Number of optimize steps to use for 3-D mesh optimization.")]
        [Id(2, 2)]
        public int OptimizeSteps3D { get { return _parameters.OptimizeSteps3D; } set { _parameters.OptimizeSteps3D = value; } }
        //
        //
        [Category("Mesh type")]
        [OrderedDisplayName(0, 10, "Second order")]
        [Description("Create second order elements.")]
        [Id(1, 3)]
        public bool SecondOrder
        {
            get {return _parameters.SecondOrder;}
            set
            {
                _parameters.SecondOrder = value;
                //
                if (!_parameters.UseMmg)
                {
                    _dctd.GetProperty(nameof(MidsideNodesOnGeometry)).SetIsBrowsable(value);
                    _dctd.PropertySortOrder = CustomSortOrder.AscendingById;
                }
            }
        }
        //
        [Category("Mesh type")]
        [OrderedDisplayName(1, 10, "Midside nodes on geometry")]
        [Description("Create midside nodes on geometry.")]
        [Id(2, 3)]
        public bool MidsideNodesOnGeometry { get { return _parameters.MidsideNodesOnGeometry; } set { _parameters.MidsideNodesOnGeometry = value; } }
        //
        [Category("Mesh type")]
        [OrderedDisplayName(2, 10, "Quad-dominated mesh")]
        [Description("Use quad-dominated mesh for shell parts.")]
        [Id(3, 3)]
        public bool QuadDominated { get { return _parameters.QuadDominated; } set { _parameters.QuadDominated = value; } }
        //
        //
        [Category("Mesh operations")]
        [OrderedDisplayName(0, 10, "Split compound mesh")]
        [Description("Split compound part mesh to unconnected part meshes.")]
        [Id(1, 4)]
        public bool SplitCompoundMesh { get { return _parameters.SplitCompoundMesh; } set { _parameters.SplitCompoundMesh = value; } }
        //
        [Category("Mesh operations")]
        [OrderedDisplayName(1, 10, "Keep model edges")]
        [Description("Select Yes to keep and No to ignore the model edges.")]
        [Id(2, 4)]
        public bool KeepModelEdges { get { return _parameters.KeepModelEdges; } set { _parameters.KeepModelEdges = value; } }


        // Constructors                                                                                                             
        public ViewMeshingParameters(MeshingParameters parameters)
        {
            _parameters = parameters;
            _dctd = ProviderInstaller.Install(this);
            // Category sorting
            _dctd.CategorySortOrder = CustomSortOrder.AscendingById;
            _dctd.PropertySortOrder = CustomSortOrder.AscendingById;    // seems not to work
            // Now lets display Yes/No instead of True/False
            _dctd.RenameBooleanPropertyToYesNo(nameof(SecondOrder));
            _dctd.RenameBooleanPropertyToYesNo(nameof(MidsideNodesOnGeometry));
            _dctd.RenameBooleanPropertyToYesNo(nameof(QuadDominated));
            _dctd.RenameBooleanPropertyToYesNo(nameof(SplitCompoundMesh));
            _dctd.RenameBooleanPropertyToYesNo(nameof(KeepModelEdges));
            //
            _dctd.GetProperty(nameof(MaxH)).SetIsBrowsable(true);
            _dctd.GetProperty(nameof(MinH)).SetIsBrowsable(true);
            _dctd.GetProperty(nameof(Grading)).SetIsBrowsable(!_parameters.UseMmg);
            _dctd.GetProperty(nameof(Elementsperedge)).SetIsBrowsable(!_parameters.UseMmg);
            _dctd.GetProperty(nameof(Elementspercurve)).SetIsBrowsable(!_parameters.UseMmg);
            _dctd.GetProperty(nameof(Hausdorff)).SetIsBrowsable(_parameters.UseMmg);            // mmg only
            _dctd.GetProperty(nameof(OptimizeSteps2D)).SetIsBrowsable(!_parameters.UseMmg);
            _dctd.GetProperty(nameof(OptimizeSteps3D)).SetIsBrowsable(!_parameters.UseMmg);
            _dctd.GetProperty(nameof(SecondOrder)).SetIsBrowsable(true);
            _dctd.GetProperty(nameof(MidsideNodesOnGeometry)).SetIsBrowsable(!_parameters.UseMmg);
            _dctd.GetProperty(nameof(QuadDominated)).SetIsBrowsable(!_parameters.UseMmg);
            _dctd.GetProperty(nameof(SplitCompoundMesh)).SetIsBrowsable(!_parameters.UseMmg);
            _dctd.GetProperty(nameof(KeepModelEdges)).SetIsBrowsable(_parameters.UseMmg);       // mmg only
            // To show/hide the MediumNodesOnGeometry property
            SecondOrder = _parameters.SecondOrder;
        }


        // Methods                                                                                                                  
        public MeshingParameters GetBase()
        {
            return _parameters;
        }
    }
}
