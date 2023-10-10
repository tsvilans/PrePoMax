using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeMesh
{
    [Serializable]
    public struct FeMaterialOrientation
    {
        // Variables                                                                                                                
        public int Id;
        public double XX;
        public double XY;
        public double XZ;
        public double YX;
        public double YY;
        public double YZ;

        // Constructors

        public FeMaterialOrientation(int id, double xx, double xy, double xz, double yx, double yy, double yz)
        {
            Id = id;
            XX = xx;
            XY = xy;
            XZ = xz;
            YX = yx;
            YY = yy;
            YZ = yz;
        }
        public FeMaterialOrientation(int id, double[] xaxis, double[] yaxis)
            : this(id, xaxis[0], xaxis[1], xaxis[2], yaxis[0], yaxis[1], yaxis[2])
        {
        }
        public FeMaterialOrientation(FeMaterialOrientation orientation)
            : this(orientation.Id, orientation.XX, orientation.XY, orientation.XZ, orientation.YX, orientation.YY, orientation.YZ)
        {
        }

        public static FeMaterialOrientation Default
        {
            get
            {
                return new FeMaterialOrientation(-1, new double[] { 1, 0, 0 }, new double[] { 0, 1, 0 });
            }
        }

        // Methods
        public void SetX(double x, double y, double z)
        {
            XX = x;
            XY = y;
            XZ = z;
        }
        public void SetY(double x, double y, double z)
        {
            YX = x;
            YY = y;
            YZ = z;
        }
        public double[] XAxis
        {
            get
            {
                return new double[] { XX, XY, XZ };
            }
            set
            {
                XX = value[0];
                XY = value[1];
                XZ = value[2];
            }
        }
        public double[] YAxis
        {
            get
            {
                return new double[] { YX, YY, YZ };
            }
            set
            {
                YX = value[0];
                YY = value[1];
                YZ = value[2];
            }
        }
        public bool IsEqual(FeMaterialOrientation orientation)
        {
            int div = 10000;
            // the <= sign solves the problem when a coordinate equals 0
            if (Id == orientation.Id && Math.Abs(XX - orientation.XX) <= Math.Abs(XX / div) && Math.Abs(XY - orientation.XY) <= Math.Abs(XY / div) && Math.Abs(XZ - orientation.XZ) <= Math.Abs(XZ / div)
                 && Math.Abs(YX - orientation.YX) <= Math.Abs(YX / div) && Math.Abs(YY - orientation.YY) <= Math.Abs(YY / div) && Math.Abs(YZ - orientation.YZ) <= Math.Abs(YZ / div))
                return true;
            else
                return false;
        }
        public FeMaterialOrientation DeepCopy()
        {
            return new FeMaterialOrientation(Id, XX, XY, XZ, YX, YY, YZ);
        }
    }
}
