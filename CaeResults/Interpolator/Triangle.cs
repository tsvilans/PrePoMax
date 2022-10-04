using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;

namespace CaeResults
{
    //
    // https://stackoverflow.com/questions/2924795/fastest-way-to-compute-point-to-triangle-distance-in-3d
    //
    public class Triangle
    {
        // Variables                                                                                                                
        public readonly int Id;
        public readonly Edge3 EdgeAb;
        public readonly Edge3 EdgeBc;
        public readonly Edge3 EdgeCa;
        public readonly Vec3D TriNorm;
        private Plane _triPlane;
        private Plane _planeAb;
        private Plane _planeBc;
        private Plane _planeCa;
        private double _va;
        private double _vb;
        private double _vc;


        // Properties                                                                                                               
        public Vec3D A { get { return EdgeAb.A; } }
        public Vec3D B { get { return  EdgeBc.A; } }
        public Vec3D C { get { return  EdgeCa.A; } }
        public Plane TriPlane { get { return _triPlane; } }
        public Plane PlaneAb { get { return _planeAb; } }
        public Plane PlaneBc { get { return _planeBc; } }
        public Plane PlaneCa { get { return _planeCa; } }


        // Constructors                                                                                                             
        public Triangle(int id, double[] a, double[] b, double[] c, double va, double vb, double vc)
            : this(id, new Vec3D(a), new Vec3D(b), new Vec3D(c), va, vb, vc)
        {
        }
        public Triangle(int id, Vec3D a, Vec3D b, Vec3D c, double va, double vb, double vc)
        {
            Id = id;
            EdgeAb = new Edge3(a, b);
            EdgeBc = new Edge3(b, c);
            EdgeCa = new Edge3(c, a);
            TriNorm = Vec3D.CrossProduct(a - b, a - c);
            // Values
            _va = va;
            _vb = vb;
            _vc = vc;
            // Triangle plane
            _triPlane = new Plane(A, TriNorm);
            _triPlane.Direction.Normalize();
            // Edge planes
            _planeAb = new Plane(EdgeAb.A, Vec3D.CrossProduct(TriNorm, EdgeAb.Delta));
            _planeBc = new Plane(EdgeBc.A, Vec3D.CrossProduct(TriNorm, EdgeBc.Delta));
            _planeCa = new Plane(EdgeCa.A, Vec3D.CrossProduct(TriNorm, EdgeCa.Delta));
        }


        // Methods                                                                                                                  
       
