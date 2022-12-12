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
    public class StringSpecificHeatConverter : TypeConverter
    {
        // Variables                                                                                                                
        protected static EnergyUnit _energyUnit = EnergyUnit.Joule;
        protected static MassUnit _massUnit = MassUnit.Kilogram;
        protected static TemperatureDeltaUnit _temperatureDeltaUnit = TemperatureDeltaUnit.DegreeCelsius;
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
                    _massUnit = (MassUnit)MyUnit.NoUnit;
                    _temperatureDeltaUnit = (TemperatureDeltaUnit)MyUnit.NoUnit;
                }
                else
                {
                    if (value == MyUnit.PoundForceInchAbbreviation) _energyUnit = MyUnit.PoundForceInch;
                    else _energyUnit = Energy.ParseUnit(value);
                }
            }
        }
        public static string SetMassUnit
        { 
            set 
            {
                if (value == "")
                {
                    _energyUnit = (EnergyUnit)MyUnit.NoUnit;
                    _massUnit = (MassUnit)MyUnit.NoUnit;
                    _temperatureDeltaUnit = (TemperatureDeltaUnit)MyUnit.NoUnit;
                }
                else _massUnit = Mass.ParseUnit(value);
            }
        }
        public static string SetTemperatureDeltaUnit
        {
            set
            {
                if (value == "")
                {
                    _energyUnit = (EnergyUnit)MyUnit.NoUnit;
                    _massUnit = (MassUnit)MyUnit.NoUnit;
                    _temperatureDeltaUnit = (TemperatureDeltaUnit)MyUnit.NoUnit;
                }
                else _temperatureDeltaUnit = TemperatureDelta.ParseUnit(value);
            }
        }
        public static string GetUnitAbbreviation(EnergyUnit energyUnit, MassUnit massUnit,
                                                 TemperatureDeltaUnit temperatureDeltaUnit)
        {
            string unit;
            if ((int)energyUnit == MyUnit.NoUnit || (int)massUnit == MyUnit.NoUnit || (int)temperatureDeltaUnit == MyUnit.NoUnit)
                unit = "";
            else
            {
                if (energyUnit == MyUnit.PoundForceInch) unit = MyUnit.PoundForceInchAbbreviation;
                else unit = Energy.GetAbbreviation(energyUnit);
                unit += "/(" + Mass.GetAbbreviation(massUnit) + "·" + TemperatureDelta.GetAbbreviation(temperatureDeltaUnit) + ")";
            }
            return unit.Replace("∆", "");
        }
        
        
        // Constructors                                                                                                             
        public StringSpecificHeatConverter()
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
                        string unit = GetUnitAbbreviation(_energyUnit, _massUnit, _temperatureDeltaUnit);
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
        public static double ConvertToCurrentUnits(string valueWithUnitString)
        {
            try
            {
                valueWithUnitString = valueWithUnitString.Trim().Replace(" ", "");
                //
                string[] tmp = valueWithUnitString.Split('/');
                if (tmp.Length != 2) throw new FormatException(error);
                //
                StringEnergyConverter converter = new StringEnergyConverter();
                double energyValue = (double)converter.ConvertFrom(tmp[0]);
                // NoUnit
                if ((int)_energyUnit == MyUnit.NoUnit || (int)_massUnit == MyUnit.NoUnit || (int)_temperatureDeltaUnit == MyUnit.NoUnit)
                    return energyValue;
                //
                tmp = tmp[1].Replace("(", "").Replace(")", "").Split(new string[] { "*", "·" }, StringSplitOptions.RemoveEmptyEntries);
                if (tmp.Length != 2) throw new FormatException(error);
                //
                MassUnit massUnit = Mass.ParseUnit(tmp[0]);
                Mass mass = Mass.From(1, massUnit).ToUnit(_massUnit);
                //
                if (!tmp[1].Contains("∆")) tmp[1] = "∆" + tmp[1];
                TemperatureDeltaUnit temperatureDeltaUnit = TemperatureDelta.ParseUnit(tmp[1]);
                TemperatureDelta temperatureDelta = TemperatureDelta.From(1, temperatureDeltaUnit).ToUnit(_temperatureDeltaUnit);
                //
                double value = energyValue / (mass.Value * temperatureDelta.Value);
                return value;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.Replace("∆", "") + Environment.NewLine + Environment.NewLine + SupportedUnitAbbreviations());
            }
        }
        public static string SupportedUnitAbbreviations()
        {
            string supportedUnitAbbreviations = StringEnergyConverter.SupportedUnitAbbreviations();
            supportedUnitAbbreviations += Environment.NewLine + Environment.NewLine;
            supportedUnitAbbreviations += StringMassConverter.SupportedUnitAbbreviations();
            supportedUnitAbbreviations += Environment.NewLine + Environment.NewLine;
            supportedUnitAbbreviations += StringTemperatureConverter.SupportedDeltaUnitAbbreviations();
            return supportedUnitAbbreviations;
        }
    }


}