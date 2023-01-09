using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using DynamicTypeDescriptor;

namespace CaeModel
{
    [Serializable]
    public enum SolutionProcedureEnum
    {
        [StandardValue("ImplicitImplicit", DisplayName = "Implicit/Implicit")]
        ImplicitImplicit = 0,
        //
        [StandardValue("ImplicitExplicit", DisplayName = "Implicit/Explicit")]
        ImplicitExplicit = 1,
        //
        [StandardValue("ExplicitImplicit", DisplayName = "Explicit/Implicit")]
        ExplicitImplicit = 2,
        //
        [StandardValue("ExplicitExplicit", DisplayName = "Explicit/Explicit")]
        ExplicitExplicit = 3
    }

    [Serializable]
    public class DynamicStep : StaticStep, ISerializable
    {
        // Variables                                                                                                                
        public static readonly double AlphaDefault = -0.05;
        private double _alpha;                              //ISerializable
        private SolutionProcedureEnum _solutionProcedure;   //ISerializable
        private bool _relativeToAbsolute;                   //ISerializable


        // Properties                                                                                                               
        public double Alpha
        {
            get { return _alpha; }
            set
            {
                _alpha = value;
                if (_alpha > 0) _alpha = 0;
                else if (_alpha < -1.0 / 3.0) _alpha = -1.0 / 3.0;
            }
        }
        public SolutionProcedureEnum SolutionProcedure { get { return _solutionProcedure; } set { _solutionProcedure = value; } }
        public bool RelativeToAbsolute { get { return _relativeToAbsolute; } set { _relativeToAbsolute = value; } }


        // Constructors                                                                                                             
        public DynamicStep(string name)
            : this(name, true)
        {
        }
        public DynamicStep(string name, bool addFieldOutputs)
            : base(name, false)
        {
            _alpha = AlphaDefault;
            _solutionProcedure = SolutionProcedureEnum.ExplicitExplicit;
            _relativeToAbsolute = false;
            _incrementationType = IncrementationTypeEnum.Automatic;
            //
            if (addFieldOutputs)
            {
                AddFieldOutput(new NodalFieldOutput("NF-Output-1", NodalFieldVariable.RF | NodalFieldVariable.U |
                                                    NodalFieldVariable.V));
                AddFieldOutput(new ElementFieldOutput("EF-Output-1", ElementFieldVariable.E | ElementFieldVariable.S));
            }
        }
        //ISerializable
        public DynamicStep(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_alpha":
                        _alpha = (double)entry.Value; break;
                    case "_explicit":
                        _solutionProcedure = (SolutionProcedureEnum)entry.Value; break;
                    case "_relativeToAbsolute":
                        _relativeToAbsolute = (bool)entry.Value; break;
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
            info.AddValue("_alpha", _alpha, typeof(double));
            info.AddValue("_explicit", _solutionProcedure, typeof(SolutionProcedureEnum));
            info.AddValue("_relativeToAbsolute", _relativeToAbsolute, typeof(bool));
        }
    }
}
