﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeModel;
using CaeMesh;
using CaeGlobals;

namespace FileInOut.Output.Calculix
{
    [Serializable]
    internal class CalGravityLoad : CalculixKeyword
    {
        // Variables                                                                                                                
        private GravityLoad _load;


        // Properties                                                                                                               


        // Constructor                                                                                                              
        public CalGravityLoad(GravityLoad load)
        {
            _load = load;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("** Name: " + _load.Name);
            sb.AppendLine("*Dload");
            return sb.ToString();
        }
        public override string GetDataString()
        {
            StringBuilder sb = new StringBuilder();

            double f1, f2, f3;
            double len = Math.Sqrt(_load.F1 * _load.F1 + _load.F2 * _load.F2 + _load.F3 * _load.F3);
            if (len != 0)
            {
                f1 = _load.F1 / len;
                f2 = _load.F2 / len;
                f3 = _load.F3 / len;
            }
            else f1 = f2 = f3 = 0;

            sb.AppendFormat("{0}, Grav, {1}, {2}, {3}, {4}", _load.RegionName, len.ToCalculiX16String(),
                            f1.ToCalculiX16String(), f2.ToCalculiX16String(), f3.ToCalculiX16String());
            sb.AppendLine();
            return sb.ToString();
        }
    }
}
