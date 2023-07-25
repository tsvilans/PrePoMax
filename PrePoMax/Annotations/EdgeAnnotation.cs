using CaeGlobals;
using CaeMesh;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrePoMax
{
    [Serializable]
    public class EdgeAnnotation : AnnotationBase
    {
        // Variables                                                                                                                
        private int _geometryId;


        // Properties                                                                                                               
        public int GeometryId { get { return _geometryId; } set { _geometryId = value; } }


        // Constructors                                                                                                             
        public EdgeAnnotation(string name, int geometryId)
            : base(name)
        {
            _geometryId = geometryId;
            _partId = FeMesh.GetPartIdFromGeometryId(geometryId);
        }


        // Methods
        public override void GetAnnotationData(out string text, out double[] coor)
        {
            int[] itemTypePartIds = FeMesh.GetItemTypePartIdsFromGeometryId(_geometryId);
            FeMesh mesh = Controller.DisplayedMesh;
            int edgeId = itemTypePartIds[0];
            BasePart part = mesh.GetPartById(itemTypePartIds[2]);
            double length;
            string lenUnit = Controller.GetLengthUnit();
            string fieldUnit = "";
            string numberFormat = Controller.Settings.Annotations.GetNumberFormat();
            //
            FeNode n1;
            FeNode n2;
            bool results = false;
            float min = float.MaxValue;
            float max = -float.MaxValue;
            float sum = 0;
            float avg = 0;
            int[] nodeIds;
            double[] nodeWeights;
            //
            mesh.GetEdgeNodeCoor(_geometryId, out nodeIds, out double[][] nodeCoor);
            nodeWeights = new double[nodeIds.Length];
            //
            if (Controller.CurrentView == ViewGeometryModelResults.Geometry ||
                Controller.CurrentView == ViewGeometryModelResults.Model)
            {
                // Coor
                n2 = mesh.Nodes[nodeIds[nodeIds.Length / 2]];
                if (nodeIds.Length == 2)
                {
                    n1 = mesh.Nodes[nodeIds[0]];
                    coor = FeMesh.GetMidNodeCoor(n1, n2);
                }
                else coor = n2.Coor;
                // Length
                length = mesh.GetEdgeLength(_geometryId);
            }
            else if (Controller.CurrentView == ViewGeometryModelResults.Results)
            {
                // Coor
                n2 = Controller.GetScaledNode(Controller.GetScale(), nodeIds[nodeIds.Length / 2]);
                if (nodeIds.Length == 2)
                {
                    n1 = Controller.GetScaledNode(Controller.GetScale(), nodeIds[0]);
                    coor = FeMesh.GetMidNodeCoor(n1, n2);
                }
                else coor = n2.Coor;
                //
                Vec3D v1;
                Vec3D v2;
                float value;
                double segLen;
                length = 0;
                FeNode[] nodes = Controller.GetScaledNodes(1, nodeIds);
                // Length
                for (int i = 0; i < nodes.Length - 1; i++)
                {
                    v1 = new Vec3D(nodes[i].Coor);
                    v2 = new Vec3D(nodes[i + 1].Coor);
                    segLen = (v2 - v1).Len;
                    nodeWeights[i] += segLen * 0.5;
                    nodeWeights[i + 1] += segLen * 0.5;
                    length += segLen;
                }
                //
                if (Controller.ViewResultsType == ViewResultsTypeEnum.ColorContours)
                {
                    results = true;
                    // Values
                    for (int i = 0; i < nodes.Length; i++)
                    {
                        value = Controller.GetNodalValue(nodeIds[i]);
                        if (value < min) min = value;
                        if (value > max) max = value;
                        sum += value;
                        avg += (float)(value * nodeWeights[i]);
                    }
                    avg /= (float)length;
                    // Units
                    fieldUnit = Controller.GetCurrentResultsUnitAbbreviation();
                    if (fieldUnit == "/") fieldUnit = "";
                }
            }
            else throw new NotSupportedException();
            //
            bool addEdgeIdData = Controller.Settings.Annotations.ShowEdgeSurId;
            bool addEdgeLengthData = Controller.Settings.Annotations.ShowEdgeSurSize;
            bool addEdgeMaxData = Controller.Settings.Annotations.ShowEdgeSurMax && results;
            bool addEdgeMinData = Controller.Settings.Annotations.ShowEdgeSurMin && results;
            bool addEdgeSumData = Controller.Settings.Annotations.ShowEdgeSurSum && results;
            if (!addEdgeLengthData && !addEdgeMaxData && !addEdgeMinData && !addEdgeSumData) addEdgeIdData = true;
            text = "";
            if (addEdgeIdData)
            {
                text += string.Format("Edge id: {0} on {1}", edgeId + 1, part.Name);
            }
            if (addEdgeLengthData)
            {
                if (text.Length > 0) text += Environment.NewLine;
                text += string.Format("Edge length: {0} {1}", length.ToString(numberFormat), lenUnit);
            }
            if (addEdgeMaxData)
            {
                if (text.Length > 0) text += Environment.NewLine;
                text += string.Format("Max: {0} {1}", max.ToString(numberFormat), fieldUnit);
            }
            if (addEdgeMinData)
            {
                if (text.Length > 0) text += Environment.NewLine;
                text += string.Format("Min: {0} {1}", min.ToString(numberFormat), fieldUnit);
            }
            if (addEdgeSumData)
            {
                if (text.Length > 0) text += Environment.NewLine;
                text += string.Format("Nodal sum: {0} {1}", sum.ToString(numberFormat), fieldUnit);
                //if (text.Length > 0) text += Environment.NewLine;
                //text += string.Format("Avg: {0} {1}", avg.ToString(numberFormat), fieldUnit);
            }
            //
            if (IsTextOverriden) text = OverridenText;
        }
    }
}
