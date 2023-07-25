using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Globalization;
using UnitsNet;
using UnitsNet.Units;

namespace CaeGlobals
{
    public class StringVolumeConverter : TypeConverter
    {
        // Variables                                                                                                                
        protected static VolumeUnit _volumeUnit = VolumeUnit.CubicMeter;


        // Properties                                                                                                               
        public static string SetUnit
        {
            set
            {
                if (value == "") _volumeUnit = (VolumeUnit)MyUnit.NoUnit;
                else _volumeUnit = Volume.ParseUnit(value);
            }
        }


        // Constructors                                                                                                             
        public StringVolumeConverter()
        {
        }


        // Methods                                                                                                                  
        public override bool CanConvertFrom(ITypeDescriptorContext context, System.Type sourceType)
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
            // Convert to string
            try
            {
                if (destinationType == typeof(string))
                {
                    if (value is double valueDouble)
                    {
                        string valueString = valueDouble.ToString();
                        if ((int)_volumeUnit != MyUnit.NoUnit) valueString += " " + Volume.GetAbbreviation(_volumeUnit);
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
                Volume volume = Volume.Parse(valueWithUnitString);
                if ((int)_volumeUnit != MyUnit.NoUnit) volume = volume.ToUnit(_volumeUnit);
                return volume.Value;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + Environment.NewLine + Environment.NewLine + SupportedUnitAbbreviations());
            }
        }
        public static string SupportedUnitAbbreviations()
        {
            string abb;
            string supportedUnitAbbreviations = "Supported volume abbreviations: ";
            var allUnits = Volume.Units;
            for (int i = 0; i < allUnits.Length; i++)
            {
                abb = Volume.GetAbbreviation(allUnits[i]);
                if (abb != null) abb.Trim();
                if (abb.Length > 0)
                {
                    supportedUnitAbbreviations += abb;
                    if (i != allUnits.Length - 1) supportedUnitAbbreviations += ", ";
                }
            }
            supportedUnitAbbreviations += ".";
            //
            return supportedUnitAbbreviations;
        }
    }

}