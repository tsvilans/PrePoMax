using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;

namespace CaeResults
{
    public struct Edge3
    {
        // Variables                                                                                                                
        public readonly Vec3D A;
        public readonly Vec3D B;
        public readonly Vec3D Delta;


        // Properties                                                                                                               
        public double LengthSquared { get { return Delta.Len2; } }


        // Constructors                                                                                                             
        public Edge3(Vec3D a, Vec3D b)
        {
            A = a;
            B = b;
            Delta = b - a;
        }
        public Edge3(Edge3 edge)
        {
            A = edge.A.DeepCopy();
            B = edge.B.DeepCopy();
            Delta = edge.Delta.DeepCopy();
        }


        // Methods                                                                                                                  
        public Vec3D PointAt(double t)
        {
            return A + t * Delta;
        }
        public double Project(Vec3D p)
        {
            return Vec3D.DotProduct(p - A, Delta) / LengthSquared;
        }
        public Edge3 DeepCopy()
        {
            return new Edge3(this);
        }

    }
}
