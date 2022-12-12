using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitsNet;
using UnitsNet.Units;
using NCalc;
using System.Runtime.CompilerServices;

namespace CaeGlobals
{
    static class MyNCalc
    {
        static public double ConvertFromString(string valueString, Func<string, double> ConvertToCurrentUnits)
        {
            double valueDouble;
            valueString = valueString.Trim();
            //
            if (valueString.Length == 0 || valueString == "=") return 0;   // empty string -> 0
            if (!double.TryParse(valueString, out valueDouble))
            {
                if (valueString.StartsWith("="))
                {
                    valueString = valueString.Substring(1, valueString.Length - 1);
                    NCalc.Expression e = MyNCalc.GetExpression(valueString);
                    if (!e.HasErrors())
                    {
                        object result = e.Evaluate();
                        if (result is int) valueDouble = (int)result;
                        else if (result is double) valueDouble = (double)result;
                    }
                    else
                    {
                        throw new CaeException("Equation error:" + Environment.NewLine + e.Error);
                    }
                }
                else valueDouble = ConvertToCurrentUnits(valueString);
            }
            //
            return valueDouble;
        }
        static public Expression GetExpression(string expression)
        {
            Expression e = new Expression(expression, EvaluateOptions.IgnoreCase);
            e.EvaluateParameter += EvaluateParameter;
            return e;
        }

        static public void EvaluateParameter(string name, ParameterArgs args)
        {
            if (name == "Pi") args.Result = Math.PI;
        }
}
}
