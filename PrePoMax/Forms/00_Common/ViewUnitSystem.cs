using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;
using DynamicTypeDescriptor;


namespace PrePoMax
{
    [Serializable]
    public class ViewUnitSystem
    {
        // Variables                                                                                                                
        private UnitSystem _unitSystem;


        // Properties                                                                                                               
        [CategoryAttribute("Base units")]
        [OrderedDisplayName(0, 10, "Length unit")]
        [DescriptionAttribute("Default length unit.")]
        [Id(1, 1)]
        public string LengthUnitAbbreviation { get { return _unitSystem.LengthUnitAbbreviation; } }
        [CategoryAttribute("Base units")]
        [OrderedDisplayName(1, 10, "Angle unit")]
        [DescriptionAttribute("Default angle unit.")]
        [Id(2, 1)]
        public string AngleUnitAbbreviation { get { return _unitSystem.AngleUnitAbbreviation; } }
        //
        [CategoryAttribute("Base units")]
        [OrderedDisplayName(2, 10, "Mass unit")]
        [DescriptionAttribute("Default mass unit.")]
        [Id(3, 1)]
        public string MassUnitAbbreviation { get { return _unitSystem.MassUnitAbbreviation; } }
        //
        [CategoryAttribute("Base units")]
        [OrderedDisplayName(3, 10, "Time unit")]
        [DescriptionAttribute("Default time unit.")]
        [Id(4, 1)]
        public string TimeUnitAbbreviation { get { return _unitSystem.TimeUnitAbbreviation; } }
        //
        [CategoryAttribute("Base units")]
        [OrderedDisplayName(4, 10, "Temperature unit")]
        [DescriptionAttribute("Default temperature unit.")]
        [Id(5, 1)]
        public string TemperatureUnitAbbreviation { get { return _unitSystem.TemperatureUnitAbbreviation; } }
        // Derived units
        [CategoryAttribute("Derived units")]
        [OrderedDisplayName(0, 50, "Area unit")]
        [DescriptionAttribute("Default area unit.")]
        [Id(1, 1)]
        public string AreaUnitAbbreviation { get { return _unitSystem.AreaUnitAbbreviation; } }
        //
        [CategoryAttribute("Derived units")]
        [OrderedDisplayName(1, 50, "Volume unit")]
        [DescriptionAttribute("Default volume unit.")]
        [Id(2, 1)]
        public string VolumeUnitAbbreviation { get { return _unitSystem.VolumeUnitAbbreviation; } }
        //
        [CategoryAttribute("Derived units")]
        [OrderedDisplayName(2, 50, "Velocity unit")]
        [DescriptionAttribute("Default velocity unit.")]
        [Id(3, 1)]
        public string VelocityUnitAbbreviation { get { return _unitSystem.VelocityUnitAbbreviation; } }
        //
        [CategoryAttribute("Derived units")]
        [OrderedDisplayName(3, 50, "Angular velocity unit")]
        [DescriptionAttribute("Default angular velocity unit.")]
        [Id(4, 1)]
        public string RotationalSpeedUnitAbbreviation { get { return _unitSystem.RotationalSpeedUnitAbbreviation; } }
        //
        [CategoryAttribute("Derived units")]
        [OrderedDisplayName(4, 50, "Acceleration unit")]
        [DescriptionAttribute("Default acceleration unit.")]
        [Id(5, 1)]
        public string AccelerationUnitAbbreviation { get { return _unitSystem.AccelerationUnitAbbreviation; } }
        //
        [CategoryAttribute("Derived units")]
        [OrderedDisplayName(5, 50, "Force unit")]
        [DescriptionAttribute("Default force unit.")]
        [Id(6, 1)]
        public string ForceUnitAbbreviation { get { return _unitSystem.ForceUnitAbbreviation; } }
        //
        [CategoryAttribute("Derived units")]
        [OrderedDisplayName(6, 50, "Force per length unit")]
        [DescriptionAttribute("Default force per length unit.")]
        [Id(7, 1)]
        public string ForcePerLengthUnitAbbreviation { get { return _unitSystem.ForcePerLengthUnitAbbreviation; } }
        //
        [CategoryAttribute("Derived units")]
        [OrderedDisplayName(7, 50, "Moment unit")]
        [DescriptionAttribute("Default moment unit.")]
        [Id(8, 1)]
        public string MomentUnitAbbreviation { get { return _unitSystem.MomentUnitAbbreviation; } }
        //
        [CategoryAttribute("Derived units")]
        [OrderedDisplayName(8, 50, "Pressure unit")]
        [DescriptionAttribute("Default pressure unit.")]
        [Id(9, 1)]
        public string PressureUnitAbbreviation { get { return _unitSystem.PressureUnitAbbreviation; } }
        //
        [CategoryAttribute("Derived units")]
        [OrderedDisplayName(9, 50, "Density unit")]
        [DescriptionAttribute("Default density unit.")]
        [Id(10, 1)]
        public string DensityUnitAbbreviation { get { return _unitSystem.DensityUnitAbbreviation; } }
        //
        [CategoryAttribute("Derived units")]
        [OrderedDisplayName(10, 50, "Energy unit")]
        [DescriptionAttribute("Default energy unit.")]
        [Id(11, 1)]
        public string EnergyUnitAbbreviation { get { return _unitSystem.EnergyUnitAbbreviation; } }
        //
        [CategoryAttribute("Derived units")]
        [OrderedDisplayName(11, 50, "Energy per volume unit")]
        [DescriptionAttribute("Default energy per volume unit.")]
        [Id(12, 1)]
        public string EnergyPerVolumeUnitAbbreviation { get { return _unitSystem.EnergyPerVolumeUnitAbbreviation; } }
        //
        [CategoryAttribute("Derived units")]
        [OrderedDisplayName(12, 50, "Power unit")]
        [DescriptionAttribute("Default power unit.")]
        [Id(13, 1)]
        public string PowerUnitAbbreviation { get { return _unitSystem.PowerUnitAbbreviation; } }
        //
        [CategoryAttribute("Derived units")]
        [OrderedDisplayName(13, 50, "Power per area unit")]
        [DescriptionAttribute("Default power per ara unit.")]
        [Id(14, 1)]
        public string PowerPerAreaUnitAbbreviation { get { return _unitSystem.PowerPerAreaUnitAbbreviation; } }
        //
        [CategoryAttribute("Derived units")]
        [OrderedDisplayName(14, 50, "Power per volume unit")]
        [DescriptionAttribute("Default power per volume unit.")]
        [Id(15, 1)]
        public string PowerPerVolumeUnitAbbreviation { get { return _unitSystem.PowerPerVolumeUnitAbbreviation; } }
        //
        [CategoryAttribute("Derived units")]
        [OrderedDisplayName(15, 50, "Frequency")]
        [DescriptionAttribute("Default frequency unit.")]
        [Id(16, 1)]
        public string FrequencyUnitAbbreviation { get { return _unitSystem.FrequencyUnitAbbreviation; } }
        //
        [CategoryAttribute("Derived units")]
        [OrderedDisplayName(16, 50, "Thermal expansion")]
        [DescriptionAttribute("Default thermal expansion unit.")]
        [Id(17, 1)]
        public string ThermalExpansion { get { return _unitSystem.ThermalExpansionUnitAbbreviation; } }
        //
        [CategoryAttribute("Derived units")]
        [OrderedDisplayName(17, 50, "Thermal conductivity")]
        [DescriptionAttribute("Default thermal conductivity unit.")]
        [Id(18, 1)]
        public string ThermalConductivity { get { return _unitSystem.ThermalConductivityUnitAbbreviation; } }
        //
        [CategoryAttribute("Derived units")]
        [OrderedDisplayName(18, 50, "Specific heat")]
        [DescriptionAttribute("Default specific heat unit.")]
        [Id(19, 1)]
        public string SpecificHeat { get { return _unitSystem.SpecificHeatUnitAbbreviation; } }
        //
        [CategoryAttribute("Derived units")]
        [OrderedDisplayName(19, 50, "Heat transfer coefficient")]
        [DescriptionAttribute("Default heat transfer coefficient unit.")]
        [Id(20, 1)]
        public string HeatTransferCoefficient { get { return _unitSystem.HeatTransferCoefficientUnitAbbreviation; } }


        // Constructors                                                                                                             
        public ViewUnitSystem(UnitSystem unitSystem)
        {
            _unitSystem = unitSystem;
        }


        // Methods                                                                                                                  
        public UnitSystem GetBase()
        {
            return _unitSystem;
        }
    }

}
