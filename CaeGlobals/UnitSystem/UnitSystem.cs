using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;
using System.Drawing;
using UnitsNet;
using UnitsNet.Units;
using System.Runtime.Serialization;
using DynamicTypeDescriptor;
using System.ComponentModel;


namespace CaeGlobals
{
    [Serializable]
    public enum UnitSystemType
    {
        [Description("Undefined")]
        Undefined = 0,
        //
        [Description("m, kg, s, °C")]
        M_KG_S_C = 1,
        //
        [Description("mm, ton, s, °C")]
        MM_TON_S_C = 2,
        //
        [Description("m, ton, s, °C")]
        M_TON_S_C = 3,
        //
        [Description("in, lb, s, °F")]
        IN_LB_S_F = 20,
        //
        [Description("Unitless")]
        UNIT_LESS = 40
    }


    [Serializable]
    public class UnitSystem : ISerializable
    {
        // Variables                                                                                                                
        private UnitSystemType _unitSystemType;                             //ISerializable
        // Base units
        private LengthUnit _lengthUnit;
        private AngleUnit _angleUnit;
        private MassUnit _massUnit;
        private DurationUnit _timeUnit;
        private TemperatureUnit _temperatureUnit;
        private TemperatureDeltaUnit _temperatureDeltaUnit;
        // Derived units
        private AreaUnit _areaUnit;
        private VolumeUnit _volumeUnit;
        private SpeedUnit _velocityUnit;
        private RotationalSpeedUnit _rotationalSpeedUnit;
        private AccelerationUnit _accelerationUnit;
        private ForceUnit _forceUnit;
        private ForcePerLengthUnit _forcePerLengthUnit;
        private TorqueUnit _momentUnit;
        private PressureUnit _pressureUnit;
        private DensityUnit _densityUnit;
        private EnergyUnit _energyUnit;
        private PowerUnit _powerUnit;
        private PowerPerAreaUnit _powerPerAreaUnit;
        private PowerPerVolumeUnit _powerPerVolumeUnit;
        private FrequencyUnit _frequencyUnit;
        // Thermal units
        private CoefficientOfThermalExpansionUnit _thermalExpansionUnit;
        private ThermalConductivityUnit _thermalConductivityUnit;
        private HeatTransferCoefficientUnit _heatTransferCoefficientUnit;
        // Constants
        private string _absoluteZero = "0 K";
        private string _stefanBoltzmann = "5.67038E-8 kg/(s^3*°C^4)";
        private string _newtonGravity = "6.67430E-11 N*m^2/kg^2";


