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
    public class StringForcePerVolumeConverter : TypeConverter
    {
        // Variables                                                                                                                
        protected static ForceUnit _forceUnit = ForceUnit.Newton;
        protected static VolumeUnit _volumeUnit = VolumeUnit.CubicMeter;
        protected static string error = "Unable to parse quantity. Expected the form \"{value} {unit abbreviation}" +
                                        "\", such as \"5.5 m\". The spacing is optional.";


        // Properties                                                                                                               
        public static string SetForceUnit 
        {
            set
            {
                if (value == "")
                {
                    _forceUnit = (ForceUnit)MyUnit.NoUnit;
                    _volumeUnit = (VolumeUnit)MyUnit.NoUnit;
                }
                else _forceUnit = Force.ParseUnit(value); 
            }
        }
        public static string SetVolumeUnit 
        { 
            set 
            {
                if (value == "")
                {
                    _forceUnit = (ForceUnit)MyUnit.NoUnit;
                    _volumeUnit = (VolumeUnit)MyUnit.NoUnit;
                }
                else _volumeUnit = Volume.ParseUnit(value);
            }
        }


        // Constructors                                                                                                             
        public StringForcePerVolumeConverter()
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
                        // NoUnit
                        if ((int)_forceUnit != MyUnit.NoUnit && (int)_volumeUnit != MyUnit.NoUnit)
                            valueString += " " + Force.GetAbbreviation(_forceUnit) + "/" + Volume.GetAbbreviation(_volumeUnit);
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
        public static double ConvertToCurrentUnits(string valueWithUnitString)
        {
            try
            {
                valueWithUnitString = valueWithUnitString.Trim().Replace(" ", "");
                //
                string[] tmp = valueWithUnitString.Split('/');
                if (tmp.Length != 2) throw new FormatException(error);
                Force force = Force.Parse(tmp[0]);
                // NoUnit
                if ((int)_forceUnit == MyUnit.NoUnit || (int)_volumeUnit == MyUnit.NoUnit) return force.Value;
                else force = force.ToUnit(_forceUnit);
                //
                tmp[1] = tmp[1].Replace("(", " ("); // fix space before (U.S.) anf (imp.)
                VolumeUnit volumeUnit = Volume.ParseUnit(tmp[1]);
                Volume volume = Volume.From(1, volumeUnit).ToUnit(_volumeUnit);
                double value = force.Value / volume.Value;
                return value;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + Environment.NewLine + Environment.NewLine + SupportedUnitAbbreviations());
            }
        }
        public static string SupportedUnitAbbreviations()
        {
            string supportedUnitAbbreviations = StringForceConverter.SupportedUnitAbbreviations();
            supportedUnitAbbreviations += Environment.NewLine + Environment.NewLine;
            supportedUnitAbbreviations += StringVolumeConverter.SupportedUnitAbbreviations();
            return supportedUnitAbbreviations;
        }
    }


}