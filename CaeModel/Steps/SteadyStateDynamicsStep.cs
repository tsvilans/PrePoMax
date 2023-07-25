using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace CaeModel
{
    [Serializable]
    public class SteadyStateDynamicsStep : Step, ISerializable
    {
        // Variables                                                                                                                
        private bool _harmonic;             //ISerializable
        private double _lowerFrequency;     //ISerializable
        private double _upperFrequency;     //ISerializable
        private int _numDataPoints;         //ISerializable
        private double _bias;               //ISerializable
        private int _numFourierTerms;       //ISerializable
        private double _timeLower;          //ISerializable
        private double _timeUpper;          //ISerializable
        private ModalDamping _modalDamping; //ISerializable


        // Properties                                                                                                               
        public bool Harmonic { get { return _harmonic; } set { _harmonic = value; } }
        public double LowerFrequency
        {
            get { return _lowerFrequency; }
            set
            {
                if (value > _upperFrequency)
                    throw new CaeGlobals.CaeException("The lower frequency value must be smaller than the upper frequency value.");
                _lowerFrequency = value;
            }
        }
        public double UpperFrequency
        {
            get { return _upperFrequency; }
            set
            {
                if (value < _lowerFrequency)
                    throw new CaeGlobals.CaeException("The upper frequency value must be larger than the lower frequency value.");
                _upperFrequency = value;
            }
        }
        public int NumDataPoints
        {
            get { return _numDataPoints; }
            set
            {
                _numDataPoints = value;
                if (_numDataPoints != int.MinValue && _numDataPoints < 2) _numDataPoints = 2;
            }
        }
        public double Bias
        {
            get { return _bias; }
            set
            {
                _bias = value;
                if (_bias != double.NaN && _bias < 1) _bias = 1;
            }
        }
        public int NumFourierTerms
        {
            get { return _numFourierTerms; }
            set
            {
                _numFourierTerms = value;
                if (_numFourierTerms < 1) _numFourierTerms = 1;
            }
        }
        public double TimeLower { get { return _timeLower; } set { _timeLower = value; } }
        public double TimeUpper { get { return _timeUpper; } set { _timeUpper = value; } }
        public ModalDamping ModalDamping { get { return _modalDamping; } set { _modalDamping = value; } }


        // Constructors                                                                                                             
        public SteadyStateDynamicsStep(string name)
            :base(name)
        {
            _harmonic = true;
            _lowerFrequency = 0;
            _upperFrequency = 10;
            _numDataPoints = 20;
            _bias = 3;
            _numFourierTerms = 20;
            _timeLower = 0;
            _timeUpper = 1;
            _modalDamping = new ModalDamping();
            //
            AddFieldOutput(new NodalFieldOutput("NF-Output-1", NodalFieldVariable.U | NodalFieldVariable.RF));
            AddFieldOutput(new ElementFieldOutput("EF-Output-1", ElementFieldVariable.E | ElementFieldVariable.S));
        }
        //ISerializable
        public SteadyStateDynamicsStep(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_harmonic":
                        _harmonic = (bool)entry.Value; break;
                    case "_lowerFrequency":
                        _lowerFrequency = (double)entry.Value; break;
                    case "_upperFrequency":
                        _upperFrequency = (double)entry.Value; break;
                    case "_numDataPoints":
                        _numDataPoints = (int)entry.Value; break;
                    case "_bias":
                        _bias = (double)entry.Value; break;
                    case "_numFourierTerms":
                        _numFourierTerms = (int)entry.Value; break;
                    case "_timeLower":
                        _timeLower = (double)entry.Value; break;
                    case "_timeUpper":
                        _timeUpper = (double)entry.Value; break;
                    case "_modalDamping":
                        _modalDamping = (ModalDamping)entry.Value; break;
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
            info.AddValue("_harmonic", _harmonic, typeof(bool));
            info.AddValue("_lowerFrequency", _lowerFrequency, typeof(double));
            info.AddValue("_upperFrequency", _upperFrequency, typeof(double));
            info.AddValue("_numDataPoints", _numDataPoints, typeof(int));
            info.AddValue("_bias", _bias, typeof(double));
            info.AddValue("_numFourierTerms", _numFourierTerms, typeof(int));
            info.AddValue("_timeLower", _timeLower, typeof(double));
            info.AddValue("_timeUpper", _timeUpper, typeof(double));
            info.AddValue("_modalDamping", _modalDamping, typeof(ModalDamping));
        }
    }
}
