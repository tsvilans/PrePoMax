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
    public class DLoad : Load, IPreviewable
    {
        // Variables                                                                                                                
        private string _surfaceName;
        private RegionTypeEnum _regionType;
        private double _magnitude;


        // Properties                                                                                                               
        public override string RegionName { get { return _surfaceName; } set { _surfaceName = value; } }
        public override RegionTypeEnum RegionType { get { return _regionType; } set { _regionType = value; } }
        public string SurfaceName { get { return _surfaceName; } set { _surfaceName = value; } }
        public double Magnitude { get { return _magnitude; } set { _magnitude = value; } }


        // Constructors                                                                                                             
        public DLoad(string name, string surfaceName, RegionTypeEnum regionType, double magnitude, bool twoD)
            : base(name, twoD)
        {
            _surfaceName = surfaceName;
            _regionType = regionType;
            _magnitude = magnitude;
        }


        // Methods                                                                                                                  
        public FeResults GetPreview(FeMesh targetMesh, string resultName, UnitSystemType unitSystemType)
        {
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
                if (nodeIds.Contains(allData.Nodes.Ids[i])) values[i] = (float)_magnitude;
                else values[i] = float.NaN;
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
    }
}
