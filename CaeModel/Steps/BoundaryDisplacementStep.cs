using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace CaeModel
{
    [Serializable]
    public class BoundaryDisplacementStep : Step, ISerializable
    {
        // Variables                                                                                                                


        // Properties                                                                                                               


        // Constructors                                                                                                             
        public BoundaryDisplacementStep(string name)                          // must be a separate constructor!
            : this(name, true)
        {
        }
        public BoundaryDisplacementStep(string name, bool addFieldOutputs)    // must be a separate constructor!
            : base(name)
        {
        }

        //ISerializable
        public BoundaryDisplacementStep(SerializationInfo info, StreamingContext context)
            :base(info, context)
        {            
        }


        // Methods                                                                                                                  
        public override bool IsBoundaryConditionSupported(BoundaryCondition boundaryCondition)
        {
            if (boundaryCondition is FixedBC ||
                boundaryCondition is DisplacementRotation)
                return true;
            else if (boundaryCondition is SubmodelBC || boundaryCondition is TemperatureBC)
                return false;
            else throw new NotSupportedException();
        }
        public override bool IsLoadTypeSupported(Type loadType)
        {
            if (loadType == typeof(CLoad) ||
                loadType == typeof(MomentLoad) ||
                loadType == typeof(DLoad) ||
                loadType == typeof(HydrostaticPressure) ||
                loadType == typeof(ImportedPressure) ||
                loadType == typeof(STLoad) ||
                loadType == typeof(ShellEdgeLoad) ||
                loadType == typeof(GravityLoad) ||
                loadType == typeof(CentrifLoad) ||
                loadType == typeof(PreTensionLoad) ||
                loadType == typeof(CFlux) ||
                loadType == typeof(DFlux) ||
                loadType == typeof(BodyFlux) ||
                loadType == typeof(FilmHeatTransfer) ||
                loadType == typeof(RadiationHeatTransfer))
            {
                return false;
            }
            else throw new NotSupportedException();
        }
        public override bool IsDefinedFieldSupported(DefinedField definedField)
        {
            if (definedField is DefinedTemperature) return false;
            else throw new NotSupportedException();
        }
        // ISerialization
        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Using typeof() works also for null fields
            base.GetObjectData(info, context);
        }
    }
}