        public Vec3D ClosestPointTo(Vec3D p)
        {
            // Find the projection of the point onto the edge
            var uab = EdgeAb.Project(p);
            var uca = EdgeCa.Project(p);
            //
            if (uca > 1 && uab < 0) return A;
            //
            var ubc = EdgeBc.Project(p);
            //
            if (uab > 1 && ubc < 0) return B;
            //
            if (ubc > 1 && uca < 0) return C;
            //
            if (0 <= uab && uab <= 1 && !_planeAb.IsAbove(p)) return EdgeAb.PointAt(uab);
            //
            if (0 <= ubc && ubc <= 1 && !_planeBc.IsAbove(p)) return EdgeBc.PointAt(ubc);
            //
            if (0 <= uca && uca <= 1 && !_planeCa.IsAbove(p)) return EdgeCa.PointAt(uca);
            // The closest point is in the triangle so project to the plane to find it
            return _triPlane.Project(p);
        }
        public bool GetClosestPointTo(Vec3D p, double len2limit, out Vec3D closestPoint)
        {
            closestPoint = null;
            Vec3D v = p - _triPlane.Point;
            Vec3D d = _triPlane.Direction * Vec3D.DotProduct(v, _triPlane.Direction);
            double d2 = d.Len2;
            //
            if (d2 >= len2limit) return false;
            else
            {
                // Find the projection of the point onto the edge
                var uab = EdgeAb.Project(p);
                var uca = EdgeCa.Project(p);
                //
                if (uca > 1 && uab < 0)
                {
                    closestPoint = A;
                    return true;
                }
                //
                var ubc = EdgeBc.Project(p);
                //
                if (uab > 1 && ubc < 0)
                {
                    closestPoint = B;
                    return true;
                }
                //
                if (ubc > 1 && uca < 0)
                {
                    closestPoint = C;
                    return true;
                }
                //
                if (0 <= uab && uab <= 1 && !_planeAb.IsAbove(p))
                {
                    closestPoint = EdgeAb.PointAt(uab);
                    return true;
                }
                //
                if (0 <= ubc && ubc <= 1 && !_planeBc.IsAbove(p))
                {
                    closestPoint = EdgeBc.PointAt(ubc);
                    return true;
                }
                //
                if (0 <= uca && uca <= 1 && !_planeCa.IsAbove(p))
                {
                    closestPoint = EdgeCa.PointAt(uca);
                    return true;
                }
                // The closest point is in the triangle so project to the plane to find it
                closestPoint = p - d;
            }
            //
            return true;
        }
        public bool GetClosestNodeTo(Vec3D p, double len2limit, out Vec3D closestPoint)
        {
            closestPoint = null;
            double da = (A - p).Len2;
            double db = (B - p).Len2;
            double dc = (C - p).Len2;
            //
            if (da <= db && da <= dc)
            {
                if (da < len2limit)
                {
                    closestPoint = A;
                    return true;
                }
            }
            else if (db <= da && db <= dc)
            {
                if (db < len2limit)
                {
                    closestPoint = B;
                    return true;
                }
            }
            else
            {
                if (dc < len2limit)
                {
                    closestPoint = C;
                    return true;
                }
            }
            //
            return false;
        }
        public double InterpolateAt(Vec3D p)
        {
            if (p.X == A.X && p.Y == A.Y && p.Z == A.Z) return _va;
            else if (p.X == B.X && p.Y == B.Y && p.Z == B.Z) return _vb;
            else if (p.X == C.X && p.Y == C.Y && p.Z == C.Z) return _vc;
            //
            double g;
            double h;
            double nx = Math.Abs(TriNorm.X);
            double ny = Math.Abs(TriNorm.Y);
            double nz = Math.Abs(TriNorm.Z);
            //
            if (nx >= ny && nx >= nz)
            {
                g = -(A.Z * (p.Y - C.Y) + C.Z * (A.Y - p.Y) + p.Z * (C.Y - A.Y)) /
                     (A.Z * (C.Y - B.Y) + B.Z * (A.Y - C.Y) + C.Z * (B.Y - A.Y));
                h = (A.Z * (p.Y - B.Y) + B.Z * (A.Y - p.Y) + p.Z * (B.Y - A.Y)) /
                    (A.Z * (C.Y - B.Y) + B.Z * (A.Y - C.Y) + C.Z * (B.Y - A.Y));
            }
            else if (ny >= nx && ny >= nz)
            {
                g = -(A.Z * (p.X - C.X) + C.Z * (A.X - p.X) + p.Z * (C.X - A.X)) /
                     (A.Z * (C.X - B.X) + B.Z * (A.X - C.X) + C.Z * (B.X - A.X));
                h = (A.Z * (p.X - B.X) + B.Z * (A.X - p.X) + p.Z * (B.X - A.X)) /
                    (A.Z * (C.X - B.X) + B.Z * (A.X - C.X) + C.Z * (B.X - A.X));
            }
            else
            {
                g = -(A.X * (p.Y - C.Y) + C.X * (A.Y - p.Y) + p.X * (C.Y - A.Y)) /
                     (A.X * (C.Y - B.Y) + B.X * (A.Y - C.Y) + C.X * (B.Y - A.Y));
                h = (A.X * (p.Y - B.Y) + B.X * (A.Y - p.Y) + p.X * (B.Y - A.Y)) /
                    (A.X * (C.Y - B.Y) + B.X * (A.Y - C.Y) + C.X * (B.Y - A.Y));
            }
            //
            return (1 - g - h) *_va + g * _vb + h * _vc;
        }
    }
}
