using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace CaeModel
{
    [Serializable]
    public class HeatTransferStep : StaticStep, ISerializable
    {
        // Variables                                                                                                                
        private bool _steadyState;                  //ISerializable
        private double _deltmx;                     //ISerializable


        // Properties                                                                                                               
        public bool SteadyState { get { return _steadyState; } set { _steadyState = value; } }
        public double Deltmx { get { return _deltmx; } set { _deltmx = value; } }


        // Constructors                                                                                                             
        public HeatTransferStep(string name)
            :this(name, true)
        {
           
        }
        public HeatTransferStep(string name, bool addFieldOutputs)
            : base(name, false)
        {
            _steadyState = true;
            _deltmx = double.PositiveInfinity;
            //
            if (addFieldOutputs)
            {
                AddFieldOutput(new NodalFieldOutput("NF-Output-1", NodalFieldVariable.NT | NodalFieldVariable.RFL));
                AddFieldOutput(new ElementFieldOutput("EF-Output-1", ElementFieldVariable.HFL));
            }
        }
        //ISerializable
        public HeatTransferStep(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_steadyState":
                        _steadyState = (bool)entry.Value; break;
                    case "_deltmx":
                        _deltmx = (double)entry.Value; break;
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
                boundaryCondition is SubmodelBC)
                return false;
            else if (boundaryCondition is TemperatureBC)
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
                loadType == typeof(PreTensionLoad))
            {
                return false;
            }
            else if (loadType == typeof(CFlux) ||
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
            // Using typeof() works also for null fields
            base.GetObjectData(info, context);
            //
            info.AddValue("_steadyState", _steadyState, typeof(bool));
            info.AddValue("_deltmx", _deltmx, typeof(double));
        }
    }
}
