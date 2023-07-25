using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;

namespace CaeMesh
{
    [Serializable]
    public class GeometryPart : BasePart
    {
        // Variables                                                                                                                
        private MeshingParameters _meshingParameters;
        private List<Enum> _elementTypeEnums;
        private int[] _freeEdgeCellIds;
        private int[] _errorEdgeCellIds;
        private int[] _freeNodeIds;
        private int[] _errorNodeIds;
        private string _cadFileData;


        // Properties                                                                                                               
        public MeshingParameters MeshingParameters
        {
            get { return _meshingParameters; }
            set
            {
                _meshingParameters = value;
                if (_meshingParameters != null && (PartType == PartType.Solid || PartType == PartType.SolidAsShell))
                    _meshingParameters.QuadDominated = false;
            }
        }
        
        public int[] FreeEdgeCellIds { get { return _freeEdgeCellIds; } set { _freeEdgeCellIds = value; } }
        public int[] ErrorEdgeCellIds { get { return _errorEdgeCellIds; } set { _errorEdgeCellIds = value; } }
        public int[] FreeNodeIds { get { return _freeNodeIds; } set { _freeNodeIds = value; } }
        public int[] ErrorNodeIds { get { return _errorNodeIds; } set { _errorNodeIds = value; } }
        public bool HasErrors
        {
            get
            {
                if (_errorEdgeCellIds != null) return true;
                else if (_errorNodeIds != null) return true;
                else return false;
            }
        }
        public bool HasFreeEdges
        {
            get
            {
                if (_freeEdgeCellIds != null) return true;
                else if (_freeNodeIds != null) return true;
                else return false;
            }
        }
        public string CADFileData
        {
            get
            {
                if (_cadFileData != null) return StringCompressor.DecompressString(_cadFileData);
                else return _cadFileData;
            }
            set
            {
                if (value == null) _cadFileData = value;
                else _cadFileData = CaeGlobals.StringCompressor.CompressString(value);
            }
        }


        // Constructors                                                                                                             
        public GeometryPart(string name, int partId,  int[] nodeLabels, int[] elementLabels, Type[] elementTypes)
            : base(name, partId, nodeLabels, elementLabels, elementTypes)
        {
            _meshingParameters = null;
            _elementTypeEnums = null;
            _freeEdgeCellIds = null;
            _errorEdgeCellIds = null;
            _freeNodeIds = null;
            _errorNodeIds = null;
            _cadFileData = null;
        }
        public GeometryPart(BasePart part)
            : base(part)
        {
            _meshingParameters = null;
            _elementTypeEnums = null;
            _freeEdgeCellIds = null;
            _errorEdgeCellIds = null;
            _freeNodeIds = null;
            _errorNodeIds = null;
            _cadFileData = null;
        }
        public GeometryPart(GeometryPart part)
            : base((BasePart)part)
        {
            _meshingParameters = part._meshingParameters != null ? part._meshingParameters.DeepClone() : null;
            _elementTypeEnums = part._elementTypeEnums != null ? part._elementTypeEnums.ToList() : null;
            _freeEdgeCellIds = part.FreeEdgeCellIds != null ? part.FreeEdgeCellIds.ToArray() : null;
            _errorEdgeCellIds = part.ErrorEdgeCellIds != null ? part.ErrorEdgeCellIds.ToArray() : null;
            _freeNodeIds = part.FreeNodeIds != null ? part.FreeNodeIds.ToArray() : null;
            _errorNodeIds = part.ErrorNodeIds != null ? part.ErrorNodeIds.ToArray() : null;
            _cadFileData = part._cadFileData != null ? part._cadFileData : null;
        }


        // Methods                                                                                                                  
        public override BasePart DeepCopy()
        {
            return new GeometryPart(this);
        }
        //
        public void CADFileDataFromFile(string brepFileName)
        {
            if (System.IO.Path.GetExtension(brepFileName) == ".brep") CADFileData = System.IO.File.ReadAllText(brepFileName);
            else throw new NotSupportedException();
        }
        //
        public override PartProperties GetProperties()
        {
            PartProperties properties = base.GetProperties();
            return properties;
        }
        public override void SetProperties(PartProperties properties)
        {
            base.SetProperties(properties);
        }
        public List<Enum> GetElementTypeEnums()
        {
            return _elementTypeEnums;
        }
        public void AddElementTypeEnums(List<Enum> elementTypeEnums)
        {            
            Dictionary<Type, Enum> oldEnums = new Dictionary<Type, Enum>();
            if (_elementTypeEnums != null)
            {
                foreach (var oldElementTypeEnum in _elementTypeEnums)
                    oldEnums.Add(oldElementTypeEnum.GetType(), oldElementTypeEnum);
            }
            //
            foreach (var newEnum in elementTypeEnums)
            {
                if (oldEnums.ContainsKey(newEnum.GetType())) oldEnums[newEnum.GetType()] = newEnum;
                else oldEnums.Add(newEnum.GetType(), newEnum);
            }
            //
            _elementTypeEnums = oldEnums.Values.ToList();
        }
        public override void RenumberVisualizationElements(Dictionary<int, int> newIds)
        {
            base.RenumberVisualizationElements(newIds);
        }
        public override void RenumberVisualizationNodes(Dictionary<int, int> newIds)
        {
            base.RenumberVisualizationNodes(newIds);
            //
            if (_errorNodeIds != null)
            {
                for (int i = 0; i < _errorNodeIds.Length; i++) _errorNodeIds[i] = newIds[_errorNodeIds[i]];
            }
            if (_freeNodeIds != null)
            {
                for (int i = 0; i < _freeNodeIds.Length; i++) _freeNodeIds[i] = newIds[_freeNodeIds[i]];
            }
        }
    }
}
