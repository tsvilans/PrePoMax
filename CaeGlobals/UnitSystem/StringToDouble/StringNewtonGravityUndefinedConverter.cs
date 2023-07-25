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
    public class StringNewtonGravityUndefinedConverter : TypeConverter
    {
        // Variables                                                                                                                
        protected static ForceUnit _forceUnit = ForceUnit.Newton;
        protected static LengthUnit _lengthUnit = LengthUnit.Meter;
        protected static MassUnit _massUnit = MassUnit.Kilogram;
        protected static string error = "Unable to parse quantity. Expected the form \"{value} {unit abbreviation}" +
                                        "\", such as \"5.5 m\". The spacing is optional.";
        //
        protected static ArrayList values;
        protected static double _initialValue = 0;      // use initial value for the constructor to work
        protected static string _undefined = "Undefined";


        // Properties                                                                                                               
        public static string SetForceUnit
        {
            set
            {
                if (value == "")
                {
                    _forceUnit = (ForceUnit)MyUnit.NoUnit;
                    _lengthUnit = (LengthUnit)MyUnit.NoUnit;
                    _massUnit = (MassUnit)MyUnit.NoUnit;
                }
                else
                {
                    _forceUnit = Force.ParseUnit(value);
                }
            }
        }
        public static string SetLengthUnit
        {
            set
            {
                if (value == "")
                {
                    _forceUnit = (ForceUnit)MyUnit.NoUnit;
                    _lengthUnit = (LengthUnit)MyUnit.NoUnit;
                    _massUnit = (MassUnit)MyUnit.NoUnit;
                }
                else _lengthUnit = Length.ParseUnit(value);
            }
        }
        public static string SetMassUnit
        {
            set
            {
                if (value == "")
                {
                    _forceUnit = (ForceUnit)MyUnit.NoUnit;
                    _lengthUnit = (LengthUnit)MyUnit.NoUnit;
                    _massUnit = (MassUnit)MyUnit.NoUnit;
                }
                else _massUnit = Mass.ParseUnit(value);
            }
        }
        public static double SetInitialValue
        {
            set
            {
                _initialValue = value;
                CreateListOfStandardValues();
            }
        }
        //
        public static string GetUnitAbbreviation(ForceUnit forceUnit, LengthUnit lengthUnit, MassUnit massUnit)
        {
            string unit;
            if ((int)forceUnit == MyUnit.NoUnit || (int)lengthUnit == MyUnit.NoUnit || (int)massUnit == MyUnit.NoUnit)
                unit = "";
            else unit = Force.GetAbbreviation(forceUnit) + "·" + Length.GetAbbreviation(lengthUnit) + "²/" +
                        Mass.GetAbbreviation(massUnit) + "²";
            return unit;
        }
        
        
        // Constructors                                                                                                             
        public StringNewtonGravityUndefinedConverter()
        {
            CreateListOfStandardValues();
        }


        // Methods                                                                                                                  
        private static void CreateListOfStandardValues()
        {
            values = new ArrayList(new double[] { double.PositiveInfinity, _initialValue });
        }
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            // Passes the local integer array.
            StandardValuesCollection svc = new StandardValuesCollection(values);
            return svc;
        }
        //
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string)) return true;
            else return base.CanConvertFrom(context, sourceType);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            // Convert from string
            if (value is string valueString)
            {
                if (string.Equals(valueString, _undefined)) return double.PositiveInfinity;
                else return MyNCalc.ConvertFromString(valueString, ConvertToCurrentUnits);
            }
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
                        if (double.IsPositiveInfinity(valueDouble)) return _undefined;
                        else
                        {
                            string valueString = valueDouble.ToString();
                            string unit = GetUnitAbbreviation(_forceUnit, _lengthUnit, _massUnit);
                            if (unit.Length > 0) valueString += " " + unit;
                            return valueString;
                        }
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
                string[] tmp = valueWithUnitString.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                if (tmp.Length != 2) throw new FormatException(error);
                string denominator = tmp[1];
                //
                tmp = tmp[0].Split(new string[] { "*", "·" }, StringSplitOptions.RemoveEmptyEntries);
                if (tmp.Length != 2) throw new FormatException(error);
                //
                Force force = Force.Parse(tmp[0]);
                // NoUnit
                if ((int)_forceUnit == MyUnit.NoUnit || (int)_lengthUnit == MyUnit.NoUnit || (int)_massUnit == MyUnit.NoUnit)
                    return force.Value;
                else force = force.ToUnit(_forceUnit);
                //
                if (tmp[1].EndsWith("²") || tmp[1].EndsWith("^2")) tmp[1] = tmp[1].Replace("²", "").Replace("^2", "");
                else throw new FormatException(error);
                LengthUnit lengthUnit = Length.ParseUnit(tmp[1]);
                Length length = Length.From(1, lengthUnit).ToUnit(_lengthUnit);
                //
                if (denominator.EndsWith("²") || denominator.EndsWith("^2"))
                    denominator = denominator.Replace("²", "").Replace("^2", "");
                else throw new FormatException(error);
                MassUnit massUnit = Mass.ParseUnit(denominator);
                Mass mass = Mass.From(1, massUnit).ToUnit(_massUnit);
                //
                double value = (double)force.Value * Math.Pow(length.Value, 2) / Math.Pow(mass.Value, 2);
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
            supportedUnitAbbreviations += StringLengthConverter.SupportedUnitAbbreviations();
            supportedUnitAbbreviations += Environment.NewLine + Environment.NewLine;
            supportedUnitAbbreviations += StringMassConverter.SupportedUnitAbbreviations();
            return supportedUnitAbbreviations;
        }
    }


}