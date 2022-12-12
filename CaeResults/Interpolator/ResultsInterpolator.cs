using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;
using DynamicTypeDescriptor;

namespace CaeResults
{
    public enum InterpolatorEnum
    {
        [StandardValue("ClosestNode", DisplayName = "Closest node")]
        ClosestNode,
        [StandardValue("ClosestPoint", DisplayName = "Closest point")]
        ClosestPoint
    }
    public class ResultsInterpolator
    {
        // Variables                                                                                                                
        private int _nx;
        private int _ny;
        private int _nz;
        private int _nxy;
        private double _deltaX;
        private double _deltaY;
        private double _deltaZ;
        private BoundingBox _sourceBox;
        private BoundingBox[] _cellBoxes;
        private BoundingBox[] _regionBoxes;     // Tag of each bounding box contains dictionary<triangleId, boundingBox>
        private Triangle[] _triangles;
        private Octree.BoundsOctree<Triangle> _trianglesOctree;
        private vtkControl.vtkMaxLocator _locator;

        // Constructor                                                                                                              
        public ResultsInterpolator(PartExchangeData source)
        {
            int numBoxes = 10000000;
            _sourceBox = ComputeAllNodesBoundingBox(source);
            double l = _sourceBox.GetDiagonal() / 128;            
            //
            _nx = (int)Math.Ceiling(_sourceBox.GetXSize() / l);
            _ny = (int)Math.Ceiling(_sourceBox.GetYSize() / l);
            _nz = (int)Math.Ceiling(_sourceBox.GetZSize() / l);
            int currNumBoxes = _nx * _ny * _nz;
            double factor = Math.Pow(numBoxes / currNumBoxes, 0.333333);
            l /= factor;
            //
            _nx = (int)Math.Ceiling(_sourceBox.GetXSize() / l);
            _ny = (int)Math.Ceiling(_sourceBox.GetYSize() / l);
            _nz = (int)Math.Ceiling(_sourceBox.GetZSize() / l);
            _nxy = _nx * _ny;
            _deltaX = _sourceBox.GetXSize() / _nx;
            _deltaY = _sourceBox.GetYSize() / _ny;
            _deltaZ = _sourceBox.GetZSize() / _nz;
            //
            _cellBoxes = ComputeCellBoundingBoxes(source);
            _regionBoxes = SplitCellBoxesToRegions(_cellBoxes, _sourceBox, _nx, _ny, _nz);
            _triangles = TriangularCellsToTriangles(source);
            //
            //_locator = new vtkControl.vtkMaxLocator(source);
            //
            //_trianglesOctree = GenerateOctree(source, _cellBoxes, _sourceBox);
            //
            //int max = 0;
            //int num = 0;
            //List<int> nums = new List<int>();
            //double average = 0;
            //foreach (var boundingBox in _regionBoxes)
            //{
            //    num = ((Dictionary<int, BoundingBox>)boundingBox.Tag).Count;
            //    nums.Add(num);
            //    if (num > max) max = num;
            //    average += num;
            //}
            //average /= _regionBoxes.Length;
            //nums.Sort();
        }
        public void InterpolateAt(double[] point, InterpolatorEnum interpolator, out double[] distance, out double value)
        {
            int i;
            int j;
            int k;
            int mini;
            int maxi;
            int minj;
            int maxj;
            int mink;
            int maxk;
            double[] sourceCoor;
            int index;
            BoundingBox bb;
            Dictionary<int, BoundingBox> regions = new Dictionary<int, BoundingBox>();
            int num;
            int delta;
            double d;
            double minD;
            Vec3D sourcePoint;
            Vec3D closestPoint;
            Vec3D bestPoint = new Vec3D();
            Triangle triangle;
            Triangle bestTriangle = null;
            bool closer;
            //
            sourceCoor = point;
            sourcePoint = new Vec3D(sourceCoor);
            i = (int)Math.Floor((sourceCoor[0] - _sourceBox.MinX) / _deltaX);
            j = (int)Math.Floor((sourceCoor[1] - _sourceBox.MinY) / _deltaY);
            k = (int)Math.Floor((sourceCoor[2] - _sourceBox.MinZ) / _deltaZ);
            if (i < 0) i = 0;
            else if (i >= _nx) i = _nx - 1;
            if (j < 0) j = 0;
            else if (j >= _ny) j = _ny - 1;
            if (k < 0) k = 0;
            else if (k >= _nz) k = _nz - 1;
            index = k * _nxy + j * _nx + i;
            bb = _regionBoxes[index];
            if (bb != null) regions.Add(index, bb);
            //
            //mini = i;
            //maxi = i;
            //minj = j;
            //maxj = j;
            //mink = k;
            //maxk = k;
            delta = 0;
            num = bb == null ? 0 : ((Dictionary<int, BoundingBox>)bb.Tag).Count;
            // Add next layer of regions
            while (num == 0 || delta < 1)
            {
                delta++;
                mini = i - delta;
                maxi = i + delta;
                minj = j - delta;
                maxj = j + delta;
                mink = k - delta;
                maxk = k + delta;
                if (mini < 0) mini = 0;
                if (maxi >= _nx) maxi = _nx - 1;
                if (minj < 0) minj = 0;
                if (maxj >= _ny) maxj = _ny - 1;
                if (mink < 0) mink = 0;
                if (maxk >= _nz) maxk = _nz - 1;
                //
                for (int kk = mink; kk <= maxk; kk++)
                {
                    for (int jj = minj; jj <= maxj; jj++)
                    {
                        for (int ii = mini; ii <= maxi; ii++)
                        {
                            index = kk * _nxy + jj * _nx + ii;
                            if (!regions.ContainsKey(index))
                            {
                                bb = _regionBoxes[index];
                                if (bb != null && ((Dictionary<int, BoundingBox>)bb.Tag).Count > 0)
                                {
                                    regions.Add(index, bb);
                                    num += ((Dictionary<int, BoundingBox>)bb.Tag).Count;
                                }
                            }
                        }
                    }
                }
            }
            //
            minD = double.MaxValue;
            //
            foreach (var regionEntry in regions)
            {
                if (regionEntry.Value.IsMaxOutsideDistance2SmallerThan(sourceCoor, minD))
                {
                    foreach (var entry in (Dictionary<int, BoundingBox>)regionEntry.Value.Tag)
                    {
                        //if (entry.Value == null) continue;  // empty boxes are null
                        //
                        triangle = _triangles[entry.Key];
                        if (entry.Value.IsMaxOutsideDistance2SmallerThan(sourceCoor, minD))
                        {
                            if (interpolator == InterpolatorEnum.ClosestNode)
                                closer = triangle.GetClosestNodeTo(sourcePoint, minD, out closestPoint);
                            else if (interpolator == InterpolatorEnum.ClosestPoint)
                                closer = triangle.GetClosestPointTo(sourcePoint, minD, out closestPoint);
                            else throw new NotSupportedException();
                            //
                            if (closer)
                            {
                                d = (closestPoint - sourcePoint).Len2;
                                //
                                if (d < minD)
                                {
                                    minD = d;
                                    bestTriangle = triangle;
                                    bestPoint.X = closestPoint.X;
                                    bestPoint.Y = closestPoint.Y;
                                    bestPoint.Z = closestPoint.Z;
                                }
                            }
                        }
                    }
                }
            }
            //
            //for (int kk = mink; kk <= maxk; kk++)
            //{
            //    for (int jj = minj; jj <= maxj; jj++)
            //    {
            //        for (int ii = mini; ii <= maxi; ii++)
            //        {
            //            bb = _regionBoxes[kk * _nxy + jj * _nx + ii];
            //            //
            //            foreach (var entry in (Dictionary<int, BoundingBox>)bb.Tag)
            //            {
            //                triangle = _triangles[entry.Key];
            //                if (entry.Value.IsMaxOutsideDistance2SmallerThan(sourceCoor, minD))
            //                {
            //                    if (interpolator == InterpolatorEnum.ClosestNode)
            //                        closer = triangle.GetClosestNodeTo(sourcePoint, minD, out closestPoint);
            //                    else if (interpolator == InterpolatorEnum.ClosestPoint)
            //                        closer = triangle.GetClosestPointTo(sourcePoint, minD, out closestPoint);
            //                    else throw new NotSupportedException();
            //                    //
            //                    if (closer)
            //                    {
            //                        d = (closestPoint - sourcePoint).Len2;
            //                        //
            //                        if (d < minD)
            //                        {
            //                            minD = d;
            //                            bestTriangle = triangle;
            //                            bestPoint.X = closestPoint.X;
            //                            bestPoint.Y = closestPoint.Y;
            //                            bestPoint.Z = closestPoint.Z;
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
            //
            distance = (bestPoint - sourcePoint).Coor;
            value = bestTriangle.InterpolateAt(bestPoint);
        }
        public void InterpolateAt1(double[] point, InterpolatorEnum interpolator, out double[] distance, out double value)
        {
            int cellId;
            double[] pointOut;
            _locator.GetClosestPoint(point, out cellId, out pointOut);
            //
            if (cellId != -1)
            {
                Vec3D sourcePoint = new Vec3D(point);
                Vec3D bestPoint = new Vec3D(pointOut);
                Triangle triangle = _triangles[cellId];
                //
                distance = (bestPoint - sourcePoint).Coor;
                value = triangle.InterpolateAt(bestPoint);
            }
            else
            {
                distance = new double[] { double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity };
                value = double.PositiveInfinity;
            }
        }
        public void InterpolateAt2(double[] point, InterpolatorEnum interpolator, out double[] distance, out double value)
        {
            double size = _sourceBox.GetDiagonal() * 1E-3;
            Octree.Point centerP = new Octree.Point(point);
            Octree.Point sizeP = new Octree.Point(size, size, size);
            Octree.BoundingBox tbb = new Octree.BoundingBox(centerP, sizeP);
            List<Triangle> colliding = new List<Triangle>();
            _trianglesOctree.GetColliding(colliding, tbb);
            //
            bool closer;
            double d;
            double minD = double.MaxValue;
            Triangle bestTriangle = colliding[0];
            Vec3D sourcePoint = new Vec3D(point);
            Vec3D closestPoint;
            Vec3D bestPoint = new Vec3D();
            //
            foreach (var triangle in colliding)
            {
                if (_cellBoxes[triangle.Id].IsMaxOutsideDistance2SmallerThan(point, minD))
                {
                    if (interpolator == InterpolatorEnum.ClosestNode)
                        closer = triangle.GetClosestNodeTo(sourcePoint, minD, out closestPoint);
                    else if (interpolator == InterpolatorEnum.ClosestPoint)
                        closer = triangle.GetClosestPointTo(sourcePoint, minD, out closestPoint);
                    else throw new NotSupportedException();
                    //
                    if (closer)
                    {
                        d = (closestPoint - sourcePoint).Len2;
                        //
                        if (d < minD)
                        {
                            minD = d;
                            bestTriangle = triangle;
                            bestPoint.X = closestPoint.X;
                            bestPoint.Y = closestPoint.Y;
                            bestPoint.Z = closestPoint.Z;
                        }
                    }
                }
            }
            //
            distance = (bestPoint - sourcePoint).Coor;
            value = bestTriangle.InterpolateAt(bestPoint);
        }
        //
        private static BoundingBox ComputeAllNodesBoundingBox(PartExchangeData pData)
        {
            BoundingBox bb = new BoundingBox();
            bb.IncludeFirstCoor(pData.Nodes.Coor[0]);
            for (int i = 0; i < pData.Nodes.Coor.Length; i++) bb.IncludeCoorFast(pData.Nodes.Coor[i]);
            return bb;
        }
        private static BoundingBox[] ComputeCellBoundingBoxes(PartExchangeData pData)
        {
            int[] cell;
            BoundingBox bb;
            BoundingBox[] bBoxes = new BoundingBox[pData.Cells.Ids.Length];
            //
            for (int i = 0; i < pData.Cells.CellNodeIds.Length; i++)
            {
                cell = pData.Cells.CellNodeIds[i];
                bb = new BoundingBox();
                bb.IncludeFirstCoor(pData.Nodes.Coor[cell[0]]);
                bb.IncludeCoorFast(pData.Nodes.Coor[cell[1]]);
                bb.IncludeCoorFast(pData.Nodes.Coor[cell[2]]);
                if (cell.Length == 4 || cell.Length == 8) bb.IncludeCoorFast(pData.Nodes.Coor[cell[3]]);
                //
                bBoxes[i] = bb;
            }
            //
            return bBoxes;
        }
        private static BoundingBox[] SplitCellBoxesToRegions(BoundingBox[] cellBoxes, BoundingBox cellBoxesBox, 
                                                             int nx, int ny, int nz)
        {
            int nxy = nx * ny;
            double deltaX = cellBoxesBox.GetXSize() / nx;
            double deltaY = cellBoxesBox.GetYSize() / ny;
            double deltaZ = cellBoxesBox.GetZSize() / nz;
            //
            BoundingBox bb;
            BoundingBox[] regions = new BoundingBox[nxy * nz];
            //for (int k = 0; k < nz; k++)
            //{
            //    for (int j = 0; j < ny; j++)
            //    {
            //        for (int i = 0; i < nx; i++)
            //        {
            //            bb = new BoundingBox();
            //            bb.MinX = cellBoxesBox.MinX + i * deltaX;
            //            bb.MaxX = bb.MinX + deltaX;
            //            bb.MinY = cellBoxesBox.MinY + j * deltaY;
            //            bb.MaxY = bb.MinY + deltaY;
            //            bb.MinZ = cellBoxesBox.MinZ + k * deltaZ;
            //            bb.MaxZ = bb.MinZ + deltaZ;
            //            bb.Tag = new Dictionary<int, BoundingBox>();
            //            regions[k * nxy + j * nx + i] = bb;
            //        }
            //    }
            //}
            //
            int count = 0;
            int mini;
            int maxi;
            int minj;
            int maxj;
            int mink;
            int maxk;
            // If cell box max value is on the border of the region division, the cell will be a member of both space regions
            foreach (var cellBox in cellBoxes)
            {
                mini = (int)Math.Floor((cellBox.MinX - cellBoxesBox.MinX) / deltaX);
                maxi = (int)Math.Floor((cellBox.MaxX - cellBoxesBox.MinX) / deltaX);
                if (maxi == nx) maxi--;
                //
                minj = (int)Math.Floor((cellBox.MinY - cellBoxesBox.MinY) / deltaY);
                maxj = (int)Math.Floor((cellBox.MaxY - cellBoxesBox.MinY) / deltaY);
                if (maxj == ny) maxj--;
                //
                mink = (int)Math.Floor((cellBox.MinZ - cellBoxesBox.MinZ) / deltaZ);
                maxk = (int)Math.Floor((cellBox.MaxZ - cellBoxesBox.MinZ) / deltaZ);
                if (maxk == nz) maxk--;
                //
                for (int k = mink; k <= maxk; k++)
                {
                    for (int j = minj; j <= maxj; j++)
                    {
                        for (int i = mini; i <= maxi; i++)
                        {
                            bb = regions[k * nxy + j * nx + i];
                            if (bb == null)
                            {
                                bb = new BoundingBox();
                                bb.MinX = cellBoxesBox.MinX + i * deltaX;
                                bb.MaxX = bb.MinX + deltaX;
                                bb.MinY = cellBoxesBox.MinY + j * deltaY;
                                bb.MaxY = bb.MinY + deltaY;
                                bb.MinZ = cellBoxesBox.MinZ + k * deltaZ;
                                bb.MaxZ = bb.MinZ + deltaZ;
                                bb.Tag = new Dictionary<int, BoundingBox>();
                                regions[k * nxy + j * nx + i] = bb;
                            }
                            ((Dictionary<int, BoundingBox>)bb.Tag).Add(count, cellBox);
                        }
                    }
                }
                //
                count++;
            }
            //
            return regions;
        }
        private static Triangle[] TriangularCellsToTriangles(PartExchangeData pData)
        {
            Triangle[] triangles = new Triangle[pData.Cells.CellNodeIds.Length];
            //
            Parallel.For(0, triangles.Length, i =>
            //for (int i = 0; i < triangles.Length; i++)
            {
                int[] cell = pData.Cells.CellNodeIds[i];
                if (cell.Length != 3) throw new NotSupportedException();
                triangles[i] = new Triangle(i, pData.Nodes.Coor[cell[0]],
                                               pData.Nodes.Coor[cell[1]],
                                               pData.Nodes.Coor[cell[2]],
                                               pData.Nodes.Values[cell[0]],
                                               pData.Nodes.Values[cell[1]],
                                               pData.Nodes.Values[cell[2]]);
            }
            );
            //
            return triangles;
        }
        private static Octree.BoundsOctree<Triangle> GenerateOctree(PartExchangeData pData, BoundingBox[] cellBoxes,
                                                                    BoundingBox cellBoxesBox)
        {
            double size = cellBoxesBox.GetDiagonal();
            double minSize = size * 1E-3;
            Octree.Point centerP = new Octree.Point(cellBoxesBox.GetCenter());
            Octree.BoundsOctree<Triangle> trianglesOctree = new Octree.BoundsOctree<Triangle>(size, centerP, minSize, 1);
            //
            int[] cell;
            Octree.Point sizeP;
            Triangle triangle;
            Octree.BoundingBox tbb;
            for (int i = 0; i < pData.Cells.CellNodeIds.Length; i++)
            {
                cell = pData.Cells.CellNodeIds[i];
                if (cell.Length != 3) throw new NotSupportedException();
                triangle = new Triangle(i, pData.Nodes.Coor[cell[0]],
                                        pData.Nodes.Coor[cell[1]],
                                        pData.Nodes.Coor[cell[2]],
                                        pData.Nodes.Values[cell[0]],
                                        pData.Nodes.Values[cell[1]],
                                        pData.Nodes.Values[cell[2]]);
                //
                centerP = new Octree.Point(cellBoxes[i].GetCenter());
                sizeP = new Octree.Point(cellBoxes[i].GetXSize(), cellBoxes[i].GetYSize(), cellBoxes[i].GetZSize());
                tbb = new Octree.BoundingBox(centerP, sizeP);
                trianglesOctree.Add(triangle, tbb);
            }
            //
            return trianglesOctree;
        }
        //
        public static void ClosestPointToShouldWork()
        {
            var r = new Random(0);
            double next() => r.NextDouble() * 5 - 1;
            var t = new Triangle(0, new Vec3D(0, 0, 0), new Vec3D(3.5, 2, 0), new Vec3D(3, 0.0, 0), 0, 0, 0);
            //
            var hash = new Vec3D(0, 0, 0);
            for (int i = 0; i < 800; i++)
            {
                var pt = new Vec3D(next(), next(), 0);
                var pc = t.ClosestPointTo(pt);
                hash += pc;
            }
            // Test the hash
            // If it doesn't match then eyeball the visualization
            // and see what has gone wrong

            //hash.ShouldBeApproximately(new Vector3(1496.28118561104, 618.196568578824, 0), 1e-5);

        }
    }
}
