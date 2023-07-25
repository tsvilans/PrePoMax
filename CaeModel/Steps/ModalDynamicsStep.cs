using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace CaeModel
{
    [Serializable]
    public class ModalDynamicsStep : StaticStep, ISerializable
    {
        // Variables                                                                                                                
        private bool _steadyState;          //ISerializable
        private ModalDamping _modalDamping; //ISerializable
        private double _relativeError;      //ISerializable


        // Properties                                                                                                               
        public bool SteadyState { get { return _steadyState; } set { _steadyState = value; } }
        public ModalDamping ModalDamping { get { return _modalDamping; } set { _modalDamping = value; } }
        public double RelativeError
        {
            get { return _relativeError; }
            set
            {
                _relativeError = value;
                if (_relativeError < 0) _relativeError = 1E-5;
                else if (_relativeError > 1) _relativeError = 1;
            }
        }



        // Constructors                                                                                                             
        public ModalDynamicsStep(string name)
            :base(name, true)
        {
            _steadyState = false;
            _modalDamping = new ModalDamping();
            _relativeError = 0.01;
            //
            _incrementationType = IncrementationTypeEnum.Automatic; // must be different from default to writhe max num increments
            InitialTimeIncrement = 0.1;
            TimePeriod = 1;
        }
        //ISerializable
        public ModalDynamicsStep(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_steadyState":
                        _steadyState = (bool)entry.Value; break;
                    case "_modalDamping":
                        _modalDamping = (ModalDamping)entry.Value; break;
                    case "_relativeError":
                        _relativeError = (double)entry.Value; break;
                }
            }
        }


        // Methods                                                                                                                  
        public override bool IsBoundaryConditionSupported(BoundaryCondition boundaryCondition)
        {
            if (boundaryCondition is FixedBC || 
                boundaryCondition is DisplacementRotation)
                return false;
            else if (boundaryCondition is SubmodelBC ||
                     boundaryCondition is TemperatureBC)
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
                loadType == typeof(PreTensionLoad))
            {
                return true;
            }
            else if (loadType == typeof(CFlux) ||
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
            //
            info.AddValue("_steadyState", _steadyState, typeof(bool));
            info.AddValue("_modalDamping", _modalDamping, typeof(ModalDamping));
            info.AddValue("_relativeError", _relativeError, typeof(double));
        }
    }
}

