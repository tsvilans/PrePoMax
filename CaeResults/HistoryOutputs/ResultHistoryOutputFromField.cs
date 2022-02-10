﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;

namespace CaeResult
{
    [Serializable]
    public class ResultHistoryOutputFromField : ResultHistoryOutput
    {
        // Variables                                                                                                                
        private string _fieldName;
        private string _componentName;


        // Properties                                                                                                               
        public string FieldName { get { return _fieldName; } set { _fieldName = value; } }
        public string ComponentName { get { return _componentName; } set { _componentName = value; } }


        // Constructors                                                                                                             
        public ResultHistoryOutputFromField(string name, string regionName, RegionTypeEnum regionType)
            : base(name, regionName, regionType)
        {
        }


        // Methods                                                                                                                  
    }
}
