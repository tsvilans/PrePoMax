using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeMesh
{
    [Serializable]
    public enum FeFaceName
    {
        Empty,
        S1,
        S2,
        S3,
        S4,
        S5,
        S6
    }


    [Serializable]
    public abstract class FeElement
    {
        // Properties                                                                                                               
        public int Id;
        public int PartId;
        public int[] NodeIds;
        

        public FeElement(int id, int[] nodeIds)
            : this(id, -1, nodeIds)
        {
        }
        public FeElement(int id, int partId, int[] nodeIds)
        {
            Id = id;
            PartId = partId;
            NodeIds = nodeIds;
        }
       

        // Methods                                                                                                                  
        public static bool IsParabolic(FeElement element)
        {
            return element is ParabolicBeamElement ||
                   element is ParabolicTriangleElement ||
                   element is ParabolicQuadrilateralElement ||
                   element is ParabolicTetraElement ||
                   element is ParabolicWedgeElement ||
                   element is ParabolicHexaElement; 
        }
        public static bool IsParabolic(Type elementType)
        {
            return elementType == typeof(ParabolicBeamElement) ||
                   elementType == typeof(ParabolicTriangleElement) ||
                   elementType == typeof(ParabolicQuadrilateralElement) ||
                   elementType == typeof(ParabolicTetraElement) ||
                   elementType == typeof(ParabolicWedgeElement) ||
                   elementType == typeof(ParabolicHexaElement);
        }
        //
        public static int VtkCellIdFromFaceName(FeFaceName faceName)
        {
            return (int)faceName - 1;
        }
        public static FeFaceName FaceNameFromVtkCellId(int vtkCellId)
        {
            return (FeFaceName)(vtkCellId + 1);
        }
        //
        public double[] GetCG(Dictionary<int, FeNode> nodes)
        {
            FeNode node;
            double[] cg = new double[3];
            //
            for (int i = 0; i < NodeIds.Length; i++)
            {
                node = nodes[NodeIds[i]];
                cg[0] += node.X;
                cg[1] += node.Y;
                cg[2] += node.Z;
            }
            //
            cg[0] /= NodeIds.Length;
            cg[1] /= NodeIds.Length;
            cg[2] /= NodeIds.Length;
            //
            return cg;
        }

        // Abstract methods                                                                                                         
        abstract public int[] GetVtkNodeIds();
        abstract public int GetVtkCellType();
        abstract public FeFaceName GetFaceNameFromSortedNodeIds(int[] nodeIds);
        abstract public int[] GetNodeIdsFromFaceName(FeFaceName faceName);
        abstract public int[] GetVtkCellFromFaceName(FeFaceName faceName);
        abstract public Dictionary<FeFaceName, double> GetFaceNamesAndAreasFromNodeSet(HashSet<int> nodeSet,
                                                                                       Dictionary<int, FeNode> nodes,
                                                                                       bool edgeFaces);
        abstract public double[] GetEquivalentForcesFromFaceName(FeFaceName faceName);
        abstract public double[] GetEquivalentForcesFromFaceName(FeFaceName faceName, double[] nodalValues);
        protected double[] GetEquivalentForces(Type elementType, double[] nodalValues)
        {
            double[] n = nodalValues;
            if (elementType == typeof(LinearBeamElement))
            {
                double a = 1.0 / 6.0;
                return new double[] { a * (2 * n[0] + n[1]),
                                      a * (n[0] + 2 * n[1])};
            }
            else if (elementType == typeof(ParabolicBeamElement))
            {
                double a = 1.0 / 30.0;
                double b = 1.0 / 15.0;
                return new double[] { a * (4 * n[0] - n[1] + 2 * n[2]),
                                      a * (-n[0] + 4 * n[1] + 2 * n[2]),
                                      b * (n[0] + n[1] + 8 * n[2])};
            }
            else if (elementType == typeof(LinearTriangleElement))
            {
                double a = 1.0 / 12.0;
                return new double[] { a * (2 * n[0] + n[1] + n[2]),
                                      a * (n[0] + 2 * n[1] + n[2]),
                                      a * (n[0] + n[1] + 2 * n[2])};
            }
            else if (elementType == typeof(ParabolicTriangleElement))
            {
                double a = 1.0 / 180;
                double b = 1.0 / 45;
                return new double[] { a * (6 * n[0] - n[1] - n[2] - 4 * n[4]),
                                      a * (- n[0] + 6 * n[1] - n[2] - 4 * n[5]),
                                      a * (- n[0] - n[1] + 6 * n[2] - 4 * n[3]),
                                      b * (- n[2] + 8 * n[3] + 4 * n[4] + 4 * n[5]),
                                      b * (- n[0] + 4 * n[3] + 8 * n[4] + 4 * n[5]),
                                      b * (- n[1] + 4 * n[3] + 4 * n[4] + 8 * n[5])};
            }
            else if (elementType == typeof(LinearQuadrilateralElement))
            {
                double a = 1.0 / 36.0;
                return new double[] { a * (4 * n[0] + 2 * n[1] + 1 * n[2] + 2 * n[3]),
                                      a * (2 * n[0] + 4 * n[1] + 2 * n[2] + 1 * n[3]),
                                      a * (1 * n[0] + 2 * n[1] + 4 * n[2] + 2 * n[3]),
                                      a * (2 * n[0] + 1 * n[1] + 2 * n[2] + 4 * n[3])};
            }
            else if (elementType == typeof(ParabolicQuadrilateralElement))
            {
                double a = 1.0 / 180;
                double b = 1.0 / 90;
                return new double[] {
                    a * (6 * n[0] + 2 * n[1] + 3 * n[2] + 2 * n[3] - 6 * n[4] - 8 * n[5] - 8 * n[6] - 6 * n[7]),
                    a * (2 * n[0] + 6 * n[1] + 2 * n[2] + 3 * n[3] - 6 * n[4] - 6 * n[5] - 8 * n[6] - 8 * n[7]),
                    a * (3 * n[0] + 2 * n[1] + 6 * n[2] + 2 * n[3] - 8 * n[4] - 6 * n[5] - 6 * n[6] - 8 * n[7]),
                    a * (2 * n[0] + 3 * n[1] + 2 * n[2] + 6 * n[3] - 8 * n[4] - 8 * n[5] - 6 * n[6] - 6 * n[7]),
                    b * (-3 * n[0] -3 * n[1] -4 * n[2] -4 * n[3] + 16 * n[4] + 10 * n[5] +  8 * n[6] + 10 * n[7]),
                    b * (-4 * n[0] -3 * n[1] -3 * n[2] -4 * n[3] + 10 * n[4] + 16 * n[5] + 10 * n[6] +  8 * n[7]),
                    b * (-4 * n[0] -4 * n[1] -3 * n[2] -3 * n[3] +  8 * n[4] + 10 * n[5] + 16 * n[6] + 10 * n[7]),
                    b * (-3 * n[0] -4 * n[1] -4 * n[2] -3 * n[3] + 10 * n[4] +  8 * n[5] + 10 * n[6] + 16 * n[7])};
            }
            else if (elementType == typeof(LinearTetraElement))
            {
                double a = 1.0 / 120.0;
                return new double[] { a * (2 * n[0] + 1 * n[1] + 1 * n[2] + 1 * n[3]),
                                      a * (1 * n[0] + 2 * n[1] + 1 * n[2] + 1 * n[3]),
                                      a * (1 * n[0] + 1 * n[1] + 2 * n[2] + 1 * n[3]),
                                      a * (1 * n[0] + 1 * n[1] + 1 * n[2] + 2 * n[3])};
            }
            else if (elementType == typeof(ParabolicTetraElement))
            {
                double a = 1.0 / 2520;
                double b = 1.0 / 1260;
                return new double[] {
                    -(6*n[8]+4*n[7]+4*n[6]+6*n[5]+4*n[4]-n[3]-n[2]-n[1]+6*n[9]-6*n[0])*a,
                    -(4*n[8]+6*n[7]+6*n[6]+4*n[5]+4*n[4]-n[3]-n[2]-6*n[1]+6*n[9]-n[0])*a,
                    -(6*n[8]+6*n[7]+4*n[6]+4*n[5]+6*n[4]-n[3]-6*n[2]-n[1]+4*n[9]-n[0])*a,
                    -(4*n[8]+4*n[7]+6*n[6]+6*n[5]+6*n[4]-6*n[3]-n[2]-n[1]+4*n[9]-n[0])*a,
                    (8*n[8]+8*n[7]+8*n[6]+8*n[5]+16*n[4]-3*n[3]-3*n[2]-2*n[1]+4*n[9]-2*n[0])*b,
                    (8*n[8]+4*n[7]+8*n[6]+16*n[5]+8*n[4]-3*n[3]-2*n[2]-2*n[1]+8*n[9]-3*n[0])*b,
                    (4*n[8]+8*n[7]+16*n[6]+8*n[5]+8*n[4]-3*n[3]-2*n[2]-3*n[1]+8*n[9]-2*n[0])*b,
                    (8*n[8]+16*n[7]+8*n[6]+4*n[5]+8*n[4]-2*n[3]-3*n[2]-3*n[1]+8*n[9]-2*n[0])*b,
                    (16*n[8]+8*n[7]+4*n[6]+8*n[5]+8*n[4]-2*n[3]-3*n[2]-2*n[1]+8*n[9]-3*n[0])*b,
                    (8*n[8]+8*n[7]+8*n[6]+8*n[5]+4*n[4]-2*n[3]-2*n[2]-3*n[1]+16*n[9]-3*n[0])*b};
            }
            else throw new NotSupportedException();
        }
        abstract public double GetArea(FeFaceName faceName, Dictionary<int, FeNode> nodes);
        abstract public double[] GetFaceCG(FeFaceName faceName, Dictionary<int, FeNode> nodes, out double area);
        abstract public FeElement DeepCopy();
    }
}
