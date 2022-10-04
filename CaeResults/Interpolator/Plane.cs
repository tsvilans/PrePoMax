using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;

namespace CaeResults
{
    public struct Plane
    {
        // Variables                                                                                                                
        public Vec3D Point;
        public Vec3D Direction;


        // Constructors                                                                                                             
        public Plane(Vec3D point, Vec3D direction)
        {
            Point = point;
            Direction = direction;
        }


        // Methods                                                                                                                  
        public bool IsAbove(Vec3D q)
        { 
            return Vec3D.DotProduct(Direction, q - Point) > 0;
        }
        public Vec3D Project(Vec3D pointToProject)
        {
            Vec3D v = pointToProject - Point;
            Direction.Normalize();
            Vec3D d = Direction * Vec3D.DotProduct(v, Direction);
            Vec3D projectedPoint = pointToProject - d;
            return projectedPoint;
        }
    }
}
