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
    public class InitialVelocity : InitialCondition, IPreviewable
    {
        // Variables                                                                                                                
        private double _v1;
        private double _v2;
        private double _v3;
        private string _nodeSetName;    // a temp variable for CalculiX output


        // Properties                                                                                                               
        public double V1 { get { return _v1; } set { _v1 = value; } }
        public double V2 { get { return _v2; } set { _v2 = value; } }
        public double V3 { get { return _v3; } set { _v3 = value; if (_twoD) _v3 = 0; } }
        public double GetComponent(int direction)
        {
            if (direction == 0) return V1;
            else if (direction == 1) return V2;
            else return V3;
        }
        public string NodeSetName { get { return _nodeSetName; } set { _nodeSetName = value; } }


        // Constructors                                                                                                             
        public InitialVelocity(string name, string regionName, RegionTypeEnum regionType,
                               double v1, double v2, double v3, bool twoD)
            : base(name, regionName, regionType, twoD)
        {
            _v1 = v1;
            _v2 = v2;
            V3 = v3;    // account for 2D
            _nodeSetName = null;
        }


        // Methods                                                                                                                  
        public FeResults GetPreview(FeMesh targetMesh, string resultName, UnitSystemType unitSystemType)
        {
            PartExchangeData allData = new PartExchangeData();
            targetMesh.GetAllNodesAndCells(out allData.Nodes.Ids, out allData.Nodes.Coor, out allData.Cells.Ids,
                                           out allData.Cells.CellNodeIds, out allData.Cells.Types);
            //
            HashSet<int> nodeIds;
            if (RegionType == RegionTypeEnum.PartName)
            {
                nodeIds = new HashSet<int>(targetMesh.Parts[RegionName].NodeLabels);
            }
            else if (RegionType == RegionTypeEnum.ElementSetName)
            {
                FeNodeSet nodeSet = targetMesh.GetNodeSetFromPartOrElementSetName(RegionName, false);
                nodeIds = new HashSet<int>(nodeSet.Labels);
            }
            else throw new NotSupportedException();
            //
            float[] values1 = new float[allData.Nodes.Coor.Length];
            float[] values2 = new float[allData.Nodes.Coor.Length];
            float[] values3 = new float[allData.Nodes.Coor.Length];
            float[] valuesAll = new float[allData.Nodes.Coor.Length];
            //
            for (int i = 0; i < allData.Nodes.Coor.Length; i++)
            {
                if (nodeIds.Contains(allData.Nodes.Ids[i]))
                {
                    values1[i] = (float)_v1;
                    values2[i] = (float)_v2;
                    values3[i] = (float)_v3;
                    valuesAll[i] = (float)Math.Sqrt(_v1 * _v1 + _v2 * _v2 + _v3 * _v3);
                }
                else
                {
                    values1[i] = float.NaN;
                    values2[i] = float.NaN;
                    values3[i] = float.NaN;
                    valuesAll[i] = float.NaN;
                }
            }
            //
            Dictionary<int, int> nodeIdsLookUp = new Dictionary<int, int>();
            for (int i = 0; i < allData.Nodes.Coor.Length; i++) nodeIdsLookUp.Add(allData.Nodes.Ids[i], i);
            FeResults results = new FeResults(resultName);
            results.SetMesh(targetMesh, nodeIdsLookUp);
            // Add group
            FieldData fieldData = new FieldData(FOFieldNames.Velo);
            fieldData.GlobalIncrementId = 1;
            fieldData.Type = StepType.Static;
            fieldData.Time = 1;
            fieldData.MethodId = 1;
            fieldData.StepId = 1;
            fieldData.StepIncrementId = 1;
            // Add values
            Field field = new Field(fieldData.Name);
            field.AddComponent(FOComponentNames.All, valuesAll);
            field.AddComponent(FOComponentNames.V1, values1);
            field.AddComponent(FOComponentNames.V2, values2);
            field.AddComponent(FOComponentNames.V3, values3);
            results.AddFiled(fieldData, field);
            // Unit system
            results.UnitSystem = new UnitSystem(unitSystemType);
            //
            return results;
        }
    }
}
