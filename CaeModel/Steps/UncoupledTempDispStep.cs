using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace CaeModel
{
    [Serializable]
    public class UncoupledTempDispStep : HeatTransferStep, ISerializable
    {
        // Variables                                                                                                                


        // Properties                                                                                                               


        // Constructors                                                                                                             
        public UncoupledTempDispStep(string name)
            :base(name, false)
        {
            AddFieldOutput(new NodalFieldOutput("NF-Output-1", NodalFieldVariable.U | NodalFieldVariable.RF |
                                                NodalFieldVariable.NT | NodalFieldVariable.RFL));
            AddFieldOutput(new ElementFieldOutput("EF-Output-1", ElementFieldVariable.E | ElementFieldVariable.S | 
                                                  ElementFieldVariable.HFL));
        }
        //ISerializable
        public UncoupledTempDispStep(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    default:
                        break;
                }
            }
        }


        // Methods                                                                                                                  
        public override bool IsBoundaryConditionSupported(BoundaryCondition boundaryCondition)
        {
            if (boundaryCondition is FixedBC ||
                boundaryCondition is DisplacementRotation ||
                boundaryCondition is SubmodelBC ||
                boundaryCondition is TemperatureBC)
                return true;
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
                return true;
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
            base.GetObjectData(info, context);
        }
    }
}
