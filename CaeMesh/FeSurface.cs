using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicTypeDescriptor;
using CaeGlobals;

namespace CaeMesh
{
    [Serializable]
    public enum FeSurfaceCreatedFrom
    {
        [StandardValue("Selection", DisplayName = "Selection")]
        Selection,
        [StandardValue("NodeSet", DisplayName = "Node set")]
        NodeSet,
        [StandardValue("Faces", Visible = false)]
        Faces
    }

    // used in Calculix inp file
    [Serializable]
    public enum FeSurfaceType
    {
        Element,
        Node
    }

    [Serializable]
    public enum FeSurfaceFaceTypes
    {
        Unknown,
        BeamFaces,
        ShellFaces,
        ShellEdgeFaces,
        SolidFaces
    }

    [Serializable]
    public class FeSurface : NamedClass
    {
        // Variables                                                                                                                
        private FeSurfaceType _type;
        private FeSurfaceCreatedFrom _createdFrom;
        private string _nodeSetName;
        private string _createdFromNodeSetName;
        private int[] _faceIds;                                                         // renumber on renumber elements ids
        private double _area;
        private Dictionary<FeFaceName, string> _elementFaces;
        private Selection _creationData;
        private FeSurfaceFaceTypes _surfaceFaceTypes = FeSurfaceFaceTypes.Unknown;


        // Properties                                                                                                               
        public FeSurfaceType Type
        {
            get { return _type; }
            set { _type = value; }
        }
        public FeSurfaceCreatedFrom CreatedFrom 
        { 
            get { return _createdFrom; } 
            set 
            {
                if (_createdFrom != value)
                {
                    Clear();
                    _createdFrom = value;
                }
            } 
        }
        /// <summary>
        /// The name of the internal node set name that represents all the nodes of the surface
        /// </summary>
        public string NodeSetName { get { return _nodeSetName; } set { _nodeSetName = value; } }
        /// <summary>
        /// The name of the internal node set name the surface was created from
        /// </summary>
        public string CreatedFromNodeSetName { get { return _createdFromNodeSetName; } set { _createdFromNodeSetName = value; } }
        public int[] FaceIds { get { return _faceIds; } set { _faceIds = value; } }
        public double Area { get { return _area; } set { _area = value; } }
        public Dictionary<FeFaceName, string> ElementFaces { get { return _elementFaces; } }
        public Selection CreationData { get { return _creationData; } set { _creationData = value; } }
        public FeSurfaceFaceTypes SurfaceFaceTypes { get { return _surfaceFaceTypes; } set { _surfaceFaceTypes = value; } }


        // Constructors                                                                                                             
        public FeSurface(string name)
            : base(name)
        {
            _type = FeSurfaceType.Element;
            Clear();
        }
        public FeSurface(string name, string nodeSetName)
            : this(name)
        {
            _createdFrom = FeSurfaceCreatedFrom.NodeSet;
            _createdFromNodeSetName = nodeSetName;
        }
        public FeSurface(string name, int[] faceIds, Selection creationDataClone)
            : this(name)
        {
            _faceIds = faceIds;
            _creationData = creationDataClone;
        }
        public FeSurface(FeSurface surface)
            : base(surface)
        {
            _type = surface._type;
            _createdFrom = surface._createdFrom;
            _nodeSetName = surface._nodeSetName;
            _createdFromNodeSetName = surface._createdFromNodeSetName;
            _faceIds = surface._faceIds != null ? surface._faceIds.ToArray() : null;
            _area = surface._area;
            _elementFaces = surface._elementFaces != null ? new Dictionary<FeFaceName, string>(surface._elementFaces) : null;
            _creationData = surface._creationData != null ? surface._creationData.DeepClone() : null;
            _surfaceFaceTypes = surface._surfaceFaceTypes;
        }


        // Methods                                                                                                                  
        private void Clear()
        {
            _createdFrom = FeSurfaceCreatedFrom.Selection;
            _nodeSetName = null;
            _createdFromNodeSetName = null;
            _faceIds = null;
            _area = -1;
            _elementFaces = null;
            _creationData = null;
            _surfaceFaceTypes = FeSurfaceFaceTypes.Unknown;
        }
        public void AddElementFace(FeFaceName faceName, string elementSetName)
        {
            if (faceName == FeFaceName.Empty) throw new CaeException("The face name of the surface can not be 'Empty'.");
            if (_elementFaces == null) _elementFaces = new Dictionary<FeFaceName, string>();
            //_type = FeSurfaceType.Element;
            _elementFaces.Add(faceName, elementSetName);
        }
        public void ClearElementFaces()
        {
            _elementFaces = null;
            _area = 0;
        }        
    }
}
