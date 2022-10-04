using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeMesh
{
    [Serializable]
    public class MeshPart : BasePart
    {
        // Variables                                                                                                                
        protected FeElementTypeLinearTria _linearTriaType;
        protected FeElementTypeParabolicTria _parabolicTriaType;
        protected FeElementTypeLinearQuad _linearQuadType;
        protected FeElementTypeParabolicQuad _parabolicQuadType;
        //
        protected FeElementTypeLinearTetra _linearTetraType;
        protected FeElementTypeParabolicTetra _parabolicTetraType;
        protected FeElementTypeLinearWedge _linearWedgeType;
        protected FeElementTypeParabolicWedge _parabolicWedgeType;
        protected FeElementTypeLinearHexa _linearHexaType;
        protected FeElementTypeParabolicHexa _parabolicHexaType;


        // Properties                                                                                                               
        public FeElementTypeLinearTria LinearTriaType { get { return _linearTriaType; } set { _linearTriaType = value; } }
        public FeElementTypeParabolicTria ParabolicTriaType { get { return _parabolicTriaType; } set { _parabolicTriaType = value; } }
        public FeElementTypeLinearQuad LinearQuadType { get { return _linearQuadType; } set { _linearQuadType = value; } }
        public FeElementTypeParabolicQuad ParabolicQuadType { get { return _parabolicQuadType; } set { _parabolicQuadType = value; } }
        //
        public FeElementTypeLinearTetra LinearTetraType { get { return _linearTetraType; } set { _linearTetraType = value; } }
        public FeElementTypeParabolicTetra ParabolicTetraType { get { return _parabolicTetraType; } set { _parabolicTetraType = value; } }
        public FeElementTypeLinearWedge LinearWedgeType { get { return _linearWedgeType; } set { _linearWedgeType = value; } }
        public FeElementTypeParabolicWedge ParabolicWedgeType { get { return _parabolicWedgeType; } set { _parabolicWedgeType = value; } }
        public FeElementTypeLinearHexa LinearHexaType { get { return _linearHexaType; } set { _linearHexaType = value; } }
        public FeElementTypeParabolicHexa ParabolicHexaType { get { return _parabolicHexaType; } set { _parabolicHexaType = value; } }


        // Constructors                                                                                                             
        public MeshPart(string name, int partId, int[] nodeLabels, int[] elementLabels, Type[] elementTypes)
            : base(name, partId, nodeLabels, elementLabels, elementTypes)
        {
            InitializeElementTypes();
        }
        public MeshPart(BasePart part)
            : base(part)
        {
            InitializeElementTypes();
        }
        public MeshPart(MeshPart part)
            : base(part)
        {
            _linearTriaType = part.LinearTriaType;
            _parabolicTriaType = part.ParabolicTriaType;
            _linearQuadType = part.LinearQuadType;
            _parabolicQuadType = part.ParabolicQuadType;
            //
            _linearTetraType = part.LinearTetraType;
            _parabolicTetraType = part.ParabolicTetraType;
            _linearWedgeType = part.LinearWedgeType;
            _parabolicWedgeType = part.ParabolicWedgeType;
            _linearHexaType = part.LinearHexaType;
            _parabolicHexaType = part.ParabolicHexaType;
        }


        // Methods                                                                                                                  
        private void InitializeElementTypes()
        {
            _linearTriaType = FeElementTypeLinearTria.None;
            _parabolicTriaType = FeElementTypeParabolicTria.None;
            _linearQuadType = FeElementTypeLinearQuad.None;
            _parabolicQuadType = FeElementTypeParabolicQuad.None;
            //
            _linearTetraType = FeElementTypeLinearTetra.None;
            _parabolicTetraType = FeElementTypeParabolicTetra.None;
            _linearWedgeType = FeElementTypeLinearWedge.None;
            _parabolicWedgeType = FeElementTypeParabolicWedge.None;
            _linearHexaType = FeElementTypeLinearHexa.None;
            _parabolicHexaType = FeElementTypeParabolicHexa.None;
            //
            HashSet<Type> types = new HashSet<Type>(_elementTypes);
            if (types.Contains(typeof(LinearTriangleElement))) _linearTriaType = FeElementTypeLinearTria.S3;
            if (types.Contains(typeof(ParabolicTriangleElement))) _parabolicTriaType = FeElementTypeParabolicTria.S6;
            if (types.Contains(typeof(LinearQuadrilateralElement))) _linearQuadType = FeElementTypeLinearQuad.S4;
            if (types.Contains(typeof(ParabolicQuadrilateralElement))) _parabolicQuadType = FeElementTypeParabolicQuad.S8;
            //
            if (types.Contains(typeof(LinearTetraElement))) _linearTetraType = FeElementTypeLinearTetra.C3D4;
            if (types.Contains(typeof(ParabolicTetraElement))) _parabolicTetraType = FeElementTypeParabolicTetra.C3D10;
            if (types.Contains(typeof(LinearWedgeElement))) _linearWedgeType = FeElementTypeLinearWedge.C3D6;
            if (types.Contains(typeof(ParabolicWedgeElement))) _parabolicWedgeType = FeElementTypeParabolicWedge.C3D15;
            if (types.Contains(typeof(LinearHexaElement))) _linearHexaType = FeElementTypeLinearHexa.C3D8;
            if (types.Contains(typeof(ParabolicHexaElement))) _parabolicHexaType = FeElementTypeParabolicHexa.C3D20;
        }
        public void AddElementTypes(Type[] elementTypes)
        {
            HashSet<Type> types = new HashSet<Type>(elementTypes);
            //
            if (types.Contains(typeof(LinearTriangleElement)) && _linearTriaType == FeElementTypeLinearTria.None)
                _linearTriaType = FeElementTypeLinearTria.S3;
            if (types.Contains(typeof(ParabolicTriangleElement)) && _parabolicTriaType == FeElementTypeParabolicTria.None)
                _parabolicTriaType = FeElementTypeParabolicTria.S6;
            if (types.Contains(typeof(LinearQuadrilateralElement)) && _linearQuadType == FeElementTypeLinearQuad.None)
                _linearQuadType = FeElementTypeLinearQuad.S4;
            if (types.Contains(typeof(ParabolicQuadrilateralElement)) && _parabolicQuadType == FeElementTypeParabolicQuad.None)
                _parabolicQuadType = FeElementTypeParabolicQuad.S8;
            //
            if (types.Contains(typeof(LinearTetraElement)) && _linearTetraType == FeElementTypeLinearTetra.None)
                _linearTetraType = FeElementTypeLinearTetra.C3D4;
            if (types.Contains(typeof(ParabolicTetraElement)) && _parabolicTetraType == FeElementTypeParabolicTetra.None)
                _parabolicTetraType = FeElementTypeParabolicTetra.C3D10;
            if (types.Contains(typeof(LinearWedgeElement)) && _linearWedgeType == FeElementTypeLinearWedge.None)
                _linearWedgeType = FeElementTypeLinearWedge.C3D6;
            if (types.Contains(typeof(ParabolicWedgeElement)) && _parabolicWedgeType == FeElementTypeParabolicWedge.None)
                _parabolicWedgeType = FeElementTypeParabolicWedge.C3D15;
            if (types.Contains(typeof(LinearHexaElement)) && _linearHexaType == FeElementTypeLinearHexa.None)
                _linearHexaType = FeElementTypeLinearHexa.C3D8;
            if (types.Contains(typeof(ParabolicHexaElement)) && _parabolicHexaType == FeElementTypeParabolicHexa.None)
                _parabolicHexaType = FeElementTypeParabolicHexa.C3D20;
            //
            HashSet<Type> allTypes = new HashSet<Type>();
            if (_elementTypes != null) allTypes.UnionWith(_elementTypes);
            if (elementTypes != null) allTypes.UnionWith(elementTypes);
            _elementTypes = allTypes.ToArray();
        }
        //
        public void CopyActiveElementTypesFrom(MeshPart part)
        {
            if (_linearTriaType != FeElementTypeLinearTria.None) _linearTriaType = part.LinearTriaType;
            if (_parabolicTriaType != FeElementTypeParabolicTria.None) _parabolicTriaType = part.ParabolicTriaType;
            if (_linearQuadType != FeElementTypeLinearQuad.None) _linearQuadType = part.LinearQuadType;
            if (_parabolicQuadType != FeElementTypeParabolicQuad.None) _parabolicQuadType = part.ParabolicQuadType;
            //
            if (_linearTetraType != FeElementTypeLinearTetra.None) _linearTetraType = part.LinearTetraType;
            if (_parabolicTetraType != FeElementTypeParabolicTetra.None) _parabolicTetraType = part.ParabolicTetraType;
            if (_linearWedgeType != FeElementTypeLinearWedge.None) _linearWedgeType = part.LinearWedgeType;
            if (_parabolicWedgeType != FeElementTypeParabolicWedge.None) _parabolicWedgeType = part.ParabolicWedgeType;
            if (_linearHexaType != FeElementTypeLinearHexa.None) _linearHexaType = part.LinearHexaType;
            if (_parabolicHexaType != FeElementTypeParabolicHexa.None) _parabolicHexaType = part._parabolicHexaType;
        }
        public void SetPropertiesFromInpElementTypeName(string[] inpElementTypeNames)
        {
            foreach (var inpElementTypeName in inpElementTypeNames)
            {
                switch (inpElementTypeName)
                {
                    // Triangular
                    case "S3":
                    case "S3R":
                        _linearTriaType = FeElementTypeLinearTria.S3; break;
                    case "M3D3":
                        _linearTriaType = FeElementTypeLinearTria.M3D3; break;
                    case "CPS3":
                        _linearTriaType = FeElementTypeLinearTria.CPS3; break;
                    case "CPE3":
                        _linearTriaType = FeElementTypeLinearTria.CPE3; break;
                    case "CAX3":
                        _linearTriaType = FeElementTypeLinearTria.CAX3; break;
                    //
                    case "S6":
                    case "STRI65":  // Abaqus
                        _parabolicTriaType = FeElementTypeParabolicTria.S6; break;
                    case "M3D6":
                        _parabolicTriaType = FeElementTypeParabolicTria.M3D6; break;
                    case "CPS6":
                        _parabolicTriaType = FeElementTypeParabolicTria.CPS6; break;
                    case "CPE6":
                        _parabolicTriaType = FeElementTypeParabolicTria.CPE6; break;
                    case "CAX6":
                        _parabolicTriaType = FeElementTypeParabolicTria.CAX6; break;
                    // Quadrilateral
                    case "S4":
                        _linearQuadType = FeElementTypeLinearQuad.S4; break;
                    case "S4R":
                        _linearQuadType = FeElementTypeLinearQuad.S4R; break;
                    case "M3D4":
                        _linearQuadType = FeElementTypeLinearQuad.M3D4; break;
                    case "M3D4R":
                        _linearQuadType = FeElementTypeLinearQuad.M3D4R; break;
                    case "CPS4":
                        _linearQuadType = FeElementTypeLinearQuad.CPS4; break;
                    case "CPS4R":
                        _linearQuadType = FeElementTypeLinearQuad.CPS4R; break;
                    case "CPE4":
                        _linearQuadType = FeElementTypeLinearQuad.CPE4; break;
                    case "CPE4R":
                        _linearQuadType = FeElementTypeLinearQuad.CPE4R; break;
                    case "CAX4":
                        _linearQuadType = FeElementTypeLinearQuad.CAX4; break;
                    case "CAX4R":
                        _linearQuadType = FeElementTypeLinearQuad.CAX4R; break;
                    //
                    case "S8":
                        _parabolicQuadType = FeElementTypeParabolicQuad.S8; break;
                    case "S8R":
                        _parabolicQuadType = FeElementTypeParabolicQuad.S8R; break;
                    case "M3D8":
                        _parabolicQuadType = FeElementTypeParabolicQuad.M3D8; break;
                    case "M3D8R":
                        _parabolicQuadType = FeElementTypeParabolicQuad.M3D8R; break;
                    case "CPS8":
                        _parabolicQuadType = FeElementTypeParabolicQuad.CPS8; break;
                    case "CPS8R":
                        _parabolicQuadType = FeElementTypeParabolicQuad.CPS8R; break;
                    case "CPE8":
                        _parabolicQuadType = FeElementTypeParabolicQuad.CPE8; break;
                    case "CPE8R":
                        _parabolicQuadType = FeElementTypeParabolicQuad.CPE8R; break;
                    case "CAX8":
                        _parabolicQuadType = FeElementTypeParabolicQuad.CAX8; break;
                    case "CAX8R":
                        _parabolicQuadType = FeElementTypeParabolicQuad.CAX8R; break;
                    // Tetrahedral
                    case "C3D4":
                        _linearTetraType = FeElementTypeLinearTetra.C3D4; break;
                    //
                    case "C3D10":
                        _parabolicTetraType = FeElementTypeParabolicTetra.C3D10; break;
                    case "C3D10T":
                        _parabolicTetraType = FeElementTypeParabolicTetra.C3D10T; break;
                    // Wedge
                    case "C3D6":
                        _linearWedgeType = FeElementTypeLinearWedge.C3D6; break;
                    //
                    case "C3D15":
                        _parabolicWedgeType = FeElementTypeParabolicWedge.C3D15; break;
                    // Hexahedral
                    case "C3D8":
                        _linearHexaType = FeElementTypeLinearHexa.C3D8; break;
                    case "C3D8R":
                        _linearHexaType = FeElementTypeLinearHexa.C3D8R; break;
                    case "C3D8I":
                        _linearHexaType = FeElementTypeLinearHexa.C3D8I; break;
                    //
                    case "C3D20":
                        _parabolicHexaType = FeElementTypeParabolicHexa.C3D20; break;
                    case "C3D20R":
                        _parabolicHexaType = FeElementTypeParabolicHexa.C3D20R; break;
                    default:
                        throw new CaeGlobals.CaeException("The inp element type '" + inpElementTypeName + "' is not supported.");
                }
            }
        }
        public override BasePart DeepCopy()
        {
            return new MeshPart(this);
        }
        public override PartProperties GetProperties()
        {
            PartProperties properties = base.GetProperties();
            //
            properties.LinearTriaType = _linearTriaType;
            properties.ParabolicTriaType = _parabolicTriaType;
            properties.LinearQuadType = _linearQuadType;
            properties.ParabolicQuadType = _parabolicQuadType;
            //
            properties.LinearTetraType = _linearTetraType;
            properties.ParabolicTetraType = _parabolicTetraType;
            properties.LinearWedgeType = _linearWedgeType;
            properties.ParabolicWedgeType = _parabolicWedgeType;
            properties.LinearHexaType = _linearHexaType;
            properties.ParabolicHexaType = _parabolicHexaType;

            return properties;
        }       
        public override void SetProperties(PartProperties properties)
        {
            base.SetProperties(properties);
            //
            if (_linearTriaType != FeElementTypeLinearTria.None && properties.LinearTriaType != FeElementTypeLinearTria.None)
                _linearTriaType = properties.LinearTriaType;
            if (_parabolicTriaType != FeElementTypeParabolicTria.None && properties.ParabolicTriaType != FeElementTypeParabolicTria.None)
                _parabolicTriaType = properties.ParabolicTriaType;
            if (_linearQuadType != FeElementTypeLinearQuad.None && properties.LinearQuadType != FeElementTypeLinearQuad.None)
                _linearQuadType = properties.LinearQuadType;
            if (_parabolicQuadType != FeElementTypeParabolicQuad.None && properties.ParabolicQuadType != FeElementTypeParabolicQuad.None)
                _parabolicQuadType = properties.ParabolicQuadType;
            //
            if (_linearTetraType != FeElementTypeLinearTetra.None && properties.LinearTetraType != FeElementTypeLinearTetra.None)
                _linearTetraType = properties.LinearTetraType;
            if (_parabolicTetraType != FeElementTypeParabolicTetra.None && properties.ParabolicTetraType != FeElementTypeParabolicTetra.None)
                _parabolicTetraType = properties.ParabolicTetraType;
            if (_linearWedgeType != FeElementTypeLinearWedge.None && properties.LinearWedgeType != FeElementTypeLinearWedge.None)
                _linearWedgeType = properties.LinearWedgeType;
            if (_parabolicWedgeType != FeElementTypeParabolicWedge.None && properties.ParabolicWedgeType != FeElementTypeParabolicWedge.None)
                _parabolicWedgeType = properties.ParabolicWedgeType;
            if (_linearHexaType != FeElementTypeLinearHexa.None && properties.LinearHexaType != FeElementTypeLinearHexa.None)
                _linearHexaType = properties.LinearHexaType;
            if (_parabolicHexaType != FeElementTypeParabolicHexa.None && properties.ParabolicHexaType != FeElementTypeParabolicHexa.None)
                _parabolicHexaType = properties.ParabolicHexaType;
        }
        //
        public List<Enum> GetElementTypeEnums()
        {
            List<Enum> elementTypeEnums = new List<Enum>();
            //
            if (_linearTriaType != FeElementTypeLinearTria.None) elementTypeEnums.Add(_linearTriaType);
            if (_parabolicTriaType != FeElementTypeParabolicTria.None) elementTypeEnums.Add(_parabolicTriaType);
            if (_linearQuadType != FeElementTypeLinearQuad.None) elementTypeEnums.Add(_linearQuadType);
            if (_parabolicQuadType != FeElementTypeParabolicQuad.None) elementTypeEnums.Add(_parabolicQuadType);
            //
            if (_linearTetraType != FeElementTypeLinearTetra.None) elementTypeEnums.Add(_linearTetraType);
            if (_parabolicTetraType != FeElementTypeParabolicTetra.None) elementTypeEnums.Add(_parabolicTetraType);
            if (_linearWedgeType != FeElementTypeLinearWedge.None) elementTypeEnums.Add(_linearWedgeType);
            if (_parabolicWedgeType != FeElementTypeParabolicWedge.None) elementTypeEnums.Add(_parabolicWedgeType);
            if (_linearHexaType != FeElementTypeLinearHexa.None) elementTypeEnums.Add(_linearHexaType);
            if (_parabolicHexaType != FeElementTypeParabolicHexa.None) elementTypeEnums.Add(_parabolicHexaType);
            //
            return elementTypeEnums;
        }
        public void SetElementTypeEnums(List<Enum> elementTypeEnums)
        {
            if (elementTypeEnums != null)
            {
                foreach (var elementEnum in elementTypeEnums)
                {
                    if (elementEnum is FeElementTypeLinearTria && _linearTriaType != FeElementTypeLinearTria.None)
                        _linearTriaType = (FeElementTypeLinearTria)elementEnum;
                    else if (elementEnum is FeElementTypeParabolicTria && _parabolicTriaType != FeElementTypeParabolicTria.None)
                        _parabolicTriaType = (FeElementTypeParabolicTria)elementEnum;
                    else if (elementEnum is FeElementTypeLinearQuad && _linearQuadType != FeElementTypeLinearQuad.None)
                        _linearQuadType = (FeElementTypeLinearQuad)elementEnum;
                    else if (elementEnum is FeElementTypeParabolicQuad && _parabolicQuadType != FeElementTypeParabolicQuad.None)
                        _parabolicQuadType = (FeElementTypeParabolicQuad)elementEnum;
                    //
                    else if (elementEnum is FeElementTypeLinearTetra && _linearTetraType != FeElementTypeLinearTetra.None)
                        _linearTetraType = (FeElementTypeLinearTetra)elementEnum;
                    else if (elementEnum is FeElementTypeParabolicTetra && _parabolicTetraType != FeElementTypeParabolicTetra.None)
                        _parabolicTetraType = (FeElementTypeParabolicTetra)elementEnum;
                    else if (elementEnum is FeElementTypeLinearWedge && _linearWedgeType != FeElementTypeLinearWedge.None)
                        _linearWedgeType = (FeElementTypeLinearWedge)elementEnum;
                    else if (elementEnum is FeElementTypeParabolicWedge && _parabolicWedgeType != FeElementTypeParabolicWedge.None)
                        _parabolicWedgeType = (FeElementTypeParabolicWedge)elementEnum;
                    else if (elementEnum is FeElementTypeLinearHexa && _linearHexaType != FeElementTypeLinearHexa.None)
                        _linearHexaType = (FeElementTypeLinearHexa)elementEnum;
                    else if (elementEnum is FeElementTypeParabolicHexa && _parabolicHexaType != FeElementTypeParabolicHexa.None)
                        _parabolicHexaType = (FeElementTypeParabolicHexa)elementEnum;
                }
            }
        }
        public void UpdateElementTypeEnums(Dictionary<Type, HashSet<Enum>> elementTypeEnums)
        {
            if (elementTypeEnums != null)
            {
                HashSet<Enum> elementEnums;
                //
                if (_linearTriaType != FeElementTypeLinearTria.None &&
                    elementTypeEnums.TryGetValue(typeof(FeElementTypeLinearTria), out elementEnums) &&
                    !elementEnums.Contains(_linearTriaType)) _linearTriaType = (FeElementTypeLinearTria)elementEnums.First();
                if (_parabolicTriaType != FeElementTypeParabolicTria.None &&
                    elementTypeEnums.TryGetValue(typeof(FeElementTypeParabolicTria), out elementEnums) &&
                    !elementEnums.Contains(_parabolicTriaType)) _parabolicTriaType = (FeElementTypeParabolicTria)elementEnums.First();
                if (_linearQuadType != FeElementTypeLinearQuad.None &&
                    elementTypeEnums.TryGetValue(typeof(FeElementTypeLinearQuad), out elementEnums) &&
                    !elementEnums.Contains(_linearQuadType)) _linearQuadType = (FeElementTypeLinearQuad)elementEnums.First();
                if (_parabolicQuadType != FeElementTypeParabolicQuad.None &&
                    elementTypeEnums.TryGetValue(typeof(FeElementTypeParabolicQuad), out elementEnums) &&
                    !elementEnums.Contains(_parabolicQuadType)) _parabolicQuadType = (FeElementTypeParabolicQuad)elementEnums.First();
                //
                if (_linearTetraType != FeElementTypeLinearTetra.None &&
                    elementTypeEnums.TryGetValue(typeof(FeElementTypeLinearTetra), out elementEnums) &&
                    !elementEnums.Contains(_linearTetraType)) _linearTetraType = (FeElementTypeLinearTetra)elementEnums.First();
                if (_parabolicTetraType != FeElementTypeParabolicTetra.None &&
                    elementTypeEnums.TryGetValue(typeof(FeElementTypeParabolicTetra), out elementEnums) &&
                    !elementEnums.Contains(_parabolicTetraType)) _parabolicTetraType = (FeElementTypeParabolicTetra)elementEnums.First();
                if (_linearWedgeType != FeElementTypeLinearWedge.None &&
                    elementTypeEnums.TryGetValue(typeof(FeElementTypeLinearWedge), out elementEnums) &&
                    !elementEnums.Contains(_linearWedgeType)) _linearWedgeType = (FeElementTypeLinearWedge)elementEnums.First();
                if (_parabolicWedgeType != FeElementTypeParabolicWedge.None &&
                    elementTypeEnums.TryGetValue(typeof(FeElementTypeParabolicWedge), out elementEnums) &&
                    !elementEnums.Contains(_parabolicWedgeType)) _parabolicWedgeType = (FeElementTypeParabolicWedge)elementEnums.First();
                if (_linearHexaType != FeElementTypeLinearHexa.None &&
                    elementTypeEnums.TryGetValue(typeof(FeElementTypeLinearHexa), out elementEnums) &&
                    !elementEnums.Contains(_linearHexaType)) _linearHexaType = (FeElementTypeLinearHexa)elementEnums.First();
                if (_parabolicHexaType != FeElementTypeParabolicHexa.None &&
                    elementTypeEnums.TryGetValue(typeof(FeElementTypeParabolicHexa), out elementEnums) &&
                    !elementEnums.Contains(_parabolicHexaType)) _parabolicHexaType = (FeElementTypeParabolicHexa)elementEnums.First();
            }
        }
        public string GetElementType(FeElement element)
        {
            if (element is LinearTriangleElement) return _linearTriaType.ToString();
            else if (element is ParabolicTriangleElement) return _parabolicTriaType.ToString();
            else if (element is LinearQuadrilateralElement) return _linearQuadType.ToString();
            else if (element is ParabolicQuadrilateralElement) return _parabolicQuadType.ToString();
            //
            else if (element is LinearTetraElement) return _linearTetraType.ToString();
            else if (element is ParabolicTetraElement) return _parabolicTetraType.ToString();
            else if (element is LinearWedgeElement) return _linearWedgeType.ToString();
            else if (element is ParabolicWedgeElement) return _parabolicWedgeType.ToString();
            else if (element is LinearHexaElement) return _linearHexaType.ToString();
            else if (element is ParabolicHexaElement) return _parabolicHexaType.ToString();
            else throw new NotSupportedException();
        }





    }
}