        // Properties                                                                                                               
        public UnitSystemType UnitSystemType { get { return _unitSystemType; } }
        // Abbreviations                                                                                
        //
        // Base units
        public string LengthUnitAbbreviation
        {
            get
            {
                if ((int)_lengthUnit == MyUnit.NoUnit) return "";
                else return Length.GetAbbreviation(_lengthUnit);
            }
        }
        public string AngleUnitAbbreviation
        {
            get
            {
                if ((int)_angleUnit == MyUnit.NoUnit) return "";
                else return Angle.GetAbbreviation(_angleUnit);
            }
        }
        public string MassUnitAbbreviation
        {
            get
            {
                if ((int)_massUnit == MyUnit.NoUnit) return "";
                else return Mass.GetAbbreviation(_massUnit);
            }
        }
        public string TimeUnitAbbreviation
        {
            get
            {
                if ((int)_timeUnit == MyUnit.NoUnit) return "";
                else return Duration.GetAbbreviation(_timeUnit);
            }
        }
        public string TemperatureUnitAbbreviation
        {
            get
            {
                if ((int)_temperatureUnit == MyUnit.NoUnit) return "";
                else return Temperature.GetAbbreviation(_temperatureUnit);
            }
        }
        public string TemperatureDeltaUnitAbbreviation
        {
            get
            {
                if ((int)_temperatureDeltaUnit == MyUnit.NoUnit) return "";
                else return TemperatureDelta.GetAbbreviation(_temperatureDeltaUnit);
            }
        }
        // Derived units
        public string AreaUnitAbbreviation
        {
            get
            {
                if ((int)_areaUnit == MyUnit.NoUnit) return "";
                else return Area.GetAbbreviation(_areaUnit);

            }
        }
        public string VolumeUnitAbbreviation
        {
            get
            {
                if ((int)_volumeUnit == MyUnit.NoUnit) return "";
                else return Volume.GetAbbreviation(_volumeUnit);
            }
        }
        public string VelocityUnitAbbreviation
        {
            get
            {
                if ((int)_velocityUnit == MyUnit.NoUnit) return "";
                else return Speed.GetAbbreviation(_velocityUnit);
            }
        }
        public string RotationalSpeedUnitAbbreviation
        {
            get
            {
                if ((int)_rotationalSpeedUnit == MyUnit.NoUnit) return "";
                else return RotationalSpeed.GetAbbreviation(_rotationalSpeedUnit);
            }
        }
        public string AccelerationUnitAbbreviation
        {
            get
            {
                if ((int)_accelerationUnit == MyUnit.NoUnit) return "";
                else return Acceleration.GetAbbreviation(_accelerationUnit);
            }
        }
        public string ForceUnitAbbreviation
        {
            get
            {
                if ((int)_forceUnit == MyUnit.NoUnit) return "";
                else return Force.GetAbbreviation(_forceUnit);
            }
        }
        public string ForcePerLengthUnitAbbreviation
        {
            get
            {
                if ((int)_forceUnit == MyUnit.NoUnit) return "";
                else return ForcePerLength.GetAbbreviation(_forcePerLengthUnit);
            }
        }
        public string MomentUnitAbbreviation
        {
            get
            {
                if ((int)_momentUnit == MyUnit.NoUnit) return "";
                else return Torque.GetAbbreviation(_momentUnit);
            }
        }
        public string PressureUnitAbbreviation
        {
            get
            {
                if ((int)_pressureUnit == MyUnit.NoUnit) return "";
                else return Pressure.GetAbbreviation(_pressureUnit);
            }
        }
        public string DensityUnitAbbreviation
        {
            get
            {
                if ((int)_densityUnit == MyUnit.NoUnit) return "";
                else return UnitsNet.Density.GetAbbreviation(_densityUnit);
            }
        }
        public string EnergyUnitAbbreviation
        {
            get
            {
                return StringEnergyConverter.GetUnitAbbreviation(_energyUnit);
            }
        }
        public string EnergyPerVolumeUnitAbbreviation
        {
            get
            {
                return StringEnergyPerVolumeConverter.GetUnitAbbreviation(_energyUnit, _volumeUnit);
            }
        }
        public string PowerUnitAbbreviation
        {
            get
            {
                return StringPowerConverter.GetUnitAbbreviation(_powerUnit);
            }
        }
        public string PowerPerAreaUnitAbbreviation
        {
            get
            {
                return StringPowerPerAreaConverter.GetUnitAbbreviation(_powerUnit, _areaUnit, _powerPerAreaUnit);
            }
        }
        public string PowerPerVolumeUnitAbbreviation
        {
            get
            {
                return StringPowerPerVolumeConverter.GetUnitAbbreviation(_powerUnit, _volumeUnit, _powerPerVolumeUnit);
            }
        }
        // Frequency units
        public string FrequencyUnitAbbreviation 
        { 
            get 
            {
                if ((int)_frequencyUnit == MyUnit.NoUnit) return "";
                else return Frequency.GetAbbreviation(_frequencyUnit); 
            } 
        }
        public string EigenvalueUnitAbbreviation
        {
            get
            {
                string unit = RotationalSpeedUnitAbbreviation;
                if (unit != null)
                {
                    unit = unit.Replace("/", "²/");
                    unit += "²";
                }
                return unit;
            }
        }
        public string TransParticipationFactorUnitAbbreviation
        {
            get
            {
                if ((int)_massUnit == MyUnit.NoUnit) return "";
                else
                {
                    string unit = "√(" + MassUnitAbbreviation + ")·" + LengthUnitAbbreviation;
                    return unit;
                }
            }
        }
        public string RotParticipationFactorUnitAbbreviation
        {
            get
            {
                if ((int)_massUnit == MyUnit.NoUnit) return "";
                else
                {
                    string unit = "√(" + MassUnitAbbreviation + ")·" + AngleUnitAbbreviation;
                    return unit;
                }
            }
        }
        public string TransEffectiveMassUnitAbbreviation
        {
            get
            {
                if ((int)_massUnit == MyUnit.NoUnit) return "";
                else
                {
                    string unit = MassUnitAbbreviation + "·" + LengthUnitAbbreviation + "²";
                    return unit;
                }
            }
        }
        public string RotEffectiveMassUnitAbbreviation
        {
            get
            {
                if ((int)_massUnit == MyUnit.NoUnit) return "";
                else
                {
                    string unit = MassUnitAbbreviation + "·" + AngleUnitAbbreviation + "²";
                    return unit;
                }
            }
        }
        // Thermal units
        public string ThermalExpansionUnitAbbreviation
        {
            get
            {
                if ((int)_thermalExpansionUnit == MyUnit.NoUnit) return "";
                else return CoefficientOfThermalExpansion.GetAbbreviation(_thermalExpansionUnit);
            }
        }
        public string ThermalConductivityUnitAbbreviation
        {
            get
            {
                return StringThermalConductivityConverter.GetUnitAbbreviation(_powerUnit, _lengthUnit, _temperatureDeltaUnit,
                                                                              _thermalConductivityUnit);
            }
        }
        public string SpecificHeatUnitAbbreviation
        {
            get
            {
                return StringSpecificHeatConverter.GetUnitAbbreviation(_energyUnit, _massUnit, _temperatureDeltaUnit);
            }
        }
        public string HeatTransferCoefficientUnitAbbreviation
        {
            get
            {
                return StringHeatTransferCoefficientConverter.GetUnitAbbreviation(_powerUnit, _areaUnit, _temperatureDeltaUnit,
                                                                                  _heatTransferCoefficientUnit);
            }
        }
        // Constants
        public double AbsoluteZero
        {
            get
            {
                if (_unitSystemType == UnitSystemType.Undefined || _unitSystemType == UnitSystemType.UNIT_LESS) return 0;
                else
                {
                    StringTemperatureConverter converter = new StringTemperatureConverter();
                    if (_unitSystemType == UnitSystemType.Undefined || _unitSystemType == UnitSystemType.UNIT_LESS) return 0;
                    else return (double)converter.ConvertFromString(_absoluteZero);
                }
            }
        }
        public double StefanBoltzmann
        {
            get
            {
                StringStefanBoltzmannUndefinedConverter converter = new StringStefanBoltzmannUndefinedConverter();
                if (_unitSystemType == UnitSystemType.Undefined || _unitSystemType == UnitSystemType.UNIT_LESS) return 0;
                else return (double)converter.ConvertFromString(_stefanBoltzmann);
            }
        }
        public double NewtonGravity
        {
            get
            {
                StringNewtonGravityUndefinedConverter converter = new StringNewtonGravityUndefinedConverter();
                if (_unitSystemType == UnitSystemType.Undefined || _unitSystemType == UnitSystemType.UNIT_LESS) return 0;
                else return (double)converter.ConvertFromString(_newtonGravity);
            }
        }


