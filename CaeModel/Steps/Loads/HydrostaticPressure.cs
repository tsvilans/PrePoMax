using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;
using CaeResults;

namespace CaeModel
{
    [Serializable]
    public class HydrostaticPressure : VariablePressure, IPreviewable
    {
        // Variables                                                                                                                
        private double[] _firstPointCoor;
        private double[] _secondPointCoor;
        private double _n1; // in coordinate form to compute the normal if value changes
        private double _n2; // in coordinate form to compute the normal if value changes
        private double _n3; // in coordinate form to compute the normal if value changes
        private double[] _nDirection;
        private double _firstPointPressure;
        private double _secondPointPressure;


        // Properties                                                                                                               
        public double[] FirstPointCoor
        {
            get { return _firstPointCoor; }
            set
            {
                _firstPointCoor = value;
                if (_twoD) _firstPointCoor[2] = 0;
            }
        }
        public double[] SecondPointCoor
        {
            get { return _secondPointCoor; }
            set
            {
                _secondPointCoor = value;
                if (_twoD) _secondPointCoor[2] = 0;
            }
        }
        public double N1
        {
            get { return _n1; }
            set
            {
                _n1 = value;
                ComputeNormalFromDirection();
            }
        }
        public double N2
        {
            get { return _n2; }
            set
            {
                _n2 = value;
                ComputeNormalFromDirection();
            }
        }
        public double N3
        {
            get { return _n3; }
            set
            {
                _n3 = value;
                if (_twoD) _n3 = 0;
                ComputeNormalFromDirection();
            }
        }
        public double FirstPointPressure { get { return _firstPointPressure; } set { _firstPointPressure = value; } }
        public double SecondPointPressure { get { return _secondPointPressure; } set { _secondPointPressure = value; } }


        // Constructors                                                                                                             
        public HydrostaticPressure(string name, string regionName, RegionTypeEnum regionType, bool twoD)
            : this(name, regionName, regionType, new double[] { 0, 0, 0 }, new double[] { 0, 0, 0 }, 0, 0, 0, 0, 0, twoD)
        {
        }
        public HydrostaticPressure(string name, string regionName, RegionTypeEnum regionType, double[] firstPointCoor,
                                   double[] secondPointCoor, double nx, double ny, double nz, double firstPointPressure,
                                   double secondPointPressure, bool twoD)
            : base(name, regionName, regionType, twoD)
        {
            FirstPointCoor = firstPointCoor;           // account for 2D
            SecondPointCoor = secondPointCoor;         // account for 2D
            _n1 = nx;
            _n2 = ny;
            N3 = nz;                                   // account for 2D
            //
            _firstPointPressure = firstPointPressure;
            _secondPointPressure = secondPointPressure;
        }


        // Methods                                                                                                                  
        private void ComputeNormalFromDirection()
        {
            Vec3D n = new Vec3D(_n1, _n2, _n3);
            n.Normalize();
            _nDirection = n.Coor;
        }
        public bool IsProperlyDefined(out string error)
        {
            error = "";
            Vec3D p1 = new Vec3D(_firstPointCoor);
            Vec3D p2 = new Vec3D(_secondPointCoor);
            Vec3D n = new Vec3D(_nDirection);
            //
            if (n.Normalize() > 0)
            {
                double d1 = -Vec3D.DotProduct(n, p1);   // d1: distance of the plane through p1 from origin
                double d2 = -Vec3D.DotProduct(n, p2);   // d2: distance of the plane through p2 from origin
                //
                if (Math.Abs(d1 - d2) < 1E-6)
                {
                    error = "The first and the second point must not be on a plane perpendicular to the pressure direction.";
                    return false;
                }
            }
            else
            {
                error = "The pressure direction is not properly defined.";
                return false;
            }
            //
            return true;
        }
        public FeResults GetPreview(FeMesh targetMesh, string resultName, UnitSystemType unitSystemType)
        {
            //
            PartExchangeData allData = new PartExchangeData();
            targetMesh.GetAllNodesAndCells(out allData.Nodes.Ids, out allData.Nodes.Coor, out allData.Cells.Ids,
                                           out allData.Cells.CellNodeIds, out allData.Cells.Types);
            //
            FeSurface surface = targetMesh.Surfaces[_surfaceName];
            FeNodeSet nodeSet = targetMesh.NodeSets[surface.NodeSetName];
            HashSet<int> nodeIds = new HashSet<int>(nodeSet.Labels);
            //
            float[] values = new float[allData.Nodes.Coor.Length];
            //
            for (int i = 0; i < values.Length; i++)
            {
                if (nodeIds.Contains(allData.Nodes.Ids[i]))
                {
                    values[i] = (float)GetPressureForPoint(allData.Nodes.Coor[i]);
                }
                else
                {
                    values[i] = float.NaN;
                }
            }
            //
            Dictionary<int, int> nodeIdsLookUp = new Dictionary<int, int>();
            for (int i = 0; i < allData.Nodes.Coor.Length; i++) nodeIdsLookUp.Add(allData.Nodes.Ids[i], i);
            FeResults results = new FeResults(resultName);
            results.SetMesh(targetMesh, nodeIdsLookUp);
            // Add distances
            FieldData fieldData = new FieldData(FOFieldNames.Imported);
            fieldData.GlobalIncrementId = 1;
            fieldData.Type = StepType.Static;
            fieldData.Time = 1;
            fieldData.MethodId = 1;
            fieldData.StepId = 1;
            fieldData.StepIncrementId = 1;
            // Add values
            Field field = new Field(fieldData.Name);
            field.AddComponent(FOComponentNames.PRESS, values);
            results.AddFiled(fieldData, field);
            // Unit system
            results.UnitSystem = new UnitSystem(unitSystemType);
            //
            return results;
        }
        public override double GetPressureForPoint(double[] point)
        {
            double d1 = -(_firstPointCoor[0] * _nDirection[0] + _firstPointCoor[1] * _nDirection[1] +
                _firstPointCoor[2] * _nDirection[2]);
            double d2 = -(_secondPointCoor[0] * _nDirection[0] + _secondPointCoor[1] * _nDirection[1] +
                _secondPointCoor[2] * _nDirection[2]);
            double d = -(point[0] * _nDirection[0] + point[1] * _nDirection[1] + point[2] * _nDirection[2]);
            //
            double p = _firstPointPressure + (_secondPointPressure - _firstPointPressure) / (d2 - d1) * (d - d1);
            //
            return p;
        }
    }
}
