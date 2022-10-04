using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace CaeModel
{
    [Serializable]
    public class StaticStep : Step, ISerializable
    {
        // Variables                                                                                                                
        private double _timePeriod;                 //ISerializable
        private double _initialTimeIncrement;       //ISerializable
        private double _minTimeIncrement;           //ISerializable
        private double _maxTimeIncrement;           //ISerializable
        private bool _direct;                       //ISerializable


        // Properties                                                                                                               
        public double TimePeriod 
        {
            get { return _timePeriod; }
            set 
            {
                if (value <= 0) throw new Exception("The time period value must be positive.");
                _timePeriod = value;
            }
        }
        public double InitialTimeIncrement
        {
            get { return _initialTimeIncrement; }
            set
            {
                if (value <= 0) throw new Exception("The initial time increment value must be positive.");
                _initialTimeIncrement = value;
            }
        }
        public double MinTimeIncrement
        {
            get { return _minTimeIncrement; }
            set
            {
                if (value <= 0) throw new Exception("The min time increment value must be positive.");
                _minTimeIncrement = value;
            }
        }
        public double MaxTimeIncrement
        {
            get { return _maxTimeIncrement; }
            set
            {
                if (value <= 0) throw new Exception("The max time increment value must be positive.");
                _maxTimeIncrement = value;
                // Initial time increment must not be larger than max time increment
                if (_initialTimeIncrement > _maxTimeIncrement) _initialTimeIncrement = _maxTimeIncrement;
            }
        }
        public bool Direct { get { return _direct; } set { _direct = value; } }


        // Constructors                                                                                                             
        public StaticStep(string name)                          // must be a separate constructor!
            : this(name, true)
        {
        }
        public StaticStep(string name, bool addFieldOutputs)    // must be a separate constructor!
            : base(name)
        {
            _timePeriod = 1;
            _initialTimeIncrement = 1;
            _minTimeIncrement = 1E-5;
            _maxTimeIncrement = 1E30;
            _direct = false;
            //
            if (addFieldOutputs)
            {
                AddFieldOutput(new NodalFieldOutput("NF-Output-1", NodalFieldVariable.U | NodalFieldVariable.RF));
                AddFieldOutput(new ElementFieldOutput("EF-Output-1", ElementFieldVariable.E | ElementFieldVariable.S));
            }
        }

        //ISerializable
        public StaticStep(SerializationInfo info, StreamingContext context)
            :base(info, context)
        {            
            // Compatibility for version v.0.5.3
            _direct = false;
            //
            int count = 0;
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_timePeriod":
                        _timePeriod = (double)entry.Value; count++; break;
                    case "_initialTimeIncrement":
                        _initialTimeIncrement = (double)entry.Value; count++; break;
                    case "_minTimeIncrement":
                        _minTimeIncrement = (double)entry.Value; count++; break;
                    case "_maxTimeIncrement":
                        _maxTimeIncrement = (double)entry.Value; count++; break;
                    case "_direct":
                        _direct = (bool)entry.Value; break;
                }
            }
            if (count != 4) throw new NotSupportedException();
        }


        // Methods                                                                                                                  
        public override bool IsBoundaryConditionSupported(BoundaryCondition boundaryCondition)
        {
            if (boundaryCondition is FixedBC ||
                boundaryCondition is DisplacementRotation ||
                boundaryCondition is SubmodelBC)
                return true;
            else if (boundaryCondition is TemperatureBC)
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
            if (definedField is DefinedTemperature) return true;
            else throw new NotSupportedException();
        }
        // ISerialization
        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Using typeof() works also for null fields
            base.GetObjectData(info, context);
            //
            info.AddValue("_timePeriod", _timePeriod, typeof(double));
            info.AddValue("_initialTimeIncrement", _initialTimeIncrement, typeof(double));
            info.AddValue("_minTimeIncrement", _minTimeIncrement, typeof(double));
            info.AddValue("_maxTimeIncrement", _maxTimeIncrement, typeof(double));
            info.AddValue("_direct", _direct, typeof(bool));
        }
    }
}
