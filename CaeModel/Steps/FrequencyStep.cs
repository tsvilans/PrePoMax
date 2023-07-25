using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace CaeModel
{
    [Serializable]
    public class FrequencyStep : Step, ISerializable
    {
        // Variables                                                                                                                
        private int _numOfFrequencies;          //ISerializable
        private double _lowerFrequency;         //ISerializable
        private double _upperFrequency;         //ISerializable
        private bool _storage;                  //ISerializable


        // Properties                                                                                                               
        public int NumOfFrequencies
        {
            get { return _numOfFrequencies; }
            set 
            {
                if (value <= 0) throw new Exception("The number of frequencies must be larger than 0.");
                _numOfFrequencies = value;
            }
        }
        public double LowerFrequency
        {
            get { return _lowerFrequency; }
            set
            {
                if (!double.IsNaN(value))
                {
                    if (value < 0) throw new Exception("The lower frequency value must be larger or equal to 0.");
                    if (!double.IsNaN(_upperFrequency) && value >= _upperFrequency)
                        throw new Exception("The lower frequency value must be smaller than the upper frequency value.");
                }
                _lowerFrequency = value;
            }
        }
        public double UpperFrequency
        {
            get { return _upperFrequency; }
            set
            {
                if (!double.IsNaN(value))
                {
                    if (value < 0) throw new Exception("The upper frequency value must be larger or equal to 0.");
                    if (!double.IsNaN(_lowerFrequency) && value <= _lowerFrequency)
                        throw new Exception("The upper frequency value must be larger than the lower frequency value.");
                }
                _upperFrequency = value;
            }
        }
        public bool Storage { get { return _storage; } set { _storage = value; } }


        // Constructors                                                                                                             
        public FrequencyStep(string name)
            :base(name)
        {
            _numOfFrequencies = 10;
            _lowerFrequency = double.NaN;
            _upperFrequency = double.NaN;
            _storage = false;
            //
            AddFieldOutput(new NodalFieldOutput("NF-Output-1", NodalFieldVariable.U | NodalFieldVariable.RF));
            AddFieldOutput(new ElementFieldOutput("EF-Output-1", ElementFieldVariable.E | ElementFieldVariable.S));
        }
        //ISerializable
        public FrequencyStep(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_numOfFrequencies":
                        _numOfFrequencies = (int)entry.Value; break;
                    case "_lowerFrequency":
                        _lowerFrequency = (double)entry.Value; break;
                    case "_upperFrequency":
                        _upperFrequency = (double)entry.Value; break;
                    case "_storage":
                        _storage = (bool)entry.Value; break;
                }
            }
        }


        // Methods                                                                                                                  
        public override bool IsBoundaryConditionSupported(BoundaryCondition boundaryCondition)
        {
            if (boundaryCondition is FixedBC || 
                boundaryCondition is DisplacementRotation)
                return true;
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
            //
            info.AddValue("_numOfFrequencies", _numOfFrequencies, typeof(int));
            info.AddValue("_lowerFrequency", _lowerFrequency, typeof(double));
            info.AddValue("_upperFrequency", _upperFrequency, typeof(double));
            info.AddValue("_storage", _storage, typeof(bool));
        }
    }
}
