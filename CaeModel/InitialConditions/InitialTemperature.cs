using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using System.ComponentModel;
using CaeGlobals;
using CaeResults;

namespace CaeModel
{
    [Serializable]
    public class InitialTemperature : InitialCondition, IPreviewable
    {
        // Variables                                                                                                                
        private double _temperature;


        // Properties                                                                                                               
        public double Temperature { get { return _temperature; } set { _temperature = value; } }


        // Constructors                                                                                                             
        public InitialTemperature(string name, string regionName, RegionTypeEnum regionType)
            : base(name, regionName, regionType, false)
        {
            _temperature = 0;
        }


        // Methods                                                                                                                  
        public FeResults GetPreview(FeMesh targetMesh, string resultName, UnitSystemType unitSystemType)
        {
            PartExchangeData allData = new PartExchangeData();
            targetMesh.GetAllNodesAndCells(out allData.Nodes.Ids, out allData.Nodes.Coor, out allData.Cells.Ids,
                                           out allData.Cells.CellNodeIds, out allData.Cells.Types);
            //
            FeNodeSet nodeSet;
            if (RegionType == RegionTypeEnum.NodeSetName)
            {
                nodeSet = targetMesh.NodeSets[RegionName];
            }
            else if (RegionType == RegionTypeEnum.SurfaceName)
            {
                FeSurface surface = targetMesh.Surfaces[RegionName];
                nodeSet = targetMesh.NodeSets[surface.NodeSetName];
            }
            else throw new NotSupportedException();
            //
            HashSet<int> nodeIds = new HashSet<int>(nodeSet.Labels);
            //
            float[] values = new float[allData.Nodes.Coor.Length];
            //
            for (int i = 0; i < values.Length; i++)
            {
                if (nodeIds.Contains(allData.Nodes.Ids[i])) values[i] = (float)_temperature;
                else values[i] = float.NaN;
            }
            //
            Dictionary<int, int> nodeIdsLookUp = new Dictionary<int, int>();
            for (int i = 0; i < allData.Nodes.Coor.Length; i++) nodeIdsLookUp.Add(allData.Nodes.Ids[i], i);
            FeResults results = new FeResults(resultName);
            results.SetMesh(targetMesh, nodeIdsLookUp);
            // Add group
            FieldData fieldData = new FieldData(FOFieldNames.NdTemp);
            fieldData.GlobalIncrementId = 1;
            fieldData.Type = StepType.Static;
            fieldData.Time = 1;
            fieldData.MethodId = 1;
            fieldData.StepId = 1;
            fieldData.StepIncrementId = 1;
            // Add values
            Field field = new Field(fieldData.Name);
            field.AddComponent(FOComponentNames.T, values);
            results.AddFiled(fieldData, field);
            // Unit system
            results.UnitSystem = new UnitSystem(unitSystemType);
            //
            return results;
        }
    }
}
