using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;

namespace CaeMesh
{
    [Serializable]
    public class BasePart : FeGroup, IComparable<BasePart>
    {
        // Variables                                                                                                                
        protected int _partId;
        protected PartType _partType;
        protected System.Drawing.Color _color;
        protected Type[] _elementTypes;
        //[NonSerialized]
        protected VisualizationData _visualization;
        protected int[] _nodeLabels;
        protected bool _smoothShaded;
        protected BoundingBox _boundingBox;
        protected double[] _offset;

        [NonSerialized]
        protected VisualizationData _visualizationCopy; // temp copy while saving


        // Properties                                                                                                               
        public int PartId { get { return _partId; } set { _partId = value; } }
        public PartType PartType { get { return _partType; } }
        public System.Drawing.Color Color
        {
            get { return _color; }
            set { _color = value; }
        }
        public Type[] ElementTypes { get { return _elementTypes; } }
        public VisualizationData Visualization  { get { return _visualization; } set { _visualization = value; } }
        public VisualizationData VisualizationCopy { get { return _visualizationCopy; } set { _visualizationCopy = value; } }
        public int[] NodeLabels { get { return _nodeLabels; } set { _nodeLabels = value; } }
        public bool SmoothShaded { get { return _smoothShaded; } set { _smoothShaded = value; } }
        public BoundingBox BoundingBox { get { return _boundingBox; } set { _boundingBox = value; } }
        public double[] Offset { get { return _offset; } set { _offset = value; } }


        // Constructors                                                                                                             
        public BasePart(string name, int partId, int[] nodeLabels, int[] elementLabels, Type[] elementTypes)
            : base(name, elementLabels)
        {
            _partId = partId;
            _nodeLabels = nodeLabels;
            _color = System.Drawing.Color.Gray;
            _elementTypes = elementTypes;
            _visualization = new VisualizationData();
            _visualizationCopy = null;
            _smoothShaded = false;
            _boundingBox = new BoundingBox();
            _offset = new double[3];
            //
            if (IsSolid()) _partType = PartType.Solid;
            else if (IsShell()) _partType = PartType.Shell;
            else if (IsBeam()) _partType = PartType.Wire;
            else _partType = PartType.Unknown;
        }
        public BasePart(BasePart part)
            : base(part.Name, part.Labels.ToArray())
        {
            _partId = part.PartId;
            //
            _partType = part.PartType;
            //
            _color = part.Color;
            //
            _elementTypes = part.ElementTypes != null ? part.ElementTypes.ToArray() : null;
            //
            _visualization = part.Visualization.DeepCopy();
            //
            if (part.VisualizationCopy != null)
                _visualizationCopy = part.VisualizationCopy.DeepCopy();
            //
            _nodeLabels = part.NodeLabels != null ? part.NodeLabels.ToArray() : null;
            //
            _smoothShaded = part.SmoothShaded;
            //
            _boundingBox = part.BoundingBox.DeepClone();
            //
            if (part.Offset != null) _offset = part.Offset.ToArray();
        }


        // Methods                                                                                                                  
        private bool IsSolid()
        {
            if (_elementTypes == null) return false;
            //
            foreach (var type in _elementTypes)
            {
                if (type != typeof(LinearTetraElement) && type != typeof(LinearWedgeElement) && 
                    type != typeof(LinearHexaElement) && type != typeof(ParabolicTetraElement) &&
                    type != typeof(ParabolicWedgeElement) && type != typeof(ParabolicHexaElement)) return false;
            }
            return true;
        }
        private bool IsShell()
        {
            if (_elementTypes == null) return false;
            //
            foreach (var type in _elementTypes)
            {
                if (type != typeof(LinearTriangleElement) && type != typeof(LinearQuadrilateralElement) &&
                    type != typeof(ParabolicTriangleElement) && type != typeof(ParabolicQuadrilateralElement)) return false;
            }
            return true;
        }
        private bool IsBeam()
        {
            if (_elementTypes == null) return false;
            //
            foreach (var type in _elementTypes)
            {
                if (type != typeof(LinearBeamElement) && type != typeof(ParabolicBeamElement)) return false;
            }
            return true;
        }
        public void SetPartType(PartType partType)
        {
            if (_partType == PartType.Shell && partType == PartType.SolidAsShell)
            {
                _partType = partType;
            }
            else throw new NotSupportedException();
        }

        public virtual BasePart DeepCopy()
        {
            return new BasePart(this);
        }
        public virtual PartProperties GetProperties()
        {
            PartProperties properties = new PartProperties();
            properties.Name = Name;
            properties.PartType = _partType;
            properties.Color = _color;
            properties.NumberOfNodes = _nodeLabels.Length;
            properties.NumberOfElements = Labels.Length;
            return properties;
        }
        public virtual void SetProperties(PartProperties properties)
        {
            Name = properties.Name;
            _color = properties.Color;
        }
        public virtual void RenumberVisualizationNodes(Dictionary<int, int> newIds)
        {
            _visualization.RenumberNodes(newIds);
        }
        public virtual void RenumberVisualizationElements(Dictionary<int, int> newIds)
        {
            _visualization.RenumberElements(newIds);
        }
        public virtual void RenumberVisualizationSurfaces(int[] orderedSurfaceIds)
        {
            _visualization.RenumberSurfaces(orderedSurfaceIds);
        }
        public virtual void RenumberVisualizationEdges(int[] orderedEdgesIds)
        {
            _visualization.RenumberEdges(orderedEdgesIds);
        }
        // Added for usage in graphs
        public int CompareTo(BasePart other)
        {
            if (_boundingBox.GetDiagonal() < other._boundingBox.GetDiagonal()) return 1;
            else if (_boundingBox.GetDiagonal() > other._boundingBox.GetDiagonal()) return -1;
            else return 0;
            //if (System.Diagnostics.Debugger.IsAttached) throw new NotImplementedException();
            //else return 0;
        }
    }
}
