﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeModel;
using CaeMesh;

namespace FileInOut.Output.Calculix
{
    [Serializable]
    internal class CalStep : CalculixKeyword
    {
        // Variables                                                                                                                
        private Step _step;


        // Properties                                                                                                               
        public override object BaseItem { get { return _step; } }


        // Events                                                                                                                   


        // Constructor                                                                                                              
        public CalStep(Step step)
        {
            _step = step;
            _active = step.Active;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            string nlGeom = _step.Nlgeom ? string.Format(", Nlgeom, Inc={0}", _step.MaxIncrements) : "";
            
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("*Step{0}", nlGeom).AppendLine();
            return sb.ToString();
        }

        public override string GetDataString()
        {
            return "";
        }
    }
}
