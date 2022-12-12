using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kitware.VTK;
using CaeGlobals;
using System.Runtime.InteropServices;

namespace vtkControl
{
    [Serializable]
    public class vtkMaxLocator
    {
        // Variables                                                                                                                
        private vtkGenericCell _cell;
        private vtkMaxActor _actor;
        

        // Properties                                                                                                               


        // Constructors                                                                                                             
        public vtkMaxLocator(PartExchangeData source)
        {
            vtkMaxActorData actorData = new vtkMaxActorData();
            actorData.Geometry = source;
            actorData.Pickable = true;
            //
            _cell = vtkGenericCell.New();
            _actor = new vtkMaxActor(actorData);
        }


        // Methods                                                                                                                  
        public void GetClosestPoint(double[] point, out int cellId, out double[] pointOut)
        {
            IntPtr x = Marshal.AllocHGlobal(3 * 8);
            Marshal.Copy(point, 0, x, 3);
            //
            IntPtr closestPoint = Marshal.AllocHGlobal(3 * 8);
            //
            long localCellId = -1;
            int subId = -1;
            double distance2 = -1;
            pointOut = new double[3];
            //
            _actor.CellLocator.FindClosestPoint(x, closestPoint, _cell, ref localCellId, ref subId, ref distance2);
            //
            cellId = (int)localCellId;
            Marshal.Copy(closestPoint, pointOut, 0, 3);
        }

    }
}
