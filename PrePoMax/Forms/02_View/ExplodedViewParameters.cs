using DynamicTypeDescriptor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrePoMax
{
    [Serializable]
    public enum ExplodedViewDirectionEnum
    {
        XYZ = 0,
        X = 1,
        Y = 2,
        Z = 3,
        XY = 4,
        XZ = 5,
        YZ = 6
    }
    //
    [Serializable]
    public enum ExplodedViewMethodEnum
    {
        [StandardValue("Default", "Default exploded view method.")]
        Default = 0,
        [StandardValue("Center point", "Center point exploded view method.")]
        CenterPoint = 1
    }
    //
    [Serializable]
    public class ExplodedViewParameters
    {
        // Variables                                                                                                                
        private ExplodedViewMethodEnum _explodedViewMethod;
        private double[] _center;
        private ExplodedViewDirectionEnum _explodedViewDirection;
        private double _scaleFactor;
        private double _magnification;


        // Properties                                                                                                               
        public ExplodedViewMethodEnum Method
        {
            get { return _explodedViewMethod; }
            set { _explodedViewMethod = value; }
        }
        public double[] Center { get { return _center; } set { _center = value; } }
        public double CenterX { get { return _center[0]; } set { _center[0] = value; } }
        public double CenterY { get { return _center[1]; } set { _center[1] = value; } }
        public double CenterZ { get { return _center[2]; } set { _center[2] = value; } }
        public ExplodedViewDirectionEnum Direction
        {
            get { return _explodedViewDirection; }
            set { _explodedViewDirection = value; }
        }
        public double ScaleFactor { get { return _scaleFactor; } set { _scaleFactor = value; } }
        public double Magnification { get { return _magnification; } set { _magnification = value; } }
        

        // Constructors                                                                                                             
        public ExplodedViewParameters()
        {
            Reset();
        }


        // Methods                                                                                                                  
        public void Reset()
        {
            _explodedViewMethod = ExplodedViewMethodEnum.Default;
            _center = new double[3];
            _explodedViewDirection = ExplodedViewDirectionEnum.XYZ;
            _scaleFactor = -1;
            _magnification = 2;
        }
    }
}