        // Constructors                                                                                                             
        public UnitSystem()
            : this(UnitSystemType.Undefined)
        {
        }
        public UnitSystem(UnitSystemType unitSystemType)
        {
            SetUnits(unitSystemType);
        }
        // ISerialization
        public UnitSystem(SerializationInfo info, StreamingContext context)
            : this()
        {
            UnitSystemType unitSystemType = UnitSystemType.Undefined;
            //
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    // Base units
                    case "_unitSystemType":
                        unitSystemType = (UnitSystemType)entry.Value; break;
                    default:
                        break;
                }
                
            }
            //
            SetUnits(unitSystemType);
        }


        // Methods                                                                                                                  
        private void SetUnits(UnitSystemType unitSystemType)
        {
            _unitSystemType = unitSystemType;
            //
            switch (_unitSystemType)
            {
                case UnitSystemType.UNIT_LESS:
                    _lengthUnit = (LengthUnit)MyUnit.NoUnit;
                    _angleUnit = (AngleUnit)MyUnit.NoUnit;
                    _massUnit = (MassUnit)MyUnit.NoUnit;
                    _timeUnit = (DurationUnit)MyUnit.NoUnit;
                    _temperatureUnit = (TemperatureUnit)MyUnit.NoUnit;
                    _temperatureDeltaUnit = (TemperatureDeltaUnit)MyUnit.NoUnit;
                    // Derived
                    _areaUnit = (AreaUnit)MyUnit.NoUnit;
                    _volumeUnit = (VolumeUnit)MyUnit.NoUnit;
                    _velocityUnit = (SpeedUnit)MyUnit.NoUnit;
                    _rotationalSpeedUnit = (RotationalSpeedUnit)MyUnit.NoUnit;
                    _accelerationUnit = (AccelerationUnit)MyUnit.NoUnit;
                    _forceUnit = (ForceUnit)MyUnit.NoUnit;
                    _forcePerLengthUnit = (ForcePerLengthUnit)MyUnit.NoUnit;
                    _momentUnit = (TorqueUnit)MyUnit.NoUnit;
                    _pressureUnit = (PressureUnit)MyUnit.NoUnit;
                    _densityUnit = (DensityUnit)MyUnit.NoUnit;
                    _energyUnit = (EnergyUnit)MyUnit.NoUnit;
                    _powerUnit = (PowerUnit)MyUnit.NoUnit;
                    _powerPerAreaUnit = (PowerPerAreaUnit)MyUnit.NoUnit;
                    _powerPerVolumeUnit = (PowerPerVolumeUnit)MyUnit.NoUnit;
                    _frequencyUnit = (FrequencyUnit)MyUnit.NoUnit;
                    // Thermal
                    _thermalExpansionUnit = (CoefficientOfThermalExpansionUnit)MyUnit.NoUnit;
                    _thermalConductivityUnit = (ThermalConductivityUnit)MyUnit.NoUnit;
                    _heatTransferCoefficientUnit = (HeatTransferCoefficientUnit)MyUnit.NoUnit;
                    break;
                case UnitSystemType.Undefined:
                case UnitSystemType.M_KG_S_C:
                    _lengthUnit = LengthUnit.Meter;
                    _angleUnit = AngleUnit.Radian;
                    _massUnit = MassUnit.Kilogram;
                    _timeUnit = DurationUnit.Second;
                    _temperatureUnit = TemperatureUnit.DegreeCelsius;
                    _temperatureDeltaUnit = TemperatureDeltaUnit.DegreeCelsius;
                    // Derived
                    _areaUnit = AreaUnit.SquareMeter;
                    _volumeUnit = VolumeUnit.CubicMeter;
                    _velocityUnit = SpeedUnit.MeterPerSecond;
                    _rotationalSpeedUnit = RotationalSpeedUnit.RadianPerSecond;
                    _accelerationUnit = AccelerationUnit.MeterPerSecondSquared;
                    _forceUnit = ForceUnit.Newton;
                    _forcePerLengthUnit = ForcePerLengthUnit.NewtonPerMeter;
                    _momentUnit = TorqueUnit.NewtonMeter;
                    _pressureUnit = PressureUnit.Pascal;
                    _densityUnit = DensityUnit.KilogramPerCubicMeter;
                    _energyUnit = EnergyUnit.Joule;
                    _powerUnit = PowerUnit.Watt;
                    _powerPerAreaUnit = (PowerPerAreaUnit)MyUnit.NoUnit;
                    _powerPerVolumeUnit = (PowerPerVolumeUnit)MyUnit.NoUnit;
                    _frequencyUnit = FrequencyUnit.Hertz;
                    // Thermal
                    _thermalExpansionUnit = CoefficientOfThermalExpansionUnit.InverseDegreeCelsius;
                    _thermalConductivityUnit = (ThermalConductivityUnit)MyUnit.NoUnit;
                    _heatTransferCoefficientUnit = (HeatTransferCoefficientUnit)MyUnit.NoUnit;
                    break;
                case UnitSystemType.MM_TON_S_C:
                    _lengthUnit = LengthUnit.Millimeter;
                    _angleUnit = AngleUnit.Radian;
                    _massUnit = MassUnit.Tonne;
                    _timeUnit = DurationUnit.Second;
                    _temperatureUnit = TemperatureUnit.DegreeCelsius;
                    _temperatureDeltaUnit = TemperatureDeltaUnit.DegreeCelsius;
                    // Derived
                    _areaUnit = AreaUnit.SquareMillimeter;
                    _volumeUnit = VolumeUnit.CubicMillimeter;
                    _velocityUnit = SpeedUnit.MillimeterPerSecond;
                    _rotationalSpeedUnit = RotationalSpeedUnit.RadianPerSecond;
                    _accelerationUnit = AccelerationUnit.MillimeterPerSecondSquared;
                    _forceUnit = ForceUnit.Newton;
                    _forcePerLengthUnit = ForcePerLengthUnit.NewtonPerMillimeter;
                    _momentUnit = TorqueUnit.NewtonMillimeter;
                    _pressureUnit = PressureUnit.Megapascal;
                    _densityUnit = DensityUnit.TonnePerCubicMillimeter;
                    _energyUnit = EnergyUnit.Millijoule;
                    _powerUnit = PowerUnit.Milliwatt;
                    _powerPerAreaUnit = (PowerPerAreaUnit)MyUnit.NoUnit;
                    _powerPerVolumeUnit = (PowerPerVolumeUnit)MyUnit.NoUnit;
                    _frequencyUnit = FrequencyUnit.Hertz;
                    // Thermal
                    _thermalExpansionUnit = CoefficientOfThermalExpansionUnit.InverseDegreeCelsius;
                    _thermalConductivityUnit = (ThermalConductivityUnit)MyUnit.NoUnit;
                    _heatTransferCoefficientUnit = (HeatTransferCoefficientUnit)MyUnit.NoUnit;
                    break;
                case UnitSystemType.M_TON_S_C:
                    _lengthUnit = LengthUnit.Meter;
                    _angleUnit = AngleUnit.Radian;
                    _massUnit = MassUnit.Tonne;
                    _timeUnit = DurationUnit.Second;
                    _temperatureUnit = TemperatureUnit.DegreeCelsius;
                    _temperatureDeltaUnit = TemperatureDeltaUnit.DegreeCelsius;
                    // Derived
                    _areaUnit = AreaUnit.SquareMeter;
                    _volumeUnit = VolumeUnit.CubicMeter;
                    _velocityUnit = SpeedUnit.MeterPerSecond;
                    _rotationalSpeedUnit = RotationalSpeedUnit.RadianPerSecond;
                    _accelerationUnit = AccelerationUnit.MeterPerSecondSquared;
                    _forceUnit = ForceUnit.Kilonewton;
                    _forcePerLengthUnit = ForcePerLengthUnit.KilonewtonPerMeter;
                    _momentUnit = TorqueUnit.KilonewtonMeter;
                    _pressureUnit = PressureUnit.Kilopascal;
                    _densityUnit = DensityUnit.TonnePerCubicMeter;
                    _energyUnit = EnergyUnit.Kilojoule;
                    _powerUnit = PowerUnit.Kilowatt;
                    _powerPerAreaUnit = (PowerPerAreaUnit)MyUnit.NoUnit;
                    _powerPerVolumeUnit = (PowerPerVolumeUnit)MyUnit.NoUnit;
                    _frequencyUnit = FrequencyUnit.Hertz;
                    // Thermal
                    _thermalExpansionUnit = CoefficientOfThermalExpansionUnit.InverseDegreeCelsius;
                    _thermalConductivityUnit = (ThermalConductivityUnit)MyUnit.NoUnit;
                    _heatTransferCoefficientUnit = (HeatTransferCoefficientUnit)MyUnit.NoUnit;
                    break;
                case UnitSystemType.IN_LB_S_F:
                    _lengthUnit = LengthUnit.Inch;
                    _angleUnit = AngleUnit.Radian;
                    _massUnit = MassUnit.Pound;
                    _timeUnit = DurationUnit.Second;
                    _temperatureUnit = TemperatureUnit.DegreeFahrenheit;
                    _temperatureDeltaUnit = TemperatureDeltaUnit.DegreeFahrenheit;
                    // Derived
                    _areaUnit = AreaUnit.SquareInch;
                    _volumeUnit = VolumeUnit.CubicInch;
                    _velocityUnit = SpeedUnit.InchPerSecond;
                    _rotationalSpeedUnit = RotationalSpeedUnit.RadianPerSecond;
                    _accelerationUnit = AccelerationUnit.InchPerSecondSquared;
                    _forceUnit = ForceUnit.PoundForce;
                    _forcePerLengthUnit = ForcePerLengthUnit.PoundForcePerInch;
                    _momentUnit = TorqueUnit.PoundForceInch;
                    _pressureUnit = PressureUnit.PoundForcePerSquareInch;
                    _densityUnit = DensityUnit.PoundPerCubicInch;
                    _energyUnit = MyUnit.PoundForceInch; // EnergyUnit.InchPound;
                    _powerUnit = MyUnit.PoundForceInchPerSecond;
                    _powerPerAreaUnit = MyUnit.PoundForcePerInchSecond;
                    _powerPerVolumeUnit = MyUnit.PoundForcePerSquareInchSecond;
                    _frequencyUnit = FrequencyUnit.Hertz;
                    // Thermal
                    _thermalExpansionUnit = CoefficientOfThermalExpansionUnit.InverseDegreeFahrenheit;
                    _thermalConductivityUnit = MyUnit.PoundForcePerSecondFahrenheit;
                    _heatTransferCoefficientUnit = MyUnit.PoundForcePerInchSecondFahrenheit;
                    break;
                default:
                    break;
            }
            //
            SetConverterUnits();

        }
        public void SetConverterUnits()
        {
            // Base units
            StringLengthConverter.SetUnit = LengthUnitAbbreviation;
            StringLengthDOFConverter.SetUnit = LengthUnitAbbreviation;
            StringLengthFixedDOFConverter.SetUnit = LengthUnitAbbreviation;
            StringLengthDefaultConverter.SetUnit = LengthUnitAbbreviation;
            StringAngleConverter.SetUnit = AngleUnitAbbreviation;
            StringAngleDOFConverter.SetUnit = AngleUnitAbbreviation;
            StringAngleFixedDOFConverter.SetUnit = AngleUnitAbbreviation;
            StringMassConverter.SetUnit = MassUnitAbbreviation;
            StringTimeConverter.SetUnit = TimeUnitAbbreviation;
            StringTemperatureConverter.SetUnit = TemperatureUnitAbbreviation;
            StringTemperatureUndefinedConverter.SetUnit = TemperatureUnitAbbreviation;
            // Derived units
            StringAreaConverter.SetUnit = AreaUnitAbbreviation;
            StringVolumeConverter.SetUnit = VolumeUnitAbbreviation;
            StringRotationalSpeedConverter.SetUnit = RotationalSpeedUnitAbbreviation;
            StringVelocityConverter.SetUnit = VelocityUnitAbbreviation;
            StringAccelerationConverter.SetUnit = AccelerationUnitAbbreviation;
            StringForceConverter.SetUnit = ForceUnitAbbreviation;
            StringForcePerLenghtConverter.SetUnit = ForcePerLengthUnitAbbreviation;
            StringMomentConverter.SetUnit = MomentUnitAbbreviation;
            StringPressureConverter.SetUnit = PressureUnitAbbreviation;
            StringDensityConverter.SetUnit = DensityUnitAbbreviation;
            StringEnergyConverter.SetUnit = EnergyUnitAbbreviation;
            StringEnergyPerVolumeConverter.SetEnergyUnit = EnergyUnitAbbreviation;
            StringEnergyPerVolumeConverter.SetVolumeUnit = VolumeUnitAbbreviation;
            StringPowerConverter.SetUnit = PowerUnitAbbreviation;
            if (_powerPerAreaUnit == MyUnit.PoundForcePerInchSecond)
            {
                StringPowerPerAreaConverter.SetUnit = MyUnit.PoundForcePerInchSecondAbbreviation;
            }
            else
            {
                StringPowerPerAreaConverter.SetPowerUnit = PowerUnitAbbreviation;
                StringPowerPerAreaConverter.SetAreaUnit = AreaUnitAbbreviation;
            }
            if (_powerPerVolumeUnit == MyUnit.PoundForcePerSquareInchSecond)
            {
                StringPowerPerVolumeConverter.SetUnit = MyUnit.PoundForcePerSquareInchSecondAbbreviation;
            }
            else
            {
                StringPowerPerVolumeConverter.SetPowerUnit = PowerUnitAbbreviation;
                StringPowerPerVolumeConverter.SetVolumeUnit = VolumeUnitAbbreviation;
            }
            StringFrequencyConverter.SetUnit = FrequencyUnitAbbreviation;
            StringReciprocalTimeConverter.SetUnit = TimeUnitAbbreviation;
            // Contact
            StringForcePerVolumeConverter.SetForceUnit = ForceUnitAbbreviation;
            StringForcePerVolumeConverter.SetVolumeUnit = VolumeUnitAbbreviation;
            StringForcePerVolumeDefaultConverter.SetForceUnit = ForceUnitAbbreviation;
            StringForcePerVolumeDefaultConverter.SetVolumeUnit = VolumeUnitAbbreviation;
            // Universal constants
            StringNewtonGravityUndefinedConverter.SetForceUnit = ForceUnitAbbreviation;
            StringNewtonGravityUndefinedConverter.SetLengthUnit = LengthUnitAbbreviation;
            StringNewtonGravityUndefinedConverter.SetMassUnit = MassUnitAbbreviation;
            //
            StringStefanBoltzmannUndefinedConverter.SetMassUnit = MassUnitAbbreviation;
            StringStefanBoltzmannUndefinedConverter.SetTimeUnit = TimeUnitAbbreviation;
            StringStefanBoltzmannUndefinedConverter.SetTemperatureDeltaUnit = TemperatureDeltaUnitAbbreviation;
            // Thermal
            StringThermalExpansionConverter.SetUnit = ThermalExpansionUnitAbbreviation;
            //
            if (_thermalConductivityUnit == MyUnit.PoundForcePerSecondFahrenheit)
            {
                StringThermalConductivityConverter.SetUnit = MyUnit.PoundForcePerSecondFahrenheitAbbreviation;
            }
            else
            {
                StringThermalConductivityConverter.SetPowerUnit = PowerUnitAbbreviation;
                StringThermalConductivityConverter.SetLengthUnit = LengthUnitAbbreviation;
                StringThermalConductivityConverter.SetTemperatureDeltaUnit = TemperatureDeltaUnitAbbreviation;
            }
            //
            StringSpecificHeatConverter.SetEnergyUnit = EnergyUnitAbbreviation;
            StringSpecificHeatConverter.SetMassUnit = MassUnitAbbreviation;
            StringSpecificHeatConverter.SetTemperatureDeltaUnit = TemperatureDeltaUnitAbbreviation;
            //
            if (_heatTransferCoefficientUnit == MyUnit.PoundForcePerInchSecondFahrenheit)
            {
                StringHeatTransferCoefficientConverter.SetUnit = MyUnit.PoundForcePerInchSecondFahrenheitAbbreviation;
            }
            else
            {
                StringHeatTransferCoefficientConverter.SetPowerUnit = PowerUnitAbbreviation;
                StringHeatTransferCoefficientConverter.SetAreaUnit = AreaUnitAbbreviation;
                StringHeatTransferCoefficientConverter.SetTemperatureDeltaUnit = TemperatureDeltaUnitAbbreviation;
            }
            // Initial values of constants
            StringTemperatureUndefinedConverter.SetInitialValue = Tools.RoundToSignificantDigits(AbsoluteZero, 6);
            StringStefanBoltzmannUndefinedConverter.SetInitialValue = Tools.RoundToSignificantDigits(StefanBoltzmann, 6);
            StringNewtonGravityUndefinedConverter.SetInitialValue = Tools.RoundToSignificantDigits(NewtonGravity, 6);
        }
        public double Convert(double value, TypeConverter converter, UnitSystem toSystem)
        {
            // Use this method to allow for added units like: Energy: in.lb
            SetConverterUnits();
            string valueWithUnit = converter.ConvertToString(value);
            toSystem.SetConverterUnits();
            double result = (double)converter.ConvertFrom(valueWithUnit);
            SetConverterUnits();
            return result;
        }
        // ISerialization
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Using typeof() works also for null fields
            info.AddValue("_unitSystemType", _unitSystemType, typeof(UnitSystemType));
        }
    }
}
