﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeMesh
{
    [Serializable]
    public class LinearBeamElement : FeElement1D
    {
        // Variables                                                                                                                
        private static int vtkCellTypeInt = (int)vtkCellType.VTK_LINE;


        // Properties                                                                                                               


        // Constructors                                                                                                             
        public LinearBeamElement(int id, int[] nodeIds)
            : base(id, nodeIds)
        {
        }
        public LinearBeamElement(int id, int partId, int[] nodeIds)
            : base(id, partId, nodeIds)
        {
        }


        // Methods                                                                                                                  
        public override int[] GetVtkNodeIds()
        {
            // return a copy -> ToArray
            return NodeIds.ToArray();
        }
        public override int GetVtkCellType()
        {
            return vtkCellTypeInt;
        }
        public override FeFaceName GetFaceNameFromSortedNodeIds(int[] nodeIds)
        {
            throw new NotImplementedException();
        }
        public override int[] GetNodeIdsFromFaceName(FeFaceName faceName)
        {
            throw new NotImplementedException();
        }
        public override int[] GetVtkCellFromFaceName(FeFaceName faceName)
        {
            throw new NotImplementedException();
        }
        public override Dictionary<FeFaceName, double> GetFaceNamesAndAreasFromNodeSet(HashSet<int> nodeSet,
                                                                                       Dictionary<int, FeNode> nodes,
                                                                                       bool edgeFaces)
        {
            throw new NotImplementedException();
        }
        public override double[] GetEquivalentForcesFromFaceName(FeFaceName faceName)
        {
            throw new NotImplementedException();
        }
        public override double GetArea(FeFaceName faceName, Dictionary<int, FeNode> nodes)
        {
            throw new NotImplementedException();
        }
        public override double[] GetCG(FeFaceName faceName, Dictionary<int, FeNode> nodes, out double area)
        {
            throw new NotImplementedException();
        }
        public override FeElement DeepCopy()
        {
            return new LinearBeamElement(Id, PartId, NodeIds.ToArray());
        }
    }
}
