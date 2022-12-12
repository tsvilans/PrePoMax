using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Globalization;
using UnitsNet.Units;
using UnitsNet;

namespace CaeGlobals
{
    public class StringEnergyPerVolumeConverter : TypeConverter
    {
        // Variables                                                                                                                
        protected static EnergyUnit _energyUnit = EnergyUnit.Joule;
        protected static VolumeUnit _volumeUnit = VolumeUnit.CubicMeter;
        protected static string error = "Unable to parse quantity. Expected the form \"{value} {unit abbreviation}" +
                                        "\", such as \"5.5 m\". The spacing is optional.";


        // Properties                                                                                                               
        public static string SetEnergyUnit
        {
            set
            {
                if (value == "")
                {
                    _energyUnit = (EnergyUnit)MyUnit.NoUnit;
                    _volumeUnit = (VolumeUnit)MyUnit.NoUnit;
                }
                else if (value == MyUnit.PoundForceInchAbbreviation) _energyUnit = MyUnit.PoundForceInch;
                else { _energyUnit = Energy.ParseUnit(value); }
            }
        }
        public static string SetVolumeUnit
        {
            set
            {
                if (value == "")
                {
                    _energyUnit = (EnergyUnit)MyUnit.NoUnit;
                    _volumeUnit = (VolumeUnit)MyUnit.NoUnit;                    
                }
                else _volumeUnit = Volume.ParseUnit(value);
            } 
        }
        public static string GetUnitAbbreviation(EnergyUnit energyUnit, VolumeUnit volumeUnit)
        {
            string unit;
            if ((int)energyUnit == MyUnit.NoUnit || (int)volumeUnit == MyUnit.NoUnit) unit = "";
            else
            {
                if (energyUnit == MyUnit.PoundForceInch) unit = MyUnit.PoundForceInchAbbreviation;
                else unit = Energy.GetAbbreviation(energyUnit);
                unit += "/" + Volume.GetAbbreviation(volumeUnit);
            }
            return unit;
        }


        // Constructors                                                                                                             
        public StringEnergyPerVolumeConverter()
        {
        }


        // Methods                                                                                                                  
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string)) return true;
            else return base.CanConvertFrom(context, sourceType);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            // Convert from string
            if (value is string valueString) return MyNCalc.ConvertFromString(valueString, ConvertToCurrentUnits);
            else return base.ConvertFrom(context, culture, value);
        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            try
            {
                if (destinationType == typeof(string))
                {
                    if (value is double valueDouble)
                    {
                        string valueString = valueDouble.ToString();
                        string unit = GetUnitAbbreviation(_energyUnit, _volumeUnit);
                        if (unit.Length > 0) valueString += " " + unit;
                        return valueString;
                    }
                }
                return base.ConvertTo(context, culture, value, destinationType);
            }
            catch
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }
        //
        private static double ConvertToCurrentUnits(string valueWithUnitString)
        {
            try
            {
                valueWithUnitString = valueWithUnitString.Trim().Replace(" ", "");
                //
                string[] tmp = valueWithUnitString.Split('/');
                if (tmp.Length != 2) throw new FormatException(error);
                //
                StringEnergyConverter converter = new StringEnergyConverter();  // this includes conversion to NoUnit
                double energyValue = (double)converter.ConvertFromString(tmp[0]);
                // NoUnit
                if ((int)_energyUnit == MyUnit.NoUnit || (int)_volumeUnit == MyUnit.NoUnit) return energyValue;
                //
                VolumeUnit volumeUnit = Volume.ParseUnit(tmp[1]);
                Volume volume = Volume.From(1, volumeUnit).ToUnit(_volumeUnit);
                double value = energyValue / volume.Value;
                //
                return value;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + Environment.NewLine + Environment.NewLine + SupportedUnitAbbreviations());
            }
        }
        public static string SupportedUnitAbbreviations()
        {
            string supportedUnitAbbreviations = StringEnergyConverter.SupportedUnitAbbreviations();
            supportedUnitAbbreviations += Environment.NewLine + Environment.NewLine;
            supportedUnitAbbreviations += StringVolumeConverter.SupportedUnitAbbreviations();
            return supportedUnitAbbreviations;
        }
    }


}